using Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class TutorialSwapper : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject mouseAndKeyboard;
        [SerializeField] private GameObject gamepads;

        private void Awake()
        {
            gameManager.OnStateChanged += OnStateChanged;
            playerInput.onControlsChanged += OnControlChanged;
        }

        private void OnStateChanged(GameManager.GameState prevstate, GameManager.GameState newstate)
        {
            OnControlChanged(playerInput);
        }

        private void OnDisable()
        {
            playerInput.onControlsChanged -= OnControlChanged;
        }

        private void OnControlChanged(PlayerInput input)
        {
            if (gameManager.CurrentState != GameManager.GameState.InGame) return;

            var isGamepad = input.currentControlScheme == "Gamepad";
            gamepads.SetActive(isGamepad);
            mouseAndKeyboard.SetActive(!isGamepad);
        }
    }
}