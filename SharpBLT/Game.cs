namespace SharpBLT;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

public class Game
{
    private static byte[] do_xmlload_invoke_bytes = [
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // NFX
            0xFF, 0x25, 0xF2, 0xFF, 0xFF, 0xFF              // JMP cs:-14
        ];

    private static byte[] node_from_xml_new_bytes = [
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // NFXNF
            0xFF, 0x25, 0xF2, 0xFF, 0xFF, 0xFF              // JMP cs:-14
        ];

    [FunctionPattern("48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 41 56 41 57 48 83 EC 50 0F 29 74 24 40 0F 29 7C 24 30 48 8B F9")]
    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    private delegate IntPtr application_update_fn(IntPtr _this, long unk0);

    [FunctionPattern("48 89 54 24 10 55 53 56 57 41 55 41 56 41 57 48 8D 6C 24 E9")]
    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate int try_open_property_match_resolver_fn();

    [FunctionPattern("4C 8B C1 85 D2 7E 23 8D 42 FF 48 63 D0 48 8B 41 20 48 8D 04 D0")]
    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr index2adr_fn(IntPtr luaState, int arg0);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    [FunctionPattern("48 89 5C 24 08 48 89 74 24 20 57 48 83 EC 50")]
    public delegate void node_from_xml_fn(IntPtr arg0, IntPtr arg1, IntPtr arg2);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void node_from_xml_new_fn(IntPtr arg0, IntPtr arg1, IntPtr arg2);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void do_xmlload_invoke_fn(IntPtr arg0, IntPtr arg1, IntPtr arg2);

#pragma warning disable CS8618
    private static application_update_fn application_update;
    private static try_open_property_match_resolver_fn try_open_property_match_resolver;
    private static index2adr_fn index2adr;

    private static Hook<application_update_fn> application_update_hook;
    private static application_update_fn old_application_update;

    private static Hook<node_from_xml_fn> node_from_xml_hook;

    private static node_from_xml_fn node_from_xml;

    private static node_from_xml_new_fn node_from_xml_new;
    private static do_xmlload_invoke_fn do_xmlload_invoke;
#pragma warning restore CS8618

    private static uint ms_main_thread_id;

    private static IntPtr NFXPtr;
    private static IntPtr NFXNFPtr;
    private static IntPtr ms_executableMemory;

    public static IdString LastLoadedName { get; set; }
    public static IdString LastLoadedExt { get; set; }

    public static index2adr_fn Index2adr { get => index2adr; }
    public static T Index2Struct<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(IntPtr L, int n)
    {
        return Marshal.PtrToStructure<T>(index2adr(L, n)!)!;
    }

    public static void Initialize()
    {
        init_game();

        init_id_string_pointers();

        init_xml();
    }

    private static void init_game()
    {
        ms_main_thread_id = Kernel32.GetCurrentThreadId();

        application_update_hook = FunctionUtils.CreateHook<application_update_fn>(nameof(application_update), application_update_new);

        try_open_property_match_resolver = FunctionUtils.ResolveFunction<try_open_property_match_resolver_fn>(nameof(try_open_property_match_resolver));
        index2adr = FunctionUtils.ResolveFunction<index2adr_fn>(nameof(index2adr));

        application_update = application_update_hook.Apply() ?? throw new Exception("Failed to apply application_update hook");
        old_application_update = application_update_hook.OldFunction;
    }

    private static void init_xml()
    {
        ms_executableMemory = Kernel32.VirtualAlloc(IntPtr.Zero, (uint)(do_xmlload_invoke_bytes.Length + node_from_xml_new_bytes.Length),
                Kernel32.AllocationType.Commit | Kernel32.AllocationType.Reserve, Kernel32.MemoryProtection.ExecuteReadWrite);

        Marshal.Copy(do_xmlload_invoke_bytes, 0, ms_executableMemory, do_xmlload_invoke_bytes.Length);
        Marshal.Copy(node_from_xml_new_bytes, 0, ms_executableMemory + do_xmlload_invoke_bytes.Length, node_from_xml_new_bytes.Length);

        NFXPtr = ms_executableMemory;
        node_from_xml_new = Marshal.GetDelegateForFunctionPointer<node_from_xml_new_fn>(NFXPtr + 8);
        NFXNFPtr = ms_executableMemory + do_xmlload_invoke_bytes.Length;
        do_xmlload_invoke = Marshal.GetDelegateForFunctionPointer<do_xmlload_invoke_fn>(NFXNFPtr + 8);

        node_from_xml_hook = FunctionUtils.CreateHook<node_from_xml_fn>(nameof(node_from_xml), node_from_xml_new_);

        Marshal.WriteIntPtr(NFXPtr, node_from_xml_hook.Address);
        Marshal.WriteIntPtr(NFXNFPtr, Marshal.GetFunctionPointerForDelegate<node_from_xml_new_fn>(node_from_xml_new_));
    }

    private static void init_id_string_pointers()
    {
        // Initialize pointers (adjust according to actual offsets)
        IntPtr tmp = Marshal.GetFunctionPointerForDelegate(try_open_property_match_resolver);
        tmp += 0x3A;
        tmp += Marshal.ReadInt32(tmp) + 4;
        LastLoadedName = new((ulong)Marshal.ReadInt64(tmp));

        tmp = Marshal.GetFunctionPointerForDelegate(try_open_property_match_resolver);
        tmp += 0x33;
        tmp += Marshal.ReadInt32(tmp) + 4;
        LastLoadedExt = new((ulong)Marshal.ReadInt64(tmp));
    }

    private static IntPtr application_update_new(IntPtr _this, long unk0)
    {
        if (Kernel32.GetCurrentThreadId() != ms_main_thread_id)
            return old_application_update(_this, unk0);

        HttpEventQueue.Instance().ProcessEvents();

        Lua.UpdatePluginsInActiveStates();

        return old_application_update(_this, unk0);
    }

    private static void node_from_xml_new_(IntPtr node, IntPtr data, IntPtr len)
    {
        Console.WriteLine("node_from_xml_new_ called");

        node_from_xml_hook.Restore();

        var modded = Tweaker.TweakRaidXml(data, Marshal.ReadInt32(len), out var newLen);

        Marshal.WriteInt32(len, newLen);

        do_xmlload_invoke(node, modded, len);

        Tweaker.FreeTweakedRaidXml(modded);

        node_from_xml = node_from_xml_hook.Apply() ?? throw new Exception($"Failed to apply hook for '{nameof(node_from_xml)}'");
    }
}
