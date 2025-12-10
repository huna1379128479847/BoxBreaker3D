using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using System;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component
{
    /* Example
    public static Comp Create(CompData data, GameDataHolder holder, IObject parent)
    {
          if (data is DamageData damage)
            {
                  return new DamageCompInfo(damage, holder, parent);
    }
    throw new System.Exception("Invalid CompData type for DamageComp");
    }
    */

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 必ず以下のstaticメソッドを実装する必要がある
    /// static Comp Create(CompData data, GameDataHolder holder, IObject parent)
    /// </remarks>
#if UNITY_EDITOR
    [Serializable]
#endif
    public abstract class Comp : IComp
    {
        private Type _cache;
        public System.Type Type
        {
            get
            {
                if (_cache == null)
                {
                    _cache = this.GetType();
                }
                return _cache;
            }
        }
        public bool ShouldbeRemoved { get; set; } = false;
        public bool IsEnableDuplicate { get; protected set; } = false;

        /// <summary>
        /// リセット時に残されるかどうか <br/>
        /// falseの場合、親オブジェクトのReset時に削除される
        /// </summary>
        public bool InitialComp { get; set; } = false;

        public Comp(bool enableDuplicate)
        {
            IsEnableDuplicate = enableDuplicate;
        }
        /// <summary>
        /// 追加時の初期化処理
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// 毎フレーム毎の更新処理
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void OnUpdate(float deltaTime) { }

        /// <summary>
        /// このコンポーネントが削除される際の処理
        /// </summary>
        public virtual void OnRemove() { }
        /// <summary>
        /// 親オブジェクトの破棄時の処理
        /// </summary>
        public virtual void OnDestroyObj() { }

        /// <summary>
        /// 衝突通知受信時の処理
        /// </summary>
        /// <param name="objectType"></param>
        public virtual void NotifyCollider(Collider other, ObjectType objectType) { }
        public virtual void NotifyCollision(Collision collision, ObjectType objectType) { }
        public virtual void Reset() { }
#if UNITY_EDITOR
        public virtual void OnDrawGizmos(Transform parentTransform) { }
#endif
    }
}
