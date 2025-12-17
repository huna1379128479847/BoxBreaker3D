using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Core;
using System;

namespace BlockBreaker3D.View.InGame
{
    /// <summary>
    /// 入力に応じてターン信号を発行するハンドラ
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class TurnHandler : GameViewBase
    {
        [Inject] private SignalBus _signal;
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private PaddleView _paddleView;
        [SerializeField] private TurnView _turnView;
        [SerializeField, Tooltip("Time scale while holding input")]
        private float _holdTimeScale = 0.25f;
        private Func<bool> _condition = () => true;

        private bool _isHolding = false;
        private Vector2 _holdDir = Vector2.zero;

        public override void Enable(bool enable)
        {
            _isEnabled = enable;
            _paddleView?.Enable(enable);
        }

        public void SetCondition(Func<bool> condition)
        {
            _condition = condition;
        }
        private void OnTurn(InputValue value)
        {
            if (!_isEnabled || !_condition()) return;

            var vector = value.Get<Vector2>();

            // Pressed (or moving) input
            if (vector != Vector2.zero)
            {
                // start holding
                if (!_isHolding)
                {
                    _isHolding = true;
                    _holdDir = vector;
                    //Debug.Log($"Turn Hold Started: {vector}");
                    // slow down game time while holding
                    GameTimeScale.SetGameTimeScale(_holdTimeScale);
                    // show paddle move
                    if (_holdDir.x > 0)
                    {
                        _turnView.Enable_Right(true);
                        _turnView.Enable_Left(false);
                    }
                    else
                    {
                        _turnView.Enable_Right(false);
                        _turnView.Enable_Left(true);
                    }
                }
                else
                {
                    // update direction while holding
                    _holdDir = vector;
                    _paddleView.MoveToSide(vector.x > 0);
                }
                return;
            }

            if (value.isPressed) return;

            // ターン予測線の無効化
            _turnView.Enable_Right(false);
            _turnView.Enable_Left(false);

            // Released input
            if (_isHolding)
            {
                _isHolding = false;
                // restore time scale
                GameTimeScale.ResetTimeScale();

                //Debug.Log($"Turn Released, dir: {_holdDir}");
                if (_holdDir.x > 0)
                {
                    _signal.Fire(new InputSignal(InputType.TurnRight));
                }
                else if (_holdDir.x < 0)
                {
                    _signal.Fire(new InputSignal(InputType.TurnLeft));
                }
                _holdDir = Vector2.zero;
            }
        }

        private void OnDisable()
        {
            // ensure timescale restored when this handler is disabled
            if (_isHolding)
            {
                _isHolding = false;
                GameTimeScale.ResetTimeScale();
                _holdDir = Vector2.zero;
            }
        }
    }
}
