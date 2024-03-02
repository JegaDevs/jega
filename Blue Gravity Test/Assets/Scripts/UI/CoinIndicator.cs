using Jega.BlueGravity.PreWrittenCode;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Jega.BlueGravity
{
    public class CoinIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;

        private SessionService sessionService;

        private void Awake()
        {
            sessionService = ServiceProvider.GetService<SessionService>();
            UpdateCoins();

            sessionService.OnCoinsUpdate += UpdateCoins;
        }
        private void OnDestroy()
        {
            sessionService.OnCoinsUpdate -= UpdateCoins;
        }

        private void UpdateCoins()
        {
            textMesh.text = sessionService.CurrentCoins.ToString();
        }
    }
}
