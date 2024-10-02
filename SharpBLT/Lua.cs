namespace SharpBLT;

using SharpBLT.Plugins;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public sealed class Lua
{
    public const CallingConvention DefaultCallingConvention = CallingConvention.StdCall;

    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int LuaCallback(IntPtr L);

    struct luaL_Reg
    {
        public IntPtr Name;
        public IntPtr Function;
    }

    [DllImport("luajit")]
    public static extern int lua_gettop(IntPtr luaState);

    [DllImport("luajit")]
    public static extern void lua_settop(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool lua_toboolean(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern long? lua_tointeger(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern double? lua_tonumber(IntPtr luaState, int arg0);

    [DllImport("luajit", EntryPoint = "lua_tolstring")]
    public static extern IntPtr lua_tolstring_ptr(IntPtr luaState, int arg0, out IntPtr arg1);

    [DllImport("luajit")]
    public static extern long lua_objlen(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern IntPtr lua_touserdata(IntPtr luaState, int arg0);

    [DllImport("luajit", EntryPoint = "luaL_loadfilex")]
    public static extern int luaL_loadfilex_ptr(IntPtr luaState, IntPtr arg0, IntPtr arg1);

    [DllImport("luajit")]
    public static extern int luaL_loadstring(IntPtr luaState, IntPtr arg0);

    [DllImport("luajit")]
    public static extern int lua_pcall(IntPtr luaState, int arg0, int arg1, int arg2);

    [DllImport("luajit", EntryPoint = "luaL_loadfilex")]
    public static extern void  lua_getfield_ptr(IntPtr luaState, int arg0, IntPtr arg1);

    [DllImport("luajit", EntryPoint = "luaL_loadfilex")]
    public static extern void lua_setfield_ptr(IntPtr luaState, int arg0, IntPtr arg1);

    [DllImport("luajit")]
    public static extern void lua_createtable(IntPtr luaState, int arg0, int arg1);

    [DllImport("luajit")]
    public static extern void lua_insert(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void lua_replace(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void lua_remove(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void lua_settable(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void lua_pushinteger(IntPtr luaState, long arg0);

    [DllImport("luajit")]
    public static extern void lua_pushboolean(IntPtr luaState, [MarshalAs(UnmanagedType.I1)] bool arg0);

    [DllImport("luajit", EntryPoint = "lua_pushcclosure")]
    public static extern void lua_pushcclosure_ptr(IntPtr luaState, IntPtr arg0, int arg1);

    [DllImport("luajit", EntryPoint = "lua_pushlstring")]
    public static extern void lua_pushlstring_ptr(IntPtr luaState, IntPtr arg0, long arg1);

    [DllImport("luajit", EntryPoint = "lua_pushstring")]
    public static extern void lua_pushstring_ptr(IntPtr luaState, IntPtr arg0);

    [DllImport("luajit")]
    public static extern void lua_pushvalue(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void lua_pushnil(IntPtr luaState);

    [DllImport("luajit")]
    public static extern int lua_checkstack(IntPtr luaState, int arg0);

    [DllImport("luajit", EntryPoint = "luaL_openlib")]
    public static extern void luaI_openlib_ptr(IntPtr luaState, IntPtr arg0, IntPtr arg1, int arg2);

    [DllImport("luajit")]
    public static extern int luaL_ref(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void lua_rawgeti(IntPtr luaState, int arg0, int arg1);

    [DllImport("luajit")]
    public static extern void lua_rawseti(IntPtr luaState, int arg0, int arg1);

    [DllImport("luajit")]
    public static extern int lua_type(IntPtr luaState, int arg0);

    [DllImport("luajit", EntryPoint = "lua_typename")]
    public static extern IntPtr lua_typename_ptr(IntPtr luaState, int arg0);

    [DllImport("luajit")]
    public static extern void luaL_unref(IntPtr luaState, int arg0, int arg1);

    [DllImport("luajit", EntryPoint = "luaL_error")]
    public static extern int luaL_error_ptr(IntPtr luaState, IntPtr arg0, IntPtr args);

    [DllImport("luajit")]
    public static extern int lua_error(IntPtr luaState);


    [FunctionPattern("48 63 C2 4C 8B D1 48 8B 51 28 48 C1 E0 03 4C 8B CA 4C 2B C8 48 8D 42 08 48 89 41 28")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate void lua_call_fn(IntPtr luaState, int arg0, int arg1);

    // TODO
    [FunctionPattern("4C 8B DC 53 56 57 48 81 EC E0 00 00 00")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int lua_loadx_fn(IntPtr luaState, IntPtr luaReader, IntPtr arg1, IntPtr arg2, IntPtr arg3);

    // TODO
    [FunctionPattern("48 89 5C 24 08 57 48 83 EC 20 4C 8B 41 10 48 8B FA")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate IntPtr lua_newuserdata_fn(IntPtr luaState, long arg0); // FIXME int/long? size_t?

    [FunctionPattern("48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 48 8B F2 48 8B E9 48 8B CE 41 B9 70")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate IntPtr lua_newstate_fn(IntPtr luaAlloc, IntPtr arg0); // FIXME? types?

    [FunctionPattern("48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 48 8B 79 10 48 8B B7 C8 00 00 00 48")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate void lua_close_fn(IntPtr luaState);

    // TODO
    [FunctionPattern("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 4C 8B 43 28 48 8B D0 49 83 E8 08")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate void lua_gettable_fn(IntPtr luaState, int arg0);

    // TODO
    [FunctionPattern("48 83 EC 28 4C 8B D9 E8 ?? ?? ?? ?? 49 8B 53 28 4C 8B C8")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int lua_setmetatable_fn(IntPtr luaState, int arg0);

    // TODO
    [FunctionPattern("48 83 EC 28 4C 8B D9 E8 ?? ?? ?? ?? 48 8B 08 48 8B C1 48 C1 F8 2F 83 F8 F4")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int lua_getmetatable_fn(IntPtr luaState, int arg0);

    // TODO
    [FunctionPattern("48 8B 41 28 F2 0F 11 08")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate void lua_pushnumber_fn(IntPtr luaState, double arg0); // FIXME double? lua_Number?

    // TODO
    [FunctionPattern("48 89 54 24 10 4C 89 44 24 18 4C 89 4C 24 20 53 48 83 EC 20 4C 8B 41 10")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate IntPtr lua_pushfstring_fn(IntPtr luaState, IntPtr arg0, IntPtr arg1);




    // TODO
    [FunctionPattern("40 53 48 83 EC 20 4C 8B D9 E8 ?? ?? ?? ?? 49 8B 5B 28")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate void lua_rawget_fn(IntPtr luaState, int arg0);

    // TODO
    [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 48 8B 73 28")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate void lua_rawset_fn(IntPtr luaState, int arg0);

    // TODO
    [FunctionPattern("48 89 5C 24 08 57 48 83 EC 20 45 8B D8 48 8B D9 E8 ?? ?? ?? ?? 41 8B D3 48 8B CB 48 8B F8 E8 ?? ?? ?? ?? 4C 8B 17")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int lua_equal_fn(IntPtr luaState, int arg0, int arg1);

    // TODO
    [FunctionPattern("48 89 5C 24 10 48 89 74 24 18 41 56 48 83 EC 20 48 8B 41 10")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int luaL_newmetatable_fn(IntPtr luaState, IntPtr arg0);

    // TODO
    [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 4D 8B D8")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate int luaL_checkudata_fn(IntPtr luaState, int arg0, IntPtr arg1);

    [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 41 0F B6 F8 0F B6 F2 48 8B D9 45 85 C9")]
    [UnmanagedFunctionPointer(DefaultCallingConvention)]
    public delegate IntPtr luaL_newstate_fn(IntPtr arg0, [MarshalAs(UnmanagedType.I1)] bool arg2,
                                         [MarshalAs(UnmanagedType.I1)] bool arg3, int arg4);

#pragma warning disable CS0649
    public static readonly lua_newstate_fn lua_newstate;

    private static readonly Hook<lua_call_fn> ms_lua_call_hook;
    private static readonly Hook<luaL_newstate_fn> ms_luaL_newstate_hook;
    private static readonly Hook<lua_close_fn> ms_lua_close_hook;

#pragma warning restore CS0649

    private static readonly lua_call_fn lua_call;
    private static readonly luaL_newstate_fn luaL_newstate;
    private static readonly lua_close_fn lua_close;

    private static readonly List<IntPtr> ms_activeStates;

    private static bool ms_notifyOverlayNotFoundLogged;

#pragma warning disable CS8618
    static Lua()
#pragma warning restore CS8618
    {
        ms_lua_call_hook = FunctionUtils.CreateHook<lua_call_fn>(nameof(lua_call), lua_newcall);
        ms_luaL_newstate_hook = FunctionUtils.CreateHook<luaL_newstate_fn>(nameof(luaL_newstate), luaL_newstate_new);
        ms_lua_close_hook = FunctionUtils.CreateHook<lua_close_fn>(nameof(lua_close), luaF_close);

        lua_call = ms_lua_call_hook.Apply() ?? throw new Exception("Failed to apply lua_call hook");
        luaL_newstate = ms_luaL_newstate_hook.Apply() ?? throw new Exception("Failed to apply luaL_newstate hook");
        lua_close = ms_lua_close_hook.Apply() ?? throw new Exception("Failed to apply lua_close hook");

        ms_activeStates = [];
    }

    public static void Initialize()
    {
        // Dummy to invoke .cctor
    }

    public const int LUA_REGISTRYINDEX = -10000;
    public const int LUA_ENVIRONINDEX = -10001;
    public const int LUA_GLOBALSINDEX = -10002;

    // From src/lauxlib.h
    public const int LUA_NOREF = -2;
    public const int LUA_REFNIL = -1;

    // more bloody lua shit
    // Thread status; 0 is OK
    public const int LUA_YIELD = 1;
    public const int LUA_ERRRUN = 2;
    public const int LUA_ERRSYNTAX = 3;
    public const int LUA_ERRMEM = 4;
    public const int LUA_ERRERR = 5;
    // From src/lauxlib.h
    // Extra error code for 'luaL_load'
    public const int LUA_ERRFILE = LUA_ERRERR + 1;

    public const int LUA_MULTRET = -1;
    public const int LUA_TNONE = -1;
    public const int LUA_TNIL = 0;
    public const int LUA_TBOOLEAN = 1;
    public const int LUA_TLIGHTUSERDATA = 2;
    public const int LUA_TNUMBER = 3;
    public const int LUA_TSTRING = 4;
    public const int LUA_TTABLE = 5;
    public const int LUA_TFUNCTION = 6;
    public const int LUA_TUSERDATA = 7;
    public const int LUA_TTHREAD = 8;

    public static Hook<lua_call_fn> GetNewCallFunctionHook()
    {
        return ms_lua_call_hook;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_pop(IntPtr L, int arg0)
    {
        lua_settop(L, -arg0 - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_newtable(IntPtr L)
    {
        lua_createtable(L, 0, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void luaL_getmetatable(IntPtr L, int arg0)
    {
        lua_getfield_ptr(L, LUA_GLOBALSINDEX, arg0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_getglobal(IntPtr L, string str)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(str);
        lua_getfield_ptr(L, LUA_GLOBALSINDEX, ptr);
        Marshal.FreeHGlobal(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_getfield(IntPtr L, int arg0, string str)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(str);
        lua_getfield_ptr(L, arg0, ptr);
        Marshal.FreeHGlobal(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_setglobal(IntPtr L, string str)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(str);
        lua_setfield_ptr(L, LUA_GLOBALSINDEX, ptr);
        Marshal.FreeHGlobal(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_setfield(IntPtr L, int arg0, string str)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(str);
        lua_setfield_ptr(L, arg0, ptr);
        Marshal.FreeHGlobal(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string? lua_tolstring(IntPtr L, int arg0, out long size)
    {
        IntPtr ptr = lua_tolstring_ptr(L, arg0, out IntPtr len);
        string? str = null;
        try
        {
            str = new((sbyte*)ptr);
            size = len.ToInt32();
        }
        catch
        {
            size = 0;
        }
        return str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string lua_tostring(IntPtr L, int arg0) => lua_tolstring(L, arg0, out _) ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isfunction(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TFUNCTION;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_istable(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TTABLE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_islightuserdata(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TLIGHTUSERDATA;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isnil(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TNIL;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isboolean(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TBOOLEAN;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isthread(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TTHREAD;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isnone(IntPtr L, int arg0) => lua_type(L, arg0) == LUA_TNONE;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isnoneornil(IntPtr L, int arg0) => lua_type(L, arg0) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_pushstring(IntPtr L, string arg0)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(arg0);
        lua_pushstring_ptr(L, ptr);
        Marshal.FreeHGlobal(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_pushlstring(IntPtr L, string arg0, long length)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(arg0);
        lua_pushlstring_ptr(L, ptr, length);
        Marshal.FreeHGlobal(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_pushlstring(IntPtr L, IntPtr arg0, int length) => lua_pushlstring_ptr(L, arg0, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void lua_pushcclosure(IntPtr L, LuaCallback func, int arg1) => lua_pushcclosure_ptr(L, Marshal.GetFunctionPointerForDelegate(func), arg1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void luaI_openlib(IntPtr L, string name, LuaReg[] reg, int arg2)
    {
        IntPtr ptrName = Marshal.StringToHGlobalAnsi(name);

        luaL_Reg[] luaL_Regs = new luaL_Reg[reg.Length + 1];

        for (int i = 0; i < reg.Length; i++)
        {
            luaL_Regs[i].Name = Marshal.StringToHGlobalAnsi(reg[i].Name);
            luaL_Regs[i].Function = Marshal.GetFunctionPointerForDelegate(reg[i].Function);
        }

        luaI_openlib_ptr(L, ptrName, Marshal.UnsafeAddrOfPinnedArrayElement(luaL_Regs, 0), arg2);

        for (int i = 0; i < luaL_Regs.Length; i++)
        {
            if (luaL_Regs[i].Name == IntPtr.Zero)
                continue;

            Marshal.FreeHGlobal(luaL_Regs[i].Name);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void luaL_openlib(IntPtr L, string name, LuaReg[] reg, int arg2) => luaI_openlib(L, name, reg, arg2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string lua_typename(IntPtr L, int arg0)
    {
        IntPtr ptr = lua_typename_ptr(L, arg0);
        string str = new((sbyte*)ptr);

        return str;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int luaL_error(IntPtr L, string str, params VariableArgument[] args)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(str);
        using CombinedVariables arguments = new(args);
        int result = luaL_error_ptr(L, ptr, arguments.GetPtr());
        Marshal.FreeHGlobal(ptr);
        return result;
    }

    public static int luaL_loadfilex(IntPtr L, string filename)
    {
        IntPtr ptrName = Marshal.StringToHGlobalAnsi(filename);
        int err = luaL_loadfilex_ptr(L, ptrName, 0);
        Marshal.FreeHGlobal(ptrName);
        return err;
    }

    private static void NotifyErrorOverlay(IntPtr L, string message)
    {
        lua_getglobal(L, "NotifyErrorOverlay");

        if (lua_isfunction(L, -1))
        {
            int args = 0;

            if (message != null)
            {
                lua_pushstring(L, message);
                args = 1;
            }

            int error = lua_pcall(L, args, 0, 0);

            if (error == LUA_ERRRUN)
            {
                // Don't bother logging the error since the error overlay is designed to be an optional component, just pop the error
                // message off the stack to keep it balanced
                lua_pop(L, 1);
                return;
            }
        }
        else
        {
            lua_pop(L, 1);

            if (!ms_notifyOverlayNotFoundLogged)
            {
                Logger.Instance().Log(LogType.Warn, "Failed to find the NotifyErrorOverlay function in the Lua environment; no in-game notifications will be displayed for caught errors\n");
                ms_notifyOverlayNotFoundLogged = true;
            }
        }
    }

    private static void lua_newcall(IntPtr L, int args, int returns)
    {
        lua_getglobal(L, "debug");
        lua_getfield(L, -1, "traceback");
        lua_remove(L, -2);

        int errorhandler = lua_gettop(L) - args - 1;
        lua_insert(L, errorhandler);

        int result = lua_pcall(L, args, returns, errorhandler);
        if (result != 0)
        {
            string? message = lua_tostring(L, -1);

            if (message != null)
            {
                // Logger.Instance().Log(LogType.Error, message);

                NotifyErrorOverlay(L, message);
                // This call pops the error message off the stack
                lua_pop(L, 1);
            }
        }

        lua_remove(L, errorhandler);
    }

    private static void luaF_close(IntPtr L)
    {
        remove_active_state(L);
        lua_close(L);
    }

    private unsafe static IntPtr luaL_newstate_new(IntPtr _this, bool unk0, bool unk1, int unk2)
    {
        IntPtr ret = luaL_newstate(_this, unk0, unk1, unk2);

        IntPtr L = new(*(void**)_this.ToPointer());

        if (L == IntPtr.Zero)
            return ret;

        add_active_state(L);

        LuaMod.Initialize(L);

        return ret;
    }

    internal static bool check_active_state(IntPtr L) => ms_activeStates.Select(it => it.Equals(L)).Any();

    private static void add_active_state(IntPtr L)
    {
        Logger.Instance().Log(LogType.Log, $"Lua-state activated: {L}");
        ms_activeStates.Add(L);
    }

    private static void remove_active_state(IntPtr L)
    {
        ms_activeStates.Remove(L);
        Logger.Instance().Log(LogType.Log, $"Lua-state closed: {L}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdString luaX_toidstring(IntPtr L, int index)
    {
        IntPtr ptr = lua_touserdata(L, index);
        ulong idValue = (ulong)Marshal.ReadInt64(ptr);
        return new IdString(ReverseBytes(idValue));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ReverseBytes(ulong value)
    {
        return ((value & 0x00000000000000FFUL) << 56) |
                ((value & 0x000000000000FF00UL) << 40) |
                ((value & 0x0000000000FF0000UL) << 24) |
                ((value & 0x00000000FF000000UL) << 8) |
                ((value & 0x000000FF00000000UL) >> 8) |
                ((value & 0x0000FF0000000000UL) >> 24) |
                ((value & 0x00FF000000000000UL) >> 40) |
                ((value & 0xFF00000000000000UL) >> 56);
    }

    internal static void RegisterPluginForActiveStates(Plugin plugin)
    {
        foreach (IntPtr state in ms_activeStates)
        {
            plugin.AddToState(state);
        }
    }

    internal static void UpdatePluginsInActiveStates()
    {
        foreach (IntPtr state in ms_activeStates)
        {
            foreach (Plugin plugin in PluginManager.GetPlugins())
            {
                plugin.Update(state);
            }
        }
    }
}
