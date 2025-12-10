namespace BlockBreaker3D.Datas.Signals
{
    public sealed class ScoreSignal
    {
        public enum Operator
        {
            Add,
            Sub,
            Reset
        }

        public Operator ScoreOperator { get; }
        public int Value { get; }

        public ScoreSignal(Operator scoreOperator, int value = 0)
        {
            ScoreOperator = scoreOperator;
            Value = value;
        }
    }
}