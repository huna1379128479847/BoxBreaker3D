namespace BlockBreaker3D.Models.Resource
{
    public interface IResourceLoader
    {
        T LoadResource<T>(string name) where T : class;
    }
}