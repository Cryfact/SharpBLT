using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpBLT
{
    public class Assets
    {
        [UnmanagedFunctionPointer(CallingConvention.FastCall)]
        private delegate void StubFn(IntPtr _this, int edx, IntPtr archive, IdString type, IdString name, int u1, int u2);

        private static Hook<StubFn>? hook_0;
        private static Hook<StubFn>? hook_1;
        private static Hook<StubFn>? hook_2;
        private static Hook<StubFn>? hook_try_open_property_match_resolver;

        private static IntPtr try_open_property_match_resolver;

        private static List<IntPtr> try_open_functions;
        private static Dictionary<IdFile, DBTargetFile> overriddenFiles;

        static Assets()
        {
            try_open_functions = new List<nint>();
            overriddenFiles = new Dictionary<IdFile, DBTargetFile>();
        }

        private static void stub_0(IntPtr _this, int edx, IntPtr archive, IdString type, IdString name, int u1, int u2)
        {
            hook_load(try_open_functions[0], hook_0, _this, archive, type, name, u1, u2);
        }

        private static void stub_1(IntPtr _this, int edx, IntPtr archive, IdString type, IdString name, int u1, int u2)
        {
            hook_load(try_open_functions[1], hook_1, _this, archive, type, name, u1, u2);
        }

        private static void stub_2(IntPtr _this, int edx, IntPtr archive, IdString type, IdString name, int u1, int u2)
        {
            hook_load(try_open_functions[2], hook_try_open_property_match_resolver, _this, archive, type, name, u1, u2);
        }

        private static void stub_try_open_property_match_resolver(IntPtr _this, int edx, IntPtr archive, 
            IdString type, IdString name, int u1, int u2)
        {
            hook_load(try_open_property_match_resolver, hook_try_open_property_match_resolver, _this, archive, type, name, u1, u2);
        }

        private static void hook_load(IntPtr orig, Hook<StubFn>? hook, IntPtr this_, 
            IntPtr archive, IdString type, IdString name, int u1, int u2)
        {
            long pos = 0, len = 0;
            string ds_name;

            hook_asset_load(new IdFile(name, type), out var store, out pos, out len, out ds_name, false);
        }

        private static bool hook_asset_load(IdFile asset_file, out BLTAbstractDataStore? out_datastore,
                                               out long out_pos, out long out_len, out string out_name,
                                               bool fallback_mode)
        {
            // First zero everything
            out_datastore = null;
            out_name = string.Empty;
            out_pos = 0;
            out_len = 0;

            if (!overriddenFiles.TryGetValue(asset_file, out var filePtr))
                return false;

            DBTargetFile target = filePtr;

            // If this target is in fallback mode (it'll only load if the base game doesn't provide such a file), and
            // we haven't yet tried loading the base game's version of the file, then stop here.
            if (target.Fallback && !fallback_mode)
            {
                return false;
            }

            long outLen = 0;
            long outPos = 0;
            BLTAbstractDataStore? outDataStore = null;

            var load_file = (string filename) =>
            {
                var ds = BLTFileDataStore.Open(filename);

                if (ds == null)
                {
                    Logger.Instance().Log(LogType.Error, 
                        $"Failed to open hooked file '{filename}' while loading {asset_file.Name}.{asset_file.Ext}");

                    throw new FileNotFoundException(filename);
                }

                outLen = ds.size();
                outDataStore = ds;
            };

            var load_bundle_item = (IdFile bundle_item) =>
            {
                var file = DieselDB.Instance().Find(bundle_item.Name, bundle_item.Ext);

                if (file == null)
                {
                    Logger.Instance().Log(LogType.Error,
                        $"Failed to open hooked asset file {bundle_item.Name}.{bundle_item.Ext} while loading {asset_file.Name}.{asset_file.Ext}");

                    throw new FileNotFoundException(bundle_item.Name.ToString());
                }

                if (file.bundle == null)
                {
                    Logger.Instance().Log(LogType.Error,
                        $"Failed to open hooked asset file {bundle_item.Name}.{bundle_item.Ext} while loading {asset_file.Name}.{asset_file.Ext}");

                    throw new FileNotFoundException(bundle_item.Name.ToString());
                }

                var ds = DieselDB.Open(file.bundle);
                outDataStore = ds;
                outPos = file.offset;

                // If this is an end-of-file asset we have to find it's length
                if (file.HasLength())
                    outLen = file.length;
                else
                    outLen = ds.size() - file.offset;
            };

            if (!string.IsNullOrEmpty(target.plain_file))
                load_file(target.plain_file);
            else if (!target.direct_bundle.IsEmpty())
                load_bundle_item(target.direct_bundle);
            else if (target.wren_loader_obj != IntPtr.Zero)
            {
                try
                {
                    WrenLoader.LockWrenVM();
                    var vm = WrenLoader.GetWrenVM();
                    var callHandle = Wren.wrenMakeCallHandle(vm, "load_file(_,_)");

                    Wren.wrenEnsureSlots(vm, 3);
                    Wren.wrenSetSlotHandle(vm, 0, target.wren_loader_obj);

                    // Set the name
                    Wren.wrenSetSlotString(vm, 1, asset_file.Name.ToHexString());

                    // Set the extension
                    Wren.wrenSetSlotString(vm, 2, asset_file.Ext.ToHexString());

                    // Invoke it - if it fails the game is very likely going to crash anyway, so make it descriptive now
                    var result = Wren.wrenCall(vm, callHandle);
                    if (result == Wren.Result.CompileError || result == Wren.Result.RuntimeError)
                    {
                        Logger.Instance().Log(LogType.Error, $"Wren asset load failed for {asset_file.Name.ToHexString()}.{asset_file.Ext.ToHexString()}: compile or runtime error!");

                        User32.MessageBox(IntPtr.Zero, "Failed to load Wren-based asset - see the log for details", "Wren Error", User32.MB_OK);
                        Environment.Exit(1);
                    }

                    // Get the wrapper, and make sure it's valid
                    var ptr = Wren.wrenGetSlotForeign(vm, 0);

                    if (ptr != IntPtr.Zero)
                    {
                        var ff = Marshal.PtrToStructure<DBForeignFile>(ptr);

                        if (ff.Magic != DBForeignFile.MAGIC_COOKIE)
                        {
                            Logger.Instance().Log(LogType.Error, $"Wren load_file function return invalid class or null - for asset {asset_file.Name.ToHexString()}.{asset_file.Ext.ToHexString()} ptr {ptr:X8}");
                            User32.MessageBox(IntPtr.Zero, "Failed to load Wren-based asset - see the log for details", "Wren Error", User32.MB_OK);
                        }

                        // TODO: check for ff.filename & ff.stringLiteral they are std::string* find the location for const char*
                        // !!!! only for build raw added !!!!
#warning "check for ff.filename & ff.stringLiteral they are std::string* find the location for const char*"

                        if (ff.filename != IntPtr.Zero)
                        {
                            load_file(Marshal.PtrToStringAnsi(ff.filename)); // load_file(*ff->filename); // we got fucked
                        }
                        else if (ff.asset.IsEmpty())
                        {
                            load_bundle_item(ff.asset);
                        }
                        else if (ff.stringLiteral != IntPtr.Zero)
                        {
                            var ds = new BLTStringDataStore(Marshal.PtrToStringAnsi(ff.stringLiteral));  // var ds = new BLTStringDataStore(*ff->stringLiteral); // we got fucked once again
                            out_datastore = ds;
                            out_len = ds.size();
                        }
                        else
                        {
                            // Should never happen
                            Logger.Instance().Log(LogType.Error, "No output contents set for DBForeignFile");
                            User32.MessageBox(IntPtr.Zero, "Failed to load Wren-based asset - see the log for details", "Wren Error", User32.MB_OK);
                        }
                    }
                    else
                    {
                        Logger.Instance().Log(LogType.Error, $"Wren load_file function return invalid class or null - for asset {asset_file.Name.ToHexString()}.{asset_file.Ext.ToHexString()} ptr {ptr:X8}");
                        User32.MessageBox(IntPtr.Zero, "Failed to load Wren-based asset - see the log for details", "Wren Error", User32.MB_OK);
                    }
                }
                finally
                {
                    WrenLoader.UnlockWrenVM();
                }
            }
            else
            {
                // File is disabled, use the regular version of the asset
                return false;
            }

            out_datastore = outDataStore;
            out_name = string.Empty; // TODO: ?!
            out_pos = outPos;
            out_len = outLen;

            return true;
        }

        public static void Initialize()
        {
            var searchPattern = new SearchPattern("6A FF 68 ?? ?? ?? ?? 64 A1 00 00 00 00 50 64 89 25 00 00 00 00 83 EC 24 8B 44 24 38 8B 54 24 44 53 56 8B F1 8B 4C 24 44 89 0D ?? ?? ?? ?? 8B 4C 24 48 33 DB 89 5C 24 0C C6 05 ?? ?? ?? ?? ?? A3 ?? ?? ?? ?? 89 0D ?? ?? ?? ?? 89 15 ?? ?? ?? ?? 8B 44 24 50 50 83 EC 08 8B C4 89 08 8B 4C 24 58 89 48 04 8B 54 24 4C 89 64 24 5C 83 EC 08 8B C4 89 10 8B 4C 24 58 89 48 04 8B CE 89 5C 24 48 89 64 24 64 E8 F8 B8 ?? ?? 3B C3 7D 51 53 68 ?? ?? ?? ?? 8D 4C 24 18 C7 44 24 30 0F 00 00 00 89 5C 24 2C");
            try_open_property_match_resolver = searchPattern.Match(SearchRange.GetStartSearchAddress(), SearchRange.GetSearchSize());

            FindAssetLoadSignatures(); // init try_open_functions
            InitAssets(); // init hooks
        }

        private static void FindAssetLoadSignatures(/*string module, SignatureCacheDB cache, out int cache_misses*/)
        {
            var searchPattern = new SearchPattern("48 89 54 24 10 55 53 56 57 41 54 41 56 41 57 48 8D 6C 24 E9 48 81 EC E0 00 00 00 49");
            int targetCount = 3;

            var dllBase = SearchRange.GetStartSearchAddress();
            var size = SearchRange.GetSearchSize();

           // cache_misses = 0;

            // we don't have a cache, and this cache was useless, he searched for the adresses even when the adress was found in cache.
            // Implement caching - if all the signatures are at the same place, assume it's still working
           // int cache_count = cache.GetAddress("asset_load_signatures_count");
           // if (cache_count == targetCount)
            {
                for (int i = 0; i < targetCount; i++)
                {
                //    var target = cache.GetAddress($"asset_load_signatures_id_{i}");

                    // Make sure this signature is in-bounds
                   // if (target >= size - searchPattern.Length)
                   //     goto cache_fail;

                    IntPtr searchedAddress = searchPattern.Match(dllBase, (int)size);

                    if (searchedAddress == IntPtr.Zero)
                        throw new Exception("invalid adress found for asset_load_signatures");

                    try_open_functions.Add(searchedAddress);
                }
                return;

           // cache_fail:
           //     try_open_functions.Clear();
            }

            // Make sure the cache gets updated afterwards
            /*++cache_misses;

            for (var i = 0; i < size - searchPattern.Length; i++)
            {
                var searchedAddress = searchPattern.Match(dllBase + i, (int)size);

                if (searchedAddress == IntPtr.Zero)
                    continue;

                // Some games (PDTH) have very similar try_open signatures, so double check here.
                if (searchedAddress == try_open_property_match_resolver)
                {
                    Logger.Instance().Log(LogType.Log, $"Asset loading signature ({searchedAddress:X8}) matched 'try_open_property_match_resolver' ({try_open_property_match_resolver_ptr:X8}) ignoring...");
                    continue;
                }

                cache.UpdateAddress($"asset_load_signatures_id_{try_open_functions.Count}", i);

                try_open_functions.Add(Marshal.GetDelegateForFunctionPointer<StubFn>(searchedAddress));

                Logger.Instance().Log(LogType.Log, $"Found signature #{try_open_functions.Count} for asset loading at {searchedAddress:X8}");
            }

            cache.UpdateAddress("asset_load_signatures_count", try_open_functions.Count);

            if (targetCount < try_open_functions.Count)
            {
                Logger.Instance().Log(LogType.Warn, "Failed to locate enough instances of the asset loading function:");
            }
            else if (targetCount > try_open_functions.Count)
            {
                Logger.Instance().Log(LogType.Warn, "Located too many instances of the asset loading function:");
            }*/
        }

        private static void InitAssets()
        {
            if (try_open_functions.Count != 0)
            {
                hook_0 = new Hook<StubFn>(try_open_functions[0], stub_0);
                hook_0.Apply();
            }

            if (try_open_functions.Count > 1)
            {
                hook_1 = new Hook<StubFn>(try_open_functions[1], stub_1);
                hook_1.Apply();
            }

            if (try_open_functions.Count > 2)
            {
                hook_2 = new Hook<StubFn>(try_open_functions[2], stub_2);
                hook_2.Apply();
            }

            hook_try_open_property_match_resolver = new Hook<StubFn>(
                try_open_property_match_resolver, stub_try_open_property_match_resolver);

            hook_try_open_property_match_resolver.Apply();
            setup_extra_asset_hooks();
        }

        private static void setup_extra_asset_hooks()
        { }
    }
}
