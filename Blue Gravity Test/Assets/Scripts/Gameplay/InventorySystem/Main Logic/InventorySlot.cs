using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using static Jega.BlueGravity.InventorySystem.Inventory;

namespace Jega.BlueGravity.InventorySystem
{
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public static SlotSwitch OnRequestOwnedSlotsSwitch;
        public static SlotSwitchInventories OnRequestClothingInventorySwitch;
        public static ItemTransaction OnItemBought;
        public static ItemTransaction OnItemSold;
        public delegate void SlotSwitch(Inventory inventory, InventorySlot slotOrigin, InventorySlot slotDestination);
        public delegate void SlotSwitchInventories(Inventory inventoryOrigin, Inventory inventoryDestination, InventoryItem itemOrigin, InventoryItem itemDest);
        public delegate void ItemTransaction(Inventory shopInventory, InventoryItem item, int amount);

        public Action OnSlotUpdated;
        public Action OnRequestAvailabilityCheck;
        public Action<PointerEventData> OnStartDrag;
        public Action<PointerEventData> OnStayDrag;
        public Action<bool> OnExitDrag;
        public Action OnPointerEnterEvent;
        public Action OnPointerExitEvent;

        [SerializeField] private bool isShop;

        private bool isEmpty;
        private int itemAmount;
        private bool isDragging;

        private int slotIndex;
        private Inventory inventoryManager;
        private InventoryItem inventoryItem;
        private SessionService sessionService;

        public Inventory InventoryManager => inventoryManager;
        public InventoryItem InventoryItem => inventoryItem;
        public int ItemAmount => itemAmount;
        public bool IsHeadItem => inventoryItem is ClothingItem clothingItem && clothingItem.Type == ClothingItem.ClothingType.Head;
        public bool IsShop => isShop;
        private bool IsShopActive => sessionService.IsShopActive;
        private List<ShopInventory.ItemPrices> shopCatalog => sessionService.CurrentShopInventory.ShopCatalog;


        private void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();
        }
        private void OnEnable()
        {
            if (isDragging)
                OnExitDrag?.Invoke(false);
        }
        private void OnDestroy()
        {
            if (inventoryManager)
            {
                inventoryManager.OnSlotUpdated -= UpdateInfo;
            }
        }

        public void RegisterManager(Inventory inventory)
        {
            Assert.IsTrue(inventory != null);

            inventoryManager = inventory;
            inventoryManager.OnSlotUpdated += UpdateInfo;
        }
        public void UpdateInfo(Inventory inventory, InventorySlot slot, StartingItem startingItem, int slotIndex)
        {
            if (slot != this) return;

            itemAmount = startingItem.IsValid ? startingItem.Item.GetCustomSavedAmount(inventory.InventorySaveKey, startingItem.Amount) : 0;
            isEmpty = itemAmount <= 0;
            inventoryItem = startingItem.Item;
            inventoryManager = inventory;
            this.slotIndex = slotIndex;
            OnSlotUpdated?.Invoke();
        }

        public void UpdateAvailability()
        {
            OnRequestAvailabilityCheck?.Invoke();
        }

        #region Draging Behavior
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isEmpty || IsShopActive) return;
            isDragging = true;
            OnStartDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isEmpty || IsShopActive) return;
            OnStayDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isEmpty || IsShopActive) return;
            isDragging = false;
            GameObject destination = eventData.pointerCurrentRaycast.gameObject;
            HandleSlotSwapInteractions(destination);
        }

        private void HandleSlotSwapInteractions(GameObject destination)
        {
            bool sucess = false;
            if (destination != null && destination.TryGetComponent(out InventorySlot newSlot) && newSlot != this)
            {
                bool isOriginClothingInventory = inventoryManager is ClothingInventory;
                bool isDestinationClothingInventory = newSlot.inventoryManager is ClothingInventory;
                ClothingInventory destClothingInventory = newSlot.inventoryManager as ClothingInventory;

                if (!isOriginClothingInventory && inventoryManager == newSlot.inventoryManager)
                    OnRequestOwnedSlotsSwitch?.Invoke(inventoryManager, this, newSlot);
                else
                {
                    if (!isOriginClothingInventory && isDestinationClothingInventory &&
                         destClothingInventory.CheckIfSwitchIsValid(inventoryItem, newSlot.slotIndex))
                    {
                        OnRequestClothingInventorySwitch(inventoryManager, destClothingInventory, InventoryItem, newSlot.inventoryItem);
                        sucess = true;

                    }
                    else if (isOriginClothingInventory && !isDestinationClothingInventory)
                    {
                        OnRequestClothingInventorySwitch(inventoryManager, newSlot.inventoryManager, InventoryItem, newSlot.inventoryItem);
                        sucess = true;
                    }
                }
            }

            OnExitDrag?.Invoke(sucess);
        }

        #endregion

        #region Shop Interactions
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isEmpty || !IsShopActive) return;
            int catalogIndex = shopCatalog.FindIndex(a => a.Item == inventoryItem);
            if (catalogIndex == -1) return;

            OnPointerEnterEvent?.Invoke();

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isEmpty || !IsShopActive) return;
            int catalogIndex = shopCatalog.FindIndex(a => a.Item == inventoryItem);
            if (catalogIndex == -1) return;

            int price = isShop ? shopCatalog[catalogIndex].BuyPrice : shopCatalog[catalogIndex].SellPrice;
            InventoryItem item = inventoryItem;

            if (isShop)
            {
                if (!sessionService.CurrentClientInventory.GetHasSpaceForTransaction(item))
                    return;

                if (sessionService.CurrentCoins >= price)
                {
                    sessionService.CurrentCoins -= price;
                    OnItemBought?.Invoke(inventoryManager, item, 1);
                }
            }
            else
            {
                if (!inventoryManager.GetHasSpaceForTransaction(item))
                    return;

                sessionService.CurrentCoins += price;
                OnItemSold?.Invoke(inventoryManager, item, 1);
            }
            if (isEmpty)
                OnPointerExitEvent?.Invoke();
        }
        #endregion
    }
}
