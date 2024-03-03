using UnityEngine;
using UnityEngine.UI;

namespace Jega.BlueGravity.PreWrittenCode
{
    [RequireComponent(typeof(Button))]
    public class UIStateButton : MonoBehaviour
    {
        [SerializeField] private UIState targetState;
        [SerializeField] private bool isToggle;
        [SerializeField] private bool isRemoveState;
        private Button button;
        private UIService uiService;
        private void Awake()
        {
            uiService = UIService.Service;
            button = GetComponent<Button>();
            button.onClick.AddListener(ChangeState);
        }

        private void ChangeState()
        {
            if (isToggle)
            {
                if (uiService.ActiveUIStates.Contains(targetState))
                    uiService.RequestRemoveUIState(targetState);
                else
                    uiService.RequestNewUIState(targetState);
            }
            else
            {
                if (!isRemoveState)
                    uiService.RequestNewUIState(targetState);
                else
                    uiService.RequestRemoveUIState(targetState);
            }
        }
    }
}
