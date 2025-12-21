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
            private string _messageKey;
            private GameObject _parent;
            public ReceiveMessage(string message) : base(true)
            {
                _messageKey = message;
            }
            public override void OnStart(IObject parent, GameDataHolder holder)
            {
                Debug.Log($"[ReceiveMessage] OnStart: Subscribing to message '{_messageKey}'.");
                if (parent is ObjectBase @base)
                {
                    _parent = @base.gameObject;
                }
                else
                {
                    Debug.LogError("[ReceiveMessage] OnStart: Parent is not ObjectBase.");
                    return;
                }
                holder.SignalBus.GetStream<Message>()
                    .Where(x => x.Text == _messageKey)
                    .Subscribe(OnReceiveMessage);
            }

            private void OnReceiveMessage(Message message)
            {
                if (message.Text == _messageKey)
                {
                    Debug.Log($"[ReceiveMessage] Received message: {message.Text}, activating GameObject.");
                    _parent.SetActive(true);
                }
            }
        }

        [SerializeField]
        private string _messageKey;

        public override Comp Create()
        {
            Debug.Log($"[ReceiveMessageGO] Creating ReceiveMessage Comp with key '{_messageKey}'.");
            return new ReceiveMessage(_messageKey);
        }
    }
}