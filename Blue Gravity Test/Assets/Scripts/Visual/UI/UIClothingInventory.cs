using Jega.BlueGravity;
using Jega.BlueGravity.InventorySystem;
using Jega.BlueGravity.PreWrittenCode;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIClothingInventory : MonoBehaviour
{
    [SerializeField] private GameObject headIndicator;
    [SerializeField] private GameObject bodyIndicator;
    [SerializeField] private Image headPreview;
    [SerializeField] private Image bodyPreview;

    private SessionService sessionService;
    private ClothingInventory ClothingInventory => sessionService.ClothingInventory;

    private void Awake()
    {
        sessionService = ServiceProvider.GetService<SessionService>();

        ClothingInventory.OnClothingInventoryUpdated += UpdateIndicators;
    }
    private void Start()
    {
        UpdateIndicators();
    }
    private void OnDestroy()
    {
        ClothingInventory.OnClothingInventoryUpdated -= UpdateIndicators;
    }
    void UpdateIndicators()
    {
        ClothingItem headItem = ClothingInventory.HeadItem;
        ClothingItem bodyItem = ClothingInventory.BodyItem;
        headIndicator.SetActive(headItem == null);
        bodyIndicator.SetActive(bodyItem == null);

        headPreview.enabled = headItem != null;
        bodyPreview.enabled = bodyItem != null;
        headPreview.sprite = headItem ? headItem.Icon : null;
        bodyPreview.sprite = bodyItem ? bodyItem.Icon : null;
    }
}
