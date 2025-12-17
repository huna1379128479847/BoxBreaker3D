using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame;
using BlockBreaker3D.View.InGame;
using System;
using UniRx;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    public class HPViewModel : ITickable, IDisposable
    {
        private readonly GameDataHolder _holder;
        private readonly HPView _view;
        private CompositeDisposable _disposable = new();
        public HPViewModel(SignalBus bus, GameDataHolder holder, HPView view)
        {
            _view = view;
            _holder = holder;
            bus.GetStream<GameSignal>()
                .Subscribe(signal =>
                {
                    if (signal.Has(GameSignal.Type.BallDamaged))
                    {
                        _view.Shake(5f, 0.3f);
                    }
                    if (signal.HasAny(GameSignal.Type.GameStarted, GameSignal.Type.Restart))
                    {
                        _view.Enable(true);
                    }
                    if (signal.HasAny(GameSignal.Type.GameOver, GameSignal.Type.GameClear))
                    {
                        _view.Enable(false);
                    }
                }).AddTo(_disposable);
            bus.GetStream<SetViewVisible>()
                .Where(s => s.HasAny(SetViewVisible.ViewType.LivesView))
                .Subscribe(s => _view.Enable(s.IsVisible))
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void Tick()
        {
            if (_holder.BallBehaviour != null)
            {
                _view.SetHP(_holder.BallBehaviour.HP);
            }
        }
    }
}