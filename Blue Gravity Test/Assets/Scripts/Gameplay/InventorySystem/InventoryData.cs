using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Jega.BlueGravity.Inventory;

namespace Jega.BlueGravity
{
    [CreateAssetMenu(fileName = "Inventory Data", menuName = "BlueGravity/InventoryData")]
    public class InventoryData : ScriptableObject
    {
        [Header("Game Design Params")]
        public List<ItemPair> startingItems;
        public int numberOfSlots;

        [Header("Programming Params")]
        [SerializeField] public InventoryItemCollection itemCollection;
        [SerializeField] public string inventorySaveKey;
    }
}
