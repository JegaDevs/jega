using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Jega.BlueGravity.InventorySystem.Inventory;

namespace Jega.BlueGravity.InventorySystem
{
    /// <summary>
    /// Generic Inventory class. Handles all main slot management logic, shouldbe used 
    /// as a base class for inventory variants.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        public SlotUpdated OnSlotUpdated;
        public delegate void SlotUpdated(Inventory inventory, InventorySlot slot, StartingItem startingItem, int slotIndex);

        [SerializeField] private InventoryData inventoryData;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private InventorySlot slotPrefab;

        [SerializeField]protected List<Slot> slots;
        protected SessionService sessionService;

        public string InventorySaveKey => inventoryData.inventorySaveKey;
        protected ReadOnlyCollection<InventoryItem> ItemCollection => inventoryData.itemCollection.Collection;
        protected int NumberOfSlots => inventoryData.numberOfSlots;
        private List<StartingItem> StartingItems => inventoryData.startingItems;
        private const string SlotSaveKey = "_Slot_";

        protected virtual void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();
            InitialInvetorySetup();

            InventorySlot.OnRequestOwnedSlotsSwitch += SwitchOwnedSlots;
            InventorySlot.OnRequestClothingInventorySwitch += HandleClothingEquiping;
            InventorySlot.OnItemBought += CheckItemBought;
            InventorySlot.OnItemSold += CheckItemSold;
        }

        protected virtual void OnDestroy()
        {
            InventorySlot.OnRequestOwnedSlotsSwitch -= SwitchOwnedSlots;
            InventorySlot.OnRequestClothingInventorySwitch -= HandleClothingEquiping;
            InventorySlot.OnItemBought -= CheckItemBought;
            InventorySlot.OnItemSold -= CheckItemSold;
        }

        protected virtual void OnEnable()
        {
            UpdateAllSlots();
        }

        #region Initial Setup
        private void InitialInvetorySetup()
        {
            slots = new List<Slot>();
            List<StartingItem> unfilledStartingItems = new List<StartingItem>();
            foreach (StartingItem startingItem in StartingItems)
                if (startingItem.Item.GetCustomSavedAmount(InventorySaveKey, startingItem.Amount) > 0)
                    unfilledStartingItems.Add(startingItem);

            //Fill saved slots
            for (int i = 0; i < NumberOfSlots; i++)
            {
                StartingItem startingItem = default;
                int storedItemIndex = PlayerPrefs.GetInt(InventorySaveKey + SlotSaveKey + i, -1);
                if (storedItemIndex >= 0)
                {
                    InventoryItem storedItem = ItemCollection[storedItemIndex];
                    int startingItemIndex = StartingItems.FindIndex(a => a.Item == storedItem);
                    int startingAmount = startingItemIndex >= 0 ? StartingItems[startingItemIndex].Amount : 0;
                    if (storedItem.GetCustomSavedAmount(InventorySaveKey, startingAmount) > 0)
                    {
                        startingItem = new StartingItem(storedItem, startingAmount);
                        unfilledStartingItems.Remove(startingItem);
                    }
                }
                InventorySlot slotManager = CreateNewSlots(i);
                slots.Add(new Slot(slotManager, i, startingItem, InventorySaveKey, storedItemIndex));
                OnSlotUpdated?.Invoke(this, slotManager, startingItem, i);
            }

            if (unfilledStartingItems.Count <= 0)
                return;

            //Fill unsaved slots (should happen only once to fill initial values on a empty save)
            foreach (StartingItem startingItem in unfilledStartingItems)
            {
                for (int i = 0; i < NumberOfSlots; i++)
                {
                    if (slots[i].IsEmpty)
                    {
                        Slot currentSlot = slots[i];
                        startingItem.Item.SetCustomSavedAmount(InventorySaveKey, startingItem.Amount);
                        int storedItemIndex = ItemCollection.IndexOf(startingItem.Item);
                        slots[i] = new Slot(currentSlot.SlotManager, currentSlot.Index, startingItem, InventorySaveKey, storedItemIndex);
                        OnSlotUpdated?.Invoke(this, currentSlot.SlotManager, startingItem, i);
                        break;
                    }
                }
            }
            Debug.Log("Filled unsaved slots. \n Attention! This should happen only once when there's no saved data!");
        }
        private InventorySlot CreateNewSlots(int index)
        {
            InventorySlot slotManager = Instantiate(slotPrefab, slotsParent);
            slotManager.RegisterManager(this);
            slotManager.name = slotPrefab.name + index;
            return slotManager;
        }

        protected void UpdateAllSlots()
        {
            int count = slots.Count;
            for (int i = 0; i < count; i++)
                UpdateTargetSlot(i);
        }
        protected void UpdateTargetSlot(int slotIndex)
        {
            Slot slot = slots[slotIndex];
            StartingItem startingItem = default;
            int storedItemIndex = PlayerPrefs.GetInt(InventorySaveKey + SlotSaveKey + slotIndex, -1);
            if (storedItemIndex >= 0)
            {
                InventoryItem storedItem = ItemCollection[storedItemIndex];
                int startingItemIndex = StartingItems.FindIndex(a => a.Item == storedItem);
                int startingAmount = startingItemIndex >= 0 ? StartingItems[startingItemIndex].Amount : 0;
                int ownedAmount = storedItem.GetCustomSavedAmount(InventorySaveKey, startingAmount);
                if (ownedAmount > 0)
                    startingItem = new StartingItem(storedItem, startingAmount);
                else
                    storedItemIndex = -1;
            }
            slots[slotIndex] = new Slot(slot.SlotManager, slot.Index, startingItem, InventorySaveKey, storedItemIndex);
            OnSlotUpdated?.Invoke(this, slots[slotIndex].SlotManager, startingItem, slotIndex);
        }
        #endregion


        #region Inventories interactions
        private void SwitchOwnedSlots(Inventory inventoryManager, InventorySlot original, InventorySlot destination)
        {
            if (inventoryManager != this) return;

            Slot originSlot = slots.Find(a => a.SlotManager == original);
            Slot destinationSlot = slots.Find(a => a.SlotManager == destination);

            slots[originSlot.Index] = new Slot(originSlot.SlotManager, originSlot.Index, destinationSlot.StartingItem, InventorySaveKey, destinationSlot.ItemId);
            slots[destinationSlot.Index] = new Slot(destinationSlot.SlotManager, destinationSlot.Index, originSlot.StartingItem, InventorySaveKey, originSlot.ItemId);

            OnSlotUpdated?.Invoke(this, slots[originSlot.Index].SlotManager, slots[originSlot.Index].StartingItem, originSlot.Index);
            OnSlotUpdated?.Invoke(this, slots[destinationSlot.Index].SlotManager, slots[destinationSlot.Index].StartingItem, destinationSlot.Index);
        }
        private void HandleClothingEquiping(Inventory inventoryOrigin, Inventory inventoryDestination, InventoryItem itemOrigin, InventoryItem itemDest)
        {
            if (inventoryOrigin != this && inventoryDestination != this) return;

            bool isThisClothingInventory = this is ClothingInventory;
            bool isThisPlayerInventory = this is not ClothingInventory;
            if (inventoryOrigin == this)
            {
                LoseItemAmount(itemOrigin, 1);
                if (itemDest != null && isThisPlayerInventory)
                    GainItemAmount(itemDest, 1);
            }
            else
            {
                if (itemDest != null && isThisClothingInventory)
                    LoseItemAmount(itemDest, 1);

                GainItemAmount(itemOrigin, 1);
            }

        }

        private void CheckItemBought(Inventory shopIventory, InventoryItem item, int amount)
        {
            if (this == sessionService.CurrentClientInventory)
                GainItemAmount(item, amount);

            else if (this == sessionService.CurrentShopInventory)
                LoseItemAmount(item, amount);

        }
        private void CheckItemSold(Inventory shopIventory, InventoryItem item, int amount)
        {
            if (this == sessionService.CurrentShopInventory)
                GainItemAmount(item, amount);
            else if (this == sessionService.CurrentClientInventory)
                LoseItemAmount(item, amount);
        }

        protected virtual void GainItemAmount(InventoryItem item, int amount)
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
                StartingItem startingItem = new StartingItem(item);
                for (int i = 0; i < NumberOfSlots; i++)
                {
                    if (slots[i].IsEmpty)
                    {
                        Slot currentSlot = slots[i];
                        int storedItemIndex = ItemCollection.IndexOf(startingItem.Item);
                        slots[i] = new Slot(currentSlot.SlotManager, currentSlot.Index, startingItem, InventorySaveKey, storedItemIndex);
                        OnSlotUpdated?.Invoke(this, slots[i].SlotManager, startingItem, i);
                        break;
                    }
                }
            }
        }
        protected virtual void LoseItemAmount(InventoryItem item, int amount)
        {
            int slotIndex = slots.FindIndex(a => a.Item == item);
            int previousOwned = item.GetCustomSavedAmount(InventorySaveKey, 0);
            int newOwned = previousOwned - amount;
            item.SetCustomSavedAmount(InventorySaveKey, newOwned);
            UpdateTargetSlot(slotIndex);
        }
        #endregion


        public bool GetHasSpaceForTransaction(InventoryItem item)
        {
            int ownedIndex = slots.FindIndex(a => a.Item == item);
            if (ownedIndex >= 0)
                return true;

            foreach (Slot slot in slots)
                if (slot.IsEmpty)
                    return true;

            return false;
        }

        #region public structs
        [Serializable]
        public struct Slot
        {
            public StartingItem StartingItem;
            public InventorySlot SlotManager;
            public int Index;
            public int ItemId;
            public Slot(InventorySlot uiSlot, int slotIndex, StartingItem startingItem, string customSlotSaveKey, int itemIndex)
            {
                StartingItem = startingItem;
                SlotManager = uiSlot;
                Index = slotIndex;
                ItemId = itemIndex;

                PlayerPrefs.SetInt(customSlotSaveKey + SlotSaveKey + slotIndex, itemIndex);
            }

            public readonly InventoryItem Item => StartingItem.Item;
            public readonly bool IsEmpty => Item == null;
        }

        [Serializable]
        public struct StartingItem
        {
            public InventoryItem Item;
            public int Amount;

            public StartingItem(InventoryItem item, int startingAmount = 0)
            {
                Item = item;
                Amount = startingAmount;
            }
            public StartingItem(StartingItem copy)
            {
                Item = copy.Item;
                Amount = copy.Amount;
            }

            public bool IsValid => Item != null;
        }
        #endregion
    }
}
