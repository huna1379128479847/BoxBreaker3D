using BlockBreaker3D.ViewModel;
using Zenject;
using UnityEngine;
using UnityEngine.UI;
namespace BlockBreaker3D.Datas
{
    public class TitleInstaller : MonoInstaller
    {
        [SerializeField] private Button _button;
        public override void InstallBindings()
        {
            Container.Bind<Button>().WithId("GoMenu").FromInstance(_button).AsCached();
            Container.Bind<LoadSystemConnector>().AsSingle().NonLazy();
        }
    }
}