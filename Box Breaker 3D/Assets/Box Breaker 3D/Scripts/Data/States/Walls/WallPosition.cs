using UnityEngine;

namespace BoxBreaker3D.Data.Walls
{
    // 壁1つが持つデータ。これをVMが参照し、3Dモデルを配置する
    public struct WallPosition
    {
        public WallClipPosition ClipPosition;
        public Vector3 LocalPosition; 
        public Vector3 Length;

        // Box管理クラスがClipPositionをもとに位置を計算し、ここにキャッシュする
        public Vector3 CachedPosition;
    }
}