using Jega.BlueGravity.InventorySystem;
using Jega.BlueGravity.PreWrittenCode;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Jega.BlueGravity
{
    [RequireComponent(typeof(InventorySlot))]
    public class UIInventorySlotVisual : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Vector2 draggingOffset;
        [SerializeField] private Vector2 headDraggingOffset;
        [SerializeField] private Vector2 headOriginalPositionOffset;

        [Header("Shop interactions")]
        [SerializeField] private Image unAvailable;
        [SerializeField] private GameObject pricePopUp;
        [SerializeField] private GameObject notAffordableIndicador;
        [SerializeField] private TextMeshProUGUI priceText;

        private InventorySlot inventorySlot;
        private RectTransform iconTransform;
        private Vector2 originalPosition;

        private SessionService sessionService;

        private InventoryItem InventoryItem => inventorySlot.InventoryItem;
        private List<ShopInventory.ItemPrices> ShopCatalog => sessionService.CurrentShopInventory.ShopCatalog;
        private bool IsShop => inventorySlot.IsShop;

        private void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();
            inventorySlot = GetComponent<InventorySlot>();
            iconTransform = iconImage.GetComponent<RectTransform>();
            originalPosition = iconTransform.anchoredPosition;

            unAvailable.gameObject.SetActive(false);
            pricePopUp.SetActive(false);

            inventorySlot.OnSlotUpdated += UpdateSlot;
            inventorySlot.OnSlotUpdated += UpdateAvailability;
            sessionService.OnCoinsUpdate += UpdateAvailability;

            inventorySlot.OnStartDrag += StartDrag;
            inventorySlot.OnStayDrag += StayDrag;
            inventorySlot.OnExitDrag += ExitDrag;
            inventorySlot.OnPointerEnterEvent += OnEnterPointer;
            inventorySlot.OnPointerExitEvent += OnExitPointer;
        }
        private void OnDestroy()
        {
            inventorySlot.OnSlotUpdated -= UpdateSlot;
            inventorySlot.OnSlotUpdated -= UpdateAvailability;
            sessionService.OnCoinsUpdate -= UpdateAvailability;

            inventorySlot.OnStartDrag -= StartDrag;
            inventorySlot.OnStayDrag -= StayDrag;
            inventorySlot.OnExitDrag -= ExitDrag;
            inventorySlot.OnPointerEnterEvent -= OnEnterPointer;
            inventorySlot.OnPointerExitEvent -= OnExitPointer;
        }

        private void OnDisable()
        {
            if (pricePopUp.gameObject.activeSelf)
                pricePopUp.gameObject.gameObject.SetActive(false);
        }

        private void UpdateSlot()
        {
            textMesh.text = string.Empty;
            iconImage.gameObject.SetActive(InventoryItem);

            if (InventoryItem != null)
            {
                if (inventorySlot.ItemAmount > 0)
                {
                    iconImage.sprite = InventoryItem.Icon;
                    textMesh.text = inventorySlot.ItemAmount.ToString();
                }
            }
            ResetIconPosition();
        }

        private void UpdateAvailability()
        {
            if (!sessionService.IsShopActive || InventoryItem == null)
            {
                unAvailable.gameObject.SetActive(false);
                return;
            }

            int catalogIndex = ShopCatalog.FindIndex(a => a.Item == InventoryItem);
            unAvailable.gameObject.SetActive(catalogIndex == -1);

            if (IsShop && catalogIndex >= 0)
            {
                int price = IsShop ? ShopCatalog[catalogIndex].BuyPrice : ShopCatalog[catalogIndex].SellPrice;
                notAffordableIndicador.SetActive(sessionService.CurrentCoins < price);
            }
        }
        private void ResetIconPosition()
        {
            Vector2 offset = Vector2.zero;
            if (inventorySlot.IsHeadItem)
                offset = headOriginalPositionOffset;

            iconTransform.anchoredPosition = originalPosition + offset;
        }


        private void StartDrag(PointerEventData eventData)
        {
            iconTransform.SetParent(inventorySlot.InventoryManager.transform.parent, false);
            iconTransform.SetAsLastSibling();
            textMesh.gameObject.SetActive(false);
        }
        private void StayDrag(PointerEventData eventData)
        {
            iconTransform.position = eventData.position;
            iconTransform.anchoredPosition += draggingOffset;
            iconTransform.anchoredPosition += inventorySlot.IsHeadItem ? headDraggingOffset : Vector3.zero;
        }
        private void ExitDrag(bool sucess)
        {
            iconTransform.SetParent(transform, false);
            iconTransform.SetAsFirstSibling();
            textMesh.gameObject.SetActive(true);
            if (!sucess)
                ResetIconPosition();
        }


        private void OnEnterPointer()
        {
            int catalogIndex = ShopCatalog.FindIndex(a => a.Item == InventoryItem);
            pricePopUp.gameObject.SetActive(true);
            int price = inventorySlot.IsShop ? ShopCatalog[catalogIndex].BuyPrice : ShopCatalog[catalogIndex].SellPrice;
            priceText.text = price.ToString();
        }
        private void OnExitPointer()
        {
            if (pricePopUp.gameObject.activeSelf)
                pricePopUp.gameObject.gameObject.SetActive(false);
        }
    }

}
