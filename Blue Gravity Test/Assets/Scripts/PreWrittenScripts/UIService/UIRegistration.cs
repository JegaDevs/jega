using UnityEngine;

namespace Jega.BlueGravity.PreWrittenCode
{
    public class UIRegistration : MonoBehaviour
    {
        private void Awake()
        {
            UIService.Service.RegisterNewScene(gameObject);
        }
    }
}
