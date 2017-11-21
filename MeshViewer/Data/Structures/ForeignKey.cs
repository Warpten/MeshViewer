using DBFilesClient.NET.Types;

namespace MeshViewer.Data.Structures
{
    public sealed class ForeignKey<T> : IObjectType<int>
    {
        public ForeignKey(int underlyingValue) : base(underlyingValue)
        {
        }

        public T Value { get; }
    }
}
