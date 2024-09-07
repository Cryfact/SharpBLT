namespace SharpBLT;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class Laux
{
    // Lua-related constants
    private const uint LJ_TNIL = 0xFFFFFFFF;
    private const uint LJ_TTRUE = 0xFFFFFFFD;
    private const uint LJ_TLIGHTUD = 0xFFFFFFFC;
    private const uint LJ_TSTR = 0xFFFFFFFB;
    private const uint LJ_TFUNC = 0xFFFFFFF7;
    private const uint LJ_TTRACE = 0xFFFFFFF6;
    private const uint LJ_TCDATA = 0xFFFFFFF5;
    private const uint LJ_TTAB = 0xFFFFFFF4;
    private const uint LJ_TUDATA = 0xFFFFFFF3;
    private const uint LJ_TNUMX = 0xFFFFFFF2;
    private const uint LJ_TISNUM = LJ_TNUMX;
    private const uint LJ_TISPRI = LJ_TTRUE;

    private static readonly bool LJ_DUALNUM = false;

    [StructLayout(LayoutKind.Explicit)]
    private struct TValue
    {
        [FieldOffset(0)]
        public ulong u64; // 64 bit pattern overlaps number
        [FieldOffset(0)]
        public double n; // lua number
        [FieldOffset(8)]
        public GCRef gcr;
        [FieldOffset(8)]
        public int i;
        [FieldOffset(12)]
        public uint it;
        [FieldOffset(16)]
        public uint lo; // Lower 32 bits of number
        [FieldOffset(20)]
        public uint hi; // Upper 32 bits of number
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct GCRef
    {
        public uint gcptr32; // Pseudo 32 bit pointer
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void tag_error(IntPtr L, int narg, int tag) => luaL_typerror(L, narg, luaL_typename(L, tag));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int luaL_argerror(IntPtr L, int narg, string extramsg)
    {
        return Lua.luaL_error(L, "bad argument #%d (%s)", narg, extramsg);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int luaL_checkoption(IntPtr L, int narg, string def, string[] lst)
    {
        string name = (def != null) ? luaL_optstring(L, narg, def) : luaL_checkstring(L, narg);
        for (int i = 0; i < lst.Length; i++)
        {
            if (lst[i] == name)
            {
                return i;
            }
        }

        string buff = $"invalid option '{name}'";
        return luaL_argerror(L, narg, buff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int luaL_typerror(IntPtr L, int narg, string tname)
    {
        string msg = $"{tname} expected, got {luaL_typename(L, narg)}";
        return luaL_argerror(L, narg, msg);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void luaL_checktype(IntPtr L, int narg, int t)
    {
        if (Lua.lua_type(L, narg) != t)
            tag_error(L, narg, t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void luaL_checkany(IntPtr L, int narg)
    {
        if (Lua.lua_type(L, narg) == Lua.LUA_TNONE)
            luaL_argerror(L, narg, "value expected");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string luaL_checklstring(IntPtr L, int narg, out int len)
    {
        string? s = Lua.lua_tolstring(L, narg, out len);
        if (s == null)
            tag_error(L, narg, Lua.LUA_TSTRING);
        return s!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string luaL_optlstring(IntPtr L, int narg, string def, out int len)
    {
        if (Lua.lua_isnoneornil(L, narg))
        {
            len = def.Length;
            return def;
        }
        else
            return luaL_checklstring(L, narg, out len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double luaL_checknumber(IntPtr L, int narg)
    {
        double? d = Lua.lua_tonumber(L, narg);
        if (d.HasValue && !lua_isnumber(L, narg))
            tag_error(L, narg, Lua.LUA_TNUMBER);
        return d!.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double luaL_optnumber(IntPtr L, int narg, double def)
    {
        return luaL_opt(L, luaL_checknumber, narg, def);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long luaL_checkinteger(IntPtr L, int narg)
    {
        long? d = Lua.lua_tointeger(L, narg);
        if (d.HasValue && !lua_isnumber(L, narg))
            tag_error(L, narg, Lua.LUA_TNUMBER);
        return d!.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long luaL_optinteger(IntPtr L, int narg, long def)
    {
        return luaL_opt(L, luaL_checkinteger, narg, def);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void luaL_checkstack(IntPtr L, int size, string msg)
    {
        int top = Lua.lua_gettop(L);
        if (Lua.lua_checkstack(L, size) != 0)
            Lua.luaL_error(L, "Could not increase stack size by %d to %d - %s", size, size + top, msg);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double numberVnum(TValue o)
    {
        if (LJ_DUALNUM && (o.it == LJ_TISNUM))
            return (double)o.i;
        else
            return o.n;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool lj_obj_equal(TValue o1, TValue o2)
    {
        if (o1.it == o2.it)
        {
            if (o1.it >= LJ_TISPRI)
                return true;
            if (o1.it < LJ_TISNUM)
                return o1.gcr.gcptr32 == o2.gcr.gcptr32;
        }
        else if (o1.it < LJ_TISNUM || o2.it < LJ_TISNUM)
        {
            return false;
        }
        return numberVnum(o1) == numberVnum(o2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_rawequal(IntPtr L, int idx1, int idx2)
    {
        TValue val1 = Game.Index2Struct<TValue>(L, idx1);
        TValue val2 = Game.Index2Struct<TValue>(L, idx2);

        // Retrieve the nilref value from the Lua state (similar to glref + offset logic in C++)
        uint glref = (uint)Marshal.ReadInt32(L + 8);
        IntPtr nilrefPtr = new(glref + 144);
        TValue nilref = Marshal.PtrToStructure<TValue>(nilrefPtr);

        // Check if either value is nilref
        if (val1.Equals(nilref) || val2.Equals(nilref))
            return false;

        return lj_obj_equal(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool lua_isnumber(IntPtr L, int n) => Lua.lua_type(L, n) == Lua.LUA_TNUMBER;

    // TODO?
    //#define luaL_argcheck(L, cond,numarg,extramsg) ((void)((cond) || luaL_argerror(L, (numarg), (extramsg))))

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string luaL_checkstring(IntPtr L, int n) => luaL_checklstring(L, n, out _);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string luaL_optstring(IntPtr L, int n, string d) => luaL_optlstring(L, n, d, out _);

    // TODO?
    //#define luaL_checkint(L,n)      ((int)luaL_checkinteger(L, (n)))

    // TODO?
    //#define luaL_optint(L,n,d)      ((int)luaL_optinteger(L, (n), (d)))

    // TODO?
    //#define luaL_checklong(L,n)     ((long)luaL_checkinteger(L, (n)))

    // TODO?
    //#define luaL_optlong(L,n,d)     ((long)luaL_optinteger(L, (n), (d)))

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T luaL_opt<T>(IntPtr L, Func<IntPtr, int, T> f, int n, T d) => Lua.lua_isnoneornil(L, n) ? d : f.Invoke(L, n);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string luaL_typename(IntPtr L, int n) => Lua.lua_typename(L, Lua.lua_type(L, n));

}
