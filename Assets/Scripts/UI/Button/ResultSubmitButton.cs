using API;
using Game;
using TMPro;
using UI.Text;
using UnityEngine;

namespace UI.Button
{
    public class ResultSubmitButton : MonoBehaviour
    {
        [SerializeField] private SubmitScreen submitScreen;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private WordLimitDisplay wordLimitDisplay;

        private UnityEngine.UI.Button _button;

        private void Start()
        {
            _button = GetComponent<UnityEngine.UI.Button>();
            wordLimitDisplay.OnValidated += b => { _button.interactable = b; };
        }

        public void SendResult()
        {
            submitScreen.OnSubmitting();
            var api = new PendulumerAPI(gameManager.ApiUri);
            var gameResult = gameManager.Result;
            gameResult.name = usernameInputField.text;
            StartCoroutine(api.PostRecord(gameResult,
                result =>
                {
                    StartCoroutine(api.GetStats(stats => { submitScreen.OnSubmitSuccess(result, stats.totalUsers); },
                        _ => { submitScreen.OnSubmitError(); }));
                }));
        }
    }
}