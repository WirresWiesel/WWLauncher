using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Core.Models
{
    public class ProcessObject : IDisposable
    {
        private IntPtr _handle;

        public ProcessObject()
        {

        }

        public void Initialize()
        {
            _handle = CreateJobObject(IntPtr.Zero, null);

            JOBOBJECT_EXTENDED_LIMIT_INFORMATION info = new();
            info.BasicLimitInformation.LimitFlags =
                JOBOBJECTLIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;

            int length = Marshal.SizeOf(info);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(info, ptr, false);

            SetInformationJobObject(
                _handle,
                JobObjectInfoType.ExtendedLimitInformation,
                ptr,
                (uint)length
            );

            Marshal.FreeHGlobal(ptr);
        }

        public void AddProcess(Process process)
        {
            AssignProcessToJobObject(_handle, process.Handle);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                CloseHandle(_handle);
                _handle = IntPtr.Zero;
            }
        }

        #region WinAPI

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CreateJobObject(IntPtr lpJobAttributes, string? name);

        [DllImport("kernel32.dll")]
        static extern bool SetInformationJobObject(
            IntPtr hJob,
            JobObjectInfoType infoType,
            IntPtr lpJobObjectInfo,
            uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll")]
        static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        enum JobObjectInfoType
        {
            ExtendedLimitInformation = 9
        }

        [Flags]
        enum JOBOBJECTLIMIT : uint
        {
            JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000
        }

        [StructLayout(LayoutKind.Sequential)]
        struct JOBOBJECT_BASIC_LIMIT_INFORMATION
        {
            public long PerProcessUserTimeLimit;
            public long PerJobUserTimeLimit;
            public JOBOBJECTLIMIT LimitFlags;
            public UIntPtr MinimumWorkingSetSize;
            public UIntPtr MaximumWorkingSetSize;
            public uint ActiveProcessLimit;
            public long Affinity;
            public uint PriorityClass;
            public uint SchedulingClass;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct IO_COUNTERS
        {
            public ulong ReadOperationCount;
            public ulong WriteOperationCount;
            public ulong OtherOperationCount;
            public ulong ReadTransferCount;
            public ulong WriteTransferCount;
            public ulong OtherTransferCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
        {
            public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
            public IO_COUNTERS IoInfo;
            public UIntPtr ProcessMemoryLimit;
            public UIntPtr JobMemoryLimit;
            public UIntPtr PeakProcessMemoryUsed;
            public UIntPtr PeakJobMemoryUsed;
        }
        #endregion
    }
}
