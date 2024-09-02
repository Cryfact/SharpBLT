using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public static class FunctionUtils
    {
        public static Hook<TDelegate> CreateHook<TDelegate>(string fieldName, TDelegate del) where TDelegate : Delegate
        {
            var attr = typeof(TDelegate).GetCustomAttribute<FunctionPatternAttribute>();

            if (attr == null)
                throw new Exception($"No Function Pattern");

            var pattern = new SearchPattern(attr.Pattern);

            var addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

            if (addr == IntPtr.Zero)
                throw new Exception($"Failed to resolve Method '{fieldName}'");

            Logger.Instance().Log(LogType.Log, $"Address for '{fieldName}' found: 0x{addr:X8}");

            return new Hook<TDelegate>(addr, del);
        }

        public static TDelegate ResolveFunction<TDelegate>(string fieldName) where TDelegate : Delegate
        {
            var attr = typeof(TDelegate).GetCustomAttribute<FunctionPatternAttribute>();

            if (attr == null)
                throw new Exception($"No Function Pattern");

            var pattern = new SearchPattern(attr.Pattern);

            var addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

            if (addr == IntPtr.Zero)
                throw new Exception($"Failed to resolve Method '{fieldName}'");

            Logger.Instance().Log(LogType.Log, $"Address for '{fieldName}' found: 0x{addr:X8}");

            var res = Marshal.GetDelegateForFunctionPointer<TDelegate>(addr);

            if (res == null)
                throw new Exception("Failed to create delegate for function pointer");

            return res;
        }
    }
}
