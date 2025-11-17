using UnityEngine;

namespace BoxBreaker3D.Data.Walls
{
    public sealed class WallInfo : ObjectInfo
    {
        private WallPosition _position;

        public WallPosition WallPosition
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                IsDirty = true;
            }
        }

        public override Vector3 Position { get => _position.CachedPosition; set => _position.CachedPosition = value; }
        public bool IsDirty { get; internal set; }
    }
}