using BlockBreaker3D.Core;
using BlockBreaker3D.View.InGame;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    public class SurfaceClearViewModel : IDisposable
    {
        private IDisposable _subscription;

        [Inject]
        public SurfaceClearViewModel(SignalBus bus, CameraLookUp camera)
        {
            //Debug.Log("SurfaceClearViewModel constructed and subscribing to GameSignal stream.");
            _subscription = bus.GetStream<Datas.Signals.GameSignal>()
                .Where(signal => signal.Has(Datas.Signals.GameSignal.Type.SurfaceBlockCleared))
                .Subscribe(_ =>
                {
                    //Debug.Log("Surface cleared signal received. Triggering camera look up and slow motion effect.");
                    camera.LookUp();
                    Slow.Play(0.4f, camera.Duration);
                });
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}