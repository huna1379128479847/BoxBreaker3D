using UnityEngine;
using BoxBreaker3D.Model.Interfaces;
using Zenject;
using System;

namespace BoxBreaker3D.ViewModel
{
    public class BallViewModel : IViewModel
    {
        private readonly IBall _model;

        [Inject]
        public BallViewModel(IBall model)
        {
            _model = model;
        }

        // 必要なら他の情報も公開（速度・面など）
        public float Speed => _model.Speed;

        public IObservable<float> OnTick => _model.OnTick;

        public Vector3 CurrentPosition => _model.Info.Position;

        public void SetPosition(Vector3 position)
        {
            _model.Info.Position = position;
        }
    }
}
