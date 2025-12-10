using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame;
using BlockBreaker3D.Models.InGame.Balls;
using BlockBreaker3D.Models.InGame.Blocks;
using BlockBreaker3D.Models.InGame.Box;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models
{
    public class SurfaceBehaviour : ObjectBase
    {
        [SerializeField] private string _surfaceType;
        [SerializeField] private Vector3 _surfaceOriginPos;
        [SerializeField] private Vector2 _size;
        [SerializeField] private Vector2 _spawnPos;
        [SerializeField] private Vector2 _pushOnSpawn;
        [SerializeField] private Vector3 _camRotate;

        [BoxGroup("Surface Transient"), Space(10)]
        [Tooltip("面の上から出るときの遷移先サーフェスタイプ")]
        [SerializeField] private string _exitOnTopToSurfaceType;
        [BoxGroup("Surface Transient")]
        [Tooltip("面の下から出るときの遷移先サーフェスタイプ")]
        [SerializeField] private string _exitOnBottomToSurfaceType;
        [BoxGroup("Surface Transient")]
        [Tooltip("面の左から出るときの遷移先サーフェスタイプ")]
        [SerializeField] private string _exitOnLeftToSurfaceType;
        [BoxGroup("Surface Transient")]
        [Tooltip("面の右から出るときの遷移先サーフェスタイプ")]
        [SerializeField] private string _exitOnRightToSurfaceType;

        [BoxGroup("GUI"), Space(10)]
        [SerializeField, Tooltip("ブロック残りの表示用Text")]
        private TMP_Text _blockRemainText;
        [SerializeField, Tooltip("テキストのポジション(2D基準)")]
        private Vector2 _blockRemainTextPos;
        private TMP_Text _instancedBlockRemainText;

        // Injected / runtime
        private SignalBus _signalBus;
#if UNITY_EDITOR
        [SerializeField, Tooltip("シリアライズはエディターでしか動作しません")]
#endif
        private BallBehaviour _ballBehaviour;
        internal BoxBehaviour _boxBehaviour;
        private readonly List<BlockBehaviour> _blocks = new();
        private readonly ReactiveProperty<Vector2> _ballLocalPosition = new(Vector2.zero);

        #region Properties
        public Surface Surface { get; private set; }
        public Vector3 SurfaceOrigin => _surfaceOriginPos;
        public Vector3 CamRotate => _camRotate;

        // Expose size so utilities can compute world positions based on this surface
        public Vector2 Size => _size;

        // Observables for monitoring the ball's 2D position relative to this surface (X=right, Y=up)
        public IReadOnlyReactiveProperty<Vector2> BallLocalPosition => _ballLocalPosition;

        public int BlockRemainCount => _blocks.Count;
        public bool IsHoldingBall => _ballBehaviour != null;
        #endregion

        #region Construction
        [Inject]
        public void Constract(SignalBus sig)
        {
            _signalBus = sig;
            Surface = new Surface(_surfaceType);
            // Instantiate block remain text if a prefab/reference is provided
            if (_blockRemainText != null)
            {
                _instancedBlockRemainText = Instantiate(_blockRemainText, transform);
                // place initially
                UpdateBlockRemainTextPosition();
                _instancedBlockRemainText.gameObject.SetActive(true);
                // _camRotate is stored as Euler angles; use Quaternion.Euler to construct a valid rotation
                _instancedBlockRemainText.transform.rotation = Quaternion.Euler(_camRotate);
                UpdateBlockRemainText();
            }
        }
        #endregion

        #region Coordinate Conversion / UI Helpers
        /// <summary>
        /// Surface のローカル2D座標（このクラスでは X = right, Y = up）をワールド座標に変換します。
        /// Spawn やブロック配置など、Surface を基準にした座標変換に使います。
        /// </summary>
        public Vector3 SurfaceLocalToWorld(Vector2 localPos)
        {
            // Delegate coordinate conversion to the Surface data struct
            return Surface.LocalToWorld(SurfaceOrigin, localPos);
        }

        /// <summary>
        /// ワールド座標を Surface のローカル2D座標（X=right, Y=up）に変換します。
        /// </summary>
        public Vector2 SurfaceWorldToLocal(Vector3 worldPos)
        {
            return Surface.WorldToLocal(SurfaceOrigin, worldPos);
        }

        /// <summary>
        /// UV (0..1, 0..1) を Surface のローカル領域（サイズ: _size）にマップしてワールド座標に変換します。
        /// UV (0,0) は左下、(1,1) は右上として扱います。
        /// </summary>
        public Vector3 SurfaceUVToWorld(Vector2 uv)
        {
            return Surface.UVToWorld(SurfaceOrigin, uv, _size);
        }

        private void UpdateBlockRemainText()
        {
            if (_instancedBlockRemainText == null) return;
            var rm = GetBlockCount();
            if (rm == 0)
            {
                _instancedBlockRemainText.text = "<color=green>☑</color>";
            }
            else
                            {
                _instancedBlockRemainText.text = rm.ToString();
            }
        }

        private void UpdateBlockRemainTextPosition()
        {
            if (_instancedBlockRemainText == null) return;
            // compute world position from surface-local 2D position
            var world = SurfaceLocalToWorld(_blockRemainTextPos);
            // get surface normal (right x up)
            var (up, right) = Surface.DefaultMove(Surface);
            var normal = Vector3.Cross(right, up).normalized;
            // offset slightly toward normal so text appears in front of surface
            float offset = -0.5f;
            _instancedBlockRemainText.transform.position = world + normal * offset;
        }
        #endregion

        #region Ball Management
        public void SpawnThis(BallBehaviour ballBehaviour)
        {
            Debug.Log($"[{name}.Surface]SpawnThis");
            EnterThis(ballBehaviour);

            // 面情報をボールに反映
            _ballBehaviour.CurrentSurface = Surface;

            // オブジェクトに占拠されていないスポーン位置を探す
            var tryPos = _spawnPos; // Surface ローカル座標 (X=right, Y=up)
            var step = 0.5f;
            var sphere = ballBehaviour.GetComponent<SphereCollider>();
            float radius = 0.5f;
            if (sphere != null)
            {
                // SphereCollider.radius はローカルスケール基準のため、目安として lossyScale を掛ける
                radius = sphere.radius * Mathf.Max(ballBehaviour.transform.lossyScale.x, ballBehaviour.transform.lossyScale.y, ballBehaviour.transform.lossyScale.z);
            }

            // tryPos (local) をワールドに変換して当たり判定を行う
            for (int i = 0; i < 20; i++)
            {
                var worldPos = SurfaceLocalToWorld(tryPos);

                Collider[] colliders = new Collider[1];

                if (Physics.OverlapSphereNonAlloc(worldPos, radius, colliders) == 0)
                {
                    // 空いている場所が見つかった
                    _ballBehaviour.transform.position = worldPos;
                    break;
                }

                // X方向 (right) に交互に広げて探索する
                tryPos.x += ((i % 2 == 0) ? 1 : -1) * step;
            }

            // 万が一ループで見つからなかった場合はデフォルトスポーン位置を使う
            if (_ballBehaviour.transform.position == Vector3.zero)
            {
                _ballBehaviour.transform.position = SurfaceLocalToWorld(_spawnPos);
            }

            // 面ローカルの方向を設定（正規化しておく）
            _ballBehaviour.gameObject.SetActive(true);
            _ballBehaviour.Direction = _pushOnSpawn.normalized;

            _ballBehaviour.PlaySpawnAnimation().Forget();
        }

        public void ClampTo()
        {
            if (_ballBehaviour == null) return;
            var worldPos = _ballBehaviour.transform.position;
            var local = SurfaceWorldToLocal(worldPos);
            // Calculate inset based on ball radius (prefer radius + small margin over configured inset)
            var sphere = _ballBehaviour.GetComponent<SphereCollider>();
            float radius = 0f;
            if (sphere != null)
            {
                radius = sphere.radius * Mathf.Max(
                    _ballBehaviour.transform.lossyScale.x,
                    _ballBehaviour.transform.lossyScale.y,
                    _ballBehaviour.transform.lossyScale.z);
            }
            const float epsilon = 0.2f;
            float inset = Mathf.Max(_boxBehaviour.TransitionInset, radius + epsilon);

            var clamped = new Vector2(
                Mathf.Clamp(local.x, 0f + inset, Size.x - inset),
                Mathf.Clamp(local.y, 0f + inset, Size.y - inset)
            );
            var newWorld = SurfaceLocalToWorld(clamped);
            _ballBehaviour.transform.position = newWorld;
        }

        public void EnterThis(BallBehaviour ballBehaviour)
        {
            _ballBehaviour = ballBehaviour;
            _ballBehaviour.CurrentSurface = Surface;
            foreach (var block in _blocks)
            {
                block.ResetMaterial();
            }
            _ballLocalPosition.Subscribe(pos =>
            {
                // pos is in surface-local coordinates (X = right, Y = up) where (0,0) corresponds to SurfaceOrigin
                if (!Surface.IsOutside(pos, _size)) return;
                var side = Surface.GetExitSide(pos, _size);
                switch (side)
                {
                    case Surface.ExitSide.Right:
                        if (!_boxBehaviour.Transition(_exitOnRightToSurfaceType))
                            ClampTo();
                        break;
                    case Surface.ExitSide.Left:
                        if (!_boxBehaviour.Transition(_exitOnLeftToSurfaceType))
                            ClampTo();
                        break;
                    case Surface.ExitSide.Top:
                        if (!_boxBehaviour.Transition(_exitOnTopToSurfaceType))
                            ClampTo();
                        break;
                    case Surface.ExitSide.Bottom:
                        if (!_boxBehaviour.Transition(_exitOnBottomToSurfaceType))
                            ClampTo();
                        break;
                    case Surface.ExitSide.None:
                    default:
                        break;
                }
            }).AddTo(this);

            // update UI when entering surface
            UpdateBlockRemainText();
            UpdateBlockRemainTextPosition();
        }

        public void ExitThis()
        {
            _ballBehaviour = null;
            _ballLocalPosition.Value = Vector2.zero;
            var mat = _boxBehaviour.SharedOnOutSurface;
            SetCoveredMaterial(mat);
        }

        public void SetCoveredMaterial(Material mat)
        {
            foreach (var block in _blocks)
            {
                block.SetCoveredMaterial(mat);
            }
        }
        #endregion

        #region Block Management
        public void RegisterBlock(BlockBehaviour block)
        {
            if (!_blocks.Contains(block))
            {
                _blocks.Add(block);
            }
        }

        public void UnregisterBlock(BlockBehaviour block)
        {
            if (_blocks.Contains(block))
            {
                _blocks.Remove(block);
            }
            _boxBehaviour.CheckClear();
            UpdateBlockRemainText();
            UpdateBlockRemainTextPosition();
        }

        public int GetBlockCount()
        {
            return _blocks.Where(b => b.IsAlive).Count();
        }

        public void NotyfyBreaked()
        {
            _boxBehaviour.CheckClear();
            if (GetBlockCount() == 0)
                _signalBus.Fire(new GameSignal(GameSignal.Type.SurfaceBlockCleared));
        }
        #endregion

        #region ObjectBase Overrides
        protected override void NotifyFixedUpdate(float deltaTime)
        {
            if (_ballBehaviour == null) return;
            var pos2D = SurfaceWorldToLocal(_ballBehaviour.transform.position);
            _ballLocalPosition.Value = pos2D;
            UpdateBlockRemainText();
        }

        protected override void OnReset()
        {
            var cp = _blocks.ToArray();
            _ballBehaviour = null;
            _blocks.Clear();
            _ballLocalPosition.Value = Vector2.zero;
            foreach (var block in cp)
            {
                block.ResetState();
            }
        }
        #endregion
    }
}