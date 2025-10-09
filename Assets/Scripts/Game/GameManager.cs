using System;
using DefaultNamespace;
using Game.Player;
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
        [SerializeField] private PlayerState playerState;
        [SerializeField] private Game.Player.Action.HookShotAction hookShotAction;
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private float timeLimit = 60;

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

        public Uri ApiUri => new(apiUrl);

        public bool IsRunning => CurrentState is GameState.InGame or GameState.Title && !IsPaused;

        public GameResult Result { get; private set; } = new();

        private void Awake()
        {
            OnStateChanged +=
                (oldState, newState) =>
                {
                    playerState.enabled = true;
                    hookShotAction.enabled = !isTitle;

                    switch (newState)
                    {
                        case GameState.Title:
                            break;
                        case GameState.PreGame:
                            playerState.Immobile = true;
                            hookShotAction.Controllable = false;
                            break;
                        case GameState.GameOver:
                            var buildInfo = Resources.Load<BuildInfo>("BuildInfo");
                            Result.posX = playerState.Rigidbody.position.x;
                            Result.posY = playerState.Rigidbody.position.y;
                            Result.playtime = timeLimit - TimeRemaining;
                            Result.score = scoreHandler.Score;
                            Result.version = buildInfo.count;
                            Result.HasSent = false;
                            OnResultReady?.Invoke(Result);

                            playerState.Immobile = true;
                            hookShotAction.Controllable = false;
                            break;
                        case GameState.InGame:
                            Result = new GameResult();
                            playerState.Immobile = false;
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
            playerState.OnJump += OnJump;
            hookShotAction.OnHookShotActivated += OnHookShotActivate;
        }

        private void Update()
        {
            if (CurrentState is not GameState.InGame) return;

            TimeRemaining -= Time.deltaTime;
            if (TimeRemaining <= 0 || playerState.IsDead) CurrentState = GameState.GameOver;
        }

        private void OnDestroy()
        {
            playerState.OnJump -= OnJump;
            hookShotAction.OnHookShotActivated -= OnHookShotActivate;
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
            playerState.Immobile = isPaused && CurrentState != GameState.InGame;
            hookShotAction.Controllable = !isPaused && CurrentState == GameState.InGame;
            UpdateTimeScale();
            UpdateCursor();
            OnPauseChanged?.Invoke(IsPaused);
        }

        private void OnJump()
        {
            if (!IsRunning) return;
            Result.jumpCount++;
        }

        private void OnHookShotActivate()
        {
            if (!IsRunning) return;
            Result.clickCount++;
        }
    }
}