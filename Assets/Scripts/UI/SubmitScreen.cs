using API;
using Game;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SubmitScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject submitScreen;
        
        [SerializeField]
        private GameObject submittingScreen;

        [SerializeField]
        private GameObject submitSuccessScreen;

        [SerializeField]
        private TextMeshProUGUI submitSuccessScreenText;

        [SerializeField]
        private GameObject submitErrorScreen;

        [SerializeField]
        private ScoreHandler scoreHandler;


        public void OnSubmitRequest()
        {
            ChangeScreen(SubmitScreenType.Input);
        }

        public void OnSubmitting()
        {
            ChangeScreen(SubmitScreenType.Submitting);
        }

        public void OnSubmitSuccess(ResultRecord result, int totalPlayers)
        {
            submitSuccessScreenText.text =
                $"You're {StringUtils.ToOrdinal(result.rank)} out of {totalPlayers} players!\nThanks for playing!";
            submitSuccessScreen.SetActive(true);
            StartCoroutine(scoreHandler.ReloadLeaderboard());

            ChangeScreen(SubmitScreenType.Success);
        }

        public void OnSubmitError()
        {
            ChangeScreen(SubmitScreenType.Error);
        }

        public void OnCloseScreen()
        {
            ChangeScreen(SubmitScreenType.None);
        }

        private void ChangeScreen(SubmitScreenType type)
        {
            submitScreen.SetActive(type != SubmitScreenType.None);
            submittingScreen.SetActive(type == SubmitScreenType.Submitting);
            submitSuccessScreen.SetActive(type == SubmitScreenType.Success);
            submitErrorScreen.SetActive(type == SubmitScreenType.Error);
        }

        private enum SubmitScreenType
        {
            None,
            Input,
            Submitting,
            Success,
            Error
        }
    }
}