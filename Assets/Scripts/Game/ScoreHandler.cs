using System;
using System.Collections;
using API;
using UnityEngine;

namespace Game
{
    [Serializable]
    public struct RankingColors
    {
        [SerializeField]
        public Color defaultColor;

        [SerializeField]
        public Color firstColor;

        [SerializeField]
        public Color secondColor;

        [SerializeField]
        public Color thirdColor;

        [SerializeField]
        public Color fourthColor;

        [SerializeField]
        public Color fifthColor;

        public Color GetColorOf(int ranking)
        {
            return ranking switch
            {
                1 => firstColor,
                2 => secondColor,
                3 => thirdColor,
                4 => fourthColor,
                5 => fifthColor,
                _ => defaultColor
            };
        }
    }

    public class ScoreHandler : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Transform stageBeginReference;
        [SerializeField] private Transform playerPositionReference;
        [SerializeField] private RankingColors rankingColors;

        private int _ranking;
        private float _score;

        public bool HasHighScoreUpdated { get; private set; }

        public static float LastHighScore { get; private set; }
        public static float HighScore { get; private set; }

        public RankingColors RankingColors => rankingColors;

        public float Score
        {
            get => _score;
            private set
            {
                if (_score > value) return;
                _score = value;

                if (Score > HighScore)
                {
                    HighScore = Score;
                    HasHighScoreUpdated = true;
                    OnHighScoreUpdated?.Invoke(HighScore);
                }

                OnScoreUpdated?.Invoke(Score);
            }
        }

        public int Ranking
        {
            get => _ranking;
            private set
            {
                if (_ranking == value) return;
                _ranking = value;
                OnRankingUpdated?.Invoke(_ranking);
            }
        }

        public LeaderboardRecord[] Records { get; private set; } = Array.Empty<LeaderboardRecord>();

        private void Start()
        {
            gameManager.OnStateChanged += (state, newState) =>
            {
                if (newState != GameManager.GameState.PreGame) return;

                _score = 0;
                LastHighScore = HighScore;
                HasHighScoreUpdated = false;
            };

            gameManager.OnResultReady += result => { StartCoroutine(UpdateRanking(result)); };

            StartCoroutine(ReloadLeaderboard());
        }

        private void Update()
        {
            if (gameManager.CurrentState != GameManager.GameState.InGame) return;
            Score = playerPositionReference.position.x - stageBeginReference.position.x;
        }

        public event Action<LeaderboardRecord[]> OnLeaderboardUpdated;
        public event Action<float> OnScoreUpdated;
        public event Action<float> OnHighScoreUpdated;
        public event Action<int> OnRankingUpdated;

        public IEnumerator UpdateRanking(GameResult result)
        {
            var api = new PendulumerAPI(gameManager.ApiUri);
            return api.GetRanking(result.score, ranking =>
            {
                Ranking = ranking;
                StartCoroutine(ReloadLeaderboard());
            });
        }

        public IEnumerator ReloadLeaderboard()
        {
            var api = new PendulumerAPI(gameManager.ApiUri);
            return api.GetLeaderboard(list =>
            {
                Records = list;
                OnLeaderboardUpdated?.Invoke(Records);
            });
        }
    }
}