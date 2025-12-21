using UnityEngine;

namespace BlockBreaker3D.Zenject
{
    [CreateAssetMenu(fileName = "GlobalSystemInstaller", menuName = "BlockBreaker3D/Scriptables/GlobalSystemInstaller")]
    public class GlobalSystemInstaller : global::Zenject.ScriptableObjectInstaller<GlobalSystemInstaller>
    {
        public override void InstallBindings()
        {
            // Bind IResourceLoader to AudioLoader as a singleton
            Container.Bind<Models.Sounds.AudioLoader>().AsSingle();
            Container.Bind<Models.Resource.SceneLoader>().AsSingle().NonLazy();
        }
    }
}