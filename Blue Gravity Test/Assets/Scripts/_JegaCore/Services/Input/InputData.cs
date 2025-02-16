using UnityEngine;
using UnityEngine.InputSystem;

namespace JegaCore
{
    public class InputData : ScriptableObject
    {
        public InputActionAsset InputAction;
        public InputActionReference HorizontalMovement;
        public InputActionReference VerticalMovement;

        private Vector2 currentMovementVector;

        public Vector2 CurrentMovementVector
        {
            private set => currentMovementVector = value; 
            get => currentMovementVector;
        }

        public void InitializeInputActions()
        {
            InputAction.Enable();
        }

        public void UnInitializeInputActions()
        {
            InputAction.Disable();
        }

        public void SetNewMovementVector(Vector2 newVector) => CurrentMovementVector = newVector;
    }
}
