namespace BlockBreaker3D.Datas.Signals
{
    public sealed class ObjectCollisionSignal
    {
        public ObjectType Object1 { get; }
        public ObjectType Object2 { get; }
        public string ColliderTag { get; }

        public ObjectCollisionSignal(ObjectType object1, ObjectType object2)
        {
            Object1 = object1;
            Object2 = object2;
            ColliderTag = $"{object1}_{object2}";
        }

        public bool EqualsPairTo(ObjectType obj1, ObjectType obj2)
        {
            return (Object1.HasAny(obj1) && Object2.HasAny(obj2)) || (Object1.HasAny(obj2) && Object2.HasAny(obj1));
        }

        public bool Contains(ObjectType obj)
        {
            return Object1.HasAny(obj) || Object2.HasAny(obj);
        }
    }
}