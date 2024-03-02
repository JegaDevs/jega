using Jega.BlueGravity.PreWrittenCode;
using Jega.PreWrittenCode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jega.BlueGravity.PreWrittenCode
{
    public class UIService : IService
    {
        public static UIStateEvent OnUpdatedUIState;
        public delegate void UIStateEvent(UIState state, int layer);

        private GameObject currentUIParent;
        private Scene currentScene;

        private UIServiceData uiData;
        private List<UIState> activeUIStates;

        public List<UIState> ActiveUIStates { get => activeUIStates; }
        public static UIService Service => ServiceProvider.GetService<UIService>();
        public const int TargetAllLayers = -1;

        #region Interface Implementation
        public int Priority => 0;


        public void Preprocess()
        {
            InitializeService();
        }

        public void Postprocess()
        {
        }
        #endregion

        private void InitializeService()
        {
            activeUIStates = new List<UIState>();
            uiData = StaticPaths.LoadScriptableOrCreateIfMissing<UIServiceData>(nameof(UIServiceData));
            SceneManager.LoadSceneAsync(uiData.uiSceneName, LoadSceneMode.Additive);
            foreach (UIState state in uiData.defaultUIStates)
                RequestNewUIState(state);
        }

        public void RegisterNewScene(GameObject uiParent)
        {
            if (currentUIParent != null && currentScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(uiParent.scene.buildIndex);
                return;
            }

            currentUIParent = uiParent;
            currentScene = uiParent.scene;
        }

        public void RequestNewUIState(UIState newState)
        {
            int index = activeUIStates.FindIndex((current) => current.Layer == newState.Layer);
            if (index < 0)
                activeUIStates.Add(newState);
            else
                activeUIStates[index] = newState;

            OnUpdatedUIState?.Invoke(newState, newState.Layer);
        }
        public void RequestRemoveUIState(UIState newState)
        {
            int index = activeUIStates.FindIndex((current) => current == newState);
            if (index >= 0)
            {
                activeUIStates.RemoveAt(index);
                OnUpdatedUIState?.Invoke(null, newState.Layer);
            }
            else
            {
                Debug.LogError("Tried to clear empty UI State: " + newState.name);
                return;
            }
        }

        public void ClearUILayer(int layer, bool allLayers = false)
        {
            if (allLayers)
            {
                activeUIStates.Clear();
                OnUpdatedUIState?.Invoke(null, TargetAllLayers);
            }
            else
            {
                int index = activeUIStates.FindIndex((current) => current.Layer == layer);
                if (index < 0)
                {
                    activeUIStates.RemoveAt(index);
                    OnUpdatedUIState?.Invoke(null, layer);
                }
                else
                {
                    Debug.LogError("Tried to clear empty UI State layer: " + layer);
                    return;
                }
            }
        }
    }
}
