using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity.PreWrittenCode
{
    public class UIStatePanel : MonoBehaviour
    {
        [SerializeField] protected List<UIState> activeStates;

        protected UIService uiService;

        private void Awake()
        {
            uiService = UIService.Service;
            CallStateAwakes();

            UIService.OnUpdatedUIState += CheckNewState;

        }
        private void Start()
        {
            TogglePannel(GetIsStateActive());
        }

        private void OnDestroy()
        {
            CallOnDestroy();
            UIService.OnUpdatedUIState -= CheckNewState;
        }


        private void CheckNewState(UIState state, int layer)
        {
            if (layer == -1)
                TogglePannel(false);
            else if (layer == activeStates[0].Layer)
                TogglePannel(activeStates.Contains(state));

        }
        protected virtual void TogglePannel(bool toggle)
        {
            if (gameObject.activeSelf == toggle)
                return;
            gameObject.SetActive(toggle);

        }
        private bool GetIsStateActive()
        {
            foreach (UIState activeState in uiService.ActiveUIStates)
            {
                foreach (UIState state in this.activeStates)
                    if (state == activeState)
                        return true;
            }
            return false;
        }



        private void CallStateAwakes()
        {
            MonoBehaviour[] scripts = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour script in scripts)
            {
                IUIStateAwake stateAwake = script.GetComponent<IUIStateAwake>();
                if (stateAwake != null)
                    stateAwake.StateAwake();
            }
        }
        private void CallOnDestroy()
        {
            MonoBehaviour[] scripts = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour script in scripts)
            {
                IUIStateOnDestroy stateOnDestroy = script.GetComponent<IUIStateOnDestroy>();
                if (stateOnDestroy != null)
                    stateOnDestroy.StateOnDestroy();
            }
        }


#if UNITY_EDITOR
        [Button]
        public void ForceStateChange()
        {
            if (Application.isPlaying == false)
            {
                Debug.LogError("Must be pressed only when playing");
                return;
            }
            UIService.Service.RequestNewUIState(activeStates[0]);
        }

#endif


        private void OnValidate()
        {
            if (activeStates == null || activeStates.Count <= 0) return;
            int layer = activeStates[0].Layer;
            UIState invalidState = null;
            foreach (UIState state in activeStates)
            {
                if (state.Layer != layer)
                {
                    Debug.LogError("Tried to add satates with different layers to UIPannel: " + state.name);
                    invalidState = state;
                    break;
                }
            }
            activeStates.Remove(invalidState);
        }
    }

    public interface IUIStateAwake
    {
        public void StateAwake();
    }

    public interface IUIStateOnDestroy
    {
        public void StateOnDestroy();
    }
}
