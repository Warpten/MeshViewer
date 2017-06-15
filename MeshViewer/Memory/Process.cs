using MeshViewer.Memory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MeshViewer.Memory
{
    public sealed class Process
    {
        private IntPtr _handle;
        private IntPtr BaseAddress { get; }

        // public set sucks, but I CBA.
        public ObjectMgr Manager { get; set; }
        public CGCamera_C Camera { get; set; }

        #region ObjectMgr shorthands
        public CGPlayer_C LocalPlayer      => Manager.LocalPlayer;

        public IEnumerable<CGUnit_C>   Units    => Manager.Units;
        public IEnumerable<CGPlayer_C> Players  => Manager.Players;
        public IEnumerable<CGItem_C>   Items    => Manager.Items;
        public IEnumerable<CGObject_C> Entities => Manager.Entities;
        #endregion

        #region Life and death
        public Process(int ID)
        {
            var process = System.Diagnostics.Process.GetProcessById(ID);
            if (process == null)
                throw new InvalidOperationException($"Unable to find process #{ID}.");

            BaseAddress = process.MainModule.BaseAddress;

            _handle = UnsafeMethods.OpenProcess(ProcessAccess.Read | ProcessAccess.Write | ProcessAccess.Operation, false, process.Id);
            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException($"Unable to open a handle to process #{ID}.");
        }

        public Process(string processName)
        {
            var process = System.Diagnostics.Process.GetProcessesByName(processName).First();
            if (process == null)
                throw new InvalidOperationException($"Unable to find {processName}.");

            BaseAddress = process.MainModule.BaseAddress;

            _handle = UnsafeMethods.OpenProcess(ProcessAccess.Read | ProcessAccess.Write | ProcessAccess.Operation, false, process.Id);
            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException($"Unable to open a handle to {processName}.");
        }

        ~Process()
        {
            UnsafeMethods.CloseHandle(_handle);

            Manager = null;
            Camera = null;
        }
        #endregion

        #region Memory reading
        public unsafe T[] ReadArray<T>(IntPtr offset, int arraySize, bool absolute = false) where T : struct
        {
            if (arraySize == 0)
                return new T[0];

            var buffer = Read(offset, SizeCache<T>.Size * arraySize, absolute);
            var array = new T[arraySize];

            if (SizeCache<T>.TypeRequiresMarshal)
            {
                IntPtr ptr = Marshal.AllocHGlobal(SizeCache<T>.Size * arraySize);
                Marshal.Copy(buffer, 0, ptr, SizeCache<T>.Size * arraySize);
                
                for (var i = 0; i < arraySize; ++i)
                    array[i] = Marshal.PtrToStructure<T>(ptr + SizeCache<T>.Size * i);

                Marshal.FreeHGlobal(ptr);
                return array;
            }

            fixed (byte* pB = buffer)
            {
                var genericPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref array[0]);
                UnsafeMethods.MoveMemory(genericPtr, pB, SizeCache<T>.Size * arraySize);
            }
            return array;
        }

        public unsafe T Read<T>(int offset, bool absolute = false) where T : struct => Read<T>(new IntPtr(offset), absolute);

        public unsafe T Read<T>(IntPtr offset, bool absolute = false) where T : struct
        {
            var buffer = Read(offset, SizeCache<T>.Size, absolute);
            if (SizeCache<T>.TypeRequiresMarshal)
            {
                IntPtr ptr = Marshal.AllocHGlobal(SizeCache<T>.Size);
                Marshal.Copy(buffer, 0, ptr, SizeCache<T>.Size);
                var mret = Marshal.PtrToStructure<T>(ptr);
                Marshal.FreeHGlobal(ptr);
                return mret;
            }

            var ret = default(T);
            fixed (byte* buf = buffer)
            {
                var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret);
                UnsafeMethods.MoveMemory(tPtr, buf, SizeCache<T>.Size);
            }
            return ret;
        }

        public byte[] Read(IntPtr offset, int size, bool absolute = false)
        {
            int bytesRead = 0;
            var buffer = new byte[size];

            UnsafeMethods.ReadProcessMemory(_handle, !absolute ? BaseAddress + offset.ToInt32() : offset, buffer, size, ref bytesRead);
            return buffer;
        }

        public string ReadCString(IntPtr offset, int maxLength, Encoding encoding, bool absolute = false)
        {
            var bytes = encoding.GetString(Read(offset, maxLength, absolute));
            if (bytes.IndexOf('\0') != 1)
                bytes = bytes.Remove(bytes.IndexOf('\0'), bytes.Length - bytes.IndexOf('\0'));
            return bytes;
        }

        public string ReadCString(IntPtr offset, int maxLength, bool absolute = false) =>
            ReadCString(offset, maxLength, Encoding.UTF8, absolute);

        #endregion
    }
}
