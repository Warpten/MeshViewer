using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace MeshViewer.Memory
{
    [Flags]
    public enum ProcessAccess
    {
        /// <summary>
        /// Required to terminate a process using TerminateProcess.
        /// </summary>
        Terminate = 0x0001,
        /// <summary>
        /// Required to create a thread.
        /// </summary>
        CreateThread = 0x0002,
        /// <summary>
        /// Required to perform an operation on the address space of a process (see VirtualProtectEx and WriteProcessMemory).
        /// </summary>
        Operation = 0x0008,
        /// <summary>
        /// Required to read memory in a process using ReadProcessMemory.
        /// </summary>
        Read = 0x0010,
        /// <summary>
        /// Required to write to memory in a process using WriteProcessMemory.
        /// </summary>
        Write = 0x0020,
        /// <summary>
        /// Required to duplicate a handle using DuplicateHandle.
        /// </summary>
        DuplicateHandle = 0x0040,
        /// <summary>
        /// Required to create a process.
        /// </summary>
        CreateProcess = 0x0080,
        /// <summary>
        /// Required to set memory limits using SetProcessWorkingSetSize.
        /// </summary>
        SetQuota = 0x0100,
        /// <summary>
        /// Required to set certain information about a process, such as its priority class (see SetPriorityClass).
        /// </summary>
        SetInformation = 0x0200,
        /// <summary>
        /// Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken).
        /// </summary>
        QueryInformation = 0x0400,
        /// <summary>
        /// Required to suspend or resume a process.
        /// </summary>
        SuspendResume = 0x800,
        /// <summary>
        /// Required to retrieve certain information about a process (see GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName).
        /// A handle that has the QUERY_INFORMATION access right is automatically granted QUERY_LIMITED_INFORMATION.
        /// </summary>
        /// <remarks>Windows Server 2003 and Windows XP:  This access right is not supported.</remarks>
        QueryLimitedInformation = 0x1000,
    }

    public unsafe sealed class UnsafeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess,
          IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public static IntPtr OpenProcess(ProcessAccess processAccess, bool v, int id)
            => OpenProcess((int)processAccess, v, id);

        /// <summary>
        ///     Calls the native "memcpy" function.
        /// </summary>
        /// <remarks>SuppressUnmanagedCodeSecurity speeds things up drastically since there is no stack-walk required before moving to native code.</remarks>
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr MoveMemory(byte* dest, byte* src, int count);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
