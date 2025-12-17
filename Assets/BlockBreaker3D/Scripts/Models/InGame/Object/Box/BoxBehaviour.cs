using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.Balls;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Box
{
    [DefaultExecutionOrder(-100)]
    public sealed class BoxBehaviour : ObjectBase
    {
        private SignalBus _signalBus;
        [SerializeField] private List<SurfaceBehaviour> _surfaces = new();
#if UNITY_EDITOR
        [SerializeField]
#endif
        private BallBehaviour _ballBehaviour;
        [SerializeField] private Material _sharedOnOutSurface; // 別のボールが存在しない面で使用するマテリアル
        [Header("Transition Settings")]
        [SerializeField, Tooltip("Cooldown (seconds) after a surface transition during which further transitions are ignored")] private float _transitionCooldown = 0.15f;
        [SerializeField, Tooltip("Margin to inset the ball into the target surface after transition to avoid immediate re-exit")] private float _transitionInset = 0.1f;

        private readonly ReactiveProperty<SurfaceBehaviour> _currentSurface = new();

        public float TransitionInset => _transitionInset;
        public SurfaceBehaviour DefaultSurface { get; internal set; }
        public IReadOnlyReactiveProperty<SurfaceBehaviour> CurrentSurface => _currentSurface;
        public bool IsEnabled { get; set; } = true;
        public Material SharedOnOutSurface => _sharedOnOutSurface;

        [Inject]
        public void Constract(SignalBus signalBus, GameDataHolder dataHolder)
        {
            dataHolder.BindBox(this);
            if (DefaultSurface == null)
                DefaultSurface = _surfaces[0];
            _currentSurface.Value = DefaultSurface;

            _signalBus = signalBus;
            _signalBus.GetStream<GameSignal>()
                .Where(_ => IsEnabled)
                .Subscribe(signal =>
                {
                    if (signal.SignalType == GameSignal.Type.Restart)
                    {
                        ResetState();
                    }
                    if (signal.SignalType == GameSignal.Type.GameStarted)
                    {
                        foreach (var surface in _surfaces)
                        {
                            surface._boxBehaviour = this;
                            if (surface != DefaultSurface)
                                surface.SetCoveredMaterial(_sharedOnOutSurface);
                        }
                        _currentSurface.Value.SpawnThis(_ballBehaviour);
                    }
                    if (signal.SignalType == GameSignal.Type.RequestRespawn)
                    {
                        Debug.Log("Respawn Ball");
                        _currentSurface.Value.SpawnThis(_ballBehaviour);
                    }
                });
        }

        public void EnterBall(BallBehaviour ball)
        {
            _ballBehaviour = ball;
            _ballBehaviour.BoxObject = this;
        }
        public bool Transition(string target, float rotate)
        {
            var targetSurface = _surfaces.Find(s => s.gameObject.name == target);
            if (targetSurface != null)
            {
                if (!_ballBehaviour.CanTransition(_transitionCooldown))
                {
                    //Debug.Log("Transition suppressed due to cooldown");
                    return false;
                }

                //Debug.Log($"Transition to {targetSurface.Surface.Name}");


                // Mark transition time to prevent immediate re-transition
                _ballBehaviour.MarkTransition();
                _currentSurface.Value.ExitThis();
                targetSurface.EnterThis(_ballBehaviour);
                // Clamp ball position into the target surface bounds to avoid immediate re-exit
                targetSurface.ClampTo();
                _ballBehaviour.Direction = Rotate(_ballBehaviour.Direction, rotate);
                _currentSurface.Value = targetSurface;
                return true;
            }
            return false;
        }

        private Vector2 Rotate(Vector2 v, float angleDeg)
        {
            return Quaternion.Euler(0, 0, angleDeg) * v;
        }

        public int GetTotalBlockCount()
        {
            int total = 0;
            foreach (var surface in _surfaces)
            {
                total += surface.GetBlockCount();
            }
            return total;
        }

        public void CheckClear()
        {
            var count = GetTotalBlockCount();
            if (count == 0)
            {
                _signalBus.Fire(new GameSignal(GameSignal.Type.BlocksAllCleared));
            }
            //Debug.Log($"Total remaining blocks: {count}");
        }

        protected override void OnReset()
        {
            foreach (var item in _surfaces)
            {
                item.ResetState();
            }
            _ballBehaviour.ResetState();
            _currentSurface.Value = DefaultSurface;
            DefaultSurface.SpawnThis(_ballBehaviour);
        }
    }
}