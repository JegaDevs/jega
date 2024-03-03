using System;
using UnityEngine;

namespace Jega.BlueGravity.InventorySystem
{
    [CreateAssetMenu(fileName = "ClothingItem", menuName = "BlueGravity/ClothingItem")]
    public class ClothingItem : InventoryItem
    {
        public ClothingType Type;
        public AnimatorOverrideController AnimatorController;

        public enum ClothingType : byte { Body, Head }
    }
}
