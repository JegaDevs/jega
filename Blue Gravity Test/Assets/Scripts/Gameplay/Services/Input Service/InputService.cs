using Jega.BlueGravity.PreWrittenCode;
using UnityEngine;

namespace Jega.BlueGravity.Services
{
    public class InputService : IService
    {
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
        }

        public void Postprocess()
        {
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



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoInitializeService()
        {
            ServiceProvider.GetService<InputService>();
        }

    }
}
