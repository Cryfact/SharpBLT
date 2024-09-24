namespace SharpBLT;

internal class WrenLoader
{
    private static readonly object _lock = new();
    private static bool available = true;
    private static IntPtr vm = IntPtr.Zero;

    internal static IntPtr GetWrenVM()
    {
        if (vm != IntPtr.Zero)
            return vm;

        if (available)
        {
            // If the main file doesn't exist, do nothing
            if (!File.Exists("mods/base/wren/base.wren"))
            // TODO
            //Util::FileType ftyp = Util::GetFileType("mods/base/wren/base.wren");
            //if (ftyp == Util::FileType_None)
            {
                Logger.Instance().Log(LogType.Warn, "Wren base file not found, Wren VM disabled - the basemod may be corrupted");
                available = false;
            }
        }
        if (!available)
            return IntPtr.Zero;

        lock (_lock)
        {
            Wren.Configuration config = new();
            Wren.wrenInitConfiguration(ref config);
            // TODO
            //config.errorFn = err;
            //config.bindForeignMethodFn = bindForeignMethod;
            //config.bindForeignClassFn = bindForeignClass;
            //config.resolveModuleFn = resolveModule;
            //config.loadModuleFn = getModulePath;
            vm = Wren.wrenNewVM(ref config);

            Wren.Result result = Wren.wrenInterpret(vm, "__root", "import \"base/base\"");
            if (result == Wren.Result.CompileError || result == Wren.Result.RuntimeError)
            {
                Logger.Instance().Log(LogType.Error, "Wren init failed: compile or runtime error!");
                _ = User32.MessageBox(IntPtr.Zero, "Failed to initialise the Wren system - see the log for details", "Wren Error", User32.MB_OK);
                throw new ApplicationException("Failed to initialise the Wren system");
            }

            return vm;
        }
    }

    public static string TransformFile(string text)
    {
        IntPtr vm = GetWrenVM();
        lock (_lock)
        {
            // If the Wren runtime is unavailable, return the original text.
            if (vm == IntPtr.Zero)
                return text;

            Wren.wrenEnsureSlots(vm, 4);

            Wren.wrenGetVariable(vm, "base/base", "BaseTweaker", 0);
            IntPtr tweakerClassHandle = Wren.wrenGetSlotHandle(vm, 0);
            IntPtr methodHandle = Wren.wrenMakeCallHandle(vm, "tweak(_,_,_)");

            Wren.wrenSetSlotHandle(vm, 0, tweakerClassHandle);

            Wren.wrenSetSlotString(vm, 1, Game.LastLoadedName.ToHexString());
            Wren.wrenSetSlotString(vm, 2, Game.LastLoadedExt.ToHexString());

            Wren.wrenSetSlotString(vm, 3, text);

            // TODO give a reasonable amount of information on what happened.
            Wren.Result result = Wren.wrenCall(vm, methodHandle);
            if (result == Wren.Result.CompileError)
            {
                Logger.Instance().Log(LogType.Error, "Wren tweak file failed: compile error!");
                return text;
            }
            else if (result == Wren.Result.RuntimeError)
            {
                Logger.Instance().Log(LogType.Error, "Wren tweak file failed: runtime error!");
                return text;
            }

            Wren.wrenReleaseHandle(vm, tweakerClassHandle);
            Wren.wrenReleaseHandle(vm, methodHandle);

            return Wren.wrenGetSlotString(vm, 0);
        }
    }
}
