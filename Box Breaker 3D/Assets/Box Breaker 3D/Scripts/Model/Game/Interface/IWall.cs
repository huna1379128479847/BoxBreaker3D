using BoxBreaker3D.Data.Walls;
using System.Collections.Generic;

namespace BoxBreaker3D.Model.Interfaces
{
    // ボールの進行を阻む壁
    public interface IWall : IModel
    {
        void AddWall(WallInfo info);
        void RemoveWall(WallInfo info);
        List<WallInfo> GetWalls();

        // キャッシュ用座標データを計算し、一括更新
        void CalcPosition();
    }
}