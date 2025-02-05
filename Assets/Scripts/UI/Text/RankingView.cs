using Game;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    public class RankingView : MonoBehaviour
    {
        [SerializeField]
        private ScoreHandler scoreHandler;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            OnRankingUpdated(scoreHandler.Ranking);
            scoreHandler.OnRankingUpdated += OnRankingUpdated;
        }

        private void OnDisable()
        {
            scoreHandler.OnRankingUpdated -= OnRankingUpdated;
        }

        private void OnRankingUpdated(int ranking)
        {
            if (ranking == 0) return;
            _text.text = $"{StringUtils.ToOrdinal(ranking)}";
            _text.color = scoreHandler.RankingColors.GetColorOf(ranking);
        }
    }
}