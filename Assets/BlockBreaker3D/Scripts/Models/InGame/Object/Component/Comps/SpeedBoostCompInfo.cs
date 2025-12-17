using BlockBreaker3D.Datas;
using BlockBreaker3D.Datas.Component;
using BlockBreaker3D.Models.InGame.Balls;
using System;
using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component
{
    [Serializable]
    public class SpeedBoostCompInfo : Comp
    {
        [Serializable]
        public class Speed : Comp
        {
            private string _id;
            [SerializeField] private readonly float _addSpeed;
            [SerializeField] private float _duration;
            private BallBehaviour _behaviour;
            public Speed(BallBehaviour behaviour, float addSpeed, float duration, string id)
                : base(true)
            {
                _behaviour = behaviour;
                _addSpeed = addSpeed;
                _duration = duration;
                _id = id;
            }

            public override void OnStart()
            {
                _behaviour.Speed += _addSpeed;
            }

            public override void OnUpdate(float deltaTime)
            {
                if (_duration > 0)
                {
                    _duration -= deltaTime;
                    if (_duration <= 0)
                    {
                        ShouldbeRemoved = true;
                    }
                }
            }
            public override void OnRemove()
            {
                _behaviour.Speed -= _addSpeed;
            }

            public bool IsSame(string id)
            {
                return _id == id;
            }
        }

        private readonly GameDataHolder _holder;
        private readonly string _compId = System.Guid.NewGuid().ToString();
        [SerializeField] private bool _enableStackingFromSameSource = true;
        [SerializeField] private float _addSpeed;
        [SerializeField] private float _duration;
        public SpeedBoostCompInfo(SpeedBoostData compData, GameDataHolder holder) : base(compData)
        {
            _holder = holder;
            _enableStackingFromSameSource = compData.EnableStackingFromSameSource;
            _addSpeed = compData.AddSpeed;
            _duration = compData.Duration;
        }

        public override void NotifyCollider(Collider other, ObjectType objectType)
        {
            if (objectType == ObjectType.Ball)
            {
                var ball = _holder.BallBehaviour;
                if (ball != null)
                {
                    if (CanAdd(ball))
                        ball.AddComp(new Speed(ball, _addSpeed, _duration, _compId));
                }
            }
        }

        public bool CanAdd(IObject t)
        {
            foreach (var comp in t.GetComps<Speed>())
            {
                if (comp.IsSame(_compId) && !_enableStackingFromSameSource)
                {
                    return false;
                }
            }
            return true;
        }
        public static Comp Create(CompData data, GameDataHolder holder, IObject _)
        {
            if (data is SpeedBoostData sp)
            {
                return new SpeedBoostCompInfo(sp, holder);
            }
            throw new System.Exception("Invalid CompData type for SpeedBoostData");
        }
    }
}