using BlockBreaker3D.Core;
using BlockBreaker3D.Datas.Signals;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame
{
    public enum GameState
    {
        Awake,
        Playing,
        Paused,
        GameOver,
        LevelCompleted,
        GameClear,
        Exit
    }

    /// <summary>
    /// ゲームの状態管理を行い、状態遷移に応じてシグナルを発行する
    /// </summary>
    public class GameStateManager : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly ReactiveProperty<GameState> _currentState = new(GameState.Awake);
        public bool IsPaused => _currentState.Value == GameState.Paused;
        public GameStateManager(SignalBus signalBus)
        {
            _signalBus = signalBus;

            #region Signal Subscriptions
            _signalBus.GetStream<GameSignal>()
                .Subscribe(signal =>
                {
                    Debug.Log($"<color=yellow>[GameStateManager]</color> Received GameSignal: <color=green>{signal.SignalType}</color>");
                    switch (signal.SignalType)
                    {
                        case GameSignal.Type.BlocksAllCleared:
                            // All blocks cleared -> complete level
                            _currentState.Value = GameState.GameClear;
                            _signalBus.Fire(new GameSignal(GameSignal.Type.GameClear));
                            break;
                        case GameSignal.Type.Restart:
                            _currentState.Value = GameState.Playing;
                            GameTimeScale.ResetTimeScale();
                            break;
                    }
                });

            #endregion
        }

        public void Initialize()
        {
            _currentState.Value = GameState.Playing;
            _signalBus.Fire(new GameSignal(GameSignal.Type.GameStarted));
        }

        public void Pause(bool pause)
        {
            GameTimeScale.Pause(pause);
            if (pause && !IsPaused)
            {
                _currentState.Value = GameState.Paused;
                _signalBus.Fire(new GameSignal(GameSignal.Type.Pause));
            }
            else if (!pause && IsPaused)
            {
                _currentState.Value = GameState.Playing;
                _signalBus.Fire(new GameSignal(GameSignal.Type.Resume));
            }
        }

        public void GameOver()
        {
            _currentState.Value = GameState.GameOver;
            _signalBus.Fire(new GameSignal(GameSignal.Type.GameOver));
        }

        public void LevelCompleted()
        {
            _currentState.Value = GameState.LevelCompleted;
            _signalBus.Fire(new GameSignal(GameSignal.Type.LevelCompleted));
        }

        public void RequestRespawn()
        {
            _signalBus.Fire(new GameSignal(GameSignal.Type.RequestRespawn));
        }
    }
}