using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity.InventorySystem
{
    [CreateAssetMenu(fileName = "Inventory Data", menuName = "BlueGravity/InventoryData")]
    public class InventoryData : ScriptableObject
    {
        [Header("Game Design Params")]
        public List<Inventory.ItemPair> startingItems;
        public int numberOfSlots;

        [Header("Programming Params")]
        [SerializeField] public InventoryItemCollection itemCollection;
        [SerializeField] public string inventorySaveKey;
    }
}
