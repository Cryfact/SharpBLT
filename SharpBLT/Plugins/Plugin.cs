namespace SharpBLT.Plugins;

using System.Runtime.InteropServices;

public enum PluginLoadResult
{
    Success,
    AlreadyLoaded
}

public abstract class Plugin(string file)
{
    private readonly string _file = file;
    private IntPtr _updateFunc;
    private IntPtr _setupStateFunc;
    private IntPtr _pushLuaFunc;

    public string File => _file;

    public void AddToState(IntPtr luaState)
    {
        var setupState = Marshal.GetDelegateForFunctionPointer<SetupStateFunc>(_setupStateFunc);
        setupState(luaState);
    }

    public void Update(IntPtr luaState)
    {
        if (_updateFunc != IntPtr.Zero)
        {
            var update = Marshal.GetDelegateForFunctionPointer<UpdateFunc>(_updateFunc);
            update(luaState);
        }
    }

    public int PushLuaValue(IntPtr luaState)
    {
        if (_pushLuaFunc == IntPtr.Zero)
            return 0;

        var pushLua = Marshal.GetDelegateForFunctionPointer<PushLuaFunc>(_pushLuaFunc);
        return pushLua(luaState);
    }

    protected abstract IntPtr ResolveSymbol(string name);

    protected void Init()
    {
        var apiRevisionPtr = ResolveSymbol("SBLT_API_REVISION");
        if (apiRevisionPtr == IntPtr.Zero) throw new Exception("Missing export SBLT_API_REVISION");

        var apiRevision = Marshal.ReadInt64(apiRevisionPtr);

        if (apiRevision != 1)
        {
            throw new Exception($"Unsupported revision {apiRevision} - you probably need to update SharpBLT");
        }

        CheckLicenseCompliance();

        _setupStateFunc = ResolveSymbol("SharpBLT_Plugin_Init_State");
        if (_setupStateFunc == IntPtr.Zero) throw new Exception("Invalid dlhandle - missing setup_state func!");

        _updateFunc = ResolveSymbol("SharpBLT_Plugin_Update");
        _pushLuaFunc = ResolveSymbol("SharpBLT_Plugin_PushLua");
    }

    private void CheckLicenseCompliance()
    {
        var moduleLicenseDeclaration = Marshal.PtrToStringAnsi(ResolveSymbol("MODULE_LICENCE_DECLARATION"));
        if (moduleLicenseDeclaration != "This module is licenced under the GNU GPL version 2 or later, or another compatible licence")
        {
            throw new Exception($"Invalid licence declaration '{moduleLicenseDeclaration}'");
        }

        var moduleSourceCodeLocation = Marshal.PtrToStringAnsi(ResolveSymbol("MODULE_SOURCE_CODE_LOCATION"));
        //var moduleSourceCodeRevision = Marshal.PtrToStringAnsi(ResolveSymbol("MODULE_SOURCE_CODE_REVISION"));

        if (string.IsNullOrEmpty(moduleSourceCodeLocation))
        {
            // Handle case where source code location is not provided
            Logger.Instance().Log(LogType.Warn, "Loading development plugin! This should never occur outside a development environment");
        }
    }

    private delegate void SetupStateFunc(IntPtr luaState);
    private delegate void UpdateFunc(IntPtr luaState);
    private delegate int PushLuaFunc(IntPtr luaState);
}
