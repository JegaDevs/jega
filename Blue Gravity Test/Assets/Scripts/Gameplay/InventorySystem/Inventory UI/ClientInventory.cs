using Jega.BlueGravity.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Jega.BlueGravity.Inventory;

namespace Jega.BlueGravity
{
    public class ClientInventory : Inventory
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            sessionService.RegisterActiveClientInventory(this);
        }
        private void OnDisable()
        {
            sessionService.UnregisterClientShopInventory();
        }
        protected override void UpdateSlotVisual(InventorySlot slotVisual, ItemPair itemPair, int slotIndex)
        {
            base.UpdateSlotVisual(slotVisual, itemPair, slotIndex);
            slotVisual.UpdateAvailability();
        }
    }
}
