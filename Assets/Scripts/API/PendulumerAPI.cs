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
            var uri = new Uri(_baseUri, $"/api/v1/leaderboard?t={GetUnixTime()}");
            var request = UnityWebRequest.Get(uri);

            SetNoCache(request);

            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                onError?.Invoke(request);
            }
            else
            {
                var result = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);
                callback.Invoke(result.records.ToArray());
            }
        }

        public IEnumerator GetRanking(float score, Action<int> callback, Action<UnityWebRequest> onError = null)
        {
            var uri = new Uri(_baseUri, $"/api/v1/leaderboard/ranking?score={score}&t={GetUnixTime()}");
            var request = UnityWebRequest.Get(uri);

            SetNoCache(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                onError?.Invoke(request);
            else
                callback.Invoke(int.Parse(request.downloadHandler.text));
        }

        public IEnumerator PostRecord(GameResult result, Action<ResultRecord> callback,
            Action<UnityWebRequest> onError = null)
        {
            if (result.HasSent)
            {
                Debug.LogWarning("[PendulumerAPI] Trying to re-send result, ignoring the call!");
                onError?.Invoke(null);
                yield break;
            }

            var request = UnityWebRequest.Post(
                new Uri(_baseUri, $"/api/v1/results?t={GetUnixTime()}"),
                JsonUtility.ToJson(result),
                "application/json"
            );

            SetNoCache(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request);
                yield break;
            }

            result.HasSent = true;
            callback.Invoke(JsonUtility.FromJson<ResultRecord>(request.downloadHandler.text));
        }

        public IEnumerator GetStats(Action<StatsRecord> callback, Action<UnityWebRequest> onError = null)
        {
            var request = UnityWebRequest.Get(new Uri(_baseUri, $"/api/v1/stats?t={GetUnixTime()}"));

            SetNoCache(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                onError?.Invoke(request);
            else
                callback.Invoke(JsonUtility.FromJson<StatsRecord>(request.downloadHandler.text));
        }

        private static int GetUnixTime()
        {
            return (int)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }

        private static void SetNoCache(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
            webRequest.SetRequestHeader("Pragma", "no-cache");
        }
    }
}