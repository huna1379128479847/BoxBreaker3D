using BoxBreaker3D.Data;
using System;
using UniRx;
using Zenject;

namespace BoxBreaker3D.Model.Interfaces
{
    public interface IModel : ITickable
    {
        IObservable<float> OnTick { get; }
        ObjectInfo Info { get; }
        void Pause();
        void Resume();
    }
}