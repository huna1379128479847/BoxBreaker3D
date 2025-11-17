using System;
using UnityEngine;

namespace BoxBreaker3D.ViewModel
{
    public interface IViewModel
    {
        IObservable<float> OnTick { get; }
        Vector3 CurrentPosition { get; }
        void SetPosition(Vector3 position);
    }
}