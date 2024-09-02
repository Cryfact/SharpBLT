using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpBLT
{
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

        [FunctionPattern("48 63 C2 4C 8B D1 48 8B 51 28 48 C1 E0 03 4C 8B CA 4C 2B C8 48 8D 42 08 48 89 41 28")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_call_fn(IntPtr luaState, int arg0, int arg1);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 48 8B 59 10 41 8B F0 4C 63 DA 4C 8B D1 0F B6 BB C1")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int lua_pcall_fn(IntPtr luaState, int arg0, int arg1, int arg2);

        [FunctionPattern("48 8B 41 28 48 2B 41 20 48 C1 F8 03 C3")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int lua_gettop_fn(IntPtr luaState);

        [FunctionPattern("40 53 48 83 EC 20 48 8B D9 85 D2 78 ?? 4C 8B 41")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_settop_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 83 EC 28 E8 ?? ?? ?? ?? 48 8B 08 33 C0 48 C1")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate bool lua_toboolean_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 83 EC 28 E8 ?? ?? ?? ?? 48 8B 10 48 8B CA 48 C1 F9 2F 83 F9 F2 ?? ?? F2 0F 10 00 F2 48 0F")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate long lua_tointeger_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 83 EC 28 E8 ?? ?? ?? ?? 48 8B 10 48 8B CA 48 C1 F9 2F 83 F9 F2 ?? ?? F2 0F 10 00 48 83 C4")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate double lua_tonumber_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 49 8B F8 8B DA 48 8B F1")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate IntPtr lua_tolstring_fn(IntPtr luaState, int arg0, out IntPtr arg1);

        [FunctionPattern("40 53 48 83 EC 20 4C 8B D9 E8 ?? ?? ?? ?? 48 8B")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate long lua_objlen_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 89 5C 24 20 55 56 57 48 81 EC 50 02 00 00 48")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int luaL_loadfilex_fn(IntPtr luaState, IntPtr arg0, IntPtr arg1);

        [FunctionPattern("48 83 EC 48 48 89 54 24 30 48 83 C8 FF 0F 1F 00")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int luaL_loadstring_fn(IntPtr luaState, IntPtr arg0);

        [FunctionPattern("48 89 5C 24 10 57 48 83 EC 20 4D 8B D8 48 8B D9")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_getfield_fn(IntPtr luaState, int arg0, IntPtr arg1);

        [FunctionPattern("48 89 5C 24 08 57 48 83 EC 20 4D 8B D8 48 8B D9")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int lua_setfield_fn(IntPtr luaState, int arg0, IntPtr arg1);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 4C 8B 49 10 41 8B F8 8B F2 48 8B D9 49 8B 41 28 49")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_createtable_fn(IntPtr luaState, int arg0, int arg1);

        [FunctionPattern("4C 8B C9 85 D2 7E ?? 8D 42 FF 48 63 D0 48 8B 41")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_insert_fn(IntPtr luaState, int arg0);

        [FunctionPattern("4C 8B C1 85 D2 7E ?? 8D 42 FF 48 63 D0 48 8B 41 20 48 8B 49 28 48 8D 04 D0 48 3B C1")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_remove_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 48 8B F2 48 8B E9 48 8B CE 41 B9 70")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_newstate_fn(IntPtr luaState, IntPtr arg0, IntPtr arg1);

        [FunctionPattern("48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 20 48 8B 79 10 48 8B B7 C8 00 00 00 48")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_close_fn(IntPtr luaState);

        [FunctionPattern("40 53 48 83 EC 20 48 8B D9 E8 ?? ?? ?? ?? 4C 8B 43 28 48 8B D0 49 83 E8 10")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_settable_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 8B 41 28 0F 57 C0 F2 48 0F 2A C2 F2 0F 11 00")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_pushinteger_fn(IntPtr luaState, long arg0);

        [FunctionPattern("48 8B 41 28 45 33 C0 85 D2 41 0F 95 C0 49 FF C0")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_pushboolean_fn(IntPtr luaState, [MarshalAs(UnmanagedType.I1)] bool arg0);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 4C 8B 49 10 48 8B F2 49 63 F8 48 8B D9 49 8B 41 28")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_pushcclosure_fn(IntPtr luaState, IntPtr arg0, int arg1);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 4C 8B 49 10 49 8B F8 48 8B F2 48 8B D9 49 8B 41 28")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_pushlstring_fn(IntPtr luaState, IntPtr arg0, int arg1);

        [FunctionPattern("48 89 5C 24 08 57 48 83 EC 20 48 8B FA 48 8B D9 48 85 D2 75 ?? 48 8B 41 28 49 83 C8 FF 4C 89 00")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_pushstring_fn(IntPtr luaState, IntPtr arg0);

        [FunctionPattern("40 53 48 83 EC 20 48 8B D9 81 FA 40 1F 00 00 7F")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_checkstack_fn(IntPtr luaState, int arg0);

        [FunctionPattern("48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 48 83 EC 20 48 8B 41 28")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void luaI_openlib_fn(IntPtr luaState, IntPtr arg0, IntPtr arg1, int arg2);

        [FunctionPattern("48 89 5C 24 20 57 48 83 EC 20 8D 82 0F 27 00 00")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int luaL_ref_fn(IntPtr luaState, int arg0);

        [FunctionPattern("40 53 48 83 EC 20 4D 63 D8 48 8B D9 E8 ?? ?? ?? ?? 48 BA FF FF FF FF FF")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_rawgeti_fn(IntPtr luaState, int arg0, int arg1);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 4D 63 D8 48 8B D9 E8")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void lua_rawseti_fn(IntPtr luaState, int arg0, int arg1);

        [FunctionPattern("48 83 EC 28 4C 8B D9 E8 B4 D9 FE FF 48 8B D0 48")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate int lua_type_fn(IntPtr luaState, int arg0);

        [FunctionPattern("45 85 C0 0F 88 ?? ?? ?? ?? 48 89 5C 24 08 48 89")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate void luaL_unref_fn(IntPtr luaState, int arg0, int arg1);

        [FunctionPattern("48 89 5C 24 08 48 89 74 24 10 57 48 83 EC 20 41 0F B6 F8 0F B6 F2 48 8B D9 45 85 C9")]
        [UnmanagedFunctionPointer(DefaultCallingConvention)]
        public delegate IntPtr luaL_newstate_fn(IntPtr arg0, [MarshalAs(UnmanagedType.I1)] bool arg2, 
                                             [MarshalAs(UnmanagedType.I1)] bool arg3, int arg4);

#pragma warning disable CS0649
        public static readonly lua_pcall_fn lua_pcall;
        public static readonly lua_gettop_fn lua_gettop;
        public static readonly lua_settop_fn lua_settop;
        public static readonly lua_toboolean_fn lua_toboolean;
        public static readonly lua_tointeger_fn lua_tointeger;
        public static readonly lua_tonumber_fn lua_tonumber;
        public static readonly lua_tolstring_fn lua_tolstring_ptr;
        public static readonly lua_objlen_fn lua_objlen;
        private static readonly luaL_loadfilex_fn luaL_loadfilex_ptr;
        public static readonly luaL_loadstring_fn luaL_loadstring;
        private static readonly lua_getfield_fn lua_getfield_ptr;
        private static readonly lua_setfield_fn lua_setfield_ptr;
        public static readonly lua_createtable_fn lua_createtable;
        public static readonly lua_insert_fn lua_insert;
        public static readonly lua_remove_fn lua_remove;
        public static readonly lua_newstate_fn lua_newstate;
        public static readonly lua_settable_fn lua_settable;
        public static readonly lua_pushinteger_fn lua_pushinteger;
        public static readonly lua_pushboolean_fn lua_pushboolean;
        private static readonly lua_pushcclosure_fn lua_pushcclosure_ptr;
        private static readonly lua_pushlstring_fn lua_pushlstring_ptr;
        private static readonly lua_pushstring_fn lua_pushstring_ptr;
        public static readonly lua_checkstack_fn lua_checkstack;
        private static readonly luaI_openlib_fn luaI_openlib_ptr;
        public static readonly luaL_ref_fn luaL_ref;
        public static readonly lua_rawgeti_fn lua_rawgeti;
        public static readonly lua_rawseti_fn lua_rawseti;
        public static readonly lua_type_fn lua_type;
        public static readonly luaL_unref_fn luaL_unref;


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

            lua_pcall = FunctionUtils.ResolveFunction<lua_pcall_fn>(nameof(lua_pcall));
            lua_gettop = FunctionUtils.ResolveFunction<lua_gettop_fn>(nameof(lua_gettop));
            lua_settop = FunctionUtils.ResolveFunction<lua_settop_fn>(nameof(lua_settop));
            lua_toboolean = FunctionUtils.ResolveFunction<lua_toboolean_fn>(nameof(lua_toboolean));
            lua_tointeger = FunctionUtils.ResolveFunction<lua_tointeger_fn>(nameof(lua_tointeger));
            lua_tonumber = FunctionUtils.ResolveFunction<lua_tonumber_fn>(nameof(lua_tonumber));
            lua_tolstring_ptr = FunctionUtils.ResolveFunction<lua_tolstring_fn>(nameof(lua_tolstring));
            lua_objlen = FunctionUtils.ResolveFunction<lua_objlen_fn>(nameof(lua_objlen));
            luaL_loadfilex_ptr = FunctionUtils.ResolveFunction<luaL_loadfilex_fn>(nameof(luaL_loadfilex));
            luaL_loadstring = FunctionUtils.ResolveFunction<luaL_loadstring_fn>(nameof(luaL_loadstring));
            lua_getfield_ptr = FunctionUtils.ResolveFunction<lua_getfield_fn>(nameof(lua_getfield));
            lua_setfield_ptr = FunctionUtils.ResolveFunction<lua_setfield_fn>(nameof(lua_setfield));
            lua_createtable = FunctionUtils.ResolveFunction<lua_createtable_fn>(nameof(lua_createtable));
            lua_insert = FunctionUtils.ResolveFunction<lua_insert_fn>(nameof(lua_insert));
            lua_remove = FunctionUtils.ResolveFunction<lua_remove_fn>(nameof(lua_remove));
            lua_settable = FunctionUtils.ResolveFunction<lua_settable_fn>(nameof(lua_settable));
            lua_pushinteger = FunctionUtils.ResolveFunction<lua_pushinteger_fn>(nameof(lua_pushinteger));
            lua_pushboolean = FunctionUtils.ResolveFunction<lua_pushboolean_fn>(nameof(lua_pushboolean));
            lua_pushcclosure_ptr = FunctionUtils.ResolveFunction<lua_pushcclosure_fn>(nameof(lua_pushcclosure));
            lua_pushlstring_ptr = FunctionUtils.ResolveFunction<lua_pushlstring_fn>(nameof(lua_pushlstring));
            lua_pushstring_ptr = FunctionUtils.ResolveFunction<lua_pushstring_fn>(nameof(lua_pushstring));
            lua_checkstack = FunctionUtils.ResolveFunction<lua_checkstack_fn>(nameof(lua_checkstack));
            luaI_openlib_ptr = FunctionUtils.ResolveFunction<luaI_openlib_fn>(nameof(luaI_openlib));
            luaL_ref = FunctionUtils.ResolveFunction<luaL_ref_fn>(nameof(luaL_ref));
            lua_rawgeti = FunctionUtils.ResolveFunction<lua_rawgeti_fn>(nameof(lua_rawgeti));
            lua_rawseti = FunctionUtils.ResolveFunction<lua_rawseti_fn>(nameof(lua_rawseti));
            lua_type = FunctionUtils.ResolveFunction<lua_type_fn>(nameof(lua_type));
            luaL_unref = FunctionUtils.ResolveFunction<luaL_unref_fn>(nameof(luaL_unref));


#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            lua_call = ms_lua_call_hook.Apply() ?? throw new Exception("Failed to apply lua_call hook");
            luaL_newstate = ms_luaL_newstate_hook.Apply() ?? throw new Exception("Failed to apply luaL_newstate hook");
            lua_close = ms_lua_close_hook.Apply() ?? throw new Exception("Failed to apply lua_close hook");
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

            ms_activeStates = new List<IntPtr>();
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
        public static void lua_getglobal(IntPtr L, string str)
        {
            var ptr = Marshal.StringToHGlobalAnsi(str);
            lua_getfield_ptr(L, LUA_GLOBALSINDEX, ptr);
            Marshal.FreeHGlobal(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_getfield(IntPtr L, int arg0, string str)
        {
            var ptr = Marshal.StringToHGlobalAnsi(str);
            lua_getfield_ptr(L, arg0, ptr);
            Marshal.FreeHGlobal(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_setfield(IntPtr L, int arg0, string str)
        {
            var ptr = Marshal.StringToHGlobalAnsi(str);
            lua_setfield_ptr(L, arg0, ptr);
            Marshal.FreeHGlobal(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static string lua_tolstring(IntPtr L, int arg0, out int size)
        {
            var ptr = lua_tolstring_ptr(L, arg0, out var len);
            var str = new string((sbyte*)ptr);

            size = len.ToInt32();

            return str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool lua_isfunction(IntPtr L, int arg0)
        {
            return lua_type(L, arg0) == LUA_TFUNCTION;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushstring(IntPtr L, string arg0)
        {
            var ptr = Marshal.StringToHGlobalAnsi(arg0);
            lua_pushstring_ptr(L, ptr);
            Marshal.FreeHGlobal(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushlstring(IntPtr L, string arg0, int length)
        {
            var ptr = Marshal.StringToHGlobalAnsi(arg0);
            lua_pushlstring_ptr(L, ptr, length);
            Marshal.FreeHGlobal(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void lua_pushcclosure(IntPtr L, LuaCallback func, int arg1)
        {
            lua_pushcclosure_ptr(L, Marshal.GetFunctionPointerForDelegate(func), arg1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void luaI_openlib(IntPtr L, string name, LuaReg[] reg, int arg2)
        {
            var ptrName = Marshal.StringToHGlobalAnsi(name);

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

        public static int luaL_loadfilex(IntPtr luaState, string filename)
        {
            var ptrName = Marshal.StringToHGlobalAnsi(filename);
            int err = luaL_loadfilex_ptr(luaState, ptrName, 0);
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
                var message = lua_tolstring(L, -1, out var len);

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

        private unsafe static IntPtr luaL_newstate_new(IntPtr thislol, bool no, bool freakin, int clue)
        {
            var ret = luaL_newstate(thislol, no, freakin, clue);

            IntPtr L = new IntPtr(*((void**)thislol.ToPointer()));

          //  PD2HOOK_LOG_LOG("Lua State: 0x{0:016x}", reinterpret_cast<uint64_t>(L));

            if (L == IntPtr.Zero)
                return ret;

            add_active_state(L);

            LuaMod.Initialize(L);

            return ret;
        }

        internal static bool check_active_state(IntPtr L)
        {
            foreach (var it in ms_activeStates)
            {
                if (it == L)
                    return true;
            }

            return false;
        }

        private static void add_active_state(IntPtr L)
        {
            ms_activeStates.Add(L);
        }

        private static void remove_active_state(IntPtr L)
        {
            ms_activeStates.Remove(L);
        }
    }
}
