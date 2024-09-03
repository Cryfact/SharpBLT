namespace SharpBLT;

using System.Runtime.InteropServices;

internal static class Kernel32
{
    public const int PAGE_EXECUTE = 0x10;
    public const int PAGE_EXECUTE_READWRITE = 0x40;

    public const int STD_OUTPUT_HANDLE = -11;

    public const ushort FOREGROUND_BLUE      = 0x0001; // text color contains blue.
    public const ushort FOREGROUND_GREEN     = 0x0002; // text color contains green.
    public const ushort FOREGROUND_RED       = 0x0004; // text color contains red.
    public const ushort FOREGROUND_INTENSITY = 0x0008; // text color is intensified.
    public const ushort BACKGROUND_BLUE      = 0x0010; // background color contains blue.
    public const ushort BACKGROUND_GREEN     = 0x0020; // background color contains green.
    public const ushort BACKGROUND_RED       = 0x0040; // background color contains red.
    public const ushort BACKGROUND_INTENSITY = 0x0080; // background color is intensified.

    [Flags]
    public enum AllocationType : uint
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags]
    public enum MemoryProtection : uint
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }

    [Flags]
    public enum FreeType
    {
        Decommit = 0x4000,
        Release = 0x8000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_INFO
    {
        public ushort wProcessorArchitecture;
        public ushort wReserved;
        public uint dwPageSize;
        public IntPtr lpMinimumApplicationAddress;
        public IntPtr lpMaximumApplicationAddress;
        public IntPtr dwActiveProcessorMask;
        public uint dwNumberOfProcessors;
        public uint dwProcessorType;
        public uint dwAllocationGranularity;
        public ushort wProcessorLevel;
        public ushort wProcessorRevision;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string? lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll")]
    public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern bool VirtualFree(IntPtr pAddress, int size, FreeType freeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void GetSystemInfo(ref SYSTEM_INFO Info);

    [DllImport("kernel32.dll")]
    public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, ushort wAttributes);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

}
