using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Datas.Signals;
using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using BlockBreaker3D.Models.InGame.Component;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Balls
{

    public class BallCollisionHandler : Comp, IBallCollisionHandler
    {
        private SignalBus _signalBus;
        private BallBehaviour _behaviour;

        [Inject]
        public BallCollisionHandler() : base(false) { }

        public override void OnStart(IObject parent, GameDataHolder dataHolder)
        {
            if (parent is BallBehaviour ball)
            {
                _behaviour = ball;
                _signalBus = dataHolder.SignalBus;
            }
        }

        public override void NotifyCollision(Collision other, IObject otherObject)
        {
            if (_behaviour == null) return;
            if (otherObject == null) return;

            // Wall と Block の両方で同じ処理を行うためまとめる
            if (otherObject.ObjectType.HasAny(ObjectType.Wall | ObjectType.Block))
            {
                ReflectOnWall(other);
                _signalBus?.Fire(new ObjectCollisionSignal(ObjectType.Ball, otherObject.ObjectType));
            }
        }

        private void ReflectOnWall(Collision col)
        {
            if (col == null || col.contacts == null || col.contacts.Length == 0) return;

            // 複数の接触点がある場合は法線を重み付き平均して継ぎ目による不安定な法線を平滑化する
            Vector3 avgNormal = Vector3.zero;
            float totalWeight = 0f;
            foreach (var cp in col.contacts)
            {
                // separation が負ならめり込んでいるのでその深さを重みとする
                float weight = 1f;
                try
                {
                    // ContactPoint.separation may be present depending on Unity version
                    weight += Mathf.Max(0f, -cp.separation);
                }
                catch { }
                avgNormal += cp.normal.normalized * weight;
                totalWeight += weight;
            }
            if (totalWeight > 0f) avgNormal /= totalWeight;
            var normal = avgNormal.normalized;

            var (up, right) = Surface.DefaultMove(_behaviour.CurrentSurface);
            var dir2 = _behaviour.Direction;
            if (dir2.sqrMagnitude <= Mathf.Epsilon) return;

            var worldDirUnnorm = up * dir2.y + right * dir2.x;
            if (worldDirUnnorm.sqrMagnitude <= Mathf.Epsilon) return;
            var worldDir = worldDirUnnorm.normalized;

            var reflectedWorldDir = Vector3.Reflect(worldDir, normal).normalized;
            float a = Vector3.Dot(reflectedWorldDir, up);
            float b = Vector3.Dot(reflectedWorldDir, right);

            var newDir = new Vector2(b, a);
            if (newDir.sqrMagnitude > Mathf.Epsilon)
                _behaviour.Direction = newDir.normalized;
        }

        public static Comp Create(CompData _)
        {
            return new BallCollisionHandler();
        }
    }
}
