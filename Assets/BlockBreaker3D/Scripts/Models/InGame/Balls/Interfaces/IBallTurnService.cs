using System;
using UniRx;

namespace BlockBreaker3D.Models.InGame.Balls.Interfaces
{
    public interface IBallTurnService
    {
        IObservable<bool> OnTurned { get; }
        void Turn(bool isRight = true);
        void Bind(BallBehaviour ball);
    }
}