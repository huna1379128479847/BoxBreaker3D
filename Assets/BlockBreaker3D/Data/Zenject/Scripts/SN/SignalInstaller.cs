using BlockBreaker3D.Datas.Signals;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    [CreateAssetMenu(fileName = "SignalInstaller", menuName = "BlockBreaker3D/Scriptable/SignalInstaller")]
    public class SignalInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<Message>();
            Container.DeclareSignal<ObjectCollisionSignal>().OptionalSubscriber();
            Container.DeclareSignal<InputSignal>();
            Container.DeclareSignal<ScoreSignal>();
            Container.DeclareSignal<GameSignal>();
            Container.DeclareSignal<RequirePlaySound>();
            Container.DeclareSignal<SetViewVisible>();
            Container.DeclareSignal<SetInputEnable>();
        }
    }
}