using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jega.BlueGravity
{
    public class ExitGameButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ExitGame);
        }
        private void ExitGame() => Application.Quit();
    }
}
