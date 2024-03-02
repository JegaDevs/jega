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
        [SerializeField] private List<Animator> animators;
        [SerializeField] private string xMovementParam;
        [SerializeField] private string yMovementParam;
        [SerializeField] private string velocityParam;

        private InputService inputService;
        private void Awake()
        {
            inputService = ServiceProvider.GetService<InputService>();
        }
        private void Update()
        {
            float velocity = inputService.MovementVector.sqrMagnitude;
            int xMovement = (int)inputService.LastPerformedInput.x;
            int yMovement = (int)inputService.LastPerformedInput.y;
            foreach(Animator animator in animators) 
            {
                if (animator.runtimeAnimatorController == null)
                    continue;
                animator.SetFloat(velocityParam, velocity);
                animator.SetInteger(xMovementParam, xMovement);
                animator.SetInteger(yMovementParam, yMovement);
            }
        }
    }
}
