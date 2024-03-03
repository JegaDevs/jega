using Jega.BlueGravity;
using Jega.BlueGravity.InventorySystem;
using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClothingInventory : MonoBehaviour
{
    [SerializeField] private GameObject headIndicator;
    [SerializeField] private GameObject bodyIndicator;

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
    }
}
