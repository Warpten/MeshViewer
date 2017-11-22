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

        public ShaderProgram Program { get; set; }

        public PrimitiveType Primitive { get; set; } = PrimitiveType.Triangles;

        protected abstract bool BindData(ref T[] vertices, ref U[] indices);

        ~IndexedModel()
        {
            // Unload();
        }
    
        private bool _GenerateGeometry()
        {
            if (Valid)
                return true;

            T[] vertices = null;
            U[] indices = null;
            if (!BindData(ref vertices, ref indices))
                return false;

            if (vertices == null || indices == null || vertices?.Length == 0 || indices?.Length == 0)
                throw new InvalidOperationException();

            IndiceCount = indices.Length;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * SizeCache<T>.Size), vertices, BufferUsageHint.StaticDraw);

            Program.VertexAttribPointer<Vector3>("vertexPosition_modelSpace", 3, VertexAttribPointerType.Float, false);
            GL.EnableVertexAttribArray(0);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * SizeCache<U>.Size), indices, BufferUsageHint.StaticDraw);
            
            GL.BindVertexArray(0);
            return true;
        }

        public virtual void Unload()
        {
            if (GL.IsBuffer(VBO))
                GL.DeleteBuffer(VBO);

            if (GL.IsBuffer(EBO))
                GL.DeleteBuffer(EBO);

            if (GL.IsVertexArray(VAO))
                GL.DeleteVertexArray(VAO);
        }

        public void Render()
        {
            if (!Valid)
                _GenerateGeometry();

            if (Valid)
                _Render();
        }

        protected virtual void _Render()
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
