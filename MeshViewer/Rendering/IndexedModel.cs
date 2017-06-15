using MeshViewer.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MeshViewer.Rendering
{
    public abstract class IndexedModel<T, U> where T : struct where U : struct
    {
        private int IndiceCount { get; set; }
        public int VAO { get; private set; }
        public int VBO { get; private set; }
        public int EBO { get; private set; }

        public bool Valid => GL.IsVertexArray(VAO);

        public PrimitiveType Primitive { get; set; } = PrimitiveType.Triangles;

        protected abstract bool GenerateGeometry(ref T[] vertices, ref U[] indices);
        
        private bool _GenerateGeometry()
        {
            if (Valid)
                return true;

            T[] vertices = null;
            U[] indices = null;
            if (!GenerateGeometry(ref vertices, ref indices))
                return false;

            if (vertices?.Length == 0 || indices?.Length == 0)
                throw new InvalidOperationException();

            IndiceCount = indices.Length;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * SizeCache<T>.Size), vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * SizeCache<U>.Size), indices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, SizeCache<Vector3>.Size, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
            return true;
        }

        public void Render()
        {
            if (!Valid && !_GenerateGeometry())
                return;

            _Render();
        }

        public virtual void _Render()
        {
            GL.BindVertexArray(VAO);
            switch (SizeCache<U>.Size)
            {
                case 4:
                    GL.DrawElements(Primitive, IndiceCount, DrawElementsType.UnsignedInt, 0);
                    break;
                case 2:
                    GL.DrawElements(Primitive, IndiceCount, DrawElementsType.UnsignedShort, 0);
                    break;
                case 1:
                    GL.DrawElements(Primitive, IndiceCount, DrawElementsType.UnsignedByte, 0);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
