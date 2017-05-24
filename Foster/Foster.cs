using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Foster
{
    static class Foster
    {
        public static IntPtr StartProcessWithParent(string Application, string CommandLineArgs, int Pid)
        {
            var proc = Process.GetProcessById(Pid).Handle;
            return StartProcessAsUser(Application, CommandLineArgs, null, true, proc);
        }

        public static IntPtr StartProcessAsUser(string szFile, string szArguments, string szDirectory, bool Inherit, IntPtr hParent)
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
