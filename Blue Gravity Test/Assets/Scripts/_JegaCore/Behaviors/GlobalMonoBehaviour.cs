using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JegaCore
{
    public class GlobalMonoBehaviour : SingletonMonoBehaviour<GlobalMonoBehaviour>
    {
        private const bool SanityChecks = true;
        
        public static event Action<bool> OnUnityApplicationPause;

        [Serializable]
        public struct UpdateMethodRegister
        {
            /// <summary>
            /// Method's priority in the global update loop. Lower priorities get called before higher priorities.
            /// </summary>
            public int priority;
            /// <summary>
            /// The method that will be called in the update loop.
            /// </summary>
            public Action method;
            
            public UpdateMethodRegister(Action method)
            {
                this.method = method;
                priority = 0;
            }
            public UpdateMethodRegister(Action method, int priority)
            {
                this.method = method;
                this.priority = priority;
            }

            public static bool operator ==(UpdateMethodRegister one, UpdateMethodRegister two) => one.method == two.method;
            public static bool operator !=(UpdateMethodRegister one, UpdateMethodRegister two) => !(one == two);
            public bool Equals(UpdateMethodRegister other) => Equals(method, other.method);
            public override bool Equals(object obj) => obj is UpdateMethodRegister other && Equals(other);
            public override int GetHashCode() => method.GetHashCode();
        }
        
        private static readonly Comparer<UpdateMethodRegister> Comparer = Comparer<UpdateMethodRegister>.Create((p1,p2)=>p1.priority.CompareTo(p2.priority));
        private static readonly SortedSet<UpdateMethodRegister> UpdateMethodsSet = new SortedSet<UpdateMethodRegister>(Comparer);
        private static readonly SortedSet<UpdateMethodRegister> LateUpdateMethodsSet = new SortedSet<UpdateMethodRegister>(Comparer);
        private static readonly SortedSet<UpdateMethodRegister> FixedUpdateMethodsSet = new SortedSet<UpdateMethodRegister>(Comparer);
        
        /// <summary>
        /// Gets a duplicated *copy* of the methods currently registered with the Update loop.
        /// </summary>
        public static SortedSet<UpdateMethodRegister> GetUpdateMethodsSet => new SortedSet<UpdateMethodRegister>(UpdateMethodsSet);
        /// <summary>
        /// Gets a duplicated *copy* of the methods currently registered with the LateUpdate loop.
        /// </summary>
        public static SortedSet<UpdateMethodRegister> GetLateUpdateMethodsSet => new SortedSet<UpdateMethodRegister>(LateUpdateMethodsSet);
        /// <summary>
        /// Gets a duplicated *copy* of the methods currently registered with the FixedUpdate loop.
        /// </summary>
        public static SortedSet<UpdateMethodRegister> GetFixedUpdateMethodsSet => new SortedSet<UpdateMethodRegister>(FixedUpdateMethodsSet);
        
        #region Unity Messages
        private void Update()
        {
            if (UpdateMethodsSet.Count <= 0) return;
            foreach (UpdateMethodRegister updateMethodRegister in UpdateMethodsSet)
                updateMethodRegister.method?.Invoke();
        }

        private void LateUpdate()
        {
            if (LateUpdateMethodsSet.Count <= 0) return;
            foreach (UpdateMethodRegister updateMethodRegister in LateUpdateMethodsSet)
                updateMethodRegister.method?.Invoke();
        }

        private void FixedUpdate()
        {
            if (FixedUpdateMethodsSet.Count <= 0) return;
            foreach (UpdateMethodRegister updateMethodRegister in FixedUpdateMethodsSet)
                updateMethodRegister.method?.Invoke();
        }

        private void OnDestroy()
        {
            /*UpdateMethodsSet.Clear();
            LateUpdateMethodsSet.Clear();
            FixedUpdateMethodsSet.Clear();*/
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnUnityApplicationPause?.Invoke(pauseStatus);
        }

        //this should only be called in a moment where a GameObject can actually be instanced; testing shows this must be either 'BeforeSceneLoad' or 'AfterSceneLoad'
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            Debug.Log("Instanced GlobalMonoBehaviour");
            
			//null-checking the instance here, so the expected behavior is that this call will create the singleton instance if it doesnt exist yet
            if (Instance == null)            
                Debug.LogError($"Failed to instantiate {nameof(GlobalMonoBehaviour)}.");
        }
        #endregion

        #region Global Update Registry
        /// <summary>
        /// Adds a method to the Unity update loop of the global host. If SanityChecks = true, avoid and send a warning
        /// about adding the same method to the same update loop twice.
        /// </summary>
        /// <param name="method">Method to add to the update loop.</param>
        /// <param name="updateMethod">Which Unity update loop to use.</param>
        /// <param name="priority">Methods with lower priority get called earlier.</param>
        /// <returns>True if the method was successfully added, false otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool RegisterUpdateMethod(Action method, UnityUpdateMethod updateMethod = UnityUpdateMethod.Update, int priority = 0)
        {
            if (method == null) return false;

            SortedSet<UpdateMethodRegister> updateSet = GetSortedSetFromUpdateMethod(updateMethod);
            if (SanityChecks)
            {
                if (updateSet.Contains(new UpdateMethodRegister(method, priority)))
                {
                    Debug.LogWarning($"Attempted to add method {nameof(method)} twice in the same update method {nameof(updateMethod)}.", Instance);
                    return false;
                }
            }
            updateSet.Add(new UpdateMethodRegister(method, priority));
            return true;
        }
        
        /// <summary>
        /// Removes a method from a global update loop registry.
        /// </summary>
        /// <param name="method">Method to be removed.</param>
        /// <param name="updateMethod">Update loop set to remove the method from.</param>
        /// <returns>True if a method was successfully removed from an update set, false otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool UnregisterUpdateMethod(Action method, UnityUpdateMethod updateMethod = UnityUpdateMethod.Update)
        {
            if (method == null) return false;

            SortedSet<UpdateMethodRegister> updateSet = GetSortedSetFromUpdateMethod(updateMethod);
            int removed = updateSet.RemoveWhere(x => x.method == method);
            return removed > 0;
        }
        
        /// <summary>
        /// Changes the priority of a method that was previously registered.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="newPriority"></param>
        /// <param name="updateMethod"></param>
        /// <returns></returns>
        public static bool SetMethodPriority(Action method, int newPriority, UnityUpdateMethod updateMethod = UnityUpdateMethod.Update)
        {
            if (method == null) return false;

            if (UnregisterUpdateMethod(method, updateMethod) == false) return false;
            return RegisterUpdateMethod(method, updateMethod, newPriority);
        }

        private static SortedSet<UpdateMethodRegister> GetSortedSetFromUpdateMethod(UnityUpdateMethod updateMethod)
        {
            return updateMethod switch
            {
                UnityUpdateMethod.Update => UpdateMethodsSet,
                UnityUpdateMethod.LateUpdate => LateUpdateMethodsSet,
                UnityUpdateMethod.FixedUpdate => FixedUpdateMethodsSet,
                _ => throw new ArgumentOutOfRangeException(nameof(updateMethod), updateMethod, null)
            };
        }
        #endregion
        
        #region Global Coroutine Host
        /// <summary>
        /// Starts a coroutine on the global host.
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public static Coroutine HostCoroutine(IEnumerator coroutine)
        {
            if (InstanceIsInvalid)
            {
                GlobalMonoBehaviour createInstance = Instance;
                if (createInstance == null)
                {
                    Debug.LogWarning($"No GlobalCoroutineHost instance found. Coroutine start failed.");
                    return null;
                }
            }
            return Instance.StartCoroutine(coroutine);
        }
        
        /// <summary>
        /// Stops the coroutine running on the global host.
        /// </summary>
        /// <param name="coroutine"></param>
        public static void KillCoroutine(IEnumerator coroutine)
        {
            if (Instance == null) return;
            Instance.StopCoroutine(coroutine);
        }
        
        /// <summary>
        /// Stops the coroutine running on the global host.
        /// </summary>
        /// <param name="coroutine"></param>
        public static void KillCoroutine(Coroutine coroutine)
        {
            if (Instance == null) return;
            Instance.StopCoroutine(coroutine);
        }
        
        /// <summary>
        /// Calls 'StopAllCoroutines' on the GlobalMonobehavior. Use with care.
        /// </summary>
        public static void KillAllCoroutines()
        {
            if (Instance == null) return;
            Instance.StopAllCoroutines();
        }
        #endregion
    }

    public enum UnityUpdateMethod
    {
        Update, LateUpdate, FixedUpdate
    };
}