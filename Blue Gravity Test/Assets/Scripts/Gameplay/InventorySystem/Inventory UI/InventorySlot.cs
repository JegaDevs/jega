using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Jega.BlueGravity.PreWrittenCode;
using static Jega.BlueGravity.Inventory;
using System.Diagnostics;

namespace Jega.BlueGravity
{
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public static SlotSwitch OnRequestSlotSwitch;
        public delegate void SlotSwitch(Inventory inventory, InventorySlot original, InventorySlot destination);
        public delegate void ItemTransaction(Inventory inventory, InventoryItem item, int amount);

        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Vector2 draggingOffset;
        [Header("Shop interactions")]
        [SerializeField] private bool isShop;
        [SerializeField] private Image unAvailable;
        [SerializeField] private GameObject pricePopUp;
        [SerializeField] private GameObject notAffordableIndicador;
        [SerializeField] private TextMeshProUGUI priceText;

        private InventoryItem inventoryItem;
        private RectTransform iconTransform;
        private Vector2 originalPosition;
        private bool isEmpty;

        private SessionService sessionService;
        private Inventory inventoryManager;

        public Inventory InventoryManager => inventoryManager;
        private bool IsShopActive => sessionService.IsShopActive;
        private List<ShopInventory.ItemPrices> shopCatalog => sessionService.CurrentShopInventory.ShopCatalog;

        private void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>(); 
            iconTransform = iconImage.GetComponent<RectTransform>();
            originalPosition = iconTransform.anchoredPosition;
            unAvailable.gameObject.SetActive(false);
            pricePopUp.SetActive(false);
        }
        public void UpdateInfo(Inventory manager, Inventory.ItemPair itemPair, string customSaveKey)
        {
            isEmpty = true;
            iconTransform.anchoredPosition = originalPosition;
            textMesh.text = string.Empty;
            iconImage.gameObject.SetActive(itemPair.IsValid);
            if (itemPair.IsValid)
            {
                int itemAmount = itemPair.Item.GetCustomSavedAmount(customSaveKey, itemPair.StartingAmount);
                if(itemAmount > 0)
                {
                    iconImage.sprite = itemPair.Item.Icon;
                    textMesh.text = itemAmount.ToString();
                    isEmpty = false;
                }
            }

            inventoryItem = itemPair.Item;
            inventoryManager = manager;
        }

        public void UpdateAvailability()
        {
            if (!sessionService.IsShopActive || inventoryItem == null)
            {
                unAvailable.gameObject.SetActive(false);
                return;
            }
            int catalogIndex = shopCatalog.FindIndex(a => a.Item == inventoryItem);
            unAvailable.gameObject.SetActive(catalogIndex == -1);

            if (isShop)
            {
                int price = isShop ? shopCatalog[catalogIndex].BuyPrice : shopCatalog[catalogIndex].SellPrice;
                notAffordableIndicador.SetActive(sessionService.CurrentCoins < price);
            }
        }

        #region Draging Behavior
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isEmpty || IsShopActive) return;

            iconTransform.SetParent(inventoryManager.transform, false);
            iconTransform.SetAsLastSibling();
            textMesh.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isEmpty || IsShopActive) return;
            iconImage.transform.position = eventData.position + draggingOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isEmpty || IsShopActive) return;

            iconTransform.SetParent(transform, false);
            iconTransform.SetAsFirstSibling();
            textMesh.gameObject.SetActive(true);

            GameObject destination = eventData.pointerCurrentRaycast.gameObject;
            if (destination != null && destination.TryGetComponent(out InventorySlot newSlot) && newSlot != this)
                OnRequestSlotSwitch?.Invoke(inventoryManager, this, newSlot);
            else
                iconTransform.anchoredPosition = originalPosition;
        }

        #endregion

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isEmpty || !IsShopActive) return;
            int catalogIndex = shopCatalog.FindIndex(a => a.Item == inventoryItem);
            if(catalogIndex == -1) return;

            pricePopUp.gameObject.SetActive(true);
            int price = isShop ? shopCatalog[catalogIndex].BuyPrice : shopCatalog[catalogIndex].SellPrice;
            priceText.text = price.ToString();
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (pricePopUp.gameObject.activeSelf)
                pricePopUp.gameObject.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isEmpty || !IsShopActive) return;
            int catalogIndex = shopCatalog.FindIndex(a => a.Item == inventoryItem);
            if (catalogIndex == -1) return;

            int price = isShop ? shopCatalog[catalogIndex].BuyPrice : shopCatalog[catalogIndex].SellPrice;

            //if(isShop)
        }
    }
}
