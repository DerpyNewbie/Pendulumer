using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public delegate void PauseStateDelegate(bool isPaused);

        public delegate void ResultDelegate(GameResult result);

        public delegate void StateDelegate(GameState prevState, GameState newState);

        public enum GameState
        {
            Title = -1,
            PreGame = 0,
            InGame = 1,
            GameOver = 2
        }

        [SerializeField] private string apiUrl = "http://localhost:5000/";
        [SerializeField] private bool isTitle;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private HookShotAction hookShotAction;
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private float timeLimit = 60;

        private GameState _currentState = GameState.GameOver;

        private GameResult _result = new();

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

        public Uri ApiUri => new(apiUrl);

        public bool IsRunning => CurrentState is GameState.InGame or GameState.Title && !IsPaused;

        public GameResult Result => _result.CloneViaSerialization();

        private void Awake()
        {
            OnStateChanged +=
                (oldState, newState) =>
                {
                    playerController.enabled = true;
                    hookShotAction.enabled = !isTitle;

                    switch (newState)
                    {
                        case GameState.Title:
                            break;
                        case GameState.PreGame:
                            playerController.Immobile = true;
                            hookShotAction.Controllable = false;
                            break;
                        case GameState.GameOver:
                            _result.posX = playerController.transform.position.x;
                            _result.posY = playerController.transform.position.y;
                            _result.playtime = timeLimit - TimeRemaining;
                            _result.score = scoreHandler.Score;
                            OnResultReady?.Invoke(_result);

                            playerController.Immobile = true;
                            hookShotAction.Controllable = false;
                            break;
                        case GameState.InGame:
                            _result = new GameResult();
                            playerController.Immobile = false;
                            hookShotAction.Controllable = true;
                            break;
                    }

                    UpdateTimeScale();
                    UpdateCursor();
                    Debug.Log($"[GameMan] State changing from {oldState.ToString()} to {newState.ToString()}");
                };

            CurrentState = isTitle ? GameState.Title : GameState.PreGame;
            TimeRemaining = timeLimit;
        }

        private void Start()
        {
            playerController.OnJump += ctx =>
            {
                if (ctx == PlayerController.EventContext.Begin) _result.jumpCount++;
            };

            hookShotAction.OnActivated += () => { _result.clickCount++; };
        }

        private void Update()
        {
            if (CurrentState is not GameState.InGame) return;

            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0 || playerController.State.IsDead) CurrentState = GameState.GameOver;
        }

        public event StateDelegate OnStateChanged;
        public event PauseStateDelegate OnPauseChanged;
        public event ResultDelegate OnResultReady;

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

        public void UpdateTimeScale()
        {
            Time.timeScale = IsPaused ? 0f : CurrentState is GameState.InGame or GameState.Title ? 1f : 0f;
        }

        public void UpdateCursor()
        {
            Cursor.visible = IsPaused || CurrentState is not (GameState.InGame or GameState.PreGame);
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void SetPaused(bool isPaused)
        {
            if (IsPaused == isPaused || (_currentState == GameState.GameOver && !IsPaused)) return;
            IsPaused = isPaused;
            playerController.Immobile = isPaused && CurrentState != GameState.InGame;
            hookShotAction.Controllable = !isPaused && CurrentState == GameState.InGame;
            UpdateTimeScale();
            UpdateCursor();
            OnPauseChanged?.Invoke(IsPaused);
        }

        private void OnJump(PlayerController.EventContext context)
        {
            if (!IsRunning) return;
            if (context == PlayerController.EventContext.Begin) _result.jumpCount++;
        }

        private void OnHookShotActivate()
        {
            if (!IsRunning) return;
            _result.clickCount++;
        }
    }
}