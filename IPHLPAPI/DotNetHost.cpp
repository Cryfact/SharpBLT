#include <Windows.h>

#define __USING_NATIVE_AOT__

#ifndef __USING_NATIVE_AOT__
#include <hostfxr.h>
#include <nethost.h>
#include <coreclr_delegates.h>
#endif

#include <string>
#include <shlwapi.h>
#include "DotNetHost.h"

#ifdef UNICODE
using string = std::wstring;
#else
using string = std::string;
#endif

#ifndef __USING_NATIVE_AOT__
static hostfxr_initialize_for_runtime_config_fn init_fptr;
static hostfxr_get_runtime_delegate_fn get_delegate_fptr;
static hostfxr_close_fn close_fptr;
#endif


void LoadDotNetRuntime()
{
    string runtimePath(MAX_PATH, '\0');

    GetModuleFileName(nullptr, runtimePath.data(), MAX_PATH);
    PathRemoveFileSpec(runtimePath.data());

    runtimePath.resize(std::wcslen(runtimePath.data()));

#ifdef __USING_NATIVE_AOT__
    using EntryPointFn = void(*)(); 

    HMODULE hModule = LoadLibraryA("mods\\base\\SharpBLT.dll");

    auto entrypoint = reinterpret_cast<EntryPointFn>(GetProcAddress(hModule, "NativeMain"));

    entrypoint();
#else
    char_t buffer[MAX_PATH];
    size_t buffer_size = sizeof(buffer) / sizeof(char_t);
    int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);

    HMODULE hHostFxr = LoadLibrary(buffer);

    if (hHostFxr == nullptr)
    {
        return;
    }

    init_fptr = reinterpret_cast<hostfxr_initialize_for_runtime_config_fn>(GetProcAddress(hHostFxr, "hostfxr_initialize_for_runtime_config"));
    get_delegate_fptr = reinterpret_cast<hostfxr_get_runtime_delegate_fn>(GetProcAddress(hHostFxr, "hostfxr_get_runtime_delegate"));
    close_fptr = reinterpret_cast<hostfxr_close_fn>(GetProcAddress(hHostFxr, "hostfxr_close"));

    if (init_fptr == nullptr || get_delegate_fptr == nullptr || close_fptr == nullptr)
    {
        return;
    }

    hostfxr_handle context;

    string configPath = runtimePath + __TEXT("\\mods\\base\\NETHostFxr.runtimeconfig.json");

    rc = init_fptr(configPath.c_str(), nullptr, &context);

    load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;

    rc = get_delegate_fptr(
        context,
        hdt_load_assembly_and_get_function_pointer,
        reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));

    typedef void (CORECLR_DELEGATE_CALLTYPE* custom_entry_point_fn)(void);
    custom_entry_point_fn entryPoint = nullptr;

    string libPath = runtimePath + __TEXT("\\mods\\base\\SharpBLT.dll");

    rc = load_assembly_and_get_function_pointer(libPath.c_str(), __TEXT("SharpBLT.Program, SharpBLT"), __TEXT("Main"),
        UNMANAGEDCALLERSONLY_METHOD, nullptr, reinterpret_cast<void**>(&entryPoint));

    entryPoint();
#endif
}
