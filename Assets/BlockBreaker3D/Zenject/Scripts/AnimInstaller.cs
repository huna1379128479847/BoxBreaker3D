using BlockBreaker3D.View.InGame;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    // UIInstallerより後に実行される必要がある
    public sealed class AnimInstaller : MonoInstaller
    {
        [SerializeField] private CameraLookUp _lookup;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CameraLookUp>().FromInstance(_lookup).AsSingle();
        }
    }
}