using BlockBreaker3D.Models.InGame.Box;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Wall
{
    public class WallBehaviour : ObjectBase
    {
        [SerializeField, Tooltip("Trueなら開始時にWall属性を追加")]
        private bool _addWallOnStart = true;

        protected override void OnReset()
        {
            // 何もしない
        }

        protected override void PostInitialize()
        {
            _surface = GetComponentInParent<SurfaceBehaviour>();
            _box = GetComponentInParent<BoxBehaviour>();
            if (_addWallOnStart)
            {
                SetObjectType(ObjectType | Datas.ObjectType.Wall);
            }
        }
    }
}