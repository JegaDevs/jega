using Jega.BlueGravity.PreWrittenCode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jega.BlueGravity
{
    [RequireComponent(typeof(Button))]
    public class GetCoinsButton : MonoBehaviour
    {
        private SessionService sessionService;
        private Button button;
        private void Awake()
        {
            button = GetComponent<Button>();    
            button.onClick.AddListener(AddCoins);

            sessionService = ServiceProvider.GetService<SessionService>();
        }
        private void OnDestroy()
        {
            button.onClick.RemoveListener(AddCoins);
        }
        private void AddCoins()
        {
            sessionService.CurrentCoins += 100;
        }
    }
}
