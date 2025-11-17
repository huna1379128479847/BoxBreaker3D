using BoxBreaker3D.Data;

namespace BoxBreaker3D.Model.Interfaces
{
    public interface ICollisionService
    {
        void OnCollisionEnter(ObjectInfo source, ObjectInfo collision);
    }
}