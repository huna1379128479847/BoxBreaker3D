using BlockBreaker3D.Datas;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component
{
    public interface IComp
    {
        System.Type Type { get; }
        bool ShouldbeRemoved { get; set; }
        bool IsEnableDuplicate { get; }
        bool InitialComp { get; set; }

        void OnStart();
        void OnUpdate(float deltaTime);
        void OnRemove();
        void OnDestroyObj();

        void NotifyCollider(Collider other, ObjectType objectType);
        void NotifyCollision(Collision collision, ObjectType objectType);
        void Reset();

#if UNITY_EDITOR
        void OnDrawGizmos(Transform parentTransform);
#endif
    }
}
