using System;
using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.Networking;

namespace API
{
    public class PendulumerAPI
    {
        private readonly Uri _baseUri;

        public PendulumerAPI(Uri apiBaseUri)
        {
            _baseUri = apiBaseUri;
        }

        public IEnumerator GetLeaderboard(Action<LeaderboardRecord[]> callback, Action<UnityWebRequest> onError = null)
        {
            var uri = new Uri(_baseUri, "/api/v1/leaderboard");
            Debug.Log($"GetLeaderboard: {uri}");
            var request = UnityWebRequest.Get(uri);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                onError?.Invoke(request);
            }
            else
            {
                var result = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);
                Debug.Log($"GetLeaderboard: {request.downloadHandler.text}");

                callback.Invoke(result.records.ToArray());
            }
        }

        public IEnumerator GetRanking(float score, Action<int> callback, Action<UnityWebRequest> onError = null)
        {
            var request = UnityWebRequest.Get(new Uri(_baseUri, $"/api/v1/leaderboard/ranking?score={score}"));
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                onError?.Invoke(request);
            else
                callback.Invoke(int.Parse(request.downloadHandler.text));
        }

        public IEnumerator PostRecord(GameResult result, Action<ResultRecord> callback,
            Action<UnityWebRequest> onError = null)
        {
            var request = UnityWebRequest.Post(
                new Uri(_baseUri, "/api/v1/results"),
                JsonUtility.ToJson(result),
                "application/json"
            );

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                onError?.Invoke(request);
            else
                callback.Invoke(JsonUtility.FromJson<ResultRecord>(request.downloadHandler.text));
        }

        public IEnumerator GetStats(Action<StatsRecord> callback, Action<UnityWebRequest> onError = null)
        {
            var request = UnityWebRequest.Get(new Uri(_baseUri, "/api/v1/stats"));
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                onError?.Invoke(request);
            else
                callback.Invoke(JsonUtility.FromJson<StatsRecord>(request.downloadHandler.text));
        }
    }
}