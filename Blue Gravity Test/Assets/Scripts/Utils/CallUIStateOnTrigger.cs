using Jega.BlueGravity.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class CallUIStateOnTrigger : MonoBehaviour
    {
        [SerializeField] private UIState state;

        private UIService uiService;
        private void Awake()
        {
            uiService = ServiceProvider.GetService<UIService>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out CharacterController controller))
                uiService.RequestNewUIState(state);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CharacterController controller))
                uiService.RequestRemoveUIState(state);
        }
    }
}
