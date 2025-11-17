namespace BoxBreaker3D.Model
{
    /// <summary>
    /// 3Dブロック崩しの１つのBoxで使われるコンテキスト。
    /// </summary>
    public sealed class BoxContext
    {
        public int BlockRemaining { get; set; }
        public int CurrentLevel { get; set; }
        public bool IsBonusLevel { get; set; }
    }
}
