using BoxBreaker3D.Data;
using System;

namespace BoxBreaker3D.Model.Interfaces
{
    // Boxの処理を行う
    public interface IBox : IModel
    {
        IWall Wall { get; }
        IObservable<BoxState> State { get; }

        void EnterBox(GameContext context, IBall ball);
        void ExitBox();
    }
}