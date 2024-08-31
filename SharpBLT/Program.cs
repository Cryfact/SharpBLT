using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public class Program
    {
        private static void ValidateModDirectories()
        {
            if (Directory.Exists("mods/downloads"))
                Directory.CreateDirectory("mods/downloads");

            if (Directory.Exists("mods/logs"))
                Directory.CreateDirectory("mods/logs");

            if (Directory.Exists("mods/saves"))
                Directory.CreateDirectory("mods/saves");

        }

        [UnmanagedCallersOnly]
        public static void Main()
        {
            ValidateModDirectories();

            Game.Initialize();
            Lua.Initialize();

            if (File.Exists("mods/developer.txt"))
                Logger.Instance().OpenConsole();
        }
    }
}
