using BoxBreaker3D.View;
using UnityEngine;
using Zenject;

namespace BoxBreaker3D.Installer
{
    [CreateAssetMenu(menuName = "BoxBreaker/Installer/BallView")]
    public class BallInstaller : ScriptableObjectInstaller<BallInstaller>
    {
        [SerializeField] private GameObject _ballView;
        public override void InstallBindings()
        {
            Debug.Log("1");
            Container.BindInterfacesAndSelfTo<BallView>()
                .FromComponentInNewPrefab(_ballView)
                .AsSingle().NonLazy();
        }
    }
}