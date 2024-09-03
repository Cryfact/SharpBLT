namespace SharpBLT;

using System.Runtime.InteropServices;

public class Program
{
    public static void Main(string[] _)
    { }

    private static void ValidateModDirectories()
    {
        if (!Directory.Exists("mods/downloads"))
            Directory.CreateDirectory("mods/downloads");

        if (!Directory.Exists("mods/logs"))
            Directory.CreateDirectory("mods/logs");

        if (!Directory.Exists("mods/saves"))
            Directory.CreateDirectory("mods/saves");

    }

    [UnmanagedCallersOnly(EntryPoint = "NativeMain")]
    public static void NativeMain()
    {
        ValidateModDirectories();

        if (File.Exists("mods/debugger.txt"))
            _ = User32.MessageBox(IntPtr.Zero, "If wanted, attach debugger now.", "Debug Me!", User32.MB_OK);

        if (File.Exists("mods/developer.txt"))
            Logger.Instance().OpenConsole();

        Game.Initialize();
        Lua.Initialize();
    }
}
