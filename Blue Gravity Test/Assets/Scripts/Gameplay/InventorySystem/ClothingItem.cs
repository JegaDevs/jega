using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Jega.BlueGravity
{
    [CreateAssetMenu(fileName = "ClothingItem", menuName = "BlueGravity/ClothingItem")]
    public class ClothingItem : InventoryItem
    {
        public ClothingType Type;
        public AnimatorOverrideController AnimatorController;

        public enum ClothingType : byte { Body, Head }
    }
}
