namespace MKW.Common
{
    [Serializable]
    public class ResourceNotOwnedException()
        : Exception("Object is not owned by the resource handler.")
    {
    }
}
