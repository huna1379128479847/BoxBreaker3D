using UnityEngine;

namespace BoxBreaker3D.Data
{
    /// <summary>
    /// 立方体の特定の面上を移動するためのデータ。
    /// </summary>
    public struct BallSurface
    {
        public Surface Face;   // 今いる面
        public Vector2 LocalDir;     // 面の中での進行方向（2Dベクトル）
        public Vector3 Position;

        public BallSurface(Surface face, Vector2 localDir, Vector3 InitialPosition)
        {
            Face = face;
            LocalDir = localDir.normalized;
            Position = InitialPosition;
        }

        /// <summary>
        /// この面における「右」と「上」のワールドベクトルを返す
        /// </summary>
        private (Vector3 right, Vector3 up) GetBasis()
        {
            return Face switch
            {
                Surface.Front => (Vector3.right, Vector3.up),
                Surface.Backward => (Vector3.left, Vector3.up),
                Surface.Top => (Vector3.right, Vector3.back),
                Surface.Bottom => (Vector3.right, Vector3.forward),
                Surface.Right => (Vector3.back, Vector3.up),
                Surface.Left => (Vector3.forward, Vector3.up),
                _ => (Vector3.right, Vector3.up)
            };
        }

        /// <summary>
        /// LocalDir をワールド座標の進行ベクトルに変換する。
        /// </summary>
        public Vector3 GetWorldDirection()
        {
            var (right, up) = GetBasis();
            var dir = right * LocalDir.x + up * LocalDir.y;
            return dir.normalized;
        }

        /// <summary>
        /// 面を跨ぐ時などに、ローカル方向を回転させるための補助。
        /// </summary>
        public void RotateLocalDir90(bool clockwise = true)
        {
            LocalDir = clockwise
                ? new Vector2(LocalDir.y, -LocalDir.x)
                : new Vector2(-LocalDir.y, LocalDir.x);
        }
    }
}
