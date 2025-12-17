using BlockBreaker3D.Models.InGame.Balls;
using BlockBreaker3D.Models.InGame.Box;
using BlockBreaker3D.Models.InGame.GameStatus;
using UniRx;
using Zenject;

namespace BlockBreaker3D.Models.InGame
{
    public sealed class GameDataHolder
    {
        private BallBehaviour _ballBehaviour;
        private readonly ReactiveProperty<BoxBehaviour> _boxBehaviour = new();

        public ScoreHolder ScoreHolder { get; private set; } 

        // ゲーム内で共通して参照される
        public BallBehaviour BallBehaviour => _ballBehaviour;
        public IReadOnlyReactiveProperty<BoxBehaviour> BoxBehaviour => _boxBehaviour;
        public SignalBus SignalBus { get; set; }

        public GameDataHolder(SignalBus signalBus, ScoreHolder scoreHolder)
        {
            SignalBus = signalBus;
            ScoreHolder = scoreHolder;
        }

        public void SetScoreHolder(ScoreHolder scoreHolder)
        {
            ScoreHolder = scoreHolder;
        }

        public void BindBall(BallBehaviour ballBehaviour) => _ballBehaviour = ballBehaviour;
        public void BindBox(BoxBehaviour boxBehaviour) => _boxBehaviour.Value = boxBehaviour;
    }
}