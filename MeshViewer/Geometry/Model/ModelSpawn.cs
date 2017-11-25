using OpenTK;
using System.IO;
using MeshViewer.Rendering;

namespace MeshViewer.Geometry.Model
{
    public class ModelSpawn
    {
        public string ModelName { get; }
        public Matrix4 PositionMatrix { get; }

        public ModelSpawn(string directory, BinaryReader reader)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return;

            var flags = reader.ReadInt32();
            reader.BaseStream.Position += 2 + 4;

            var position = reader.Read<Vector3>();
            position.X -= 32 * 533.333313f;
            position.Y -= 32 * 533.333313f;

            var rotation = reader.Read<Vector3>();
            var rotationMatrix = RenderingExtensions.FromEulerAngles(rotation.Z * MathHelper.Pi / 180.0f,
                rotation.X * MathHelper.Pi / 180.0f,
                rotation.Y * MathHelper.Pi / 180.0f);

            var translationMatrix = Matrix4.CreateTranslation(position);
            var scaleMatrix = Matrix4.CreateScale(reader.ReadSingle());

            PositionMatrix = rotationMatrix * scaleMatrix * translationMatrix;

            if ((flags & 0x4) != 0)
                reader.BaseStream.Position += 4 * 6; // BBox

            var nameLength = reader.ReadInt32();
            ModelName = string.Intern(System.Text.Encoding.UTF8.GetString(reader.ReadBytes(nameLength)));
        }
    }
}
