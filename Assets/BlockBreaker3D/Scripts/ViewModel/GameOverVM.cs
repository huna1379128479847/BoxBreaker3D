using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.GameStatus;
using BlockBreaker3D.View.InGame;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

namespace BlockBreaker3D.ViewModel
{
    public class GameOverVM
    {
        private ScoreHolder _scoreHolder;
        private GameOverView _view;
        [Inject]
        public GameOverVM(SignalBus bus, ScoreHolder holder, GameOverView view)
        {
            _scoreHolder = holder;
            _view = view;
            bus.GetStream<GameSignal>()
                .Subscribe(SignelReceived);
        }

        private void SignelReceived(GameSignal signal)
        {
            if (signal.HasAny(GameSignal.Type.GameStarted, GameSignal.Type.Restart))
            {
                _view.Disable();
            }
            else if (signal.HasAny(GameSignal.Type.GameOver))
            {
                _view.Enable();
                _view.UpdateScore(_scoreHolder.Score.Value);
                _view.PlayGameOverAnim().Forget();
            }
        }
    }
}