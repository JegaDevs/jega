using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace JegaCore
{
    public class CoreInputData : ScriptableObject
    {
        public Action OnSpecialButton1Pressed;
        public Action OnSpecialButton2Pressed;
        public Action OnSpecialButton3Pressed;
        
        public InputActionAsset InputAction;
        public InputActionReference HorizontalMovement;
        public InputActionReference VerticalMovement;
        public InputActionReference SpecialButton1;
        public InputActionReference SpecialButton2;
        public InputActionReference SpecialButton3;

        private Vector2 currentMovementVector;
        private Vector2 lastMovementVector;

        public Vector2 CurrentMovementVector
        {
            private set => currentMovementVector = value; 
            get => currentMovementVector;
        }

        public void InitializeInputActions()
        {
            InputAction.Enable();
            if(SpecialButton1 != null)
                SpecialButton1.action.started += SpecialButton1Started;
            if(SpecialButton2 != null) 
                SpecialButton2.action.started += SpecialButton2Started;
            if(SpecialButton2 != null)
                SpecialButton3.action.started += SpecialButton3Started;
        }

        public void UnInitializeInputActions()
        {
            InputAction.Disable();
            if(SpecialButton1 != null)
                SpecialButton1.action.started -= SpecialButton1Started;
            if(SpecialButton2 != null) 
                SpecialButton2.action.started -= SpecialButton2Started;
            if(SpecialButton2 != null)
                SpecialButton3.action.started -= SpecialButton3Started;
        }

        public void SetNewMovementVector(Vector2 newVector) => CurrentMovementVector = newVector;
        private void SpecialButton1Started(InputAction.CallbackContext context) => OnSpecialButton1Pressed?.Invoke();
        private void SpecialButton2Started(InputAction.CallbackContext context) => OnSpecialButton1Pressed?.Invoke();
        private void SpecialButton3Started(InputAction.CallbackContext context) => OnSpecialButton1Pressed?.Invoke();
    }
}
