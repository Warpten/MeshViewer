using OpenTK;
using System.IO;

namespace MeshViewer.Geometry
{
    public sealed class BIH
    {
        public Vector3 BoundingBoxLow { get; }
        public Vector3 BoundingBoxHigh { get; }

        public uint[] Tree { get; }
        public uint[] Objects { get; }

        public BIH(BinaryReader reader)
        {
            BoundingBoxLow = reader.Read<Vector3>();
            BoundingBoxHigh = reader.Read<Vector3>();

            var treeSize = reader.ReadInt32();
            Tree = reader.Read<uint>(treeSize);

            var objectCount = reader.ReadInt32();
            Objects = reader.Read<uint>(objectCount);
        }
    }
}
