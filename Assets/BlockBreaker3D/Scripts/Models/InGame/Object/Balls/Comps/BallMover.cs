using BlockBreaker3D.Core;
using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using BlockBreaker3D.Models.InGame.Component;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Balls
{
    // TODO : BallComp系をすべて Comp ベースに移行する☑
    // 曲がり入力中、スロー演出をかけてプレイヤーにわかりやすくする☑
    // 面の端に残りブロック数を示す数字を表示する☑
    // 今いる面とほかの面にあるブロックは、半透明にして見えるようにする☑
    // 今いる面のブロックをすべて破壊したとき、ズームイン＆スロー演出をかける☑
    public class BallMover : Comp, IBallMover
    {
        private BallBehaviour _ball;
        private Transform _transform;
        private Rigidbody _rigidbody;

        public BallMover() : base(false) { }

        public override void OnStart(IObject parent, GameDataHolder dataHolder)
        {
            if (parent is BallBehaviour ball)
            {
                _ball = ball;
                _transform = ball.transform;
                _rigidbody = ball.GetComponent<Rigidbody>();
                Debug.Log("BallMover bound to BallBehaviour.");
            }
        }

        public void Reflect()
        {
            if (_ball == null) return;
            // Simple fallback reflection: invert both local X and Y components of direction.
            // More advanced collision-based reflection is handled by BallCollisionHandler.
            _ball.ReverseX();
            _ball.ReverseY();
            Debug.Log("BallMover.Reflect: direction inverted.");
        }

        public override void OnUpdate(IObject parent, GameDataHolder holder, float deltaTime)
        {
            if (_ball == null) return;
            _rigidbody.MovePosition(GetNextPosition(deltaTime));
            //_transform.position = GetNextPosition(deltaTime);
        }

        private Vector3 GetNextPosition(float deltaTime)
        {
            var tuple = Surface.DefaultMove(_ball.CurrentSurface);
            var up = _ball.Direction.y * tuple.up;
            var right = _ball.Direction.x * tuple.right;
            var moveVector = _ball.Speed * (up + right);
            return _transform.position + moveVector * deltaTime;
        }

        public static Comp Create(CompData _)
        {
            return new BallMover();
        }
    }
}