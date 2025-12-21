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
            public Speed(float addSpeed, float duration, string id)
                : base(true)
            {
                _addSpeed = addSpeed;
                _duration = duration;
                _id = id;
            }

            public override void OnStart(IObject parent, GameDataHolder holder)
            {
                if (parent is BallBehaviour ball)
                    ball.Speed += _addSpeed;
            }

            public override void OnUpdate(IObject parent, GameDataHolder holder, float deltaTime)
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
            public override void OnRemove(IObject parent)
            {
                if (parent is BallBehaviour ball)
                    ball.Speed -= _addSpeed;
            }

            public bool IsSame(string id)
            {
                return _id == id;
            }
        }

        private readonly string _compId = System.Guid.NewGuid().ToString();
        [SerializeField] private bool _enableStackingFromSameSource = true;
        [SerializeField] private float _addSpeed;
        [SerializeField] private float _duration;
        public SpeedBoostCompInfo(SpeedBoostData compData) : base(compData)
        {
            _enableStackingFromSameSource = compData.EnableStackingFromSameSource;
            _addSpeed = compData.AddSpeed;
            _duration = compData.Duration;
        }

        public override void NotifyCollider(Collider other, IObject otherObj)
        {
            if (otherObj.ObjectType.HasAny(ObjectType.Ball) &&
                otherObj is BallBehaviour ball)
            {
                if (CanAdd(ball))
                    ball.AddComp(new Speed(_addSpeed, _duration, _compId));
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
        public static Comp Create(CompData data)
        {
            if (data is SpeedBoostData sp)
            {
                return new SpeedBoostCompInfo(sp);
            }
            throw new System.Exception("Invalid CompData type for SpeedBoostData");
        }
    }
}