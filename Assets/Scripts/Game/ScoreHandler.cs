using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform stageBeginReference;
        [SerializeField] private Transform playerPositionReference;

        private float _score;

        public bool HasHighScoreUpdated { get; private set; }

        public static float LastHighScore { get; private set; }
        public static float HighScore { get; private set; }

        public float Score
        {
            get => _score;
            private set
            {
                if (_score > value) return;
                _score = value;

                if (Score < HighScore) return;

                HighScore = Score;
                HasHighScoreUpdated = true;
            }
        }

        private void Start()
        {
            gameManager.OnStateChanged += (state, newState) =>
            {
                if (newState != GameManager.GameState.PreGame) return;

                _score = 0;
                LastHighScore = HighScore;
                HasHighScoreUpdated = false;
            };
        }

        private void Update()
        {
            if (gameManager.CurrentState != GameManager.GameState.InGame) return;
            Score = playerPositionReference.position.x - stageBeginReference.position.x;
        }
    }
}