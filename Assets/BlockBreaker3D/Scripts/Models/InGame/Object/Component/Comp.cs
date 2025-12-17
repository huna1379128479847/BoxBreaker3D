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
        /// Construct from CompData. If the CompData defines a boolean field or property
        /// named "enableDuplicate" (case-insensitive), its value will be used to
        /// initialize IsEnableDuplicate. Otherwise false is used.
        /// </summary>
        public Comp(CompData data)
        {
            if (data == null)
            {
                IsEnableDuplicate = false;
                return;
            }

            // try to read a boolean field/property named "enableDuplicate" (case-insensitive)
            var t = data.GetType();
            var field = t.GetField("enableDuplicate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
            if (field != null && field.FieldType == typeof(bool))
            {
                IsEnableDuplicate = (bool)field.GetValue(data);
                return;
            }
            var prop = t.GetProperty("enableDuplicate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.IgnoreCase);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                IsEnableDuplicate = (bool)prop.GetValue(data);
                return;
            }

            IsEnableDuplicate = false;
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
        /// 親オブジェクトが破壊される際の処理
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
