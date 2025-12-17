using BlockBreaker3D.View;
using BlockBreaker3D.View.InGame;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    // UIInstallerより後に実行される必要がある
    public sealed class AnimInstaller : MonoInstaller
    {
        [SerializeField] private CameraLookUp _lookup;
        [SerializeField] private SoundManager _soundManager;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SoundManager>().FromInstance(_soundManager).AsSingle();
            Container.BindInterfacesAndSelfTo<CameraLookUp>().FromInstance(_lookup).AsSingle();
        }
    }
}