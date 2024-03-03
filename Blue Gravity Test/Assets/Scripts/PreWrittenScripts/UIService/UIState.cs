using UnityEngine;

namespace Jega.BlueGravity.PreWrittenCode
{
    [CreateAssetMenu(fileName = nameof(UIState), menuName = "Jega/" + nameof(UIState))]
    public class UIState : ScriptableObject
    {
        public int Layer = 0;
    }
}
