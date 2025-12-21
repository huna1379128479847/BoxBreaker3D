using BlockBreaker3D.Datas;
using UnityEngine;
using BlockBreaker3D.Datas.Component;
using System;

namespace BlockBreaker3D.Models.InGame.Component
{
    [Serializable]
    public class DamageCompInfo : Comp
    {
        [SerializeField]
        private int _value;
        private GameDataHolder _gameDataHolder;
        public DamageCompInfo(DamageData data)
            : base(data)
        {
            _value = data.Value;
        }

        public override void OnStart(IObject parent, GameDataHolder dataHolder)
        {
            parent.SetObjectType(parent.ObjectType | ObjectType.Damage);
            _gameDataHolder = dataHolder;
        }
        public override void NotifyCollider(Collider _, IObject otherObject)
        {
            if (otherObject.ObjectType.HasAny(ObjectType.Ball))
            {
                _gameDataHolder.BallBehaviour.TakeDamage(_value);
            }
        }
        public override void NotifyCollision(Collision _, IObject otherObject)
        {
            if (otherObject.ObjectType.HasAny(ObjectType.Ball))
            {
                _gameDataHolder.BallBehaviour.TakeDamage(_value);
            }
        }

        public override void OnRemove(IObject parent)
        {
            parent.SetObjectType(parent.ObjectType & ~ObjectType.Damage);
        }

        public static Comp Create(CompData data)
        {
            if (data is DamageData damage)
            {
                return new DamageCompInfo(damage);
            }
            throw new System.Exception("Invalid CompData type for DamageComp");
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos(Transform parentTransform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(parentTransform.position + Vector3.up * 0.5f, Vector3.one * 0.5f);
        }
#endif
    }
}
