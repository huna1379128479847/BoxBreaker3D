using BlockBreaker3D.Models.InGame;
using BlockBreaker3D.Models.InGame.Balls;
using BlockBreaker3D.Models.InGame.GameStatus;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{

    public class ModelServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ScoreHolder>().AsSingle().NonLazy();
            Container.Bind<GameDataHolder>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BallTurnService>().AsSingle();
            Debug.Log("UIInstaller bindings installed.");
        }
    }
}