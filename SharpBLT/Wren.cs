namespace SharpBLT;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class Wren
{
    public enum Result
    {
        Success,
        CompileError,
        RuntimeError,
    }

    public enum ErrorType
    {
        Compile,
        Runtime,
        Stacktrace,
    }

    public enum WrenType
    {
        Bool,
        Num,
        Foreign,
        List,
        Map,
        Null,
        String,
        Unknown,
    }

    public struct WrenLoadModuleResult
    {
        IntPtr source;
        WrenLoadModuleCompleteFn onComplete;
        IntPtr userData;
    }

    struct WrenForeignClassMethods
    {
        // The callback invoked when the foreign object is created.
        //
        // This must be provided. Inside the body of this, it must call
        // [wrenSetSlotNewForeign()] exactly once.
        WrenForeignMethodFn allocate;

        // The callback invoked when the garbage collector is about to collect a
        // foreign object's memory.
        //
        // This may be `NULL` if the foreign class does not need to finalize.
        WrenFinalizerFn finalize;
    }

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void WrenLoadModuleCompleteFn(IntPtr vm, IntPtr name, WrenLoadModuleResult result);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void WrenForeignMethodFn(IntPtr vm);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void WrenFinalizerFn(IntPtr vm);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr WrenReallocateFn(IntPtr memory, IntPtr newSize, IntPtr userData);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr WrenResolveModuleFn(IntPtr vm, IntPtr importer, IntPtr name);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate WrenLoadModuleResult WrenLoadModuleFn(IntPtr vm, IntPtr name);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate WrenForeignMethodFn WrenBindForeignMethodFn(IntPtr vm, IntPtr module, IntPtr userData, IntPtr className,
        [MarshalAs(UnmanagedType.I1)] bool isStatic, IntPtr signature);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr WrenBindForeignClassFn(IntPtr vm, IntPtr module, IntPtr className);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr WrenWriteFn(IntPtr vm, IntPtr test);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr WrenErrorFn(IntPtr vm, ErrorType type, IntPtr module, int line, IntPtr message);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate Result wrenCallFn(IntPtr vm, IntPtr method);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenFreeVMFn(IntPtr vm);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenCollectGarbageFn(IntPtr vm);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenMakeCallHandleFn(IntPtr vm, IntPtr signature);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenReleaseHandleFn(IntPtr vm, IntPtr handle);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate int wrenGetSlotCountFn(IntPtr vm);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenEnsureSlotsFn(IntPtr vm, int numSlots);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate WrenType wrenGetSlotTypeFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool wrenGetSlotBoolFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenGetSlotBytesFn(IntPtr vm, int slot, out int length);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate double wrenGetSlotDoubleFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenGetSlotForeignFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenGetSlotStringFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenGetSlotHandleFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotBoolFn(IntPtr vm, int slot, bool value);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotBytesFn(IntPtr vm, int slot, IntPtr bytes, IntPtr length);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotDoubleFn(IntPtr vm, int slot, double value);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenSetSlotNewForeignFn(IntPtr vm, int slot, int classSlot, IntPtr size);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotNewListFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotNewMapFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotNullFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotStringFn(IntPtr vm, int slot, IntPtr text);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetSlotHandleFn(IntPtr vm, int slot, IntPtr handle);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate int wrenGetListCountFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenGetListElementFn(IntPtr vm, int listSlot, int index, int elementSlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetListElementFn(IntPtr vm, int listSlot, int index, int elementSlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenInsertInListFn(IntPtr vm, int listSlot, int index, int elementSlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate int wrenGetMapCountFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool wrenGetMapContainsKeyFn(IntPtr vm, int mapSlot, int keySlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenGetMapValueFn(IntPtr vm, int mapSlot, int keySlot, int valueSlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetMapValueFn(IntPtr vm, int mapSlot, int keySlot, int valueSlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenRemoveMapValueFn(IntPtr vm, int mapSlot, int keySlot,
                            int removedValueSlot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenGetVariableFn(IntPtr vm, IntPtr module, IntPtr name,
                 int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool wrenHasVariableFn(IntPtr vm, IntPtr module, IntPtr name);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool wrenHasModuleFn(IntPtr vm, IntPtr module);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenAbortFiberFn(IntPtr vm, int slot);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenGetUserDataFn(IntPtr vm);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenSetUserDataFn(IntPtr vm, IntPtr userData);

    [StructLayout(LayoutKind.Sequential)]
    public struct Configuration
    {
        // The callback Wren will use to allocate, reallocate, and deallocate memory.
        //
        // If `NULL`, defaults to a built-in function that uses `realloc` and `free`.
        public WrenReallocateFn reallocateFn;

        // The callback Wren uses to resolve a module name.
        //
        // Some host applications may wish to support "relative" imports, where the
        // meaning of an import string depends on the module that contains it. To
        // support that without baking any policy into Wren itself, the VM gives the
        // host a chance to resolve an import string.
        //
        // Before an import is loaded, it calls this, passing in the name of the
        // module that contains the import and the import string. The host app can
        // look at both of those and produce a new "canonical" string that uniquely
        // identifies the module. This string is then used as the name of the module
        // going forward. It is what is passed to [loadModuleFn], how duplicate
        // imports of the same module are detected, and how the module is reported in
        // stack traces.
        //
        // If you leave this function NULL, then the original import string is
        // treated as the resolved string.
        //
        // If an import cannot be resolved by the embedder, it should return NULL and
        // Wren will report that as a runtime error.
        //
        // Wren will take ownership of the string you return and free it for you, so
        // it should be allocated using the same allocation function you provide
        // above.
        public WrenResolveModuleFn resolveModuleFn;

        // The callback Wren uses to load a module.
        //
        // Since Wren does not talk directly to the file system, it relies on the
        // embedder to physically locate and read the source code for a module. The
        // first time an import appears, Wren will call this and pass in the name of
        // the module being imported. The method will return a result, which contains
        // the source code for that module. Memory for the source is owned by the 
        // host application, and can be freed using the onComplete callback.
        //
        // This will only be called once for any given module name. Wren caches the
        // result internally so subsequent imports of the same module will use the
        // previous source and not call this.
        //
        // If a module with the given name could not be found by the embedder, it
        // should return NULL and Wren will report that as a runtime error.
        public WrenLoadModuleFn loadModuleFn;

        // The callback Wren uses to find a foreign method and bind it to a class.
        //
        // When a foreign method is declared in a class, this will be called with the
        // foreign method's module, class, and signature when the class body is
        // executed. It should return a pointer to the foreign function that will be
        // bound to that method.
        //
        // If the foreign function could not be found, this should return NULL and
        // Wren will report it as runtime error.
        public WrenBindForeignMethodFn bindForeignMethodFn;

        // The callback Wren uses to find a foreign class and get its foreign methods.
        //
        // When a foreign class is declared, this will be called with the class's
        // module and name when the class body is executed. It should return the
        // foreign functions uses to allocate and (optionally) finalize the bytes
        // stored in the foreign object when an instance is created.
        public WrenBindForeignClassFn bindForeignClassFn;

        // The callback Wren uses to display text when `System.print()` or the other
        // related functions are called.
        //
        // If this is `NULL`, Wren discards any printed text.
        public WrenWriteFn writeFn;

        // The callback Wren uses to report errors.
        //
        // When an error occurs, this will be called with the module name, line
        // number, and an error message. If this is `NULL`, Wren doesn't report any
        // errors.
        public WrenErrorFn errorFn;

        // The number of bytes Wren will allocate before triggering the first garbage
        // collection.
        //
        // If zero, defaults to 10MB.
        public IntPtr initialHeapSize;

        // After a collection occurs, the threshold for the next collection is
        // determined based on the number of bytes remaining in use. This allows Wren
        // to shrink its memory usage automatically after reclaiming a large amount
        // of memory.
        //
        // This can be used to ensure that the heap does not get too small, which can
        // in turn lead to a large number of collections afterwards as the heap grows
        // back to a usable size.
        //
        // If zero, defaults to 1MB.
        public IntPtr minHeapSize;

        // Wren will resize the heap automatically as the number of bytes
        // remaining in use after a collection changes. This number determines the
        // amount of additional memory Wren will use after a collection, as a
        // percentage of the current heap size.
        //
        // For example, say that this is 50. After a garbage collection, when there
        // are 400 bytes of memory still in use, the next collection will be triggered
        // after a total of 600 bytes are allocated (including the 400 already in
        // use.)
        //
        // Setting this to a smaller number wastes less memory, but triggers more
        // frequent garbage collections.
        //
        // If zero, defaults to 50.
        public int heapGrowthPercent;

        // User-defined data associated with the VM.
        IntPtr userData;
    }

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate void wrenInitConfigurationFn(ref Configuration config);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate IntPtr wrenNewVMFn(ref Configuration config);

    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    public delegate Result wrenInterpretFn(IntPtr vm, IntPtr module, IntPtr source);

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.

    /// <summary>
    /// Initializes <paramref name="configuration"/> with all of its default values.
    ///
    /// Call this before setting the particular fields you care about.
    /// </summary>
    public static wrenInitConfigurationFn wrenInitConfiguration;

    /// <summary>
    /// Creates a new Wren virtual machine using the given <paramref name="configuration"/>. Wren
    /// will copy the configuration data, so the argument passed to this can be
    /// freed after calling this. If <paramref name="configuration"/> is `NULL`, uses a default
    /// configuration.
    /// </summary>
    public static wrenNewVMFn wrenNewVM;

    /// <summary>
    /// Disposes of all resources is use by <paramref name="vm"/>, which was previously created by a
    /// call to <see cref="wrenNewVM"/>.
    /// </summary>
    public static wrenFreeVMFn wrenFreeVM;

    /// <summary>
    /// Immediately run the garbage collector to free unused memory.
    /// </summary>
    public static wrenCollectGarbageFn wrenCollectGarbage;


    /// <summary>
    /// Calls <paramref name="method"/>, using the receiver and arguments previously set up on the
    /// stack.
    ///
    /// <paramref name="method"/> must have been created by a call to <see cref="wrenMakeCallHandle"/>. The
    /// arguments to the method must be already on the stack. The receiver should be
    /// in slot 0 with the remaining arguments following it, in order. It is an
    /// error if the number of arguments provided does not match the method's
    /// signature.
    ///
    /// After this returns, you can access the return value from slot 0 on the stack.
    /// </summary>
    public static wrenCallFn wrenCall;

    /// <summary>
    /// Releases the reference stored in <paramref name="handle"/>. After calling this, <paramref name="handle"/> can
    /// no longer be used.
    /// </summary>
    public static wrenReleaseHandleFn wrenReleaseHandle;

    /// <summary>
    /// Returns the number of slots available to the current foreign method.
    /// </summary>
    public static wrenGetSlotCountFn wrenGetSlotCount;

    /// <summary>
    /// Ensures that the foreign method stack has at least <paramref name="numSlots"/> available for
    /// use, growing the stack if needed.
    ///
    /// Does not shrink the stack if it has more than enough slots.
    ///
    /// It is an error to call this from a finalizer.
    /// </summary>
    public static wrenEnsureSlotsFn wrenEnsureSlots;

    /// <summary>
    /// Gets the type of the object in <paramref name="slot"/>.
    /// </summary>
    public static wrenGetSlotTypeFn wrenGetSlotType;

    /// <summary>
    /// Reads a boolean value from <paramref name="slot"/>.
    ///
    /// It is an error to call this if the slot does not contain a boolean value.
    /// </summary>
    public static wrenGetSlotBoolFn wrenGetSlotBool;

    /// <summary>
    /// Reads a number from <paramref name="slot"/>.
    ///
    /// It is an error to call this if the slot does not contain a number.
    /// </summary>
    public static wrenGetSlotDoubleFn wrenGetSlotDouble;

    /// <summary>
    /// Reads a foreign object from <paramref name="slot"/> and returns a pointer to the foreign data
    /// stored with it.
    ///
    /// It is an error to call this if the slot does not contain an instance of a
    /// foreign class.
    /// </summary>
    public static wrenGetSlotForeignFn wrenGetSlotForeign;

    /// <summary>
    /// Creates a handle for the value stored in <paramref name="slot"/>.
    ///
    /// This will prevent the object that is referred to from being garbage collected
    /// until the handle is released by calling <see cref="wrenReleaseHandle"/>.
    /// </summary>
    public static wrenGetSlotHandleFn wrenGetSlotHandle;

    /// <summary>
    /// Stores the boolean <paramref name="value"/> in <paramref name="slot"/>.
    /// </summary>
    public static wrenSetSlotBoolFn wrenSetSlotBool;

    /// <summary>
    /// Stores the numeric <paramref name="value"/> in <paramref name="slot"/>.
    /// </summary>
    public static wrenSetSlotDoubleFn wrenSetSlotDouble;

    /// <summary>
    /// Creates a new instance of the foreign class stored in <paramref name="classSlot"/> with <paramref name="size"/>
    /// bytes of raw storage and places the resulting object in <paramref name="slot"/>.
    ///
    /// This does not invoke the foreign class's constructor on the new instance. If
    /// you need that to happen, call the constructor from Wren, which will then
    /// call the allocator foreign method. In there, call this to create the object
    /// and then the constructor will be invoked when the allocator returns.
    /// </summary>
    /// <returns>Returns a pointer to the foreign object's data.</returns>
    public static wrenSetSlotNewForeignFn wrenSetSlotNewForeign;

    /// <summary>
    /// Stores a new empty list in <paramref name="slot"/>.
    /// </summary>
    public static wrenSetSlotNewListFn wrenSetSlotNewList;

    /// <summary>
    /// Stores a new empty map in <paramref name="slot"/>.
    /// </summary>
    public static wrenSetSlotNewMapFn wrenSetSlotNewMap;

    /// <summary>
    /// Stores null in <paramref name="slot"/>.
    /// </summary>
    public static wrenSetSlotNullFn wrenSetSlotNull;

    /// <summary>
    /// Stores the value captured in <paramref name="handle"/> in <paramref name="slot"/>.
    ///
    /// This does not release the handle for the value.
    /// </summary>
    public static wrenSetSlotHandleFn wrenSetSlotHandle;

    /// <summary>
    /// Returns the number of elements in the list stored in <paramref name="slot"/>.
    /// </summary>
    public static wrenGetListCountFn wrenGetListCount;

    /// <summary>
    /// Reads element <paramref name="index"/> from the list in <paramref name="listSlot"/> and stores it in
    /// [elementSlot].
    /// </summary>
    public static wrenGetListElementFn wrenGetListElement;

    /// <summary>
    /// Sets the value stored at <paramref name="index"/> in the list at <paramref name="listSlot"/>, 
    /// to the value from <paramref name="elementSlot"/>. 
    /// </summary>
    public static wrenSetListElementFn wrenSetListElement;

    /// <summary>
    /// Takes the value stored at <paramref name="elementSlot"/> and inserts it into the list stored
    /// at <paramref name="listSlot"/> at <paramref name="index"/>.
    ///
    /// As in Wren, negative indexes can be used to insert from the end. To append
    /// an element, use `-1` for the index.
    /// </summary>
    public static wrenInsertInListFn wrenInsertInList;

    /// <summary>
    /// Returns the number of entries in the map stored in <paramref name="slot"/>.
    /// </summary>
    public static wrenGetMapCountFn wrenGetMapCount;

    /// <summary>
    /// Returns true if the key in <paramref name="keySlot"/> is found in the map placed in <paramref name="mapSlot"/>.
    /// </summary>
    public static wrenGetMapContainsKeyFn wrenGetMapContainsKey;

    /// <summary>
    /// Retrieves a value with the key in <paramref name="keySlot"/> from the map in <paramref name="mapSlot"/> and
    /// stores it in <paramref name="valueSlot"/>.
    /// </summary>
    public static wrenGetMapValueFn wrenGetMapValue;

    /// <summary>
    /// Takes the value stored at <paramref name="source"/>[valueSlot] and inserts it into the map stored
    /// at <paramref name="mapSlot"/> with key <paramref name="keySlot"/>.
    /// </summary>
    public static wrenSetMapValueFn wrenSetMapValue;

    /// <summary>
    /// Removes a value from the map in <paramref name="mapSlot"/>, with the key from <paramref name="keySlot"/>,
    /// and place it in <paramref name="removedValueSlot"/>. If not found, <paramref name="removedValueSlot"/> is
    /// set to null, the same behaviour as the Wren Map API.
    /// </summary>
    public static wrenRemoveMapValueFn wrenRemoveMapValue;

    /// <summary>
    /// Sets the current fiber to be aborted, and uses the value in <paramref name="slot"/> as the
    /// runtime error object.
    /// </summary>
    public static wrenAbortFiberFn wrenAbortFiber;

    /// <summary>
    /// Returns the user data associated with the WrenVM.
    /// </summary>
    public static wrenGetUserDataFn wrenGetUserData;

    /// <summary>
    /// Sets user data associated with the WrenVM.
    /// </summary>
    public static wrenSetUserDataFn wrenSetUserData;

    private static wrenInterpretFn wrenInterpretPtr;
    private static wrenMakeCallHandleFn wrenMakeCallHandlePtr;
    private static wrenGetSlotBytesFn wrenGetSlotBytesPtr;
    private static wrenSetSlotBytesFn wrenSetSlotBytesPtr;
    private static wrenSetSlotStringFn wrenSetSlotStringPtr;
    private static wrenGetVariableFn wrenGetVariablePtr;
    private static wrenHasVariableFn wrenHasVariablePtr;
    private static wrenHasModuleFn wrenHasModulePtr;
    private static wrenGetSlotStringFn wrenGetSlotStringPtr;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.


    [StructLayout(LayoutKind.Sequential)]
    struct WrenFunctions
    {
        public wrenInitConfigurationFn wrenInitConfiguration;
        public wrenNewVMFn wrenNewVM;
        public wrenFreeVMFn wrenFreeVM;
        public wrenCollectGarbageFn wrenCollectGarbage;
        public wrenInterpretFn wrenInterpret;
        public wrenMakeCallHandleFn wrenMakeCallHandle;
        public wrenCallFn wrenCall;
        public wrenReleaseHandleFn wrenReleaseHandle;
        public wrenGetSlotCountFn wrenGetSlotCount;
        public wrenEnsureSlotsFn wrenEnsureSlots;
        public wrenGetSlotTypeFn wrenGetSlotType;
        public wrenGetSlotBoolFn wrenGetSlotBool;
        public wrenGetSlotBytesFn wrenGetSlotBytes;
        public wrenGetSlotDoubleFn wrenGetSlotDouble;
        public wrenGetSlotForeignFn wrenGetSlotForeign;
        public wrenGetSlotStringFn wrenGetSlotString;
        public wrenGetSlotHandleFn wrenGetSlotHandle;
        public wrenSetSlotBoolFn wrenSetSlotBool;
        public wrenSetSlotBytesFn wrenSetSlotBytes;
        public wrenSetSlotDoubleFn wrenSetSlotDouble;
        public wrenSetSlotNewForeignFn wrenSetSlotNewForeign;
        public wrenSetSlotNewListFn wrenSetSlotNewList;
        public wrenSetSlotNewMapFn wrenSetSlotNewMap;
        public wrenSetSlotNullFn wrenSetSlotNull;
        public wrenSetSlotStringFn wrenSetSlotString;
        public wrenSetSlotHandleFn wrenSetSlotHandle;
        public wrenGetListCountFn wrenGetListCount;
        public wrenGetListElementFn wrenGetListElement;
        public wrenSetListElementFn wrenSetListElement;
        public wrenInsertInListFn wrenInsertInList;
        public wrenGetMapCountFn wrenGetMapCount;
        public wrenGetMapContainsKeyFn wrenGetMapContainsKey;
        public wrenGetMapValueFn wrenGetMapValue;
        public wrenSetMapValueFn wrenSetMapValue;
        public wrenRemoveMapValueFn wrenRemoveMapValue;
        public wrenGetVariableFn wrenGetVariable;
        public wrenHasVariableFn wrenHasVariable;
        public wrenHasModuleFn wrenHasModule;
        public wrenAbortFiberFn wrenAbortFiber;
        public wrenGetUserDataFn wrenGetUserData;
        public wrenSetUserDataFn wrenSetUserData;
    }

    public static unsafe void Initialize(IntPtr ptr)
    {
        ref var funcs = ref Unsafe.AsRef<WrenFunctions>(ptr.ToPointer());

        // funcs is living on the stack - copy to ours static members
        wrenInitConfiguration = funcs.wrenInitConfiguration;
        wrenNewVM = funcs.wrenNewVM;
        wrenInterpretPtr = funcs.wrenInterpret;
        wrenMakeCallHandlePtr = funcs.wrenMakeCallHandle;
        wrenCall = funcs.wrenCall;
        wrenReleaseHandle = funcs.wrenReleaseHandle;
        wrenGetSlotCount = funcs.wrenGetSlotCount;
        wrenEnsureSlots = funcs.wrenEnsureSlots;
        wrenGetSlotType = funcs.wrenGetSlotType;
        wrenGetSlotBool = funcs.wrenGetSlotBool;
        wrenGetSlotBytesPtr = funcs.wrenGetSlotBytes;
        wrenGetSlotDouble = funcs.wrenGetSlotDouble;
        wrenGetSlotForeign = funcs.wrenGetSlotForeign;
        wrenGetSlotStringPtr = funcs.wrenGetSlotString;
        wrenGetSlotHandle = funcs.wrenGetSlotHandle;
        wrenSetSlotBool = funcs.wrenSetSlotBool;
        wrenSetSlotBytesPtr = funcs.wrenSetSlotBytes;
        wrenSetSlotDouble = funcs.wrenSetSlotDouble;
        wrenSetSlotNewForeign = funcs.wrenSetSlotNewForeign;
        wrenSetSlotNewList = funcs.wrenSetSlotNewList;
        wrenSetSlotNewMap = funcs.wrenSetSlotNewMap;
        wrenSetSlotNull = funcs.wrenSetSlotNull;
        wrenSetSlotStringPtr = funcs.wrenSetSlotString;
        wrenSetSlotHandle = funcs.wrenSetSlotHandle;
        wrenGetListCount = funcs.wrenGetListCount;
        wrenGetListElement = funcs.wrenGetListElement;
        wrenSetListElement = funcs.wrenSetListElement;
        wrenInsertInList = funcs.wrenInsertInList;
        wrenGetMapCount = funcs.wrenGetMapCount;
        wrenGetMapContainsKey = funcs.wrenGetMapContainsKey;
        wrenGetMapValue = funcs.wrenGetMapValue;
        wrenSetMapValue = funcs.wrenSetMapValue;
        wrenRemoveMapValue = funcs.wrenRemoveMapValue;
        wrenGetVariablePtr = funcs.wrenGetVariable;
        wrenHasVariablePtr = funcs.wrenHasVariable;
        wrenHasModulePtr = funcs.wrenHasModule;
        wrenAbortFiber = funcs.wrenAbortFiber;
        wrenGetUserData = funcs.wrenGetUserData;
        wrenSetUserData = funcs.wrenSetUserData;
    }

    /// <summary>
    /// Runs <paramref name="source"/>, a string of Wren source code in a new fiber in <paramref name="vm"/> in the context of resolved <paramref name="module"/>.
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="module"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Result wrenInterpret(IntPtr vm, string module, string source)
    {
        var mod = Marshal.StringToHGlobalAnsi(module);
        var src = Marshal.StringToHGlobalAnsi(source);

        var res = wrenInterpretPtr(vm, mod, src);

        Marshal.FreeHGlobal(mod);
        Marshal.FreeHGlobal(src);

        return res;
    }

    /// <summary>
    /// Creates a handle that can be used to invoke a method with <paramref name="signature"/> on
    /// using a receiver and arguments that are set up on the stack.
    /// This handle can be used repeatedly to directly invoke that method from C code using <see cref="wrenCall"/>.
    /// When you are done with this handle, it must be released using <see cref="wrenReleaseHandle"/>
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="signature"></param>
    /// <returns></returns>
    public static IntPtr wrenMakeCallHandle(IntPtr vm, string signature)
    {
        var sig = Marshal.StringToHGlobalAnsi(signature);

        var res = wrenMakeCallHandlePtr(vm, sig);

        Marshal.FreeHGlobal(sig);

        return res;
    }

    /// <summary>
    /// Stores the array <paramref name="bytes"/> in <paramref name="slot"/>.
    /// </summary>
    /// <param name="vm"></param>
    /// <param name="slot"></param>
    /// <param name="bytes"></param>
    /// <param name="idx"></param>
    public static void wrenSetSlotBytes(IntPtr vm, int slot, byte[] bytes, int idx = 0)
    {
        wrenSetSlotBytesPtr(vm, slot, Marshal.UnsafeAddrOfPinnedArrayElement(bytes, idx), bytes.Length - idx);
    }

    /// <summary>
    /// Stores the string <paramref name="str"/> in <paramref name="slot"/>.
    /// </summary>
    public static void wrenSetSlotString(IntPtr vm, int slot, string str)
    {
        var s = Marshal.StringToHGlobalAnsi(str);

        wrenSetSlotStringPtr(vm, slot, s);

        Marshal.FreeHGlobal(s);
    }

    /// <summary>
    /// Looks up the top level variable with <paramref name="name"/> in resolved <paramref name="module"/> and stores it in <paramref name="slot"/>.
    /// </summary>
    public static void wrenGetVariable(IntPtr vm, string module, string name, int slot)
    {
        var mod = Marshal.StringToHGlobalAnsi(module);
        var n = Marshal.StringToHGlobalAnsi(name);

        wrenGetVariablePtr(vm, mod, n, slot);

        Marshal.FreeHGlobal(mod);
        Marshal.FreeHGlobal(n);
    }

    /// <summary>
    /// Looks up the top level variable with <paramref name="name"/> in resolved <paramref name="module"/>.
    /// use <see cref="wrenHasModule"/> to ensure that before calling.
    /// </summary>
    /// <returns>returns false if not found. The module must be imported at the time, </returns>
    public static bool wrenHasVariable(IntPtr vm, string module, string name)
    {
        var mod = Marshal.StringToHGlobalAnsi(module);
        var n = Marshal.StringToHGlobalAnsi(name);

        var r = wrenHasVariablePtr(vm, mod, n);

        Marshal.FreeHGlobal(mod);
        Marshal.FreeHGlobal(n);

        return r;
    }

    /// <returns>Returns true if <paramref name="module"/> has been imported/resolved before, false if not.</returns>
    public static bool wrenHasModule(IntPtr vm, string module)
    {
        var mod = Marshal.StringToHGlobalAnsi(module);

        var r = wrenHasModulePtr(vm, mod);

        Marshal.FreeHGlobal(mod);

        return r;
    }

    /// <summary>
    /// Reads a byte array from <paramref name="slot"/>
    /// It is an error to call this if the slot does not contain a string.
    /// </summary>
    /// <returns></returns>
    public static byte[] wrenGetSlotBytes(IntPtr vm, int slot)
    {
        var d = wrenGetSlotBytesPtr(vm, slot, out var len);

        byte[] buffer = new byte[len];

        Marshal.Copy(d, buffer, 0, len);

        return buffer;
    }

    /// <summary>
    /// Reads a string from <paramref name="slot"/>
    /// It is an error to call this if the slot does not contain a string.
    /// </summary>
    /// <returns></returns>
    public static string wrenGetSlotString(IntPtr vm, int slot)
    {
        var s = wrenGetSlotStringPtr(vm, slot);

        return Marshal.PtrToStringAnsi(s) ?? string.Empty;
    }
}