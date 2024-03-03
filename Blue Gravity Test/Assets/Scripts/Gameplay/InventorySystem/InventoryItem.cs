using UnityEngine;

namespace Jega.BlueGravity.InventorySystem
{
    public abstract class InventoryItem : ScriptableObject
    {
        public Sprite Icon;

        public int GetCustomSavedAmount(string customKey, int startingAmount)
        {
            return PlayerPrefs.GetInt(customKey + name + "_ItemAmount", startingAmount);
        }
        public void SetCustomSavedAmount(string customKey, int startingAmount)
        {
            PlayerPrefs.SetInt(customKey + name + "_ItemAmount", startingAmount);
        }

    }
}
