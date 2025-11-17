using BoxBreaker3D.Data;
using BoxBreaker3D.Model.Interfaces;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace BoxBreaker3D.Model
{
    public abstract class BoxBase : MonoBehaviour, IBox
    {
        [SerializeField] private GameObject _box;
        [SerializeField] private GameObject _pillerContainer;

        #region Tick
        private bool _tickable = false;
        private readonly ReactiveCommand<bool> _enableTick = new();
        private ReactiveCommand<float> _command;

        #endregion

        [Inject] private IWall _wall;
        private ReactiveProperty<BoxState> _state = new(BoxState.Disable);
        public float Speed { get; set; }

        public ObjectInfo Info => null;

        public IObservable<float> OnTick => _command;

        public IWall Wall => _wall;

        public IObservable<BoxState> State => _state;

        public void EnterBox(GameContext context, IBall ball)
        {
            _state.Value = BoxState.Entering;
        }

        public void ExitBox()
        {
            _state.Value = BoxState.Disable;
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
            //var dir = CurrentSurface.GetWorldDirection();
            //Info.Position += dir * Speed * dt;
            _command.Execute(dt);
            // 衝突判定や Box とのやり取りもここで
        }

        private void Awake()
        {
            _command = new(_enableTick, true);
        }
    }
}