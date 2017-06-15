using MeshViewer.Memory;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshViewer.Rendering
{
    public abstract class InstancedIndexedModel<T, U> where T : struct where U : struct
    {
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

        protected abstract bool GenerateGeometry(ref T[] vertices, ref U[] indices, ref Matrix4[] instancePositions);

        private bool _GenerateGeometry()
        {
            if (Valid)
                return true;

            T[] vertices = null;
            U[] indices = null;
            Matrix4[] instancePositions = null;
            if (!GenerateGeometry(ref vertices, ref indices, ref instancePositions))
                return false;

            if (vertices == null || vertices?.Length == 0 || indices?.Length == 0 || instancePositions?.Length == 0 )
                return false;

            IndiceCount = indices.Length;

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * SizeCache<T>.Size), vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * SizeCache<U>.Size), indices, BufferUsageHint.StaticDraw);

            InstancesPositionVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, InstancesPositionVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(instancePositions.Length * SizeCache<Matrix4>.Size), instancePositions, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VAO);

            GL.VertexAttribPointer(Program.Attributes[VerticeAttribute], 3, VertexAttribPointerType.Float, false, SizeCache<Vector3>.Size, 0);
            GL.EnableVertexAttribArray(Program.Attributes[VerticeAttribute]);
            
            GL.VertexAttribPointer(Program.Attributes[InstancePositionAttribute] + 0, 4, VertexAttribPointerType.Float, false, SizeCache<Matrix4>.Size, 0);
            GL.VertexAttribPointer(Program.Attributes[InstancePositionAttribute] + 1, 4, VertexAttribPointerType.Float, false, SizeCache<Matrix4>.Size, 16);
            GL.VertexAttribPointer(Program.Attributes[InstancePositionAttribute] + 2, 4, VertexAttribPointerType.Float, false, SizeCache<Matrix4>.Size, 24);
            GL.VertexAttribPointer(Program.Attributes[InstancePositionAttribute] + 3, 4, VertexAttribPointerType.Float, false, SizeCache<Matrix4>.Size, 32);
            GL.EnableVertexAttribArray(Program.Attributes[InstancePositionAttribute] + 0);
            GL.EnableVertexAttribArray(Program.Attributes[InstancePositionAttribute] + 1);
            GL.EnableVertexAttribArray(Program.Attributes[InstancePositionAttribute] + 2);
            GL.EnableVertexAttribArray(Program.Attributes[InstancePositionAttribute] + 3);
            GL.VertexAttribDivisor(Program.Attributes[InstancePositionAttribute] + 0, 1);
            GL.VertexAttribDivisor(Program.Attributes[InstancePositionAttribute] + 1, 1);
            GL.VertexAttribDivisor(Program.Attributes[InstancePositionAttribute] + 2, 1);
            GL.VertexAttribDivisor(Program.Attributes[InstancePositionAttribute] + 3, 1);

            GL.BindVertexArray(0);

            InstanceCount = instancePositions.Length;
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
