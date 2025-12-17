using Zenject;
using UnityEngine;
using BlockBreaker3D.Datas.Signals;
using UniRx;

namespace BlockBreaker3D.Models.InGame.Component.GameObjectComp
{
    public class ReceiveMessageGO : GameObjectComp
    {
        public class ReceiveMessage : Comp
        {
            private SignalBus _signalBus;
            private string _messageKey;
            public ReceiveMessage(SignalBus bus, string message) : base(true)
            {
                _signalBus = bus;
                _messageKey = message;
            }
            public override void OnStart()
            {
                _signalBus.GetStream<Message>()
                    .Where(x => x.Text == _messageKey)
                    .Subscribe(OnReceiveMessage);
            }

            private void OnReceiveMessage(Message message)
            {
                if (message.Text == _messageKey)
                {
                    // 仮で親オブジェクトの有効化だけ
                    
                }
            }
        }

        [SerializeField]
        private string _messageKey;
        private SignalBus _bus;

        
        [Inject]
        public void Construct(SignalBus bus) => _bus = bus;
        public override Comp Create()
        {
            return new ReceiveMessage(_bus, _messageKey);
        }
    }
}