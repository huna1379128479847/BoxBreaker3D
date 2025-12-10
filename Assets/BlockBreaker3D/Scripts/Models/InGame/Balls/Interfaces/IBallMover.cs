using BlockBreaker3D.Models.InGame.Component;
using UnityEngine;
using Zenject;

namespace BlockBreaker3D.Models.InGame.Balls.Interfaces
{
    public interface IBallMover : IBallComp
    {
        void Reflect();
    }
}