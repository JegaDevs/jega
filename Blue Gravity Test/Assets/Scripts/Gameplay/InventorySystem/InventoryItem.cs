using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public abstract class InventoryItem : ScriptableObject
    {
        public Sprite Icon;

        public int GetCustomSavedAmount(string customKey, int startingAmount = 0)
        {
            return PlayerPrefs.GetInt(customKey + name + "_ItemAmount", startingAmount);
        }
        public void SetCustomSavedAmount(string customKey, int startingAmount = 0)
        {
            PlayerPrefs.SetInt(customKey + name + "_ItemAmount", startingAmount);
        }

    }
}
