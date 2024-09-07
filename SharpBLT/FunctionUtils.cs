namespace SharpBLT;

using System.Reflection;
using System.Runtime.InteropServices;

public static class FunctionUtils
{
    public static Hook<TDelegate> CreateHook<TDelegate>(string fieldName, TDelegate del) where TDelegate : Delegate
    {
        FunctionPatternAttribute attr = typeof(TDelegate).GetCustomAttribute<FunctionPatternAttribute>() ?? throw new Exception($"No Function Pattern");

        SearchPattern pattern = new(attr.Pattern);

        IntPtr addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

        if (addr == IntPtr.Zero)
            throw new Exception($"Failed to resolve Method '{fieldName}'");

        Logger.Instance().Log(LogType.Log, $"Address for '{fieldName}' found: 0x{addr:X8}");

        return new Hook<TDelegate>(addr, del);
    }

    public static void ResolveFuntionDelegate<TDelegate>(string fieldName, IntPtr ptr, out TDelegate del) where TDelegate : Delegate
    {
        del = Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);

        if (del == null)
            throw new Exception($"Failed to resolve Method '{fieldName}' -> Ptr: {ptr:X8}");
    }

    public static TDelegate ResolveFunction<TDelegate>(string fieldName) where TDelegate : Delegate
    {
        FunctionPatternAttribute attr = typeof(TDelegate).GetCustomAttribute<FunctionPatternAttribute>() ?? throw new Exception($"No Function Pattern");

        SearchPattern pattern = new(attr.Pattern);

        IntPtr addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

        if (addr == IntPtr.Zero)
            throw new Exception($"Failed to resolve Method '{fieldName}'");

        Logger.Instance().Log(LogType.Log, $"Address for '{fieldName}' found: 0x{addr:X8}");

        TDelegate res = Marshal.GetDelegateForFunctionPointer<TDelegate>(addr) ?? throw new Exception("Failed to create delegate for function pointer");

        return res;
    }
}
