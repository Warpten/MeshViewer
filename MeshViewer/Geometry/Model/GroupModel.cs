using OpenTK;
using System.IO;
using MeshViewer.Rendering;
using MeshViewer.Memory;

namespace MeshViewer.Geometry.Model
{
    public sealed class GroupModel : IndexedModelBatch<Vector3, uint>
    {
        public Vector3[] _modelVertices { get; private set; }
        public uint[] _modelIndices { get; private set; }

        public GroupModel(BinaryReader reader)
        {
            reader.BaseStream.Position += 2 * SizeCache<Vector3>.Size; // BBox
            var mogpFlags = reader.ReadInt32();
            var groupWmoID = reader.ReadInt32();

            if (reader.ReadInt32() == 0x54524556) // VERT
            {
                var chunkSize = reader.ReadInt32();
                var count = reader.ReadInt32();
                if (count == 0)
                    return;

                _modelVertices = reader.Read<Vector3>(count);
            }

            if (reader.ReadInt32() == 0x4D495254) // TRIM
            {
                var chunkSize = reader.ReadInt32();
                var count = reader.ReadInt32();

                _modelIndices = reader.Read<uint>(count * 3);
            }

            if (reader.ReadInt32() == 0x4849424D) // MBIH
                BIH.Skip(reader);

            if (reader.ReadInt32() == 0x5551494C) // LIQU
            {
                var chunkSize = reader.ReadInt32();
                if (chunkSize == 0)
                    return;

                var tileX = reader.ReadInt32();
                var tileY = reader.ReadInt32();
                reader.BaseStream.Position += SizeCache<Vector3>.Size;
                reader.BaseStream.Position += 4;
                reader.BaseStream.Position += 4 * ( tileX + 1 ) * ( tileY + 1 );
                reader.BaseStream.Position += tileX * tileY;
            }
        }

        protected override bool BindData(ref Vector3[] vertices, ref uint[] indices)
        {
            if (_modelVertices == null || _modelIndices == null)
                return true;

            vertices = _modelVertices;
            indices = _modelIndices;

            _modelVertices = null;
            _modelIndices = null;

            return true;
        }
        
        public void InvertIndices()
        {
            if (_modelIndices == null)
                return;

            for (var i = 0; i < _modelIndices.Length; i += 3)
            {
                var tmp = _modelIndices[i];
                _modelIndices[i] = _modelIndices[i + 2];
                _modelIndices[i + 2] = tmp;
            }
        }
    }
}
