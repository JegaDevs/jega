using Jega.BlueGravity.Services;
using Jega.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class CharacterController : MonoBehaviour
    {
        private InputService inputService;

        private void Awake()
        {
            inputService = ServiceProvider.GetService<InputService>();
        }

        private void Update()
        {
            var test = inputService.MovementVector;
        }
    }
}
