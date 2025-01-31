using Game;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class InGameScreen : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private readonly int _animHashState = Animator.StringToHash("State");
        private readonly int _animHashReady = Animator.StringToHash("Ready");
        private readonly int _animHashPaused = Animator.StringToHash("Paused");

        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private AudioSource countdownBeep;
        [SerializeField] private AudioSource startBeep;

        private InputAction _moveAction;
        private InputAction _fireAction;
        private InputAction _jumpAction;

        private InputAction _cancelAction;

        private bool _waitingForGameStart;

        private void Awake()
        {
            _moveAction = InputSystem.actions.FindAction("Move");
            _fireAction = InputSystem.actions.FindAction("Attack");
            _jumpAction = InputSystem.actions.FindAction("Jump");

            _cancelAction = InputSystem.actions.FindAction("Cancel");

            _moveAction.performed += OnReadyUpCallback;
            _fireAction.performed += OnReadyUpCallback;
            _jumpAction.performed += OnReadyUpCallback;

            _cancelAction.performed += TogglePauseCallback;
            
            gameManager.OnPauseChanged += OnPauseChangedCallback;
        }

        private void OnDestroy()
        {
            _cancelAction.performed -= TogglePauseCallback;
        }

        public void Update()
        {
            animator.SetInteger(_animHashState, (int)gameManager.CurrentState);
            scoreText.text = $"Score: {scoreHandler.Score:F0}m";
            highScoreText.text = $"High Score: {ScoreHandler.HighScore:F0}m";
        }

        public void OnGameStart()
        {
            gameManager.ChangeState(GameManager.GameState.InGame);
            _waitingForGameStart = false;
        }

        public void PlayCountdownBeep()
        {
            countdownBeep.PlayOneShot(countdownBeep.clip);
        }

        public void PlayStartBeep()
        {
            startBeep.PlayOneShot(startBeep.clip);
        }

        public void OnReadyUpCallback(InputAction.CallbackContext _)
        {
            Debug.Log("[UIHandler] Performed called");
            if (gameManager.CurrentState != GameManager.GameState.PreGame || _waitingForGameStart) return;

            animator.SetTrigger(_animHashReady);
            _waitingForGameStart = true;

            _moveAction.performed -= OnReadyUpCallback;
            _fireAction.performed -= OnReadyUpCallback;
            _jumpAction.performed -= OnReadyUpCallback;
        }

        public void OnPauseChangedCallback(bool isPaused)
        {
            animator.SetBool(_animHashPaused, isPaused);
        }

        public void TogglePauseCallback(InputAction.CallbackContext _)
        {
            gameManager.SetPaused(!gameManager.IsPaused);
        }
    }
}