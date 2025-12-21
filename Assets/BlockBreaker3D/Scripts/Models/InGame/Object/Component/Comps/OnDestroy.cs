using System;

namespace BlockBreaker3D.Models.InGame.Component
{
    public class OnDestroy : Comp
    {
        private Action _action;
        public OnDestroy(Action action) : base(true) => _action = action;

        public override void OnDestroyObj(IObject _)
        {
            _action?.Invoke();
        }
    }
}