using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SharpBLT
{
    internal class Raid
    {
        private static byte[] do_xmlload_invoke_bytes = [
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // NFX
                0xFF, 0x25, 0xF2, 0xFF, 0xFF, 0xFF // JMP cs:-14
            ];

        private static byte[] node_from_xml_new_bytes = [
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // NFXNF
                0xFF, 0x25, 0xF2, 0xFF, 0xFF, 0xFF // JMP cs:-14
            ];

        [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
        [FunctionPattern("48 89 5C 24 08 48 89 74 24 20 57 48 83 EC 50")]
        public delegate void node_from_xml_fn(IntPtr arg0, IntPtr arg1, IntPtr arg2);

        [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
        public delegate void node_from_xml_new_fn(IntPtr arg0, IntPtr arg1, IntPtr arg2);

        [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
        public delegate void do_xmlload_invoke_fn(IntPtr arg0, IntPtr arg1, IntPtr arg2);

        private static IntPtr NFXPtr;
        private static IntPtr NFXNFPtr;
        private static IntPtr ms_executableMemory;

        private static Hook<node_from_xml_fn> node_from_xml_hook;

        private static node_from_xml_fn node_from_xml;

        private static node_from_xml_new_fn node_from_xml_new;
        private static do_xmlload_invoke_fn do_xmlload_invoke;

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        static Raid()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
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

        public static void Initialize()
        {
            // Dummy to invoke .cctor
        }

        private static void node_from_xml_new_(IntPtr node, IntPtr data, IntPtr len)
        {
            node_from_xml_hook.Restore();

            var modded = Tweaker.TweakRaidXml(data, Marshal.ReadInt32(len), out var newLen);

            Marshal.WriteInt32(len, newLen);

            do_xmlload_invoke(node, modded, len);

            Tweaker.FreeTweakedRaidXml(modded);

            node_from_xml_hook.Apply();
        }
    }
}
