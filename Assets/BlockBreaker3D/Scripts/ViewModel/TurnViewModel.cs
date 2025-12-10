using BlockBreaker3D.Datas.Signals;
using UniRx;
using Zenject;
using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using BlockBreaker3D.View.InGame;
using BlockBreaker3D.Models.InGame;
using UnityEngine;
using System;
using BlockBreaker3D.Datas;

namespace BlockBreaker3D.ViewModel
{
    public class TurnViewModel : ITickable, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly GameDataHolder _holder;
        private readonly IBallTurnService _service;
        private readonly TurnView _turnView;
        private readonly TurnHandler _turnHandler;

        private CompositeDisposable _dis = new();

        [Inject]
        public TurnViewModel(
            SignalBus signalBus,
            GameDataHolder holder,
            IBallTurnService turnService,
            TurnView turnView,
            TurnHandler turnHandler)
        {
            _holder = holder;
            _turnHandler = turnHandler;
            _service = turnService;
            _signalBus = signalBus;
            _turnView = turnView;

            _signalBus.GetStream<GameSignal>()
                .Subscribe(s =>
                {
                    if (s.HasAny(GameSignal.Type.GameClear, GameSignal.Type.GameOver))
                    {
                        Debug.Log("TurnViewModel: Disabling turn view on game end.");
                        _turnView.SetActive(false);
                        _turnHandler.Disable();
                    }
                    if (s.HasAny(GameSignal.Type.GameStarted, GameSignal.Type.Restart))
                    {
                        _turnView.SetActive(true);
                        _turnHandler.Enable();
                    }
                });

            _service.OnTurned.Subscribe(isRight =>
            {
                _turnView.SpawnEffect(isRight);
            }).AddTo(_dis);
        }

        public void Tick()
        {
            // ボールがターン時にどの程度曲がるかの目安を表示
            if (_holder == null || _holder.BallBehaviour == null || _turnView == null) return;

            var turn = _holder.BallBehaviour.TurningAngle;

            // Get surface-local axes (up, right) and compute a forward vector for the surface
            var (up, right) = Surface.DefaultMove(_holder.BallBehaviour.CurrentSurface);

            // Ball direction is stored as surface-local 2D (x = right, y = up)
            var dir2 = _holder.BallBehaviour.Direction;
            if (dir2 == Vector2.zero)
            {
                dir2 = Vector2.up; // fallback
            }

            // Helper to rotate a 2D vector by degrees
            static Vector2 Rotate2D(Vector2 v, float deg)
            {
                var rad = deg * Mathf.Deg2Rad;
                var c = Mathf.Cos(rad);
                var s = Mathf.Sin(rad);
                return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
            }

            var left2 = Rotate2D(dir2, turn);   // left is positive angle
            var right2 = Rotate2D(dir2, -turn); // right is negative angle

            // Convert surface-local 2D vectors to world 3D directions
            var leftWorld = (right * left2.x + up * left2.y).normalized;
            var rightWorld = (right * right2.x + up * right2.y).normalized;

            // Compute Euler angles that map Vector3.forward to these world directions
            var leftEuler = Quaternion.FromToRotation(Vector3.forward, leftWorld).eulerAngles;
            var rightEuler = Quaternion.FromToRotation(Vector3.forward, rightWorld).eulerAngles;

            _turnView.UpdateAngle(
                leftEuler,
                rightEuler,
                _holder.BallBehaviour.transform.position);
        }

        public void Dispose()
        {
            if (_dis == null) return;
            _dis.Dispose();
            _dis = null;
        }

        public void Initialize()
        {
            _holder.BallBehaviour.TurnRemaining.Subscribe(count =>
            {
                _turnView.SetText($"Turn Charge: {count}/{_holder.BallBehaviour.MaxTurn}");
            }).AddTo(_dis);
        }
    }
}
