using MeshViewer.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MeshViewer.Rendering
{
    public abstract class InstancedIndexedModel<T, U> where T : struct where U : struct
    {
        ~InstancedIndexedModel()
        {
            // GL.DeleteVertexArray(VAO);
            // GL.DeleteBuffer(VBO);
            // GL.DeleteBuffer(EBO);
            // GL.DeleteBuffer(InstancesPositionVBO);
        }

        private int IndiceCount { get; set; }
        public int VAO { get; private set; }
        public int VBO { get; private set; }
        public int EBO { get; private set; }

        public int InstancesPositionVBO { get; private set; }

        public ShaderProgram Program { get; set; }

        public string VerticeAttribute { get; set; }
        public string InstancePositionAttribute { get; set; }

        public bool Valid => GL.IsVertexArray(VAO);

        public PrimitiveType Primitive { get; set; } = PrimitiveType.Triangles;
        public int InstanceCount { get; set; } = 0;

        protected abstract bool BindData(ref T[] vertices, ref U[] indices, ref Matrix4[] instancePositions);

        private bool _GenerateGeometry()
        {
            if (Valid)
                return true;

            T[] vertices = null;
            U[] indices = null;
            Matrix4[] instancePositions = null;
            if (!BindData(ref vertices, ref indices, ref instancePositions))
                return false;

            if (vertices == null || vertices?.Length == 0 || indices?.Length == 0 || instancePositions?.Length == 0 )
                return false;

            // if (indices.Length != 3546)
            //     return false;

            IndiceCount = indices.Length;
            InstanceCount = instancePositions.Length;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * SizeCache<T>.Size), vertices, BufferUsageHint.StaticDraw);

            Program.VertexAttribPointer<Vector3>(VerticeAttribute, 3, VertexAttribPointerType.Float);
            Program.EnableVertexAttribArray<Vector3>(VerticeAttribute);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * SizeCache<U>.Size), indices, BufferUsageHint.StaticDraw);

            InstancesPositionVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, InstancesPositionVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(instancePositions.Length * SizeCache<Matrix4>.Size), instancePositions, BufferUsageHint.StaticDraw);

            Program.VertexAttribPointer<Matrix4>(InstancePositionAttribute, 16, VertexAttribPointerType.Float); // Internally calls x4
            Program.VertexAttribDivisor<Matrix4>(InstancePositionAttribute, 1); // Same
            Program.EnableVertexAttribArray<Matrix4>(InstancePositionAttribute); // Aaand again

            GL.BindVertexArray(0);

            return true;
        }

        public void Render()
        {
            if (!Valid)
                _GenerateGeometry();

            if (Valid)
                _Render();
        }

        public virtual void _Render()
        {
            GL.BindVertexArray(VAO);
            switch (SizeCache<U>.Size)
            {
                case 4:
                    GL.DrawElementsInstanced(Primitive, IndiceCount, DrawElementsType.UnsignedInt, IntPtr.Zero, InstanceCount);
                    break;
                case 2:
                    GL.DrawElementsInstanced(Primitive, IndiceCount, DrawElementsType.UnsignedShort, IntPtr.Zero, InstanceCount);
                    break;
                case 1:
                    GL.DrawElementsInstanced(Primitive, IndiceCount, DrawElementsType.UnsignedByte, IntPtr.Zero, InstanceCount);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
