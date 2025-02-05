using Game;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    [RequireComponent(typeof(TMP_Text))]
    public class ScoreView : MonoBehaviour
    {
        [SerializeField]
        private ScoreHandler scoreHandler;

        private TMP_Text _scoreText;

        private void Awake()
        {
            _scoreText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            OnScoreUpdated(scoreHandler.Score);
            scoreHandler.OnScoreUpdated += OnScoreUpdated;
        }

        private void OnDestroy()
        {
            scoreHandler.OnScoreUpdated -= OnScoreUpdated;
        }

        private void OnScoreUpdated(float score)
        {
            _scoreText.text = $"{score:F2}m";
        }
    }
}