using BlockBreaker3D.Datas.Component.Balls;
using BlockBreaker3D.Datas.Scriptable;
using BlockBreaker3D.Models.InGame.Balls;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    [CreateAssetMenu(fileName = "BallInject", menuName = "BlockBreaker3D/Scriptable/BallInject")]
    public class BallInject : ScriptableObjectInstaller
    {
        [SerializeField] private AnimationData _animationData;
        [SerializeField] private BallMoverData _ball;
        [SerializeField] private BallCollisionHandlerData _ballCollisionHandlerData;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BallMoverData>().FromInstance(_ball).AsSingle();
            Container.BindInterfacesAndSelfTo<BallCollisionHandlerData>().FromInstance(_ballCollisionHandlerData).AsSingle();
            Container.Bind<AnimationData>().FromInstance(_animationData).AsSingle();
            Debug.Log("BallInject bindings installed.");
        }
    }
}