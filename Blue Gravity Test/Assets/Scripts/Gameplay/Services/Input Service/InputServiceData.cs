using UnityEngine;
using UnityEngine.InputSystem;

namespace Jega.BlueGravity
{
    public class InputServiceData : ScriptableObject
    {
        public InputActionAsset InputAction;
        public InputActionReference HorizontalMovement;
        public InputActionReference VerticalMovement;

        public void InitializeInputActions()
        {
            InputAction.Enable();
        }

        public void UnInitializeInputActions()
        {
            InputAction.Disable();
        }
    }
}
