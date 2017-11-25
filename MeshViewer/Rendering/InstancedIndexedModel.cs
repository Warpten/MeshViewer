using MeshViewer.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace MeshViewer.Rendering
{
    public abstract class IndexedModelBatch<TVertices, TIndices> where TVertices : struct where TIndices : struct
    {
        private int IndiceCount { get; set; }
        public int VertexArray { get; private set; }
        public int _verticesBuffer { get; private set; }
        public int _indicesBuffer { get; private set; }

        public ShaderProgram Program { get; set; }

        public bool Valid => GL.IsVertexArray(VertexArray);

        public PrimitiveType Primitive { get; set; } = PrimitiveType.Triangles;

        protected abstract bool BindData(ref TVertices[] vertices, ref TIndices[] indices);

        private bool _GenerateGeometry()
        {
            if (IndiceCount != 0)
                return true;

            TVertices[] vertices = null;
            TIndices[] indices = null;
            if (!BindData(ref vertices, ref indices))
                return false;

            if (vertices?.Length == 0 || indices?.Length == 0 || indices == null || vertices == null)
                return false;

            IndiceCount = indices.Length;

            _verticesBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _verticesBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * SizeCache<TVertices>.Size), vertices, BufferUsageHint.StaticDraw);

            Program.VertexAttribPointer<Vector3>("vertexPosition_modelSpace", 3, VertexAttribPointerType.Float);
            Program.EnableVertexAttribArray<Vector3>("vertexPosition_modelSpace");

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indicesBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * SizeCache<TIndices>.Size), indices, BufferUsageHint.StaticDraw);

            return true;
        }

        public void CreateBuffers()
        {
            VertexArray = GL.GenVertexArray();
            _verticesBuffer = GL.GenBuffer();
            _indicesBuffer = GL.GenBuffer();
        }

        public void Render(int instanceCount)
        {
            if (!_GenerateGeometry())
                return;

            // There's no need to bind the VAO here - we are already binding it in InstancedBatchedIndexedModel!

            switch (SizeCache<TIndices>.Size)
            {
                case 4:
                    GL.DrawElementsInstanced(Primitive, IndiceCount, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceCount);
                    break;
                case 2:
                    GL.DrawElementsInstanced(Primitive, IndiceCount, DrawElementsType.UnsignedShort, IntPtr.Zero, instanceCount);
                    break;
                case 1:
                    GL.DrawElementsInstanced(Primitive, IndiceCount, DrawElementsType.UnsignedByte, IntPtr.Zero, instanceCount);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public virtual void _Render()
        {
            throw new InvalidOperationException();
        }
    }
}
