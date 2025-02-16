using Jega.BlueGravity.PreWrittenCode;
using Jega.BlueGravity.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JegaCore;

namespace Jega.BlueGravity
{
    public class FirstInputDisplay : MonoBehaviour
    {
        private const string firstInputSaveKey = "firstInputKey";

        private InputService inputService;

        private void Awake()
        {
            if (PlayerPrefs.HasKey(firstInputSaveKey))
                gameObject.SetActive(false);
            inputService = ServiceProvider.GetService<InputService>();
        }

        private void Update()
        {
            if(inputService.MovementVector.sqrMagnitude > 0)
            {
                PlayerPrefs.SetInt(firstInputSaveKey, 1);
                gameObject.SetActive(false);
            }
        }
    }
}
