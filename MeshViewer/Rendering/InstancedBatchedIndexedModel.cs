using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using MeshViewer.Memory;
using System.Collections.Concurrent;

namespace MeshViewer.Rendering
{
    public class InstancedBatchedIndexedModel<TIndice, TVertice, TInstance> where TIndice : struct where TVertice : struct where TInstance : struct
    {
        private ShaderProgram _program;
        private List<IndexedModelBatch<TVertice, TIndice>> _batches = new List<IndexedModelBatch<TVertice, TIndice>>();

        private int _instanceVBO;
        private ulong _instanceGuidGenerator;
        private bool _dirty;

        private ConcurrentDictionary<ulong, Matrix4> _instances = new ConcurrentDictionary<ulong, Matrix4>();

        public InstancedBatchedIndexedModel(string shaderProgramName)
        {
            _program = ShaderProgramCache.Instance.Get(shaderProgramName);

            _instanceGuidGenerator = 0;
            _dirty = true;
        }

        public ulong AddInstance(ref Matrix4 instanceMatrix)
        {
            _instances.TryAdd(_instanceGuidGenerator, instanceMatrix);
            var instanceID = _instanceGuidGenerator;
            _instanceGuidGenerator++;

            _dirty = true;

            return instanceID;
        }

        public void RemoveInstance(ulong instanceGUID)
        {
            _instances.TryRemove(instanceGUID, out var tmp);

            _dirty = true;
        }

        public void Render()
        {
            if (_instances.Count == 0)
                return;

            var firstFrame = !GL.IsBuffer(_instanceVBO);

            foreach (var batch in _batches)
            {
                if (firstFrame)
                    batch.CreateBuffers();

                GL.BindVertexArray(batch.VertexArray);

                if (firstFrame)
                {
                    _instanceVBO = GL.GenBuffer();

                    GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
                    if (_dirty)
                        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(SizeCache<TInstance>.Size * _instances.Count), _instances.Values.ToArray(), BufferUsageHint.StaticDraw);

                    _program.VertexAttribPointer<Matrix4>("instance_position", 16, VertexAttribPointerType.Float);
                    _program.VertexAttribDivisor<Matrix4>("instance_position", 1);
                    _program.EnableVertexAttribArray<Matrix4>("instance_position");

                    // Stop buffering the data to the GPU
                    _dirty = false;
                }
                else if (_dirty)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(SizeCache<TInstance>.Size * _instances.Count), _instances.Values.ToArray(), BufferUsageHint.StaticDraw);

                    _program.VertexAttribPointer<Matrix4>("instance_position", 16, VertexAttribPointerType.Float);
                    _program.VertexAttribDivisor<Matrix4>("instance_position", 1);
                    _program.EnableVertexAttribArray<Matrix4>("instance_position");

                    // Stop buffering the data to the GPU
                    _dirty = false;
                }

                batch.Render(_instances.Count);
            }
        }

        protected void AddBatch(IndexedModelBatch<TVertice, TIndice> batch)
        {
            _batches.Add(batch);
        }
    }
}
