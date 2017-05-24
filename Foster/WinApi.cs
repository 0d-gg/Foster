using System;
using System.Runtime.InteropServices;

namespace Foster
{
    /// <summary>
    /// Wrapper for Windows API functionality.
    /// </summary>
    static class WinApi
    {
        private const string KERNEL32 = "kernel32.dll";
        private const string ADVAPI32 = "advapi32.dll";

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        public struct STARTUPINFOEX
        {
            public STARTUPINFO StartupInfo;
            public IntPtr lpAttributeList;
        }

        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public byte lpSecurityDescriptor;
            public int bInheritHandle;
        }

        public enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        public enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        public enum TOKEN_ACCESS
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),
            TOKEN_ALL_ACCESS =
            STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE |
            TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES |
            TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms724211(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa366887(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern IntPtr VirtualAlloc(IntPtr lpAddress, int dwSize, int flAllocationType, int flProtect);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa366892(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern bool VirtualFree(IntPtr lpAddress, int dwSize, int dwFreeType);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa379295(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern bool OpenProcessToken(IntPtr hProcess, UInt32 DesiredAccess, ref IntPtr TokenHandle);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms682429(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern Boolean CreateProcessAsUserA(IntPtr hToken, string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFOEX si, ref PROCESS_INFORMATION pi);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms682559(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern void DeleteProcThreadAttributeList(IntPtr lpAttributeList);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms683481(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms686880(v=vs.85).aspx
        /// </summary>
        [DllImport(KERNEL32)]
        public static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, ref IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa446617(v=vs.85).aspx
        /// </summary>
        [DllImport(ADVAPI32)]
        public extern static bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, out SECURITY_ATTRIBUTES lpTokenAttributes, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);

    }
}
