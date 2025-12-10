using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Balls
{
    public class BallTurnService : IBallTurnService
    {
        private readonly SignalBus _signalBus = null;
        private BallBehaviour _behaviour;

        private readonly Subject<bool> _onTurned = new();

        public IObservable<bool> OnTurned => _onTurned;


        // 注入順序が前後してしまう(このスクリプトはSceneContext, BallBehaviourはGameObjectContext)ため、Bind メソッドで後からバインドする
        // これにより、安全に参照できるのはStart以降になると思われる
        public void Bind(BallBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void Turn(bool isRight)
        {
            Debug.Log($"BallTurnService: Turning {(isRight ? "right" : "left")}");
            // 外からも使うなら property 経由で
            var dir = _behaviour.Direction;
            if (dir == Vector2.zero) return;

            // 右回転か左回転か
            float angle = isRight ? -_behaviour.TurningAngle : _behaviour.TurningAngle;

            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            // 2D回転
            float x = dir.x * cos - dir.y * sin;
            float y = dir.x * sin + dir.y * cos;

            _onTurned.OnNext(isRight);
            _behaviour.Direction = new Vector2(x, y); // setter で normalize される
        }
    }
}