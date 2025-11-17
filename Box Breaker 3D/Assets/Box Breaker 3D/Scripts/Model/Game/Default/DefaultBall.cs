using BoxBreaker3D.Data;
using BoxBreaker3D.Model.Interfaces;
using System;
using UniRx;
using UnityEngine;

namespace BoxBreaker3D.Model
{
    public class DefaultBall : IBall
    {
        private bool _tickable = false;
        private readonly ReactiveCommand<bool> _enableTick = new();
        private ReactiveCommand<float> _command;

        private BallInfo _ballInfo = new();
        public float Speed { get; set; }
        public BallSurface CurrentSurface { get => _ballInfo.BallSurface; set => _ballInfo.BallSurface = value; }
        public IBox CurrentBox { get; set; }

        public ObjectInfo Info => _ballInfo;

        public IObservable<float> OnTick => _command;

        public DefaultBall()
        {
            _ballInfo = new BallInfo();
            _command = new(_enableTick, true);
            Debug.Log("AAAAA");
        }

        public void Pause()
        {
            _tickable = false;
            _enableTick.Execute(_tickable);
        }

        public void Resume()
        {
            _tickable = true;
            _enableTick.Execute(_tickable);
        }

        public void Tick()
        {
            if (!_tickable) return;
            var dt = Time.deltaTime;
            // ここでは transform は触らない
            var dir = CurrentSurface.GetWorldDirection();
            Info.Position += dir * Speed * dt;
            _command.Execute(dt);
            // 衝突判定や Box とのやり取りもここで
        }
    }
}
