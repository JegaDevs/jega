using Jega.BlueGravity.PreWrittenCode;
using Jega.BlueGravity.Services;
using UnityEngine;
using JegaCore;

namespace Jega.BlueGravity
{
    [RequireComponent((typeof(Rigidbody2D)))]
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private CharacterStatsData characterStats;
        
        private Rigidbody2D body;
        private Vector2 velocityVector;

        private InputService inputService;

        public Rigidbody2D Body => body;
        public Vector2 VelocityVector => velocityVector;

        private void Awake()
        {
            inputService = ServiceProvider.GetService<InputService>();
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            velocityVector = inputService.MovementVector.normalized;
            velocityVector.x *= characterStats.HorizontalVelocity;
            velocityVector.y *= characterStats.VerticalVelocity;

            body.velocity = velocityVector;
        }
    }
}
