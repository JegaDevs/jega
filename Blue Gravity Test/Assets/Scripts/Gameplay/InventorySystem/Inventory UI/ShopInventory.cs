using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class ShopInventory : Inventory
    {
        [SerializeField] private List<ItemPrices> shopCatalog;

        public List<ItemPrices> ShopCatalog => new List<ItemPrices>(shopCatalog);

        protected override void Awake()
        {
            base.Awake();
            sessionService.OnCoinsUpdate += UpdateSlotsVisuals;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            sessionService.OnCoinsUpdate -= UpdateSlotsVisuals;
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


        protected override void UpdateSlotVisual(InventorySlot slotVisual, ItemPair itemPair)
        {
            base.UpdateSlotVisual(slotVisual, itemPair);
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
