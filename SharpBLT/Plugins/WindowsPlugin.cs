namespace SharpBLT.Plugins;

using System;
using System.Runtime.InteropServices;

internal class WindowsPlugin : Plugin
{
    private readonly IntPtr _module;

    private delegate void InitFunc(Func<string, IntPtr> getFuncByName);

    public WindowsPlugin(string file) : base(file)
    {
        _module = Kernel32.LoadLibrary(file);

        if (_module == IntPtr.Zero)
        {
            throw new Exception($"Failed to load module: ERR {Marshal.GetLastWin32Error()}");
        }

        Init();

        var initFunc = Kernel32.GetProcAddress<InitFunc>(_module, "SuperBLT_Plugin_Setup") ?? throw new Exception("Invalid module - missing init function.");
        initFunc(GetFunctionPointer);
    }

    protected override IntPtr ResolveSymbol(string name)
    {
        return Marshal.GetFunctionPointerForDelegate(Kernel32.GetProcAddress<InitFunc>(_module, name)!);
    }

    private IntPtr GetFunctionPointer(string name)
    {
        return name switch
        {
            "raid_log" => Marshal.GetFunctionPointerForDelegate(RaidLog),
            "is_active_state" => Marshal.GetFunctionPointerForDelegate(Lua.check_active_state),
            "luaL_checkstack" => Marshal.GetFunctionPointerForDelegate(Laux.luaL_checkstack),
            "lua_rawequal" => Marshal.GetFunctionPointerForDelegate(Laux.lua_rawequal),
            _ => IntPtr.Zero
        };
    }

    private static void RaidLog(string message, int level, string file, int line)
    {
        Logger.Instance().Log((LogType)level, string.Format(message, file, line));
    }

    // Ensure to free the module handle when it's no longer needed
    ~WindowsPlugin()
    {
        if (_module != IntPtr.Zero)
        {
            Kernel32.FreeLibrary(_module);
        }
    }
}