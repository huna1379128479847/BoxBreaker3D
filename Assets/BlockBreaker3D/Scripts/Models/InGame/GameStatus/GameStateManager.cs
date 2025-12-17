using BlockBreaker3D.Core;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
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
    public class GameStateManager : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;


        private readonly ReactiveProperty<GameState> _currentState = new(GameState.Awake);

        public bool IsPaused => _currentState.Value == GameState.Paused;
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;
        public GameStateManager(SignalBus signalBus)
        {
            _signalBus = signalBus;

            #region Signal Subscriptions
            _signalBus.GetStream<GameSignal>()
                .Subscribe(signal =>
                {
                    BDebug.Log($"Received GameSignal:{signal.SignalType.ToString().ColorText(BDebug.BColor.green)}", BDebug.BColor.yellow, nameof(GameStateManager));
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
        #region イベント管理
        // 並列処理
        private readonly Queue<Func<CancellationToken, UniTask>> _preInvokedGameStartEvts = new(); // ゲーム開始前に実行する非同期イベント
        private readonly Queue<Func<CancellationToken, UniTask>> _postInvokedGameStartEvts = new(); // ゲーム開始前に実行する非同期イベント

        private CancellationTokenSource _source = new();

        public void RegisterEventPreStart(Func<CancellationToken, UniTask> e)
        {
            _preInvokedGameStartEvts.Enqueue(e);
        }

        public async UniTask InvokePreStartEvents(CancellationToken token)
        {
            var tasks = new List<UniTask>();
            while (_preInvokedGameStartEvts.Count > 0)
            {
                var evt = _preInvokedGameStartEvts.Dequeue();
                tasks.Add(evt(_source.Token));
            }
            await UniTask.WhenAll(tasks).AttachExternalCancellation(token);
        }
        public void RegisterEventPostStart(Func<CancellationToken, UniTask> e)
        {
            _postInvokedGameStartEvts.Enqueue(e);
        }

        public async UniTask InvokePostStartEvents(CancellationToken token)
        {
            var tasks = new List<UniTask>();
            while (_postInvokedGameStartEvts.Count > 0)
            {
                var evt = _postInvokedGameStartEvts.Dequeue();
                tasks.Add(evt(_source.Token));
            }
            await UniTask.WhenAll(tasks).AttachExternalCancellation(token);
        }
        #endregion

        public virtual void Initialize()
        {
            GameStart(_source.Token).Forget();
        }
        
        
        private async UniTask GameStart(CancellationToken token)
        {
            await InvokePreStartEvents(token);
            _currentState.Value = GameState.Playing;
            _signalBus.Fire(new GameSignal(GameSignal.Type.GameStarted));
            await InvokePostStartEvents(token);
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

        public void Dispose()
        {
            _source.Cancel();
            _source.Dispose();
            _preInvokedGameStartEvts.Clear();
        }
    }
}