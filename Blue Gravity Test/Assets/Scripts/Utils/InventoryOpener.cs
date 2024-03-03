using Jega.BlueGravity.PreWrittenCode;
using Jega.BlueGravity.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class InventoryOpener : MonoBehaviour
    {
        [SerializeField] private UIState inventoryState;

        private UIService uiService;

        public void Awake()
        {
            uiService = ServiceProvider.GetService<UIService>();
            InputService.OnInventoryInputPressed += RequestInventoryState;
        }
        private void OnDestroy()
        {
            InputService.OnInventoryInputPressed -= RequestInventoryState;
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
