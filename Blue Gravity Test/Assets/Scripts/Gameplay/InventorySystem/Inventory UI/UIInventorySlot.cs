using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jega.BlueGravity
{
    public class UIInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI textMesh;


        public void UpdateInfo(UIInventoryManager.ItemPair itemPair, string customSaveKey)
        {
            iconImage.gameObject.SetActive(itemPair.IsValid);
            textMesh.text = string.Empty;
            if (itemPair.IsValid)
            {
                iconImage.sprite = itemPair.Item.Icon;
                textMesh.text = itemPair.Item.GetCustomSavedAmount(customSaveKey, itemPair.StartingAmount).ToString();
            }
        }
    }
}
