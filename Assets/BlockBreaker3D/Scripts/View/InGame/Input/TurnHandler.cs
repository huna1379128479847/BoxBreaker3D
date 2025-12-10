using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Core;

namespace BlockBreaker3D.View.InGame
{
    [RequireComponent(typeof(PlayerInput))]
    public class TurnHandler : GameViewBase
    {
        [Inject] private SignalBus _signal;
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private PaddleView _paddleView;
        [SerializeField, Tooltip("Time scale while holding input")] 
        private float _holdTimeScale = 0.25f;

        private bool _isHolding = false;
        private Vector2 _holdDir = Vector2.zero;

        public override void Disable()
        {
            _isEnabled = false;
            _paddleView.Enable(false);
        }

        public override void Enable()
        {
            _isEnabled = true;
            _paddleView.Enable(true);
        }

        private void OnTurn(InputValue value)
        {
            if (!_isEnabled) return;

            var vector = value.Get<Vector2>();

            // Pressed (or moving) input
            if (vector != Vector2.zero)
            {
                // start holding
                if (!_isHolding)
                {
                    _isHolding = true;
                    _holdDir = vector;
                    Debug.Log($"Turn Hold Started: {vector}");
                    // slow down game time while holding
                    GameTimeScale.SetGameTimeScale(_holdTimeScale);
                    // show paddle move
                    _paddleView.MoveToSide(vector.x > 0);
                }
                else
                {
                    // update direction while holding
                    _holdDir = vector;
                }
                return;
            }

            // Released input
            if (_isHolding)
            {
                _isHolding = false;
                // restore time scale
                GameTimeScale.ResetTimeScale();

                Debug.Log($"Turn Released, dir: {_holdDir}");
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
