using BlockBreaker3D.Datas.Signals;
using System;
using UniRx;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    public abstract class ViewModelBase : IInitializable, IDisposable
    {
        public abstract SetViewVisible.ViewType ViewType { get; }
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        [Inject]
        public void ConstructBus(SignalBus signalBus)
        {
            // React to game lifecycle signals
            signalBus.GetStream<GameSignal>()
                .Subscribe(s => OnReceiveGameSignal(s.SignalType))
                .AddTo(Disposables);
            signalBus
                .GetStream<SetViewVisible>()
                .Where(s => s.HasAny(ViewType))
                .Subscribe(s => SetVisible(s.IsVisible))
                .AddTo(Disposables);
            PostConstruct(signalBus);
        }

        protected virtual void PostConstruct(SignalBus bus) { }
        protected abstract void SetVisible(bool isEnable);
        public virtual void Initialize() { }
        public abstract void OnReceiveGameSignal(GameSignal.Type type);
        public virtual void Dispose() => Disposables.Dispose();
    }
}