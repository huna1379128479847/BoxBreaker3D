namespace BoxBreaker3D.Data.Walls
{
    // 直方体を上から見たとき、Wall(支柱のようなもの)を
    // Boxのどこにクリッピングするかどうか
    public enum WallClipPosition
    {
        Top_UpperLeft = 0,
        Top_UpperRight = 1,
        Top_LowerLeft = 2,
        Top_LowerRight = 3,

        Bottom_UpperLeft = 4,
        Bottom_UpperRight = 5,
        Bottom_LowerLeft = 6,
        Bottom_LowerRight = 7,
    }
}