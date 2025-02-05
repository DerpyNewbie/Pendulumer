using Game;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    public class GameTimerView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            _text.text = gameManager.TimeRemaining.ToString("F0");
        }
    }
}