using BlockBreaker3D.ViewModel;
using HighElixir.Implements;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    public class ViewModelInstaller : MonoInstaller
    {
        // 注入順序 SignalInstaller -> SceneInstaller -> GameObject単位のInstaller
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TurnViewModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScoreLeaper>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BlockRemainingViewModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CamViewModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameOverVM>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SurfaceClearViewModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HPViewModel>().AsSingle().NonLazy();
            var dis = Disposable.Empty;
        }
    }
}