using Zenject;

namespace BlockBreaker3D.Zenject
{
    public class LoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Models.LoadSceneMono>().FromComponentInHierarchy().AsSingle().NonLazy();
        }
    }
}