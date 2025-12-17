using BlockBreaker3D.Datas;
using BlockBreaker3D.Models.InGame.Box;
using BlockBreaker3D.Models.InGame.Component;
using BlockBreaker3D.Models.InGame.GameStatus;
using Sirenix.OdinInspector;


//using BlockBreaker3D.Models.InGame.Blocks.Interfaces;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Blocks
{
    [RequireComponent(typeof(Collider))]
    public class BlockBehaviour : ObjectBase
    {
        // params
        [SerializeField] private int _hp = 1;
        [SerializeField] private int _holdScore = 100;
        // Inject
        [Inject] private ScoreHolder _scoreHolder;
        private SurfaceBehaviour _cache;

        public override IObject SurfaceObject
        {
            get
            {
                return _surface;
            }
            set
            {
                _surface = value;
                if (_surface is SurfaceBehaviour surfaceBehaviour)
                {
                    _cache = surfaceBehaviour;
                }
                else
                {
                    _cache = null;
                }
            }
        }

        public bool IsAlive { get => GetComp<HaveLifeComp>().IsAlive; set => GetComp<HaveLifeComp>().SetAriveState(value); }
        public int HP { get => _hp; internal set => _hp = value < 0 ? 0 : value; }
        // 他に必要なプロパティやメソッドをここに追加
        [Inject]
        public void Construct(ScoreHolder score)
        {
            _scoreHolder = score;
            SurfaceObject = GetComponentInParent<SurfaceBehaviour>();
            _box = GetComponentInParent<BoxBehaviour>();

            //Debug.Log("BlockBehaviour constructed with injected dependencies.");
            SetObjectType(ObjectType | ObjectType.Block);
            AddCompAsStartMember(new HaveLifeComp(_hp));
            Refresh();
        }

        protected override void NotifyCollisionEnter(Collision collision, ObjectType otherType)
        {
            if (otherType == ObjectType.Ball)
            {
                var comp = GetComp<HaveLifeComp>();
                comp.DecreaseLife(1);
                //Debug.Log($"Block hit by Ball. Remaining HP: {HP}");
                if (!comp.IsAlive)
                {
                    DestroyBlock();
                }
            }
        }


        public void DestroyBlock(bool addScore = true, bool playEffect = true)
        {
            if (playEffect)
            {
                // エフェクト再生のロジックをここに追加
                Debug.Log("Playing block destruction effect.");
            }
            if (addScore)
            {
                _scoreHolder.AddScore(_holdScore);
                //Debug.Log($"Added {_holdScore} to score.");
            }
            // ブロックの破壊処理
            IsAlive = false;
            gameObject.SetActive(false);
            foreach (var comp in _comps)
            {
                comp.OnDestroyObj();
            }
            _cache.NotyfyBreaked();
        }

        [Button("Reset Block")]
        protected override void OnReset()
        {
            _cache.RegisterBlock(this);
            gameObject.SetActive(true);
            Refresh();
        }


        public void Refresh()
        {
            var comp = GetComp<HaveLifeComp>();
            comp.ResetLife();
        }


        private GameObject _copy;
        public void SetCoveredMaterial(Material mat)
        {
            if (_copy == null)
            {
                _copy = Instantiate(gameObject, transform.position, transform.rotation, transform);
                _copy.transform.localScale = Vector3.one * 1.03f; // 少し大きくして覆う
                _copy.GetComponent<Collider>().enabled = false; // 衝突判定は無効化
                Destroy(_copy.GetComponent<BlockBehaviour>()); // BlockBehaviourは無効化
                if (_copy.TryGetComponent<Renderer>(out var renderer))
                {
                    renderer.material = mat;
                }
            }
            _copy.SetActive(true);
        }

        // 
        public void ResetMaterial()
        {
            if (_copy != null)
            {
                _copy.SetActive(false);
            }
        }

        protected override void Start()
        {
            base.Start();
            _cache ??= GetComponentInParent<SurfaceBehaviour>();
            _cache.RegisterBlock(this);
        }
    }
}