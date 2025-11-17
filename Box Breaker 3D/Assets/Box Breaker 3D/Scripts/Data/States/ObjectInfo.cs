using System;
using UnityEngine;

namespace BoxBreaker3D.Data
{
    public abstract class ObjectInfo
    {
        private readonly int _id = Guid.NewGuid().GetHashCode();
        public int ID => _id;
        public ObjectType Type { get; set; }

        public abstract Vector3 Position { get; set; }
    }
}