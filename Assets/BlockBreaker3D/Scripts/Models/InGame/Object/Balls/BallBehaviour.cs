using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component.Balls;
using BlockBreaker3D.Datas.Scriptable;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Balls
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallBehaviour : ObjectBase
    {
        // params
        [SerializeField] private int _hp = 1;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _turningAngle = 45f;
        [SerializeField] private Vector2 _direction = Vector2.zero;
        [SerializeField] private int _maxTurn = 2;

#if UNITY_EDITOR
        [SerializeField]
#endif
        private int _initialHP = 0;

        private readonly IntReactiveProperty _turnRemaining = new();

        // Injected Components
        private GameStateManager _stateManager;
        private AnimationData _animationData;
        private IBallTurnService _turnService;

        private Action _spawnAnim;
        private Action _despawnAnim;

        public int HP { get => _hp; internal set => _hp = value; }
        // 自動で正規化される
        public Vector2 Direction { get => _direction; internal set => _direction = value.normalized; }
        public Surface CurrentSurface { get; internal set; } = Surface.Front;
        public float Speed { get => _speed; internal set => _speed = value <= 0 ? 1 : value; }
        public float TurningAngle { get => _turningAngle; internal set => _turningAngle = value <= 0 ? 45 : value; }

        public IReactiveProperty<int> TurnRemaining => _turnRemaining;
        public int MaxTurn => _maxTurn;
        // Transition cooldown tracking to avoid immediate re-transition loops
        private float _lastTransitionTime = -Mathf.Infinity;

        public void MarkTransition()
        {
            _lastTransitionTime = Time.time;
        }

        public bool CanTransition(float cooldown)
        {
            return Time.time - _lastTransitionTime >= cooldown;
        }

        [Inject]
        public void Construct(
            SignalBus signalBus,
            AnimationData animationData,
            IBallTurnService turnService,
            GameDataHolder holder,
            GameStateManager gameStateManager,

            // null許容でCompを差し替え可能に
            BallMoverData mover = null,
            BallCollisionHandlerData collisionHandler = null)
        {
            var rigidbody = GetComponent<Rigidbody>();
            if (!rigidbody.isKinematic)
                rigidbody.isKinematic = true;
            _animationData = animationData;
            _turnService = turnService;
            _turnService.Bind(this);

            // Comp初期化
            if (mover != null)
                _compDatas.Add(mover);
            else
                AddCompAsStartMember(new BallMover());
            if (collisionHandler != null)
                _compDatas.Add(collisionHandler);
            else
                AddCompAsStartMember(new BallCollisionHandler());

            holder.BindBall(this);
            _stateManager = gameStateManager;
            _initialHP = _hp;

            signalBus.GetStream<InputSignal>()
                .Where(sig => sig.HasFlagAnyFast(InputType.Turn))
                .Subscribe(sig =>
                {
                    if (_turnRemaining.Value <= 0) return;
                    _turnService.Turn(sig.HasFlagAnyFast(InputType.TurnRight));
                    _turnRemaining.Value--;
                }).AddTo(this);
            //Debug.Log("BallBehaviour constructed with injected dependencies.");
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            PlayDespawnAnimation().Forget();
            gameObject.SetActive(false);

            // 残機が0未満ならゲームオーバー
            if (HP < 0)
            {
                var cs = _stateManager.CurrentState.Value;
                if (cs != GameState.Playing) return;
                _stateManager.GameOver();
            }
            else
            {
                UniTask.Create(async () =>
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(2.5f)); // 2.5秒待機
                    //Debug.Log("Ball damaged but still alive. Requesting respawn.");
                    _stateManager.RequestRespawn();
                }).Forget();
            }
        }
        public async UniTask PlaySpawnAnimation()
        {
            if (_animationData.OnSpawn != null)
            {
                var spawnEffect = Instantiate(_animationData.OnSpawn, transform.position, Quaternion.identity);
                spawnEffect.Play();
                _spawnAnim = () =>
                {
                    spawnEffect.Stop();
                    Destroy(spawnEffect.gameObject);
                };
                await UniTask.Delay((int)(spawnEffect.main.duration * 1000));
                StopSpawnAnimation();
            }
        }
        public void StopSpawnAnimation()
        {
            _spawnAnim?.Invoke();
            _spawnAnim = null;
        }
        public async UniTask PlayDespawnAnimation()
        {
            if (_animationData.OnDespawn != null)
            {
                var despawnEffect = Instantiate(_animationData.OnDespawn, transform.position, Quaternion.identity);
                despawnEffect.Play();
                _despawnAnim = () =>
                {
                    despawnEffect.Stop();
                    Destroy(despawnEffect.gameObject);
                };
                await UniTask.Delay((int)(despawnEffect.main.duration * 1000));
                StopDespawnAnimation();
            }
        }
        public void StopDespawnAnimation()
        {
            _despawnAnim?.Invoke();
            _despawnAnim = null;
        }

        public void ReverseX()
        {
            _direction.x = -_direction.x;
        }

        public void ReverseY()
        {
            _direction.y = -_direction.y;
        }

        public void Reflect()
        {
            GetComp<IBallMover>().Reflect();
        }

        protected override void NotifyCollisionEnter(Collision collision, ObjectType otherType)
        {
            if (_turnRemaining.Value < _maxTurn)
            {
                _turnRemaining.Value++;
            }
        }

#if UNITY_EDITOR
        [BoxGroup("Debug Options")]
        [Button("Push")]
        public void PushUp()
        {
            if (!Application.isPlaying) return;
            Direction = Vector3.up;
        }

        private enum TestDirection
        {
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back
        }

        [BoxGroup("Debug Options")]
        [SerializeField]
        private TestDirection _testDirection = TestDirection.Front;

        [BoxGroup("Debug Options")]
        [Button("Change")]
        public void ChangeSurface()
        {
            if (!Application.isPlaying) return;
            switch (_testDirection)
            {
                case TestDirection.Top:
                    CurrentSurface = Surface.Top;
                    break;
                case TestDirection.Bottom:
                    CurrentSurface = Surface.Bottom;
                    break;
                case TestDirection.Left:
                    CurrentSurface = Surface.Left;
                    break;
                case TestDirection.Right:
                    CurrentSurface = Surface.Right;
                    break;
                case TestDirection.Front:
                    CurrentSurface = Surface.Front;
                    break;
                case TestDirection.Back:
                    CurrentSurface = Surface.Back;
                    break;
            }
        }

        protected override void OnReset()
        {
            _hp = _initialHP;
            gameObject.SetActive(true);
            _direction = Vector2.zero;
            StopDespawnAnimation();
            StopSpawnAnimation();
        }
#endif
    }
}