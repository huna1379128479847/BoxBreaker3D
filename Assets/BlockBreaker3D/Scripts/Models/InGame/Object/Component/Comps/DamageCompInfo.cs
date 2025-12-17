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
        private readonly GameDataHolder _gameDataHolder;
        private readonly IObject _parent;
        public DamageCompInfo(DamageData data, GameDataHolder gameDataHolder, IObject parent)
            : base(data)
        {
            _value = data.Value;
            _gameDataHolder = gameDataHolder;
            _parent = parent;
        }

        public override void OnStart()
        {
            _parent.SetObjectType(_parent.ObjectType | ObjectType.Damage);
        }
        public override void NotifyCollider(Collider _, ObjectType objectType)
        {
            if (objectType == ObjectType.Ball)
            {
                _gameDataHolder.BallBehaviour.TakeDamage(_value);
            }
        }
        public override void NotifyCollision(Collision _, ObjectType objectType)
        {
            if (objectType == ObjectType.Ball)
            {
                _gameDataHolder.BallBehaviour.TakeDamage(_value);
            }
        }

        public override void OnRemove()
        {
            _parent.SetObjectType(_parent.ObjectType & ~ObjectType.Damage);
        }

        public static Comp Create(CompData data, GameDataHolder holder, IObject parent)
        {
            if (data is DamageData damage)
            {
                return new DamageCompInfo(damage, holder, parent);
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
