using BlockBreaker3D.Core;
using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Models.InGame.Balls.Interfaces;
using BlockBreaker3D.Models.InGame.Component;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Balls
{
    public class RigidbodyMover : Comp, IBallMover
    {
        private BallBehaviour _ball;
        private Transform _transform;
        private Rigidbody _rigidbody;

        public RigidbodyMover()
            : base(false)
        {
        }

        public void Bind(BallBehaviour ball)
        {
            _ball = ball;
            _transform = ball.transform;
            _rigidbody = ball.GetComponent<Rigidbody>();
            Debug.Log("BallMover bound to BallBehaviour.");
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

        // FixedUpdate で呼び出されることを想定
        public override void OnUpdate(float deltaTime)
        {
            if (_ball == null) return;
            Move();
        }

        private void Move()
        {
            var tuple = Surface.DefaultMove(_ball.CurrentSurface);
            var up = _ball.Direction.y * tuple.up;
            var right = _ball.Direction.x * tuple.right;
            var moveVector = GameTimeScale.RelativeTimeScale() * _ball.Speed * (up + right).normalized;
            _rigidbody.linearVelocity = moveVector;
        }
    }
}