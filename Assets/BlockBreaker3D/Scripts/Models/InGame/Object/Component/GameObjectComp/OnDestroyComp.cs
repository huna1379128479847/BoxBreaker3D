using BlockBreaker3D.Datas.Signals;
using UnityEngine;
using Zenject;
namespace BlockBreaker3D.Models.InGame.Component
{
    public class OnDestroyComp : GameObjectComp.GameObjectComp
    {
        [SerializeField] private string _sendMessage = "";

        private SignalBus _bus;
        [Inject]
        public void Contruct(SignalBus bus)
        {
            _bus = bus;
        }
        public override Comp Create()
        {
            return new OnDestroy(() => _bus.Fire(new Message(_sendMessage)));
        }
    }
}