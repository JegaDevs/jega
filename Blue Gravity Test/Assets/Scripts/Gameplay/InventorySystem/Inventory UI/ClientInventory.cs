using Jega.BlueGravity.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class ClientInventory : Inventory
    {
        private SessionService sessionService;
        protected override void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>(); 
            base.Awake();
        }
        protected override void UpdateSlotVisual(UIInventorySlot slotVisual, ItemPair itemPair)
        {
            base.UpdateSlotVisual(slotVisual, itemPair);
            slotVisual.UpdateAvailability();
        }
    }
}
