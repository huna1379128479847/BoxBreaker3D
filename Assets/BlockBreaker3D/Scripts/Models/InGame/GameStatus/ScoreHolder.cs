using BlockBreaker3D.Datas.Signals;
using System;
using UniRx;
using Zenject;

namespace BlockBreaker3D.Models.InGame.GameStatus
{
    public class ScoreHolder : IDisposable
    {
        private readonly SignalBus _signalBus;
        private CompositeDisposable _disposable = new();
        private readonly IntReactiveProperty _scoreProp = new();
        private readonly IntReactiveProperty _destroyedBlock = new(); // 破壊したブロック数
        public bool IsEnabled { get; set; } = true;
        public IReactiveProperty<int> Score => _scoreProp;
        public IReactiveProperty<int> DestroyedBlock => _destroyedBlock;

        public ScoreHolder(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.GetStream<ScoreSignal>()
                .Subscribe(sig =>
                {
                    switch (sig.ScoreOperator)
                    {
                        case ScoreSignal.Operator.Add:
                            AddScore(sig.Value);
                            break;
                        case ScoreSignal.Operator.Sub:
                            SubScore(sig.Value);
                            break;
                        case ScoreSignal.Operator.Reset:
                            ResetScore();
                            break;
                    }
                }).AddTo(_disposable);
            _signalBus.GetStream<GameSignal>()
                .Subscribe(s =>
                {
                    if (s.Has(GameSignal.Type.DestroyedBlock))
                    {
                        _destroyedBlock.Value += 1;
                    }
                    if (s.HasAny(GameSignal.Type.GameOver, GameSignal.Type.GameClear))
                    {
                        IsEnabled = false;
                    }
                    if (s.HasAny(GameSignal.Type.GameStarted, GameSignal.Type.Restart))
                    {
                        IsEnabled = true;
                        ResetScore();
                    }
                }).AddTo(_disposable);
        }

        public void AddScore(int amount)
        {
            if (!IsEnabled) return;
            if (amount <= 0) return;
            _scoreProp.Value += amount;
        }

        public void SubScore(int amount)
        {
            if (!IsEnabled) return;
            if (amount <= 0) return;
            _scoreProp.Value -= amount;
            if (_scoreProp.Value < 0) _scoreProp.Value = 0;
        }
        public void ResetScore()
        {
            if (!IsEnabled) return;
            _scoreProp.Value = 0;
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _disposable = null;
        }
    }
}