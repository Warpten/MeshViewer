using MeshViewer.Memory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MeshViewer.Memory
{
    public sealed class Game
    {
        private IntPtr _handle;
        private IntPtr BaseAddress { get; }

        private static ObjectMgr Manager { get; set; }
        public static CGCamera Camera { get; private set; }

        public static bool IsValid => Instance != null;

        #region Events
        public static event Action OnWorldUpdate
        {
            add {
                if (Manager == null)
                    throw new InvalidOperationException("Binding to OnWorldUpdate before the manager initialized!");

                Manager.OnWorldUpdate += value;
            }
            remove {
                if (Manager != null)
                    Manager.OnWorldUpdate -= value;
            }
        }

        public static event Action<CGObject_C> OnEntityDespawn
        {
            add {
                if (Manager == null)
                    throw new InvalidOperationException("Binding to OnEntityDespawn before the manager initialized!");

                Manager.OnEntityDespawn += value;
            }
            remove {
                if (Manager != null)
                    Manager.OnEntityDespawn -= value;
            }
        }

        public static event Action<CGObject_C> OnSpawn
        {
            add {
                if (Manager == null)
                    throw new InvalidOperationException("Binding to OnSpawn before the manager initialized!");

                Manager.OnEntitySpawn += value;
            }
            remove {
                if (Manager != null)
                    Manager.OnEntitySpawn -= value;
            }
        }

        public static event Action<CGObject_C> OnEntityUpdated
        {
            add
            {
                if (Manager == null)
                    throw new InvalidOperationException("Binding to OnEntityUpdated before the manager initialized!");

                Manager.OnEntityUpdated += value;
            }
            remove
            {
                if (Manager != null)
                    Manager.OnEntityUpdated -= value;
            }
        }
        #endregion

        #region ObjectMgr shorthands
        public static int CurrentMap => Manager?.CurrentMap ?? -1;
        public static bool InGame => Manager?.InGame ?? false;

        public static CGPlayer_C LocalPlayer      => Manager?.LocalPlayer ?? null;

        public static IEnumerable<CGUnit_C>       Units       => Manager?.Units ?? null;
        public static IEnumerable<CGPlayer_C>     Players     => Manager?.Players ?? null;
        public static IEnumerable<CGItem_C>       Items       => Manager?.Items ?? null;
        public static IEnumerable<CGObject_C>     Entities    => Manager?.Entities ?? null;
        public static IEnumerable<CGGameObject_C> GameObjects => Manager?.GameObjects ?? null;
        #endregion

        private static Game Instance { get; set; }

        #region Life and death
        private Game(IntPtr baseAddress, int pid)
        {
            BaseAddress = baseAddress;

            _handle = UnsafeMethods.OpenProcess(ProcessAccess.Read | ProcessAccess.Write | ProcessAccess.Operation, false, pid);
            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException($"Unable to open a handle to process #{pid}.");
            
            Camera = new CGCamera();
            Manager = new ObjectMgr();
        }

        public static void Open(int ID)
        {
            var process = System.Diagnostics.Process.GetProcessById(ID);
            if (process == null)
                throw new InvalidOperationException($"Unable to find process #{ID}.");

            Instance = new Game(process.MainModule.BaseAddress, process.Id);
        }

        public static void Open(string processName)
        {
            var process = System.Diagnostics.Process.GetProcessesByName(processName).First();
            if (process == null)
                throw new InvalidOperationException($"Unable to find {processName}.");

            Instance = new Game(process.MainModule.BaseAddress, process.Id);
        }

        public static void Close()
        {
            Instance = null;
        }

        ~Game()
        {
            Manager = null;
            Camera = null;

            UnsafeMethods.CloseHandle(_handle);
        }
        #endregion

        public static void Update() => Manager?.Update();

        #region Memory reading
        public static unsafe T[] ReadArray<T>(IntPtr offset, int arraySize, bool absolute = false) where T : struct
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

        public static unsafe T Read<T>(int offset, bool absolute = false) where T : struct => Read<T>(new IntPtr(offset), absolute);

        public static unsafe T Read<T>(IntPtr offset, bool absolute = false) where T : struct
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

        public static byte[] Read(IntPtr offset, int size, bool absolute = false)
        {
            int bytesRead = 0;
            var buffer = new byte[size];

            if (Instance == null)
                return buffer;

            UnsafeMethods.ReadProcessMemory(Instance._handle, !absolute ? Instance.BaseAddress + offset.ToInt32() : offset, buffer, size, ref bytesRead);
            return buffer;
        }

        public static string ReadCString(IntPtr offset, int maxLength, Encoding encoding, bool absolute = false)
        {
            var bytes = encoding.GetString(Read(offset, maxLength, absolute));
            if (bytes.IndexOf('\0') != 1)
                bytes = bytes.Remove(bytes.IndexOf('\0'), bytes.Length - bytes.IndexOf('\0'));
            return bytes;
        }

        public static string ReadCString(IntPtr offset, int maxLength, bool absolute = false) =>
            ReadCString(offset, maxLength, Encoding.UTF8, absolute);

        #endregion

        public static T GetEntity<T>(ObjectGuid guid) where T : CGObject_C => Manager?.GetEntity<T>(guid) ?? null;
    }
}
