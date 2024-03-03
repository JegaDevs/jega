using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Jega.BlueGravity.InventorySystem
{
    [CreateAssetMenu(fileName = "Item Colection", menuName = "BlueGravity/ItemCollection")]
    public class InventoryItemCollection : ScriptableObject
    {
        [SerializeField] private List<InventoryItem> collection;

        public ReadOnlyCollection<InventoryItem> Collection => collection.AsReadOnly();
    }
}
