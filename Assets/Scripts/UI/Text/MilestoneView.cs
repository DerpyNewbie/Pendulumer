using API;
using Game;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    public class MilestoneView : MonoBehaviour
    {
        [SerializeField]
        private ScoreHandler scoreHandler;

        [SerializeField]
        private GameObject noNextRecordObject;

        private LeaderboardRecord? _nextRecord;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();

            OnLeaderboardUpdated(scoreHandler.Records);
            scoreHandler.OnLeaderboardUpdated += OnLeaderboardUpdated;
        }

        private void Update()
        {
            if (!_nextRecord.HasValue)
            {
                _text.enabled = false;
                noNextRecordObject.SetActive(true);
                return;
            }

            _text.enabled = true;
            noNextRecordObject.SetActive(false);

            var diff = _nextRecord.Value.score - scoreHandler.Score;
            _text.text =
                $"<color=#{ColorUtility.ToHtmlStringRGB(scoreHandler.RankingColors.GetColorOf(_nextRecord.Value.rank))}>{StringUtils.ToOrdinal(_nextRecord.Value.rank)}</color> in {diff:0.00}m";

            if (diff < 0) _nextRecord = FindNext(scoreHandler.Records, scoreHandler.Score);
        }

        private LeaderboardRecord? FindNext(LeaderboardRecord[] leaderboard, float score)
        {
            for (var i = leaderboard.Length - 1; i >= 0; i--)
                if (leaderboard[i].score > score)
                    return leaderboard[i];
            return null;
        }

        private void OnLeaderboardUpdated(LeaderboardRecord[] obj)
        {
            _nextRecord = FindNext(obj, scoreHandler.Score);
        }
    }
}