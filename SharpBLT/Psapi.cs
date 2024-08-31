using System.Runtime.InteropServices;

namespace SharpBLT
{
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

    }
}
