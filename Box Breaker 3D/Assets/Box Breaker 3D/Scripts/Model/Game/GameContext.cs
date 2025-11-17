namespace BoxBreaker3D.Model
{
    public sealed class GameContext
    {
        public int CurrentLevel { get; set; }
        public int CurrentScore { get; set; }

        // ブロック1つ破壊する毎のスコア
        public int BlockScore { get; set; }
    }
}