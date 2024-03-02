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

        private SessionService sessionService;

        public List<ItemPrices> ShopCatalog => new List<ItemPrices>(shopCatalog);

        protected override void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();
            base.Awake();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            sessionService.RegisterActiveShopInventory(this);
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
