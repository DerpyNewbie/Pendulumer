using API;
using Game;
using UnityEngine;

namespace UI.LeaderboardLine
{
    public class LeaderboardLineView : MonoBehaviour
    {
        [SerializeField]
        private ScoreHandler scoreHandler;

        [SerializeField]
        private LeaderboardLineElement[] elements;

        [SerializeField]
        private bool setupOnAwake = true;

        private void Awake()
        {
            UpdateView(scoreHandler.Records);
            scoreHandler.OnLeaderboardUpdated += UpdateView;
        }

        private void UpdateView(LeaderboardRecord[] records)
        {
            foreach (var element in elements)
                element.Reset();

            for (var i = 0; i < elements.Length && i < records.Length; i++)
                elements[i].Set(records[i].score);
        }
    }
}