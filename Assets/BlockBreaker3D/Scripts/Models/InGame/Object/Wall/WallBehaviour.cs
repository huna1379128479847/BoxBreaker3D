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

        [Inject]
        protected void Initialize()
        {
            _surface = GetComponentInParent<SurfaceBehaviour>();
            _box = GetComponentInParent<BoxBehaviour>();
        }
        protected override void Start()
        {
            base.Start();
            if (_addWallOnStart)
            {
                SetObjectType(ObjectType | Datas.ObjectType.Wall);
            }
        }
    }
}