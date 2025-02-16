using UnityEngine;

namespace JegaCore
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("Singleton")]
        public bool dontDestroyOnLoad = true;

        #region Properties
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;

                // Search for existing instance.
                instance = (T)FindObjectOfType(typeof(T));
                
                if (instance != null) return instance;

                GameObject newGo = new GameObject
                {
                    name = typeof(T).Name + "'s Singleton"
                };
                instance = newGo.AddComponent<T>();
                
                return instance;
            }
        }
        public static bool InstanceIsValid => instance != null;
        public static bool InstanceIsInvalid => !InstanceIsValid;
        #endregion

        #region Unity Messages
        protected virtual bool Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return false;
            }
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            return true;
        }
        #endregion
    }
}