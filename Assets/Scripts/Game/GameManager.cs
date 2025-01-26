using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public enum GameState
        {
            PreGame = 0,
            InGame = 1,
            GameOver = 2
        }

        [SerializeField] private PlayerController playerController;
        [SerializeField] private HookShotAction hookShotAction;
        [SerializeField] private float timeLimit = 60;

        public delegate void StateDelegate(GameState prevState, GameState newState);

        public delegate void PauseStateDelegate(bool isPaused);

        public event StateDelegate OnStateChanged;
        public event PauseStateDelegate OnPauseChanged;

        private GameState _currentState = GameState.GameOver;

        public GameState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState == value) return;

                var lastState = _currentState;
                _currentState = value;
                OnStateChanged?.Invoke(lastState, CurrentState);
            }
        }

        public bool IsPaused { get; private set; }

        public float TimeRemaining { get; private set; }

        private void Awake()
        {
            OnStateChanged +=
                (oldState, newState) =>
                {
                    playerController.enabled = true;
                    hookShotAction.enabled = true;

                    switch (newState)
                    {
                        case GameState.PreGame:
                            playerController.Immobile = true;
                            hookShotAction.Controllable = false;
                            Cursor.visible = false;
                            Cursor.lockState = CursorLockMode.Confined;
                            Time.timeScale = 0f;
                            break;
                        case GameState.GameOver:
                            playerController.Immobile = true;
                            hookShotAction.Controllable = false;
                            Cursor.visible = true;
                            Cursor.lockState = CursorLockMode.Confined;
                            Time.timeScale = 0f;
                            break;
                        case GameState.InGame:
                            playerController.Immobile = false;
                            hookShotAction.Controllable = true;
                            Cursor.visible = false;
                            Cursor.lockState = CursorLockMode.Confined;
                            Time.timeScale = 1f;
                            break;
                    }

                    Debug.Log($"[GameMan] State changing from {oldState.ToString()} to {newState.ToString()}");
                };

            CurrentState = GameState.PreGame;
            TimeRemaining = timeLimit;
        }

        private void Update()
        {
            switch (CurrentState)
            {
                case GameState.PreGame:
                    return;
                case GameState.InGame:
                    TimeRemaining -= Time.deltaTime;
                    if (TimeRemaining <= 0 || playerController.State.IsDead) CurrentState = GameState.GameOver;
                    return;
                case GameState.GameOver:
                    return;
            }
        }

        public void ChangeState(GameState state)
        {
            switch (state)
            {
                default:
                case GameState.PreGame:
                    CurrentState = GameState.PreGame;
                    break;
                case GameState.InGame:
                    Assert.AreEqual(CurrentState, GameState.PreGame);
                    CurrentState = GameState.InGame;
                    break;
                case GameState.GameOver:
                    Assert.AreEqual(CurrentState, GameState.InGame);
                    CurrentState = GameState.GameOver;
                    break;
            }
        }

        public void SetPaused(bool isPaused)
        {
            if (IsPaused == isPaused) return;
            IsPaused = isPaused;
            Time.timeScale = !isPaused && CurrentState == GameState.InGame ? 1f : 0f;
            playerController.Immobile = isPaused && CurrentState != GameState.InGame;
            hookShotAction.Controllable = !isPaused && CurrentState == GameState.InGame;
            OnPauseChanged?.Invoke(IsPaused);
        }
    }
}