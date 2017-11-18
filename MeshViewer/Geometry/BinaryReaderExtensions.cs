using MeshViewer.Memory;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace MeshViewer.Geometry
{
    public static unsafe class BinaryReaderExtensions
    {
        /// <summary>
        ///     Calls the native "memcpy" function.
        /// </summary>
        // Note: SuppressUnmanagedCodeSecurity speeds things up drastically since there is no stack-walk required before moving to native code.
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr MoveMemory(byte* dest, byte* src, int count);

        /// <summary>
        ///     Reads a generic structure from the current stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="br"></param>
        /// <returns></returns>
        public static T Read<T>(this BinaryReader br) where T : struct
        {
            if (SizeCache<T>.TypeRequiresMarshal)
            {
                IntPtr ptr = Marshal.AllocHGlobal(SizeCache<T>.Size);
                Marshal.Copy(br.ReadBytes(SizeCache<T>.Size), 0, ptr, SizeCache<T>.Size);
                var mret = Marshal.PtrToStructure<T>(ptr);
                Marshal.FreeHGlobal(ptr);
                return mret;
            }

            // OPTIMIZATION!
            var ret = new T();
            fixed (byte* b = br.ReadBytes(SizeCache<T>.Size))
            {
                var tPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret);
                MoveMemory(tPtr, b, SizeCache<T>.Size);
            }
            return ret;
        }

        /// <summary>
        /// This method is a marshal-less type. It's very fast, but if the type requires any marshaling, it will fail miseraby. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="br"></param>
        /// <returns></returns>
        public static T ReadUsingRefType<T>(this BinaryReader br) where T : struct
        {
            T result = default(T);
            TypedReference refResult = __makeref(result);
            fixed (byte* pBuffer = br.ReadBytes(SizeCache<T>.Size))
            {
                *(byte**)&refResult = pBuffer;
                return __refvalue(refResult, T);
            }
        }

        /// <summary>
        /// This method is a marshal-less type. It's very fast, but if the type requires any marshaling, it will fail miseraby. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="br"></param>
        /// <returns></returns>
        public static T[] ReadUsingRefType<T>(this BinaryReader br, int count) where T : struct
        {
            T[] result = new T[count];
            TypedReference refResult = __makeref(result);
            fixed (byte* pBuffer = br.ReadBytes(SizeCache<T>.Size * count))
            {
                *(byte**)&refResult = pBuffer;
                return __refvalue(refResult, T[]);
            }
        }

        public static T[] Read<T>(this BinaryReader br, int count) where T : struct
        {
            if (count == 0)
                return new T[0];

            var buffer = br.ReadBytes(SizeCache<T>.Size * count);

            if (SizeCache<T>.TypeRequiresMarshal)
            {
                IntPtr ptr = Marshal.AllocHGlobal(SizeCache<T>.Size * count);
                Marshal.Copy(buffer, 0, ptr, SizeCache<T>.Size * count);
                T[] arr = new T[count];
                // Unfortunate part of the marshaler, is that each instance needs to be pulled in separately.
                // Can't just do a bulk memcpy.
                for (int i = 0; i < count; i++)
                {
                    arr[i] = Marshal.PtrToStructure<T>(ptr + (SizeCache<T>.Size * i));
                }
                Marshal.FreeHGlobal(ptr);
                return arr;
            }

            var ret = new T[count];
            fixed (byte* pB = buffer)
            {
                var genericPtr = (byte*)SizeCache<T>.GetUnsafePtr(ref ret[0]);
                MoveMemory(genericPtr, pB, SizeCache<T>.Size * count);
            }
            return ret;
        }
    }
}
