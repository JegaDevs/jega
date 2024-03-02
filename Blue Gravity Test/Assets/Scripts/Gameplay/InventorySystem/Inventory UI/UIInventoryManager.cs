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
                ItemPair itemPair = default;
                int storedItemIndex = PlayerPrefs.GetInt(inventorySaveKey + slotSaveKey + i, -1);
                if (storedItemIndex >= 0 && allowedItems[storedItemIndex].Item.GetCustomSavedAmount(inventorySaveKey, allowedItems[storedItemIndex].InitialAmount) > 0)
                {
                    ItemPair allowedPair = allowedItems[storedItemIndex];
                    if(allowedPair.Item.GetCustomSavedAmount(inventorySaveKey, allowedPair.InitialAmount) > 0)
                    {
                        itemPair = new ItemPair(allowedItems[storedItemIndex]);
                        unfilledItemPairs.Remove(itemPair);
                    }
                }
                UIInventorySlot slotSetup = CreateNewSlot(itemPair);
                int itemIndex = allowedItems.FindIndex(a => a.Item == itemPair.Item);
                slots.Add(new Slot(slotSetup, i, itemPair, inventorySaveKey, itemIndex));
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
                        int itemIndex = allowedItems.FindIndex(a => a.Item == itemPair.Item);
                        slots[i] = new Slot(currentSlot.UISlot, currentSlot.Index, itemPair, inventorySaveKey, itemIndex);
                        slots[i].UISlot.UpdateInfo(itemPair, inventorySaveKey);
                        break;
                    }
                }
            }
            Debug.Log("Filled unsaved slots. \n Attention! This should happen only once when there's no saved data!");
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
            public Slot(UIInventorySlot uiSlot, int slotIndex, ItemPair itemPair, string customSlotSaveKey, int itemIndex = -1)
            {
                ItemPair = itemPair;
                UISlot = uiSlot;
                Index = slotIndex;

                if(itemPair.Item != null && ItemPair.Item.GetCustomSavedAmount(customSlotSaveKey, ItemPair.InitialAmount) > 0)
                    PlayerPrefs.SetInt(customSlotSaveKey + slotSaveKey + slotIndex, itemIndex);
                
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
