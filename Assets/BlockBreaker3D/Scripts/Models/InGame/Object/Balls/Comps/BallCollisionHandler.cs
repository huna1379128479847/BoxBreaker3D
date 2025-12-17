using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using BlockBreaker3D.Models.InGame.Component;
using System.Text;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Balls
{

    public class BallCollisionHandler : Comp, IBallCollisionHandler
    {
        private readonly SignalBus _signalBus;
        private BallBehaviour _behaviour;

        [Inject]
        public BallCollisionHandler(SignalBus signalBus, BallBehaviour behaviour)
            : base(false)
        {
            _behaviour = behaviour;
            _signalBus = signalBus;
        }

        public override void NotifyCollision(Collision collision, ObjectType objectType)
        {
            if (_behaviour == null) return;

            if (objectType.HasAny(ObjectType.Wall))
            {
                ReflectOnWall(collision);
                _signalBus?.Fire(new ObjectCollisionSignal(ObjectType.Ball, objectType));
            }
            if (objectType.HasAny(ObjectType.Block))
            {
                ReflectOnWall(collision);
                _signalBus?.Fire(new ObjectCollisionSignal(ObjectType.Ball, objectType));
            }
        }

        private void ReflectOnWall(Collision col)
        {
            var hitNormalOpt = col.GetContact(0).normal;
            var (up, right) = Surface.DefaultMove(_behaviour.CurrentSurface);
            var dir2 = _behaviour.Direction;
            if (dir2 == Vector2.zero) return;

            var worldDir = (up * dir2.y + right * dir2.x).normalized;

            Vector3 normal = hitNormalOpt.normalized; // ★ ここで衝突法線を優先


            var reflectedWorldDir = Vector3.Reflect(worldDir, normal).normalized;
            float a = Vector3.Dot(reflectedWorldDir, up);
            float b = Vector3.Dot(reflectedWorldDir, right);

            var sb = new StringBuilder();
            sb.AppendLine($"<color=blue>Before</color> {_behaviour.Direction}");
            _behaviour.Direction = new Vector2(b, a).normalized;
            sb.AppendLine($"<color=blue>After</color> {_behaviour.Direction}");
            //Debug.Log(sb.ToString());
        }

        public static Comp Create(CompData _, GameDataHolder holder, IObject parent)
        {
            if (parent is BallBehaviour ball)
            {
                return new BallCollisionHandler(holder.SignalBus, ball);
            }
            throw new System.Exception("Invalid CompData type for DamageComp");
        }
    }
}
