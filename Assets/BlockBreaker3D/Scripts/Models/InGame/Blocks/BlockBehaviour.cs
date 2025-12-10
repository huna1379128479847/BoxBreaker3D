
using BlockBreaker3D.Datas;
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
        private SurfaceBehaviour _surfaceBehaviour;
        // Inject
        [Inject] private ScoreHolder _scoreHolder;

        public bool IsAlive { get => GetComp<HaveLifeComp>().IsAlive; set => GetComp<HaveLifeComp>().SetAriveState(value); }
        public int HP { get => _hp; internal set => _hp = value < 0 ? 0 : value; }
        // 他に必要なプロパティやメソッドをここに追加
        [Inject]
        public void Construct(ScoreHolder score)
        {
            _scoreHolder = score;
            _surfaceBehaviour = GetComponentInParent<SurfaceBehaviour>();
            _surfaceBehaviour.RegisterBlock(this);
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
            _surfaceBehaviour.NotyfyBreaked();
        }

        [Button("Reset Block")]
        protected override void OnReset()
        {
            _surfaceBehaviour.RegisterBlock(this);
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
                var renderer = _copy.GetComponent<Renderer>();
                if (renderer != null)
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
    }
}