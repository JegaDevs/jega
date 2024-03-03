using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity.InventorySystem
{
    [DefaultExecutionOrder(-1)]
    public class ShopInventory : Inventory
    {
        [SerializeField] private List<ItemPrices> shopCatalog;

        public List<ItemPrices> ShopCatalog => new List<ItemPrices>(shopCatalog);

        protected override void OnEnable()
        {
            sessionService.RegisterActiveShopInventory(this);
            base.OnEnable();
        }
        private void OnDisable()
        {
            sessionService.UnregisterActiveShopInventory();
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
