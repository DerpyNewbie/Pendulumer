using System;
using System.Collections;
using API;
using Game;
using UnityEngine;
using UnityEngine.Networking;

namespace UI.Button
{
    public class SubmitAnonResultButton : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private UnityEngine.UI.Button _button;

        private void Awake()
        {
            _button = GetComponent<UnityEngine.UI.Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            SceneLoader.StartStaticCoroutine(
                ResultSendCoroutine.SubmitAnonResult(gameManager.Result, gameManager.ApiUri,
                    v => { Debug.Log($"Successfully sent anonymous record: {v}"); },
                    v => { Debug.Log($"Failed to send anonymous record: {v.error}"); }
                ));
        }
    }

    public static class ResultSendCoroutine
    {
        public static IEnumerator SubmitAnonResult(GameResult result, Uri apiUri, Action<ResultRecord> callback = null,
            Action<UnityWebRequest> onError = null)
        {
            result.name = "Anonymous";
            var api = new PendulumerAPI(apiUri);
            return api.PostRecord(result, callback, onError);
        }
    }
}