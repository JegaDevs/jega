using Jega.BlueGravity.PreWrittenCode;
using Jega.BlueGravity.Services;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class CharacterVisual : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorParam("animator")] private string xMovementParam;
        [SerializeField, AnimatorParam("animator")] private string yMovementParam;
        [SerializeField, AnimatorParam("animator")] private string velocityParam;

        private InputService inputService;
        private void Awake()
        {
            inputService = ServiceProvider.GetService<InputService>();
        }
        private void Update()
        {
            animator.SetFloat(velocityParam, inputService.MovementVector.sqrMagnitude);
            animator.SetInteger(xMovementParam, (int)inputService.LastPerformedInput.x);
            animator.SetInteger(yMovementParam, (int)inputService.LastPerformedInput.y);
        }
    }
}
