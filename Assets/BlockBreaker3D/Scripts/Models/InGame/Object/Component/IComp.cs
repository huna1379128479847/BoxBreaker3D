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

        void OnStart(IObject parent, GameDataHolder dataHolder);
        void OnUpdate(IObject parent, GameDataHolder dataHolder, float deltaTime);
        void OnRemove(IObject parent);
        void OnDestroyObj(IObject parent);

        void NotifyCollider(Collider other, IObject otherObject);
        void NotifyCollision(Collision collision, IObject otherObject);
        void Reset(IObject parent, GameDataHolder dataHolder);

#if UNITY_EDITOR
        void OnDrawGizmos(Transform parentTransform);
#endif
    }
}
