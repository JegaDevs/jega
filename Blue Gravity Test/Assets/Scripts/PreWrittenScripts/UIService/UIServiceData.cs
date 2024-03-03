using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity.PreWrittenCode
{
    public class UIServiceData : ScriptableObject
    {
        [Scene] public string uiSceneName;
        public List<UIState> defaultUIStates;

        private void OnValidate()
        {
            if (defaultUIStates == null || defaultUIStates.Count <= 0) return;
            int layer = defaultUIStates[0].Layer;
            UIState invalidState = null;
            foreach (UIState state in defaultUIStates)
            {
                if (state.Layer == layer && state != defaultUIStates[0])
                {
                    Debug.LogError("Tried to add multiple states with same layer to defaultUIStates: " + state.name);
                    invalidState = state;
                    break;
                }
            }
            defaultUIStates.Remove(invalidState);
        }
    }
}
