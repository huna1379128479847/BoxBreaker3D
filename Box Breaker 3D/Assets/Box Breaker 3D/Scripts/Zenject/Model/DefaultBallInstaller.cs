using BoxBreaker3D.Model;
using BoxBreaker3D.ViewModel;
using UnityEngine;
using Zenject;

namespace BoxBreaker3D.Installer
{
    [CreateAssetMenu(menuName = "BoxBreaker/Installer/Default")]
    public class DefaultInstaller : ScriptableObjectInstaller<DefaultInstaller>
    {
        public override void InstallBindings()
        {
            Debug.Log("2");
            Container.BindInterfacesAndSelfTo<DefaultBall>().AsSingle();
            Container.Bind<BallViewModel>().AsSingle();
            Container.Bind<GameContext>().AsSingle();
        }
    }
}