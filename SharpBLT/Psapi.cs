namespace SharpBLT;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static class Psapi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MODULEINFO
    {
        public IntPtr lpBaseOfDll;
        public uint SizeOfImage;
        public IntPtr EntryPoint;
    }

    [DllImport("psapi.dll", SetLastError = true)]
    private static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo, uint cb);


    public static bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out MODULEINFO lpmodinfo)
    {
        return GetModuleInformation(hProcess, hModule, out lpmodinfo, (uint)Marshal.SizeOf<MODULEINFO>());
    }

    public static MODULEINFO GetModuleInfo(string szModule)
    {
        var modinfo = new MODULEINFO();
        var hModule = Kernel32.GetModuleHandle(szModule);

        if (hModule == 0)
            return modinfo;
        GetModuleInformation(Kernel32.GetCurrentProcess(), hModule, out modinfo, (uint)Unsafe.SizeOf<MODULEINFO>());
        return modinfo;
    }

}
