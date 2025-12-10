using System;
using UnityEngine;

namespace BlockBreaker3D.Datas
{
    [Flags]
    public enum ObjectType
    {
        Unknown = 0,
        Ball = 1 << 0,
        Block = 1 << 1,
        Paddle = 1 << 2,
        Wall = 1 << 3,
        Damage = 1 << 4,
    }

    public static class ObjectTypeExtensions
    {
        public static bool HasType(this ObjectType objectType, ObjectType checkType)
        {
            return (objectType & checkType) == checkType;
        }

        public static bool HasAny(this ObjectType objectType, ObjectType checkType)
        {
           return (objectType & checkType) != 0;
        }
    }
}
