using Jega.BlueGravity.InventorySystem;
using Jega.BlueGravity.PreWrittenCode;
using System.Collections.Generic;
using UnityEngine;
using JegaCore;

namespace Jega.BlueGravity
{
    public class CharacterVisual : MonoBehaviour
    {
        [SerializeField] private Animator baseAnimator;
        [SerializeField] private Animator headAnimator;
        [SerializeField] private Animator bodyAnimator;
        [SerializeField] private string xMovementParam;
        [SerializeField] private string yMovementParam;
        [SerializeField] private string velocityParam;

        private List<Animator> animators;
        private InputService inputService;
        private SessionService sessionService;

        private ClothingInventory ClothingInventory => sessionService.ClothingInventory;
        private void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();
            inputService = ServiceProvider.GetService<InputService>();
            animators = new List<Animator> { baseAnimator, headAnimator, bodyAnimator };

            ClothingInventory.OnClothingInventoryUpdated += UpdateSkin;
        }
        private void Start()
        {
            UpdateSkin();
        }
        private void OnDestroy()
        {
            ClothingInventory.OnClothingInventoryUpdated -= UpdateSkin;
        }

        private void Update()
        {
            float velocity = inputService.MovementVector.sqrMagnitude;
            int xMovement = (int)inputService.MovementVector.x;
            int yMovement = (int)inputService.MovementVector.y;
            
            foreach (Animator animator in animators)
            {
                if (animator.runtimeAnimatorController == null)
                    continue;
                animator.SetFloat(velocityParam, velocity);
                if (xMovement + yMovement == 0) continue;
                animator.SetInteger(xMovementParam, xMovement);
                animator.SetInteger(yMovementParam, yMovement);
            }
        }

        private void UpdateSkin()
        {
            ClothingItem headItem = ClothingInventory.HeadItem;
            ClothingItem bodyItem = ClothingInventory.BodyItem;
            headAnimator.runtimeAnimatorController = headItem ? headItem.AnimatorController : null;
            bodyAnimator.runtimeAnimatorController = bodyItem ? bodyItem.AnimatorController : null;
        }
    }
}
