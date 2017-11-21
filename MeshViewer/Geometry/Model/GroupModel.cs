using OpenTK;
using System.IO;
using MeshViewer.Rendering;
using MeshViewer.Memory;
using System.Collections.Generic;

namespace MeshViewer.Geometry.Model
{
    public sealed class GroupModel : InstancedIndexedModel<Vector3, uint>
    {
        // public BIH Tree { get; }
        public Vector3[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }

        private List<Matrix4> _instancePositions = new List<Matrix4>();
        public List<Matrix4> Positions
        {
            get
            {
                lock (_instancePositions)
                    return _instancePositions;
            }
        }

        public GroupModel(BinaryReader reader)
        {
            var bbox = reader.Read<float>(6);
            // var boundingBoxLo = reader.Read<Vector3>();
            // var boundingBoxHi = reader.Read<Vector3>();
            var mogpFlags = reader.ReadInt32();
            var groupWmoID = reader.ReadInt32();

            if (reader.ReadInt32() == 0x54524556) // VERT
            {
                var chunkSize = reader.ReadInt32();
                var count = reader.ReadInt32();
                if (count == 0)
                    return;

                Vertices = reader.Read<Vector3>(count);
            }

            if (reader.ReadInt32() == 0x4D495254) // TRIM
            {
                var chunkSize = reader.ReadInt32();
                var count = reader.ReadInt32();

                Indices = reader.Read<uint>(count * 3);
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

        public void AddInstance(ref Matrix4 spawn)
        {
            lock (_instancePositions)
                _instancePositions.Add(spawn);
        }

        public void RemoveInstance(ref Matrix4 spawn)
        {
            lock (_instancePositions)
                _instancePositions.Remove(spawn);
        }

        protected override bool BindData(ref Vector3[] vertices, ref uint[] indices, ref Matrix4[] instancePositions)
        {
            vertices = Vertices;
            indices = Indices;
            lock (_instancePositions)
                instancePositions = _instancePositions.ToArray();

            Vertices = null;
            Indices = null;
            // _instancePositions = null;

            return true;
        }
        
        public void InvertIndices()
        {
            if (Indices == null)
                return;

            for (var i = 0; i < Indices.Length; i += 3)
            {
                var tmp = Indices[i];
                Indices[i] = Indices[i + 2];
                Indices[i + 2] = tmp;
            }
        }
    }
}
