namespace SharpBLT;

using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

public class LuaMod
{
    static int _httpRequestCounter = 0;

    public static void Initialize(IntPtr L)
    {
        Lua.lua_pushcclosure(L, luaF_print, 0);
        Lua.lua_setfield(L, Lua.LUA_GLOBALSINDEX, "log");

        Lua.lua_pushcclosure(L, luaF_pcall, 0);
        Lua.lua_setfield(L, Lua.LUA_GLOBALSINDEX, "pcall");

        Lua.lua_pushcclosure(L, luaF_dofile, 0);
        Lua.lua_setfield(L, Lua.LUA_GLOBALSINDEX, "dofile");

        Lua.lua_pushcclosure(L, luaF_unzipfile, 0);
        Lua.lua_setfield(L, Lua.LUA_GLOBALSINDEX, "unzip");

        Lua.lua_pushcclosure(L, luaF_dohttpreq, 0);
        Lua.lua_setfield(L, Lua.LUA_GLOBALSINDEX, "dohttpreq");

        LuaReg[] consoleLib = [
            new("CreateConsole", luaF_createconsole),
            new("DestroyConsole", luaF_destroyconsole),
        ];
        Lua.luaI_openlib(L, "console", consoleLib, 0);

        LuaReg[] fileLib = [
            new("GetDirectories", luaF_getdir),
            new("GetFiles", luaF_getfiles),
            new("RemoveDirectory", luaF_removeDirectory),
            new("DirectoryExists", luaF_directoryExists),
            new("DirectoryHash", luaF_directoryhash),
            new("FileExists", luaF_fileExists),
            new("FileHash", luaF_filehash),
            new("MoveDirectory", luaF_moveDirectory),
            new("CreateDirectory", luaF_createDirectory),
        ];
        Lua.luaI_openlib(L, "file", fileLib, 0);

        LuaReg[] bltLib = [
            new("ispcallforced", luaF_ispcallforced),
            new("forcepcalls", luaF_forcepcalls),
            new("blt_info", luaF_blt_info),
            new("GetDllVersion", luaF_GetDllVersion),
            new("EnableApplicationLog", luaF_EnableApplicationLog),
            new("pcall", luaF_pcall_proper), // Lua pcall shouldn't print errors, however BLT's global pcall does (leave it for compat)
		    new("xpcall", luaF_xpcall),
            new("parsexml", luaF_parsexml),
            new("structid", luaF_structid),
            //new("ignoretweak", luaF_ignoretweak), // TODO?
            //new("load_native", luaF_load_native), // TODO? ooof!
        ];
        Lua.luaI_openlib(L, "blt", bltLib, 0);

        Logger.Instance().Log(LogType.Log, $"Loading SharpBLT Lua base mod onto {L}");

        validate_mod_base();

        var result = Lua.luaL_loadfilex(L, "mods/base/base.lua");

        if (result == Lua.LUA_ERRSYNTAX)
        {
            Logger.Instance().Log(LogType.Error, Lua.lua_tolstring(L, -1, out _));
            return;
        }

        result = Lua.lua_pcall(L, 0, 1, 0);

        if (result == Lua.LUA_ERRRUN)
        {
            Logger.Instance().Log(LogType.Error, Lua.lua_tolstring(L, -1, out _));
            return;
        }

        Logger.Instance().Log(LogType.Log, $"SharpBLT on {L} ready!");
    }

    private static void validate_mod_base()
    {
        // TODO: Implement this
        //if (!std::filesystem::exists("mods/base/mod.xml") || !std::filesystem::exists("mods/base/base.lua"))
        //{
        //    PD2HOOK_LOG_LOG("Downloading Mod Base");
        //
        //    if (!pd2hook::HTTPManager::GetSingleton()->AreLocksInit())
        //        pd2hook::HTTPManager::GetSingleton()->init_locks();
        //
        //    pd2hook::HTTPManager::GetSingleton()->DownloadFile("https://api.modworkshop.net/mods/21618/download", "mods/base.zip");
        //
        //    pd2hook::ExtractZIPArchive("mods/base.zip", "mods");
        //
        //    if (std::filesystem::exists("mods/base.zip"))
        //        std::filesystem::remove("mods/base.zip");
        //}
    }

    private static int luaF_ispcallforced(IntPtr L)
    {
        Lua.lua_pushboolean(L, Lua.GetNewCallFunctionHook().IsEnabled);
        return 1;
    }

    private static int luaF_forcepcalls(IntPtr L)
    {
        int n = Lua.lua_gettop(L); // Number of arguments
        if (n < 1)
        {
            Logger.Instance().Log(LogType.Warn, "blt.forcepcalls(): Called with no arguments, ignoring request");
            return 0;
        }

        var lua_call_hook = Lua.GetNewCallFunctionHook();

        if (Lua.lua_toboolean(L, 1))
        {
            if (!lua_call_hook.IsEnabled)
            {
                lua_call_hook.Apply();
                Logger.Instance().Log(LogType.Log, "blt.forcepcalls(): Protected calls will now be forced");
            }
            //		else Logging::Log("blt.forcepcalls(): Protected calls are already being forced, ignoring request", Logging::LOGGING_WARN);
        }
        else
        {
            if (lua_call_hook.IsEnabled)
            {
                lua_call_hook.Restore();
                Logger.Instance().Log(LogType.Log, "blt.forcepcalls(): Protected calls are no longer being forced");
            }
            //		else Logging::Log("blt.forcepcalls(): Protected calls are not currently being forced, ignoring request", Logging::LOGGING_WARN);
        }
        return 0;
    }

    private static int luaF_EnableApplicationLog(IntPtr L)
    {
        int n = Lua.lua_gettop(L); // Number of arguments
        if (n < 1)
        {
            Logger.Instance().Log(LogType.Warn, "blt.EnableApplicationLog(): Called with no arguments");
            return 0;
        }

        // TODO: Implement this
        //EnableApplicationLog(lua_toboolean(L, 1));
        return 0;
    }

    private static int luaF_GetDllVersion(IntPtr L)
    {
        // TODO: Implement this
        //HMODULE hModule;
        //GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPCTSTR)luaF_GetDllVersion, &hModule);
        //char path[MAX_PATH + 1];
        //size_t pathSize = GetModuleFileName(hModule, path, sizeof(path) - 1);
        //path[pathSize] = '\0';
        //
        //DWORD verHandle = 0;
        //UINT size = 0;
        //LPBYTE lpBuffer = NULL;
        //uint32_t verSize = GetFileVersionInfoSize(path, &verHandle);
        //
        //if (verSize == 0)
        //{
        //    PD2HOOK_LOG_ERROR("Error occurred while calling 'GetFileVersionInfoSize': {}", GetLastError());
        Lua.lua_pushstring(L, "0.0.0.0");
        //    return 1;
        //}
        //
        //std::string verData;
        //verData.resize(verSize);
        //
        //if (!GetFileVersionInfo(path, verHandle, verSize, verData.data()))
        //{
        //    PD2HOOK_LOG_ERROR("Error occurred while calling 'GetFileVersionInfo': {}", GetLastError());
        //    lua_pushstring(L, "0.0.0.0");
        //    return 1;
        //}
        //
        //if (!VerQueryValue(verData.data(), "\\", (VOID FAR * FAR *) & lpBuffer, &size))
        //{
        //    PD2HOOK_LOG_ERROR("Error occurred while calling 'VerQueryValue': {}", GetLastError());
        //    lua_pushstring(L, "0.0.0.0");
        //    return 1;
        //}
        //
        //if (size == 0)
        //{
        //    PD2HOOK_LOG_ERROR("Invalid version value buffer Size");
        //    lua_pushstring(L, "0.0.0.0");
        //    return 1;
        //}
        //
        //VS_FIXEDFILEINFO* verInfo = (VS_FIXEDFILEINFO*)lpBuffer;
        //if (verInfo->dwSignature != 0xfeef04bd)
        //{
        //    PD2HOOK_LOG_ERROR("Invalid version signature");
        //    lua_pushstring(L, "0.0.0.0");
        //    return 1;
        //}
        //
        //std::string strVersion = std::format("{}.{}.{}.{}",
        //                                     (verInfo->dwFileVersionMS >> 16) & 0xFFFF,
        //                                     (verInfo->dwFileVersionMS >> 0) & 0xFFFF,
        //                                     (verInfo->dwFileVersionLS >> 16) & 0xFFFF,
        //                                     (verInfo->dwFileVersionLS >> 0) & 0xFFFF);
        //
        //lua_pushstring(L, strVersion.c_str());
        return 1;
    }

    private static int luaF_print(IntPtr L)
    {
        int top = Lua.lua_gettop(L);

        for (int i = 0; i < top; ++i)
        {
            string str = Lua.lua_tolstring(L, i + 1, out _);
            Logger.Instance().Log(LogType.Lua, (i > 0 ? "    " : "") + str);
        }

        return 0;
    }

    private static int luaF_pcall(IntPtr L)
    {
        int args = Lua.lua_gettop(L) - 1;

        Lua.lua_getglobal(L, "debug");
        Lua.lua_getfield(L, -1, "traceback");
        Lua.lua_remove(L, -2);
        // Do not index from the top (i.e. don't use a negative index)
        int errorhandler = Lua.lua_gettop(L) - args - 1;
        Lua.lua_insert(L, errorhandler);

        int result = Lua.lua_pcall(L, args, Lua.LUA_MULTRET, errorhandler);
        // lua_pcall() automatically pops the callee function and its arguments off the stack. Then, if no errors were encountered
        // during execution, it pushes the return values onto the stack, if any. Otherwise, if an error was encountered, it pushes
        // the error message onto the stack, which should manually be popped off when done using to keep the stack balanced
        if (result == Lua.LUA_ERRRUN)
        {
            Logger.Instance().Log(LogType.Error, Lua.lua_tolstring(L, -1, out _));
            // This call pops the error message off the stack
            Lua.lua_pop(L, 1);
            return 0;
        }
        // Do not use lua_pop() as the callee function's return values may be present, which would pop one of those instead and leave
        // the error handler on the stack
        Lua.lua_remove(L, errorhandler);
        Lua.lua_pushboolean(L, result == 0);
        Lua.lua_insert(L, 1);

        // if (result != 0) return 1;

        return Lua.lua_gettop(L);
    }

    private static int luaF_pcall_proper(IntPtr L)
    {
        Lua.luaL_checkany(L, 1);
        int status = Lua.lua_pcall(L, Lua.lua_gettop(L) - 1, Lua.LUA_MULTRET, 0);
        Lua.lua_pushboolean(L, status == 0);
        Lua.lua_insert(L, 1);
        return Lua.lua_gettop(L); // return status + all results
    }

    private static int luaF_xpcall(IntPtr L)
    {
        // Args: func, err, ...

        // Move err from the 2nd index to the 1st index, so we have a continuous range for the function call
        Lua.lua_pushvalue(L, 2); // Copy err to the top
        Lua.lua_remove(L, 2); // Remove err
        Lua.lua_insert(L, 1); // Put error function under function to be called

        int status = Lua.lua_pcall(L, Lua.lua_gettop(L) - 2, Lua.LUA_MULTRET, 1);

        // Replace the error function (1st position) with the result
        Lua.lua_pushboolean(L, (status == 0));
        Lua.lua_replace(L, 1);

        return Lua.lua_gettop(L);  // return entire stack - status, possible err + all results
    }

    private static int luaF_dofile(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string filename = Lua.lua_tolstring(L, 1, out _);

        //Logger.Instance().Log(LogType.Lua, $"luaF_dofile: {filename}");

        int error = Lua.luaL_loadfilex(L, filename);
        if (error != 0)
        {
            Logger.Instance().Log(LogType.Error, Lua.lua_tolstring(L, -1, out _));
        }
        else
        {
            Lua.lua_getglobal(L, "debug");
            Lua.lua_getfield(L, -1, "traceback");
            Lua.lua_remove(L, -2);
            // Example stack for visualization purposes:
            // 3 debug.traceback
            // 2 compiled code as a self-contained function
            // 1 filename
            // Do not index from the top (i.e. don't use a negative index)
            int errorhandler = 2;
            Lua.lua_insert(L, errorhandler);

            error = Lua.lua_pcall(L, 0, 0, errorhandler);
            if (error == Lua.LUA_ERRRUN)
            {
                Logger.Instance().Log(LogType.Error, Lua.lua_tolstring(L, -1, out _));
                // This call pops the error message off the stack
                Lua.lua_pop(L, 1);
            }
            // Do not use lua_pop() as the callee function's return values may be present, which would pop one of those instead and
            // leave the error handler on the stack
            Lua.lua_remove(L, errorhandler);
        }

        return 0;
    }

    private static int luaF_unzipfile(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string archivePath = Lua.lua_tolstring(L, 1, out _);
        string extractPath = Lua.lua_tolstring(L, 2, out _);

        ZipFile.ExtractToDirectory(archivePath, extractPath);

        return 0;
    }

    private static int luaF_dohttpreq(IntPtr L)
    {
        Logger.Instance().Log(LogType.Log, "Incoming HTTP Request/Request");

        int args = Lua.lua_gettop(L);
        int progressReference = 0;
        if (args >= 3)
        {
            progressReference = Lua.luaL_ref(L, Lua.LUA_REGISTRYINDEX);
        }

        int functionReference = Lua.luaL_ref(L, Lua.LUA_REGISTRYINDEX);
        string url = Lua.lua_tolstring(L, 1, out _);

        _httpRequestCounter++;
        int requestId = _httpRequestCounter;

        Logger.Instance().Log(LogType.Log, $"{url} - (request {requestId}) sent..");

        _ = Http.DoHttpReqAsync(
            url,
            new()
            {
                id = requestId,
                functionReference = functionReference,
                progressReference = progressReference,
                L = L,
                url = url,
            },
            HttpRequestDone,
            progressReference != 0 ? HttpRequestProgress : null
        );

        Lua.lua_pushinteger(L, requestId);

        return 1;
    }

    private static void HttpRequestProgress(HttpEventData data, long progress, long total)
    {
        if (!Lua.check_active_state(data.L))
        {
            return;
        }

        if (data.progressReference == 0)
            return;

        Lua.lua_rawgeti(data.L, Lua.LUA_REGISTRYINDEX, data.progressReference);
        Lua.lua_pushinteger(data.L, data.id);
        Lua.lua_pushinteger(data.L, (int)progress);
        Lua.lua_pushinteger(data.L, (int)total);
        Lua.lua_pcall(data.L, 3, 0, 0);
    }

    private static void HttpRequestDone(HttpEventData data, byte[] result)
    {
        if (!Lua.check_active_state(data.L))
        {
            return;
        }

        Lua.lua_rawgeti(data.L, Lua.LUA_REGISTRYINDEX, data.functionReference);
        Lua.lua_pushlstring(data.L, Marshal.UnsafeAddrOfPinnedArrayElement(result, 0), result.Length);
        Lua.lua_pushinteger(data.L, data.id);
        Lua.lua_pcall(data.L, 2, 0, 0);

        Lua.luaL_unref(data.L, Lua.LUA_REGISTRYINDEX, data.functionReference);
        if (data.progressReference != 0)
            Lua.luaL_unref(data.L, Lua.LUA_REGISTRYINDEX, data.progressReference);
    }

    private static int luaF_createconsole(IntPtr L)
    {
        Logger.Instance().OpenConsole();
        return 0;
    }

    private static int luaF_destroyconsole(IntPtr L)
    {
        Logger.Instance().DestroyConsole();
        return 0;
    }

    private static int luaF_getdir(IntPtr L)
    {
        return luaH_getcontents(L, true);
    }

    private static int luaF_getfiles(IntPtr L)
    {
        return luaH_getcontents(L, false);
    }

    private static int luaF_removeDirectory(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string dir = Lua.lua_tolstring(L, 1, out _);

        try
        {
            Directory.Delete(dir, true);

            Lua.lua_pushboolean(L, true);
        }
        catch (Exception ex)
        {
            Logger.Instance().Log(LogType.Warn, ex.Message);

            Lua.lua_pushboolean(L, false);
        }

        return 1;
    }

    private static int luaF_directoryExists(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string dir = Lua.lua_tolstring(L, 1, out _);

        Lua.lua_pushboolean(L, Directory.Exists(dir));

        return 1;
    }

    private static int luaF_directoryhash(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string path = Lua.lua_tolstring(L, 1, out _);

        string hash = Hasher.GetDirectoryHash(path);

        Lua.lua_pushlstring(L, hash, hash.Length);

        return 1;
    }

    private static int luaF_fileExists(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string path = Lua.lua_tolstring(L, 1, out _);

        Lua.lua_pushboolean(L, File.Exists(path));

        return 1;
    }

    private static int luaF_filehash(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string path = Lua.lua_tolstring(L, 1, out _);

        string hash = Hasher.GetFileHash(path);

        Lua.lua_pushlstring(L, hash, hash.Length);

        return 1;
    }

    private static int luaF_moveDirectory(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string src = Lua.lua_tolstring(L, 1, out _);
        string dst = Lua.lua_tolstring(L, 2, out _);

        try
        {
            Directory.Move(src, dst);
            Lua.lua_pushboolean(L, true);
        }
        catch (Exception)
        {
            Lua.lua_pushboolean(L, false);
        }

        return 1;
    }

    private static int luaF_createDirectory(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string path = Lua.lua_tolstring(L, 1, out _);

        DirectoryInfo dirInfo = Directory.CreateDirectory(path);

        Lua.lua_pushboolean(L, dirInfo != null && dirInfo.Exists);

        return 1;
    }

    private static int luaH_getcontents(IntPtr L, bool files)
    {
        //int n = Lua.lua_gettop(L);

        string dir = Lua.lua_tolstring(L, 1, out _);
        string[] directories;

        try
        {
            if (!files)
                directories = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
            else
                directories = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);

            directories = directories.Select((x) => x.Replace(dir, string.Empty)).ToArray();
        }
        catch (Exception)
        {
            Lua.lua_pushboolean(L, false);
            return 1;
        }

        Lua.lua_createtable(L, 0, 0);

        int index = 1;
        foreach (var it in directories)
        {
            if (it == "." || it == "..")
                continue;

            Lua.lua_pushinteger(L, index);
            Lua.lua_pushlstring(L, it, it.Length);
            Lua.lua_settable(L, -3);
            index++;
        }

        return 1;
    }

    private static int luaF_blt_info(IntPtr L)
    {
        Lua.lua_newtable(L);

        Lua.lua_pushstring(L, "mswindows");
        Lua.lua_setfield(L, -2, "platform");

        Lua.lua_pushstring(L, "x86-64");
        Lua.lua_setfield(L, -2, "arch");

        Lua.lua_pushstring(L, "raid");
        Lua.lua_setfield(L, -2, "game");

        return 1;
    }

    private static int luaF_parsexml(IntPtr L)
    {
        //int n = Lua.lua_gettop(L);

        string xml = Lua.lua_tolstring(L, 1, out _);

        try
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xml);

            XmlNode? baseNode = xmlDoc.DocumentElement;

            if (baseNode != null && baseNode.Name.StartsWith("?xml"))
            {
                baseNode = baseNode.FirstChild;
            }

            if (baseNode != null)
            {
                BuildXmlTree(L, baseNode);
            }
            else
            {
                Logger.Instance().Log(LogType.Error, $"Parsed XML does not contain any nodes{Environment.NewLine}{xml}");
                Lua.lua_pushnil(L);
            }
        }
        catch (XmlException ex)
        {
            Logger.Instance().Log(LogType.Error, $"Could not parse XML: Error and original file below{Environment.NewLine}{ex.Message}{Environment.NewLine}{xml}");
            Lua.lua_pushnil(L);
        }

        return 1;
    }
    private static void BuildXmlTree(IntPtr L, XmlNode node)
    {
        // Create the main table
        Lua.lua_newtable(L);

        // Set the element name
        Lua.lua_pushstring(L, node.Name);
        Lua.lua_setfield(L, -2, "name");

        // Create the parameters table
        Lua.lua_newtable(L);
        if (node.Attributes != null)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                Lua.lua_pushstring(L, attr.Value);
                Lua.lua_setfield(L, -2, attr.Name);
            }
        }
        Lua.lua_setfield(L, -2, "params");

        // Add all the child nodes
        XmlNode? child = node.FirstChild;
        int i = 1;
        while (child != null)
        {
            if (child.NodeType == XmlNodeType.Element && !child.Name.StartsWith("!--"))
            {
                BuildXmlTree(L, child);
                Lua.lua_rawseti(L, -2, i++);
            }
            child = child.NextSibling;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TValue
    {
        public uint gcptr32;  // Pseudo 32-bit pointer
        public uint it;       // Internal object tag (must overlap with MSW of number)
    }

    private static unsafe ref TValue GetTValuePtr(IntPtr L)
    {
        return ref Unsafe.AsRef<TValue>(*(TValue**)(L + (sizeof(IntPtr) * 2)));
    }

    // Basically the same thing as lua_topointer
    private static int luaF_structid(IntPtr L)
    {
        int n = Lua.lua_gettop(L);

        if (n != 1)
        {
            Lua.luaL_error(L, "Signature: structid(struct)");
        }

        IntPtr valuePtr = IntPtr.Zero;

        ref TValue value = ref GetTValuePtr(L);

        if (Lua.lua_type(L, 1) == Lua.LUA_TUSERDATA || Lua.lua_islightuserdata(L, 1))
        {
            valuePtr = Lua.lua_touserdata(L, 1);
        }
        else if (value.it > (~13u))
        {
            valuePtr = (IntPtr)(ulong)value.gcptr32;
        }
        else
        {
            Lua.luaL_error(L, "Illegal argument - should be tvgcv (table) or userdata");
        }

        // Convert the pointer to a hexadecimal string
        string buffer = string.Format("{0:X8}", valuePtr.ToInt64());

        // Push the string onto the Lua stack
        Lua.lua_pushstring(L, buffer);

        return 1;
    }

    //private static int luaF_ignoretweak(lua_State* L)
    //{
    //    blt::idfile file;

    //    file.name = luaX_toidstring(L, 1);
    //    file.ext = luaX_toidstring(L, 2);

    //    tweaker::ignore_file(file);

    //    return 0;
    //}

    //private static int luaF_load_native(IntPtr L)
    //{
    //    string file = Lua.lua_tostring(L, 1);

    //    try
    //    {
    //        blt::plugins::Plugin* plugin = NULL;
    //        blt::plugins::PluginLoadResult result = blt::plugins::LoadPlugin(file, &plugin);

    //        // TODO some kind of UUID system to prevent issues with multiple mods having the same DLL

    //        int count = plugin->PushLuaValue(L);

    //        if (result == blt::plugins::plr_AlreadyLoaded)
    //        {
    //            Lua.lua_pushstring(L, "Already loaded");
    //        }
    //        else
    //        {
    //            Lua.lua_pushboolean(L, true);
    //        }

    //        Lua.lua_insert(L, -1 - count);
    //        return count + 1;

    //    }
    //    catch (Exception e)
    //    {
    //        Lua.luaL_error(L, e.Message);
    //        return 0; // Fix the no-return compiler warning, but this will never be called
    //    }
    //}
}
