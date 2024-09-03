namespace SharpBLT;

using System.Reflection;
using System.Runtime.InteropServices;

public static class FunctionUtils
{
    public static Hook<TDelegate> CreateHook<TDelegate>(string fieldName, TDelegate del) where TDelegate : Delegate
    {
        var attr = typeof(TDelegate).GetCustomAttribute<FunctionPatternAttribute>() ?? throw new Exception($"No Function Pattern");

        var pattern = new SearchPattern(attr.Pattern);

        var addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

        if (addr == IntPtr.Zero)
            throw new Exception($"Failed to resolve Method '{fieldName}'");

        Logger.Instance().Log(LogType.Log, $"Address for '{fieldName}' found: 0x{addr:X8}");

        return new Hook<TDelegate>(addr, del);
    }

    public static TDelegate ResolveFunction<TDelegate>(string fieldName) where TDelegate : Delegate
    {
        var attr = typeof(TDelegate).GetCustomAttribute<FunctionPatternAttribute>() ?? throw new Exception($"No Function Pattern");

        var pattern = new SearchPattern(attr.Pattern);

        var addr = pattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

        if (addr == IntPtr.Zero)
            throw new Exception($"Failed to resolve Method '{fieldName}'");

        Logger.Instance().Log(LogType.Log, $"Address for '{fieldName}' found: 0x{addr:X8}");

        var res = Marshal.GetDelegateForFunctionPointer<TDelegate>(addr) ?? throw new Exception("Failed to create delegate for function pointer");

        return res;
    }
}
