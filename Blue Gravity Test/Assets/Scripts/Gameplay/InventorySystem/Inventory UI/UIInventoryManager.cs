using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class UIInventoryManager : MonoBehaviour
    {
        [Header("Game Design Params")]
        [SerializeField] private List<ItemPair> allowedItems;
        [SerializeField] private int numberOfSlots;

        [Header("Programming Params")]
        [SerializeField] private string inventorySaveKey;
        [SerializeField] private Transform slotsParent;
        [SerializeField] private UIInventorySlot slotPrefab;

        [SerializeField] private List<Slot> slots;

        private const string slotSaveKey = "_Slot_";
        private void Awake()
        {
            InitialInvetorySetup();
        }

        private void InitialInvetorySetup()
        {
            slots = new List<Slot>();
            List<ItemPair> unfilledItemPairs = new List<ItemPair>();
            foreach(ItemPair itemPair in allowedItems)
                if(itemPair.Item.GetCustomSavedAmount(inventorySaveKey, itemPair.InitialAmount) > 0)
                    unfilledItemPairs.Add(itemPair);

            //Fill saved slots
            for (int i = 0; i < numberOfSlots; i++)
            {
                ItemPair currentSlotItemPair = default;
                int storedItemIndex = PlayerPrefs.GetInt(inventorySaveKey + slotSaveKey + i, -1);
                if (storedItemIndex >= 0 && allowedItems[storedItemIndex].Item.GetCustomSavedAmount(inventorySaveKey) > 0)
                {
                    currentSlotItemPair = new ItemPair(allowedItems[storedItemIndex]);
                    unfilledItemPairs.Remove(currentSlotItemPair);
                }
                UIInventorySlot slotSetup = CreateNewSlot(currentSlotItemPair);
                slots.Add(new Slot(slotSetup, i, currentSlotItemPair));
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
                        slots[i] = new Slot(currentSlot.UISlot, currentSlot.Index, itemPair);
                        slots[i].UISlot.UpdateInfo(itemPair, inventorySaveKey);
                        break;
                    }
                }
                Debug.Log("Filled unsaved slots");
            }
        }

        private UIInventorySlot CreateNewSlot(ItemPair itemPair)
        {
            UIInventorySlot uiSlot = Instantiate(slotPrefab, slotsParent);
            uiSlot.UpdateInfo(itemPair, inventorySaveKey);
            return uiSlot;
        }

        [Serializable]
        public struct Slot
        {
            public ItemPair ItemPair;
            public UIInventorySlot UISlot;
            public int Index;
            public Slot(UIInventorySlot uiSlot, int index, ItemPair itemPair = default, string customSlotSaveKey = default)
            {
                ItemPair = itemPair;
                UISlot = uiSlot;
                Index = index;

                if(itemPair.Item != null && ItemPair.Item.GetCustomSavedAmount(customSlotSaveKey, ItemPair.InitialAmount) > 0)
                    PlayerPrefs.SetInt(customSlotSaveKey + slotSaveKey + index, -1);
            }

            public InventoryItem Item => ItemPair.Item;
            public bool IsEmpty => Item == null;
        }

        [Serializable]
        public struct ItemPair
        {
            public InventoryItem Item;
            public int InitialAmount;

            public ItemPair(ItemPair copy)
            {
                Item = copy.Item;
                InitialAmount = copy.InitialAmount;
            }

            public bool IsValid => Item != null;
        }
    }
}
