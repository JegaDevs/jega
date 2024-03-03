using System;
using System.Collections.Generic;

namespace Jega.BlueGravity.InventorySystem
{
    public class ClothingInventory : Inventory
    {
        public static Action OnClothingInventoryUpdated;

        private const int HeadSlotIndex = 0;
        private const int BodySlotIndex = 1;

        public List<Slot> Slots => slots;
        public ClothingItem HeadItem => slots[HeadSlotIndex].Item as ClothingItem;
        public ClothingItem BodyItem => slots[BodySlotIndex].Item as ClothingItem;
        protected override void OnEnable()
        {
            base.OnEnable();
            sessionService.RegisterClothingInventory(this);
        }

        public bool CheckIfSwitchIsValid(InventoryItem item, int slotIndex)
        {
            if (item is not ClothingItem clothing) return false;
            if (clothing.Type == ClothingItem.ClothingType.Head)
                return slotIndex == HeadSlotIndex;
            else
                return slotIndex == BodySlotIndex;
        }


        protected override void GainItemAmount(InventoryItem item, int amount)
        {
            int previousOwned = item.GetCustomSavedAmount(InventorySaveKey, 0);
            int newOwned = previousOwned + amount;
            item.SetCustomSavedAmount(InventorySaveKey, newOwned);
            StartingItem startingItem = new StartingItem(item);
            if (item is ClothingItem clothingItem)
            {
                if (clothingItem.Type == ClothingItem.ClothingType.Body)
                    SetSlot(BodySlotIndex, startingItem);
                else
                    SetSlot(HeadSlotIndex, startingItem);
            }
            OnClothingInventoryUpdated?.Invoke();

            void SetSlot(int slotIndex, StartingItem startingItem)
            {
                Slot currentSlot = slots[slotIndex];
                int storedItemIndex = ItemCollection.IndexOf(startingItem.Item);
                slots[slotIndex] = new Slot(currentSlot.SlotManager, currentSlot.Index, startingItem, InventorySaveKey, storedItemIndex);
                UpdateSlotManager(slots[slotIndex].SlotManager, startingItem, slotIndex);
            }
        }

        protected override void LoseItemAmount(InventoryItem item, int amount)
        {
            base.LoseItemAmount(item, amount);
            OnClothingInventoryUpdated?.Invoke();
        }
    }
}
