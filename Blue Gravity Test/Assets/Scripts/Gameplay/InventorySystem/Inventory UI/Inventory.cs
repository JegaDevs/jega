using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Jega.BlueGravity.Inventory;

namespace Jega.BlueGravity
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private InventoryData inventoryData;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private InventorySlot slotPrefab;

        [SerializeField] protected List<Slot> slots;

        protected SessionService sessionService;

        private ReadOnlyCollection<InventoryItem> ItemCollection => inventoryData.itemCollection.Collection;
        private List<ItemPair> StartingItems => inventoryData.startingItems;
        private string InventorySaveKey => inventoryData.inventorySaveKey;
        private int NumberOfSlots => inventoryData.numberOfSlots;
        private const string SlotSaveKey = "_Slot_";

        protected virtual void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();

            InitialInvetorySetup();
            InventorySlot.OnRequestSlotSwitch += SwitchSlots;
            InventorySlot.OnItemBought += CheckItemBought;
            InventorySlot.OnItemSold += CheckItemSold;
        }

        protected virtual void OnDestroy()
        {
            InventorySlot.OnRequestSlotSwitch -= SwitchSlots;
        }

        protected virtual void OnEnable()
        {
            UpdateSlotsRegistries();
        }

        #region initial Setup
        private void InitialInvetorySetup()
        {
            slots = new List<Slot>();
            List<ItemPair> unfilledItemPairs = new List<ItemPair>();
            foreach(ItemPair itemPair in StartingItems)
                if(itemPair.Item.GetCustomSavedAmount(InventorySaveKey, itemPair.StartingAmount) > 0)
                    unfilledItemPairs.Add(itemPair);

            //Fill saved slots
            for (int i = 0; i < NumberOfSlots; i++)
            {
                ItemPair itemPair = default;
                int storedItemIndex = PlayerPrefs.GetInt(InventorySaveKey + SlotSaveKey + i, -1);
                if (storedItemIndex >= 0)
                {
                    InventoryItem storedItem = ItemCollection[storedItemIndex];
                    int startingItemIndex = StartingItems.FindIndex(a => a.Item == storedItem);
                    int startingAmount = startingItemIndex >= 0 ? StartingItems[startingItemIndex].StartingAmount : 0;
                    if (storedItem.GetCustomSavedAmount(InventorySaveKey, startingAmount) > 0)
                    {
                        itemPair = new ItemPair(storedItem, startingAmount);
                        unfilledItemPairs.Remove(itemPair);
                    }
                }
                InventorySlot slotSetup = CreateNewSlotVisual(itemPair, i);
                slots.Add(new Slot(slotSetup, i, itemPair, InventorySaveKey, storedItemIndex));
            }


            if (unfilledItemPairs.Count <= 0) 
                return;

            //Fill unsaved slots (should happen only once to fill initial values on a clear save)
            foreach (ItemPair itemPair in unfilledItemPairs)
            {
                for (int i = 0; i < NumberOfSlots; i++)
                {
                    if (slots[i].IsEmpty)
                    {
                        Slot currentSlot = slots[i];
                        itemPair.Item.SetCustomSavedAmount(InventorySaveKey, itemPair.StartingAmount);
                        int storedItemIndex = ItemCollection.IndexOf(itemPair.Item);
                        slots[i] = new Slot(currentSlot.UISlot, currentSlot.Index, itemPair, InventorySaveKey, storedItemIndex);
                        UpdateSlotVisual(slots[i].UISlot, itemPair);
                        break;
                    }
                }
            }
            Debug.LogError("Filled unsaved slots. \n Attention! This should happen only once when there's no saved data!");
        }

        private InventorySlot CreateNewSlotVisual(ItemPair itemPair, int index)
        {
            InventorySlot uiSlot = Instantiate(slotPrefab, slotsParent);
            UpdateSlotVisual(uiSlot, itemPair);
            uiSlot.name = slotPrefab.name + index;
            return uiSlot;
        }


        private void UpdateSlotsRegistries()
        {
            int count = slots.Count;
            for (int i = 0; i < count; i++)
            {
                UpdateTargetSlot(i);
            }
        }
        protected void UpdateTargetSlot(int slotIndex)
        {
            Slot slot = slots[slotIndex];
            ItemPair itemPair = default;
            int storedItemIndex = PlayerPrefs.GetInt(InventorySaveKey + SlotSaveKey + slotIndex, -1);
            if (storedItemIndex >= 0)
            {
                InventoryItem storedItem = ItemCollection[storedItemIndex];
                int startingItemIndex = StartingItems.FindIndex(a => a.Item == storedItem);
                int startingAmount = startingItemIndex >= 0 ? StartingItems[startingItemIndex].StartingAmount : 0;
                int ownedAmount = storedItem.GetCustomSavedAmount(InventorySaveKey, startingAmount);
                if (ownedAmount > 0)
                    itemPair = new ItemPair(storedItem, startingAmount);
                else
                    storedItemIndex = -1;
            }
            slots[slotIndex] = new Slot(slot.UISlot, slot.Index, itemPair, InventorySaveKey, storedItemIndex);
            UpdateSlotVisual(slot.UISlot, itemPair);
        }
        #endregion

        protected virtual void UpdateSlotVisual(InventorySlot slotVisual, ItemPair itemPair)
        {
            slotVisual.UpdateInfo(this, itemPair, InventorySaveKey);
        }

        protected void UpdateSlotsVisuals()
        {
            foreach (Slot slot in slots)
                UpdateSlotVisual(slot.UISlot, slot.ItemPair);
        }



        void SwitchSlots(Inventory inventoryManager, InventorySlot original, InventorySlot destination)
        {
            if (inventoryManager != this) return;

            Slot originSlot = slots.Find(a => a.UISlot == original);
            Slot destinationSlot = slots.Find(a => a.UISlot == destination);

            slots[originSlot.Index] = new Slot(originSlot.UISlot, originSlot.Index, destinationSlot.ItemPair, InventorySaveKey, destinationSlot.ItemIndex);
            slots[destinationSlot.Index] = new Slot(destinationSlot.UISlot, destinationSlot.Index, originSlot.ItemPair, InventorySaveKey, originSlot.ItemIndex);

            UpdateSlotVisual(original, slots[originSlot.Index].ItemPair);
            UpdateSlotVisual(destination, slots[destinationSlot.Index].ItemPair);
        }

        private void CheckItemBought(Inventory shopIventory, InventoryItem item, int amount)
        {
            if(this == sessionService.CurrentClientInventory)
                GainItemamount(item, amount);
            
            else if(this == sessionService.CurrentShopInventory)
                LoseItemAmount(item, amount);
            
        }


        private void CheckItemSold(Inventory shopIventory, InventoryItem item, int amount)
        {
            if (this == sessionService.CurrentShopInventory)
                GainItemamount(item, amount);
            else if (this == sessionService.CurrentClientInventory)
                LoseItemAmount(item, amount);
        }

        private void GainItemamount(InventoryItem item, int amount)
        {
            int previousOwned = item.GetCustomSavedAmount(InventorySaveKey, 0);
            int newOwned = previousOwned + amount;
            item.SetCustomSavedAmount(InventorySaveKey, newOwned);
            bool isSlotAlreadyFilled = previousOwned > 0;
            if (isSlotAlreadyFilled)
            {
                int slotIndex = slots.FindIndex(a => a.Item == item);
                UpdateTargetSlot(slotIndex);
            }
            else
            {
                ItemPair itemPair = new ItemPair(item);
                for (int i = 0; i < NumberOfSlots; i++)
                {
                    if (slots[i].IsEmpty)
                    {
                        Slot currentSlot = slots[i];
                        int storedItemIndex = ItemCollection.IndexOf(itemPair.Item);
                        slots[i] = new Slot(currentSlot.UISlot, currentSlot.Index, itemPair, InventorySaveKey, storedItemIndex);
                        UpdateSlotVisual(slots[i].UISlot, itemPair);
                        break;
                    }
                }
            }
        }
        private void LoseItemAmount(InventoryItem item, int amount)
        {
            int previousOwned = item.GetCustomSavedAmount(InventorySaveKey, 0);
            int newOwned = previousOwned - amount;
            item.SetCustomSavedAmount(InventorySaveKey, newOwned);
            int slotIndex = slots.FindIndex(a => a.Item == item);
            UpdateTargetSlot(slotIndex);
        }


        #region public straucks
        [Serializable]
        public struct Slot
        {
            public ItemPair ItemPair;
            public InventorySlot UISlot;
            public int Index;
            public int ItemIndex;
            public Slot(InventorySlot uiSlot, int slotIndex, ItemPair itemPair, string customSlotSaveKey, int itemIndex)
            {
                ItemPair = itemPair;
                UISlot = uiSlot;
                Index = slotIndex;
                ItemIndex = itemIndex;

                PlayerPrefs.SetInt(customSlotSaveKey + SlotSaveKey + slotIndex, itemIndex);
                
            }

            public InventoryItem Item => ItemPair.Item;
            public bool IsEmpty => Item == null;
        }

        [Serializable]
        public struct ItemPair
        {
            public InventoryItem Item;
            public int StartingAmount;


            public ItemPair(InventoryItem item, int startingAmount = 0)
            {
                Item = item;
                StartingAmount = startingAmount;
            }
            public ItemPair(ItemPair copy)
            {
                Item = copy.Item;
                StartingAmount = copy.StartingAmount;
            }

            public bool IsValid => Item != null;
        }
        #endregion
    }
}
