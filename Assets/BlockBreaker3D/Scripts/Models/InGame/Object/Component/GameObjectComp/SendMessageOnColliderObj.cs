using BlockBreaker3D.Datas;
using BlockBreaker3D.Utils;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component.GameObjectComp
{
    public class SendMessageOnColliderObj : GameObjectComp
    {
        public class SendMessageCompInfo : Comp
        {
            private string _message;
            private ObjectType _target;
            public SendMessageCompInfo(string message, ObjectType target) : base(true)
            {
                _message = message;
                _target = target;
            }

            public override void NotifyCollider(Collider other, IObject otherObject)
            {
                if (otherObject.ObjectType.HasAny(_target))
                {
                    LogPainter.Debug($"SendMessageOnColliderObj: TargetObjectType={_target}, Message={_message}", BColor.cyan);
                    otherObject.FireMessage(_message);
                }
            }
        }

        [SerializeField] private string _message;
        [SerializeField] private ObjectType _target;
        public override Comp Create()
        {
            return new SendMessageCompInfo(_message, _target);
        }
    }
}