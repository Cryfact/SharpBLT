﻿namespace SharpBLT;

using System.Runtime.CompilerServices;

public sealed class SearchRange
{
    private static readonly IntPtr ms_startSearchAddress;
    private static readonly int ms_searchSize;

    static SearchRange()
    {
        IntPtr hModule = Kernel32.GetModuleHandle(null);

        if (hModule == IntPtr.Zero)
            throw new Exception("Failed to get current Module Handle");

        Psapi.GetModuleInformation(Kernel32.GetCurrentProcess(), hModule, out Psapi.MODULEINFO modinfo);

        ms_startSearchAddress = modinfo.lpBaseOfDll;
        ms_searchSize = (int)modinfo.SizeOfImage;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntPtr GetStartSearchAddress()
    {
        return ms_startSearchAddress;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSearchSize()
    {
        return ms_searchSize;
    }
}
