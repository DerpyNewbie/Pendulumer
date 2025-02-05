using Game;
using TMPro;
using UnityEngine;

namespace UI.Leaderboard
{
    public class LeaderboardElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI ranking;
        [SerializeField] private TextMeshProUGUI username;
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private RankingColors rankingColors;

        public void Set(int nextRank, string nextName, float nextScore)
        {
            ranking.text = $"{nextRank}.";
            ranking.color = rankingColors.GetColorOf(nextRank);
            username.text = nextName;
            score.text = $"{nextScore:0.00}m";
        }
    }
}