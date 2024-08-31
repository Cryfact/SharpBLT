using System.Runtime.InteropServices;

namespace SharpBLT
{
    public class Program
    {
        private static void ValidateModDirectories()
        {
            if (!Directory.Exists("mods/downloads"))
                Directory.CreateDirectory("mods/downloads");

            if (!Directory.Exists("mods/logs"))
                Directory.CreateDirectory("mods/logs");

            if (!Directory.Exists("mods/saves"))
                Directory.CreateDirectory("mods/saves");

        }

        [UnmanagedCallersOnly]
        public static void Main()
        {
            ValidateModDirectories();

            User32.MessageBox(IntPtr.Zero, "Debug Me", "Debug Me", User32.MB_OK);

           // if (File.Exists("mods/developer.txt"))
                Logger.Instance().OpenConsole();

            Game.Initialize();
            Lua.Initialize();
        }
    }
}
