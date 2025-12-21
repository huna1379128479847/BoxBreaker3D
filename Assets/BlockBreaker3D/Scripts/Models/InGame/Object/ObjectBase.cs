using BlockBreaker3D.Core;
using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.Component;
using BlockBreaker3D.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame
{
    public abstract class ObjectBase : MonoBehaviour, IObject
    {
        [BoxGroup("Object Properties"), Space(5)]
        [SerializeField, EnumButtons]
        private ObjectType _type;
        private GameDataHolder _holder;
        public ObjectType ObjectType => _type;

        protected IObject _box;
        protected IObject _surface;

        public virtual IObject BoxObject { get => _box; set => _box = value; }
        public virtual IObject SurfaceObject { get => _surface; set => _surface = value; }

        public virtual void ResetState()
        {
            OnReset();
            foreach (var comp in _comps)
            {
                if (!comp.InitialComp)
                {
                    comp.ShouldbeRemoved = true;
                }
                else
                {
                    comp.Reset(this, _holder);
                }
            }
            CheckRemove();
        }

        protected abstract void OnReset();

        public void FireMessage(string message)
        {
            if (_holder != null)
                _holder.SignalBus.Fire(new Message(message));
        }
        #region Comp Management
        [BoxGroup("Comps")]
        [SerializeField] protected List<CompData> _compDatas = new();
        [BoxGroup("Comps")]
#if UNITY_EDITOR
        [DisableInEditorMode]
        [SerializeReference, InfoBox("UnityEditor限定でSerializable属性を付与されたComp継承クラスの監視、値の編集が可能。この編集はPlayMode離脱時に保存されない", Icon = SdfIconType.BatteryFull)]
#endif
        protected List<IComp> _comps = new();

        public void AddComp(IComp comp, bool callOnStart = true)
            => AddCompInternal(comp, callOnStart);

        public void AddCompAsStartMember(IComp comp, bool callOnStart = true)
        {
            comp.InitialComp = true;
            AddCompInternal(comp, callOnStart);
        }
        private void AddCompInternal(IComp comp, bool callOnStart)
        {
            if (!comp.IsEnableDuplicate)
            {
                var id = _comps.FindIndex(c => c.Type == comp.Type);
                if (id != -1)
                {
                    _comps[id].OnRemove(this);
                    _comps[id] = comp;
                    if (callOnStart)
                        comp.OnStart(this, _holder);
                    return;
                }
            }
            _comps.Add(comp);
            if (callOnStart)
                comp.OnStart(this, _holder);
        }


        public void RemoveComp(IComp comp)
        {
            comp.OnRemove(this);
            _comps.Remove(comp);
        }

        public IEnumerable<T> GetComps<T>() where T : IComp
            => _comps.OfType<T>();

        public T GetComp<T>() where T : IComp
            => _comps.OfType<T>().FirstOrDefault();

        public void RemoveComps<T>() where T : IComp
        {
            MarkCompsForRemoval<T>();
            CheckRemove();
        }

        public void MarkCompForRemoval(IComp comp)
        {
            comp.ShouldbeRemoved = true;
        }

        public void MarkCompsForRemoval<T>() where T : IComp
        {
            var comps = GetComps<T>();
            foreach (var comp in comps)
            {
                comp.ShouldbeRemoved = true;
            }
        }
        private void CheckRemove()
        {
            for (int i = _comps.Count - 1; i >= 0; i--)
            {
                if (_comps[i].ShouldbeRemoved)
                {
                    _comps[i].OnRemove(this);
                    _comps.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Setter and Injection
        public void SetObjectType(ObjectType objectType)
        {
            _type = objectType;
        }

        [Inject]
        public void SetGameDataHolder(GameDataHolder holder)
        {
            _holder = holder;
        }
        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime * GameTimeScale.RelativeTimeScale();
            NotifyFixedUpdate(deltaTime);
            foreach (var comp in _comps)
            {
                comp.OnUpdate(this, _holder, deltaTime);
            }
            CheckRemove();
        }

        // コンポーネントの更新より前に呼ばれる
        protected virtual void NotifyFixedUpdate(float deltaTime)
        {
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<ObjectBase>(out var otherObj))
            {
                NotifyTriggerEnter(other, otherObj.ObjectType);
                foreach (var comp in _comps)
                {
                    comp.NotifyCollider(other, otherObj);
                }
                CheckRemove();
            }
        }

        // コンポーネントの更新より前に呼ばれる
        protected virtual void NotifyTriggerEnter(Collider other, ObjectType otherType)
        {
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<ObjectBase>(out var otherObj))
            {
                NotifyCollisionEnter(collision, otherObj.ObjectType);
                foreach (var comp in _comps)
                {
                    comp.NotifyCollision(collision, otherObj);
                }
                CheckRemove();
            }
        }

        protected virtual void NotifyCollisionEnter(Collision collision, ObjectType otherType)
        {
        }

        private void OnDestroy()
        {
            if (_comps == null) return;
            foreach (var comp in _comps)
            {
                comp.OnRemove(this);
                comp.ShouldbeRemoved = true;
            }
            CheckRemove();
        }

        public void Initialize()
        {
            //BDebug.Log($"[{name}] Initialize called.", BDebug.BColor.green);
            if (_holder == null)
            {
                Debug.LogError($"[{name}] GameDataHolder is not set. Please ensure it is injected via SetGameDataHolder before Start.");

                return;
            }
            //Debug.Log($"[{name}] starting Initialize");
            foreach (var compData in _compDatas)
            {
                try
                {
                    var comp = CompCreator.Create(compData);
                    comp.InitialComp = true;
                    AddCompInternal(comp, false);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[{name}] Failed to create component: {compData.ClassName}\n{ex}");
                }
            }
            foreach (var comp in _comps)
            {
                comp.OnStart(this, _holder);
            }
            CheckRemove();
            PostInitialize();
        }

        protected virtual void PostInitialize()
        {
            // 初期化後の追加処理が必要な場合はオーバーライドして実装
        }
#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                // Comps は実行時にしか存在しないため、実行中のみ描画を行う
                foreach (var comp in _comps)
                {
                    comp.OnDrawGizmos(transform);
                }
            }
        }

#endif
        #endregion
    }
}