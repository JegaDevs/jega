using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity.InventorySystem
{
    public class ShopInventory : Inventory
    {
        [SerializeField] private List<ItemPrices> shopCatalog;

        public List<ItemPrices> ShopCatalog => new List<ItemPrices>(shopCatalog);

        protected override void Awake()
        {
            base.Awake();
            sessionService.OnCoinsUpdate += UpdateAllSlotsVisuals;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            sessionService.OnCoinsUpdate -= UpdateAllSlotsVisuals;
        }
        protected override void OnEnable()
        {
            sessionService.RegisterActiveShopInventory(this);
            base.OnEnable();
        }
        private void OnDisable()
        {
            sessionService.UnregisterActiveShopInventory();
        }


        protected override void UpdateSlotVisual(InventorySlot slotVisual, ItemPair itemPair, int slotIndex)
        {
            base.UpdateSlotVisual(slotVisual, itemPair, slotIndex);
            slotVisual.UpdateAvailability();
        }

        [Serializable]
        public struct ItemPrices
        {
            public InventoryItem Item;
            public int BuyPrice;
            public int SellPrice;
        }
    }
}
