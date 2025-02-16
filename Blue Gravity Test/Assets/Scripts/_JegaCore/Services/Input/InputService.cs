using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JegaCore
{
    public class InputService : IService
    {
        private Vector2 movementVector;

        public int Priority => 0;
        
        public CoreInputData CoreInputData { get; private set; }

        public Vector2 MovementVector => CoreInputData.CurrentMovementVector;
        public Vector2 MovementVectorNormalized => MovementVector.normalized;
        public static InputService Service => ServiceProvider.GetService<InputService>();
        
        public void Preprocess()
        {
            CoreInputData = StaticPaths.LoadScriptableOrCreateIfMissing<CoreInputData>("CoreInputData");
            CoreInputData.InitializeInputActions();
            GlobalMonoBehaviour.RegisterUpdateMethod(GetMovementVector, UnityUpdateMethod.Update);
        }

        public void Postprocess()
        {
            CoreInputData.UnInitializeInputActions();
        }

        private void GetMovementVector()
        {
            movementVector.x = CoreInputData.HorizontalMovement.action.ReadValue<float>();
            movementVector.y = CoreInputData.VerticalMovement.action.ReadValue<float>();
            CoreInputData.SetNewMovementVector(movementVector);
        }
        
        /*void RegisterInventoryInput(InputAction.CallbackContext obj)
        {
            OnInventoryInputPressed?.Invoke();
        }*/


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInitializeService()
        {
            ServiceProvider.GetService<InputService>();
        }
    }
}
