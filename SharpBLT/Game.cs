﻿namespace SharpBLT;

using System.Runtime.InteropServices;

public class Game
{
    [FunctionPattern("48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 41 56 41 57 48 83 EC 50 0F 29 74 24 40 0F 29 7C 24 30 48 8B F9")]
    [UnmanagedFunctionPointer(Lua.DefaultCallingConvention)]
    private delegate IntPtr application_update_fn(IntPtr _this, long unk0);

#pragma warning disable CS8618

    private static Hook<application_update_fn> ms_game_update_hook;
    private static application_update_fn old_application_update;

    private static uint ms_main_thread_id;
    //private static int ms_updates;
#pragma warning restore CS8618


    public static void Initialize()
    {
        ms_main_thread_id = Kernel32.GetCurrentThreadId();

        ms_game_update_hook = FunctionUtils.CreateHook<application_update_fn>("application_update", application_update_new);
        ms_game_update_hook.Apply();
        old_application_update = ms_game_update_hook.OldFunction;
    }


    private static IntPtr application_update_new(IntPtr _this, long unk0)
    {
        if (Kernel32.GetCurrentThreadId() != ms_main_thread_id)
            return old_application_update(_this, unk0);

        // TODO: need?
        //if (ms_updates == 0)
        //{
        //    // if (!pd2hook::HTTPManager::GetSingleton()->AreLocksInit())
        //    //  pd2hook::HTTPManager::GetSingleton()->init_locks();
        //    ++ms_updates;
        //}
        //if (ms_updates > 1)
        //{
        HttpEventQueue.Instance().ProcessEvents();
        //}

        // TODO: need?
        //ms_updates++;
        return old_application_update(_this, unk0);
    }
}
