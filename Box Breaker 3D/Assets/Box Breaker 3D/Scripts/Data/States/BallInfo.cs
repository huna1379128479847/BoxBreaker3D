using UnityEngine;

namespace BoxBreaker3D.Data
{
    public sealed class BallInfo : ObjectInfo
    {
        private BallSurface _surface;

        public BallSurface BallSurface { get => _surface; set => _surface = value; }
        public override Vector3 Position { get => _surface.Position; set => _surface.Position = value; }
    }
}