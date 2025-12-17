using BlockBreaker3D.View.InGame;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    public sealed class UIInstaller : MonoInstaller
    {
        [SerializeField] private TMP_Text _turnText;
        [SerializeField] private ScoreView _scoreText;
        [SerializeField] private TMP_Text _blockText;
        [SerializeField] private TurnView _turnView;
        [SerializeField] private TurnHandler _turnHandler;
        [SerializeField] private CinemachineCamera _ballView;
        [SerializeField] private GameOverView _gameOverView;
        [SerializeField] private HPView _hpView;

        public override void InstallBindings()
        {
            Container.Bind<TMP_Text>().WithId("TurnText").FromInstance(_turnText).AsCached();
            Container.Bind(typeof(AbstractScoreView), typeof(IInitializable)).To<ScoreView>().FromInstance(_scoreText).AsCached();
            Container.Bind<TMP_Text>().WithId("BlockText").FromInstance(_blockText).AsCached();
            Container.BindInterfacesAndSelfTo<TurnView>().FromInstance(_turnView).AsCached();
            Container.BindInterfacesAndSelfTo<TurnHandler>().FromInstance(_turnHandler).AsCached();
            Container.Bind<CinemachineCamera>().WithId("BallView").FromInstance(_ballView).AsCached();
            Container.BindInterfacesAndSelfTo<GameOverView>().FromInstance(_gameOverView).AsCached();
            Container.BindInterfacesAndSelfTo<HPView>().FromInstance(_hpView).AsCached();
        }
    }
}