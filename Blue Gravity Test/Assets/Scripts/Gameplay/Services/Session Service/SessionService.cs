using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class SessionService : IService
    {
        public Action OnCoinsUpdate;
        public int Priority => 0;
        private ShopInventory currentShopInventory;
        private ClientInventory currentClientInventory;
        private ClothingInventory clothingInventory;

        public ShopInventory CurrentShopInventory => currentShopInventory;
        public ClientInventory CurrentClientInventory => currentClientInventory;
        public ClothingInventory ClothingInventory => clothingInventory;
        public bool IsShopActive => currentShopInventory != null;

        public int CurrentCoins
        {
            get => PlayerPrefs.GetInt("CurrentPlayerCoins", 0);
            set
            {
                PlayerPrefs.SetInt("CurrentPlayerCoins", value);
                OnCoinsUpdate?.Invoke();
            }
        }
        public void Preprocess()
        {
        }
        public void Postprocess()
        {
        }


        public void RegisterActiveShopInventory(ShopInventory shopInventory) 
        {
            currentShopInventory = shopInventory;
        }

        public void UnregisterActiveShopInventory()
        {
            currentShopInventory = null;
        }
        public void RegisterActiveClientInventory(ClientInventory clientInventory)
        {
            currentClientInventory = clientInventory;
        }

        public void UnregisterClientShopInventory()
        {
            currentClientInventory = null;
        }

        public void RegisterClothingInventory (ClothingInventory inventory)
        {
            clothingInventory = inventory;
        }

    }
}
