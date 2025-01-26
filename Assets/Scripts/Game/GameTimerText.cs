using System;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameTimerText : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TextMeshProUGUI timerText;

        private void Update()
        {
            timerText.text = gameManager.TimeRemaining.ToString("F0");
        }
    }
}