using Jega.BlueGravity.Services;
using Jega.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    [RequireComponent((typeof(Rigidbody2D)))]
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float horizontalVelocity;
        [SerializeField] private float verticalVelocity;
        private Rigidbody2D body;
        private Vector2 velocityVector;

        private InputService inputService;
        private void Awake()
        {
            inputService = ServiceProvider.GetService<InputService>();
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            velocityVector = inputService.MovementVector.normalized;
            velocityVector.x *= horizontalVelocity;
            velocityVector.y *= verticalVelocity;

            body.velocity = velocityVector;
        }
    }
}
