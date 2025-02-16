using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JegaCore
{
    public class JegaInputService : IService
    {
        public static Action OnInventoryInputPressed;

        private InputData inputData;
        private Vector2 movementVector;

        public int Priority => 0;
        public Vector2 MovementVector => inputData.CurrentMovementVector;
        public Vector2 MovementVectorNormalized => MovementVector.normalized;
        public void Preprocess()
        {
            inputData = StaticPaths.LoadScriptableOrCreateIfMissing<InputData>("InputData");
            inputData.InitializeInputActions();
            GlobalMonoBehaviour.RegisterUpdateMethod(GetMovementVector, UnityUpdateMethod.Update);
            //inputData.OpenInventory.action.performed += RegisterInventoryInput;
        }

        public void Postprocess()
        {
            //inputData.OpenInventory.action.performed -= RegisterInventoryInput;
            inputData.UnInitializeInputActions();
        }

        private void GetMovementVector()
        {
            movementVector.x = inputData.HorizontalMovement.action.ReadValue<float>();
            movementVector.y = inputData.VerticalMovement.action.ReadValue<float>();
            inputData.SetNewMovementVector(movementVector);
        }
        
        /*void RegisterInventoryInput(InputAction.CallbackContext obj)
        {
            OnInventoryInputPressed?.Invoke();
        }*/


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInitializeService()
        {
            ServiceProvider.GetService<JegaInputService>();
        }
    }
}
