using Jega.BlueGravity.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Jega.BlueGravity.Inventory;

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
        protected override void UpdateSlotVisual(InventorySlot slotVisual, ItemPair itemPair)
        {
            base.UpdateSlotVisual(slotVisual, itemPair);
            slotVisual.UpdateAvailability();
        }
    }
}
