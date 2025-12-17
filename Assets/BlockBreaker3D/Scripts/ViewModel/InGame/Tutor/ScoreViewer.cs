using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Utils.Graphic;
using BlockBreaker3D.View.InGame;
using Cysharp.Threading.Tasks;
using System;
using UniRx;
using Zenject;

namespace BlockBreaker3D.ViewModel.Tutor
{
    public class ScoreViewer : IDisposable
    {
        private IDisposable _disposable;
        public ScoreViewer(SignalBus bus)
        {
            _disposable = bus.GetStream<Message>()
                .Where(message => message.Text == "ShowScore")
                .Subscribe(_ =>
                {
                    bus.Fire(new SetViewVisible(SetViewVisible.ViewType.ScoreView | SetViewVisible.ViewType.LivesView | SetViewVisible.ViewType.TurnHandView, true, false));
                    bus.Fire(SetInputEnable.SetTurn(true));
                    PostProcesses.Fade(PostProcesses.ProcessType.Glitch, 1f, 0f, 1.5f, DG.Tweening.Ease.OutSine).Forget();
                });
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}