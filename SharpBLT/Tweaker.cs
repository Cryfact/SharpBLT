namespace SharpBLT;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class Tweaker
{
    // HashSet to manage the buffers we need to track for memory management.
    private static readonly HashSet<IntPtr> buffers = [];

    // Set to keep track of files that should be ignored.
    private static readonly SortedSet<IdFile> ignoredFiles = [];

    // The last parsed file, to avoid processing the same file multiple times.
    private static IdFile? lastParsed;

    // Static property to enable/disable tweaker functionality.
    public static bool TweakerEnabled { get; set; }

    // Method to tweak Raid XML files. Takes the XML as a string and processes it.
    public static IntPtr TweakRaidXml(IntPtr textPtr, int textLength, out int newLen)
    {
        if (!TweakerEnabled)
        {
            newLen = textLength;
            return textPtr;
        }

        // Convert IntPtr to string
        string text = Marshal.PtrToStringAnsi(textPtr, textLength);

        IdFile file = new IdFile(Game.LastLoadedName, Game.LastLoadedExt);

        // Check if this is the same file being parsed again.
        if (lastParsed == file)
        {
            newLen = textLength;
            return textPtr;
        }

        lastParsed = file;

        // Ignore .model and .texture files
        if (file.Ext.Equals(0xaf612bbc207e00bdUL) ||  // idstring("model")
            file.Ext.Equals(0x5368e150b05a5b8cUL))   // idstring("texture")
        {
            newLen = textLength;
            return textPtr;
        }

        // Check if the file is in the ignored list.
        if (ignoredFiles.Contains(file))
        {
            newLen = textLength;
            return textPtr;
        }

        // Transform the file (You'd replace this with the actual transform logic).
        string newText = WrenLoader.TransformFile(text);

        // If no transformation is needed, return the original pointer.
        if (newText == text)
        {
            newLen = textLength;
            return textPtr;
        }

        // Allocate new memory for the transformed text and copy it.
        int newLength = newText.Length + 1;
        IntPtr buffer = Marshal.AllocHGlobal(newLength);
        buffers.Add(buffer);

        // Copy string into unmanaged memory (accounting for null termination)
        Marshal.Copy(newText.ToCharArray(), 0, buffer, newText.Length);
        Marshal.WriteByte(buffer, newLength - 1, 0); // Null-terminate the string

        newLen = newText.Length;
        return buffer;
    }

    public static void FreeTweakedRaidXml(IntPtr textPtr)
    {
        if (buffers.Remove(textPtr))
        {
            Marshal.FreeHGlobal(textPtr);
        }
    }

    public static void IgnoreFile(IdFile file)
    {
        ignoredFiles.Add(file);
    }
}
