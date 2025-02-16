using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JegaCore;

namespace Jega.BlueGravity
{
    public class InventoryOpener : MonoBehaviour
    {
        [SerializeField] private UIState inventoryState;

        private UIService uiService;
        private CoreInputData inputData;

        public void Awake()
        {
            uiService = ServiceProvider.GetService<UIService>();
            inputData = InputService.Service.CoreInputData;
            inputData.OnSpecialButton1Pressed += RequestInventoryState;
        }
        private void OnDestroy()
        {
            inputData.OnSpecialButton1Pressed -= RequestInventoryState;
        }
        void RequestInventoryState()
        {
            if (uiService.ActiveUIStates.Contains(inventoryState))
                uiService.RequestRemoveUIState(inventoryState);
            else
                uiService.RequestNewUIState(inventoryState);
        }

    }
}
