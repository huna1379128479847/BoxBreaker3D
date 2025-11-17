using BoxBreaker3D.Model;
using BoxBreaker3D.Model.Interfaces;
using UnityEngine;
using Zenject;

namespace BoxBreaker3D.Installer
{
    public class BoxInstaller : MonoInstaller
    {
        [SerializeReference] private BoxBase _box;
        public override void InstallBindings()
        {
            Container.Bind<BoxContext>().AsSingle();
            Container.Bind<IBox>().FromComponentOn(_box.gameObject).AsSingle();
        }
    }
}