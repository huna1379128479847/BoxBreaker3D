using BlockBreaker3D.Models.InGame;
using Zenject;
using UnityEngine;
using BlockBreaker3D.Models.InGame.Box;

namespace BlockBreaker3D.Zenject
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private BoxBehaviour _boxBehaviour;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateManager>().AsSingle().NonLazy();
            Container.Bind<BoxBehaviour>().FromInstance(_boxBehaviour).AsSingle().NonLazy();
        }
    }
}