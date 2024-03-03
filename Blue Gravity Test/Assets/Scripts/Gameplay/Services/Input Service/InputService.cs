using Jega.BlueGravity.PreWrittenCode;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jega.BlueGravity.Services
{
    public class InputService : IService
    {
        public static Action OnInventoryInputPressed;

        private InputServiceData serviceData;
        private Vector2 movementVector;
        private Vector2 lastPerformedInput;

        public int Priority => 0;
        public Vector2 MovementVector => GetMovementVector();
        public Vector2 LastPerformedInput => lastPerformedInput;
        public void Preprocess()
        {
            serviceData = StaticPaths.LoadScriptableOrCreateIfMissing<InputServiceData>("InputServiceData");
            serviceData.InitializeInputActions();
            serviceData.OpenInventory.action.performed += RegisterInventoryInput;
        }

        public void Postprocess()
        {
            serviceData.OpenInventory.action.performed -= RegisterInventoryInput;
            serviceData.UnInitializeInputActions();
        }

        private Vector2 GetMovementVector()
        {
            movementVector.x = serviceData.HorizontalMovement.action.ReadValue<float>();
            movementVector.y = serviceData.VerticalMovement.action.ReadValue<float>();
            if (movementVector.sqrMagnitude > 0)
                lastPerformedInput = movementVector;
            return movementVector;
        }
        void RegisterInventoryInput(InputAction.CallbackContext obj)
        {
            OnInventoryInputPressed?.Invoke();
        }



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInitializeService()
        {
            ServiceProvider.GetService<InputService>();
        }

    }
}
