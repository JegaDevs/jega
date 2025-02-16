using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    [CreateAssetMenu(menuName = "CharacterStats")]
    public class CharacterStatsData : ScriptableObject
    {
        [SerializeField] private float horizontalVelocity;
        [SerializeField] private float verticalVelocity;

        public float HorizontalVelocity => horizontalVelocity;
        public float VerticalVelocity => verticalVelocity;
    }
}
