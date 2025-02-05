using API;
using Game;
using UnityEngine;

namespace UI.Leaderboard
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private GameObject leaderboardElementPrefab;
        [SerializeField] private Transform leaderboardElementContainer;

        private int phase;

        private void OnEnable()
        {
            UpdateView(scoreHandler.Records);
            scoreHandler.OnLeaderboardUpdated += UpdateView;
        }

        private void OnDisable()
        {
            scoreHandler.OnLeaderboardUpdated -= UpdateView;
        }

        private void UpdateView(LeaderboardRecord[] records)
        {
            phase++;
            for (var i = 0; i < leaderboardElementContainer.childCount; i++)
                Destroy(leaderboardElementContainer.GetChild(i).gameObject);

            foreach (var record in records)
            {
                var elem = Instantiate(leaderboardElementPrefab, leaderboardElementContainer)
                    .GetComponent<LeaderboardElement>();
                elem.Set(record.rank, record.name, record.score);
                elem.gameObject.name = $"LeaderboardElement{phase}-{record.rank}";
            }
        }
    }
}