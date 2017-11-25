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
        private int _instanceDiff; // Total amount of modifications to instances since last draw.
        private bool _dirty; // Set when modifying the amount of instances
        private bool _forcedDirty; // Set when updating an instance

        private ConcurrentDictionary<ulong, Matrix4> _instances = new ConcurrentDictionary<ulong, Matrix4>();

        public InstancedBatchedIndexedModel(string shaderProgramName)
        {
            _program = ShaderProgramCache.Instance.Get(shaderProgramName);

            _dirty = true;
            _instanceGuidGenerator = 0;
            _instanceDiff = 0;

            _forcedDirty = false;
        }

        public ulong AddInstance(ref Matrix4 instanceMatrix)
        {
            _instances.TryAdd(_instanceGuidGenerator, instanceMatrix);
            var instanceID = _instanceGuidGenerator;
            _instanceGuidGenerator++;

            ++_instanceDiff;
            _dirty = true;

            return instanceID;
        }

        public void RemoveInstance(ulong instanceGUID)
        {
            _instances.TryRemove(instanceGUID, out var tmp);

            --_instanceDiff;
            _dirty = true;
        }

        public bool UpdateInstance(ulong instanceGUID, ref Matrix4 instanceMatrix)
        {
            if (!_instances.ContainsKey(instanceGUID))
                return false;

            _instances[instanceGUID] = instanceMatrix;

            _forcedDirty = true;
            _dirty = true;
            return true;
        }

        public void Render()
        {
            if (_instances.Count == 0)
                return;

            var firstDrawCall = !GL.IsBuffer(_instanceVBO);
            for (var i = 0; i < _batches.Count; ++i)
            {
                var batch = _batches[i];

                if (firstDrawCall)
                {
                    batch.CreateBuffers();
                    if (i == 0)
                        _instanceVBO = GL.GenBuffer();
                }

                GL.BindVertexArray(batch.VertexArray);

                if (firstDrawCall || _dirty)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
                    if (i == 0 && (_instanceDiff != 0 || _forcedDirty))
                    {
                        //if (_instanceDiff >= 0)
                            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(SizeCache<TInstance>.Size * _instances.Count), _instances.Values.ToArray(), BufferUsageHint.StaticDraw);
                        //else
                        //    GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(SizeCache<TInstance>.Size * _instances.Count), _instances.Values.ToArray());

                        _instanceDiff = 0;
                    }
                    _program.VertexAttribPointer<Matrix4>("instance_position", 16, VertexAttribPointerType.Float);
                    _program.VertexAttribDivisor<Matrix4>("instance_position", 1);
                    _program.EnableVertexAttribArray<Matrix4>("instance_position");
                }

                batch.Render(_instances.Count);
            }

            _dirty = false;
            _forcedDirty = false;
        }

        protected void AddBatch(IndexedModelBatch<TVertice, TIndice> batch)
        {
            _batches.Add(batch);
        }
    }
}
