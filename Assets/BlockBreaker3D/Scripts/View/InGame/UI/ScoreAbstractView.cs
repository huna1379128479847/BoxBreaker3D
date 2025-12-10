namespace BlockBreaker3D.View.InGame
{
    public abstract class AbstractScoreView : GameViewBase, IGameView
    {
        public abstract void UpdateScore(int newScore);
    }
}