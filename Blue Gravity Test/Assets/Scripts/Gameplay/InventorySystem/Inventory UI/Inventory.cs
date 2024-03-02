using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class Inventory : MonoBehaviour
    {
        [Header("Game Design Params")]
        [SerializeField] private List<ItemPair> startingItems;
        [SerializeField] private int numberOfSlots;

        [Header("Programming Params")]
        [SerializeField] private InventoryItemCollection itemCollection;
        [SerializeField] private string inventorySaveKey;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private UIInventorySlot slotPrefab;

        [SerializeField] private List<Slot> slots;

        private const string slotSaveKey = "_Slot_";
        protected virtual void Awake()
        {
            InitialInvetorySetup();
            UIInventorySlot.OnRequestSlotSwitch += SwitchSlots;
        }
        private void OnDestroy()
        {
            UIInventorySlot.OnRequestSlotSwitch -= SwitchSlots;
        }

        #region initial Setup
        private void InitialInvetorySetup()
        {
            slots = new List<Slot>();
            List<ItemPair> unfilledItemPairs = new List<ItemPair>();
            foreach(ItemPair itemPair in startingItems)
                if(itemPair.Item.GetCustomSavedAmount(inventorySaveKey, itemPair.StartingAmount) > 0)
                    unfilledItemPairs.Add(itemPair);

            //Fill saved slots
            for (int i = 0; i < numberOfSlots; i++)
            {
                ItemPair itemPair = default;
                int storedItemIndex = PlayerPrefs.GetInt(inventorySaveKey + slotSaveKey + i, -1);
                if (storedItemIndex >= 0)
                {
                    InventoryItem storedItem = itemCollection.Collection[storedItemIndex];
                    int startingItemIndex = startingItems.FindIndex(a => a.Item == storedItem);
                    int startingAmount = startingItemIndex >= 0 ? startingItems[startingItemIndex].StartingAmount : 0;
                    if (storedItem.GetCustomSavedAmount(inventorySaveKey, startingAmount) > 0)
                    {
                        itemPair = new ItemPair(storedItem, startingAmount);
                        unfilledItemPairs.Remove(itemPair);
                    }
                }
                UIInventorySlot slotSetup = CreateNewSlot(itemPair, i);
                slots.Add(new Slot(slotSetup, i, itemPair, inventorySaveKey, storedItemIndex));
            }


            if (unfilledItemPairs.Count <= 0) 
                return;

            //Fill unsaved slots (should happen only once to fill initial values on a clear save)
            foreach (ItemPair itemPair in unfilledItemPairs)
            {
                for (int i = 0; i < numberOfSlots; i++)
                {
                    Slot currentSlot = slots[i];
                    if (slots[i].IsEmpty)
                    {
                        itemPair.Item.SetCustomSavedAmount(inventorySaveKey, itemPair.StartingAmount);
                        int storedItemIndex = itemCollection.Collection.IndexOf(itemPair.Item);
                        slots[i] = new Slot(currentSlot.UISlot, currentSlot.Index, itemPair, inventorySaveKey, storedItemIndex);
                        UpdateSlotVisual(slots[i].UISlot, itemPair);
                        break;
                    }
                }
            }
            Debug.LogError("Filled unsaved slots. \n Attention! This should happen only once when there's no saved data!");
        }

        private UIInventorySlot CreateNewSlot(ItemPair itemPair, int index)
        {
            UIInventorySlot uiSlot = Instantiate(slotPrefab, slotsParent);
            UpdateSlotVisual(uiSlot, itemPair);
            uiSlot.name = slotPrefab.name + index;
            return uiSlot;
        }
        #endregion

        void SwitchSlots(Inventory inventoryManager, UIInventorySlot original, UIInventorySlot destination)
        {
            if (inventoryManager != this) return;

            Slot originSlot = slots.Find(a => a.UISlot == original);
            Slot destinationSlot = slots.Find(a => a.UISlot == destination);

            slots[originSlot.Index] = new Slot(originSlot.UISlot, originSlot.Index, destinationSlot.ItemPair, inventorySaveKey, destinationSlot.ItemIndex);
            slots[destinationSlot.Index] = new Slot(destinationSlot.UISlot, destinationSlot.Index, originSlot.ItemPair, inventorySaveKey, originSlot.ItemIndex);

            UpdateSlotVisual(original, slots[originSlot.Index].ItemPair);
            UpdateSlotVisual(destination, slots[destinationSlot.Index].ItemPair);
        }

        protected virtual void UpdateSlotVisual(UIInventorySlot slotVisual, ItemPair itemPair)
        {
            slotVisual.UpdateInfo(this, itemPair, inventorySaveKey);
        }

        #region public straucks
        [Serializable]
        public struct Slot
        {
            public ItemPair ItemPair;
            public UIInventorySlot UISlot;
            public int Index;
            public int ItemIndex;
            public Slot(UIInventorySlot uiSlot, int slotIndex, ItemPair itemPair, string customSlotSaveKey, int itemIndex)
            {
                ItemPair = itemPair;
                UISlot = uiSlot;
                Index = slotIndex;
                ItemIndex = itemIndex;

                PlayerPrefs.SetInt(customSlotSaveKey + slotSaveKey + slotIndex, itemIndex);
                
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
