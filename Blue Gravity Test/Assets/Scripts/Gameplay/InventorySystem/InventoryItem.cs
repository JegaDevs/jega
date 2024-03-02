using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public abstract class InventoryItem : ScriptableObject
    {
        public Sprite Icon;
        public int StartingAmount;

        public int CurrentAmount
        {
            get => PlayerPrefs.GetInt(name + "_ItemAmount", StartingAmount);
            set => PlayerPrefs.SetInt(name + "_ItemAmount", StartingAmount);
        }

    }
}
