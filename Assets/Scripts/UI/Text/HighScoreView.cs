using Game;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class HighScoreView : MonoBehaviour
    {
        [SerializeField] private ScoreHandler scoreHandler;
        private TMP_Text _scoreText;

        private void Awake()
        {
            _scoreText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            OnHighScoreUpdated(ScoreHandler.HighScore);
            scoreHandler.OnHighScoreUpdated += OnHighScoreUpdated;
        }

        private void OnDisable()
        {
            scoreHandler.OnHighScoreUpdated -= OnHighScoreUpdated;
        }

        private void OnHighScoreUpdated(float score)
        {
            _scoreText.text = $"{score:F2}m";
        }
    }
}