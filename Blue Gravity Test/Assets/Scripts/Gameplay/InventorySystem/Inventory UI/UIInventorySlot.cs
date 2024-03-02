using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;

namespace Jega.BlueGravity
{
    public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static SlotSwitch OnRequestSlotSwitch;
        public delegate void SlotSwitch(InventoryManager inventoryManager, UIInventorySlot original, UIInventorySlot destination);

        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Vector2 draggingOffset;

        private InventoryManager inventoryManager;
        private RectTransform iconTransform;
        private Vector2 originalPosition;
        private bool isEmpty;

        public InventoryManager InventoryManager => inventoryManager;

        private void Awake()
        {
            iconTransform = iconImage.GetComponent<RectTransform>();
            originalPosition = iconTransform.anchoredPosition;
        }
        public void UpdateInfo(InventoryManager manager, InventoryManager.ItemPair itemPair, string customSaveKey)
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

            inventoryManager = manager;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isEmpty) return;

            iconTransform.SetParent(inventoryManager.transform, false);
            iconTransform.SetAsLastSibling();
            textMesh.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isEmpty) return;
            iconImage.transform.position = eventData.position + draggingOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isEmpty) return;

            iconTransform.SetParent(transform, false);
            iconTransform.SetAsFirstSibling();
            textMesh.gameObject.SetActive(true);

            GameObject destination = eventData.pointerCurrentRaycast.gameObject;
            if (destination != null && destination.TryGetComponent(out UIInventorySlot newSlot) && newSlot != this)
                OnRequestSlotSwitch?.Invoke(inventoryManager, this, newSlot);
            else
                iconTransform.anchoredPosition = originalPosition;
        }
    }
}
