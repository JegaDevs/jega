using Jega.BlueGravity.InventorySystem;
using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sellItemAudio;
        [SerializeField] private AudioSource buyItemAudio;
        [SerializeField] private AudioSource equipSkinItemAudio;

        private void Awake()
        {
            InventorySlot.OnItemSold += PlaySellSound;
            InventorySlot.OnItemBought += PlayBuySound;
            ClothingInventory.OnClothingInventoryUpdated += PlayEquipSkinAudio;
        }
        void PlayBuySound(Inventory shopInventory, InventoryItem item, int amount)
        {
            PlaySound(buyItemAudio);
        }
        void PlaySellSound(Inventory shopInventory, InventoryItem item, int amount)
        {
            PlaySound(sellItemAudio);
        }
        void PlayEquipSkinAudio()
        {
            PlaySound(equipSkinItemAudio);
        }

        private void PlaySound(AudioSource source)
        {
            source.Stop();
            source.Play();
        }

    }
}
