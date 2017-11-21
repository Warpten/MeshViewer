using OpenTK;
using System.IO;
using MeshViewer.Rendering;

namespace MeshViewer.Geometry.Model
{
    public sealed class ModelSpawn
    {
        public uint Flags { get; }
        public ushort AdtID { get; }
        public uint ID { get; }

        private Matrix4 _positionMatrix;

        public Vector3 BoundingBoxLow { get; }
        public Vector3 BoundingBoxHigh { get; }

        public string Name { get; }

        public bool HasBoundingBox => (Flags & 0x4) != 0;
        public bool Valid { get; }

        private WorldModel Model { get; }

        public ModelSpawn(string directory, BinaryReader reader)
        {
            Valid = false;
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return;

            Valid = true;
            Flags = reader.ReadUInt32();
            AdtID = reader.ReadUInt16();
            ID = reader.ReadUInt32();

            var position = reader.Read<Vector3>();
            position.X -= 32 * 533.333313f;
            position.Y -= 32 * 533.333313f;

            var rotation = reader.Read<Vector3>();
            var rotationMatrix = RenderingExtensions.FromEulerAngles(rotation.Z * MathHelper.Pi / 180.0f,
                rotation.X * MathHelper.Pi / 180.0f,
                rotation.Y * MathHelper.Pi / 180.0f);

            var translationMatrix = Matrix4.CreateTranslation(position);
            var scaleMatrix = Matrix4.CreateScale(reader.ReadSingle());

            _positionMatrix = rotationMatrix * scaleMatrix * translationMatrix;

            if (HasBoundingBox)
                reader.BaseStream.Position += 4 * 6; // BBox

            var nameLength = reader.ReadInt32();
            Name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(nameLength));

            Model = WorldModelCache.OpenInstance(directory, Name);
            Model.AddInstance(ref _positionMatrix);
        }

        public void Render() => Model.Render();

        public void InvertIndices() => Model.InvertIndices();
    }
}
