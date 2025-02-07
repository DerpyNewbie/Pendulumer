using System;
using System.Collections;
using API;
using Game;
using TMPro;
using UnityEngine;

namespace UI.Text
{
    public class StatsView : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        private TMP_Text _text;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            StartCoroutine(UpdateText());
        }

        public IEnumerator UpdateText()
        {
            var api = new PendulumerAPI(gameManager.ApiUri);
            yield return api.GetStats(stats =>
            {
                _text.text = $"Recorded Playtime : {TimeSpan.FromSeconds(stats.totalPlaytime):dd\\.hh\\:mm\\:ss}\n" +
                             $"Recorded Players  : {stats.totalUsers}\n" +
                             $"Recorded Games    : {stats.totalResults}\n" +
                             $"Recorded Hookshots: {stats.totalClickCount}\n" +
                             $"Recorded Jumps    : {stats.totalJumpCount}";
                ;
            });
        }
    }
}