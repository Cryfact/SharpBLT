namespace SharpBLT;

using System.Runtime.InteropServices;

public sealed class ConsoleEx
{
    public ConsoleEx()
    {
        if (!Kernel32.AllocConsole())
            throw new Exception("Failed to alloc console");
    }

    public static void Write(byte[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            if (i == data.Length - 1)
                Console.Write($"0x{data[i]:X2}");
            else
                Console.Write($"0x{data[i]:X2}, ");
        }
    }

    public static void WriteLine(byte[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            if (i == data.Length - 1)
                Console.Write($"0x{data[i]:X2}");
            else
                Console.Write($"0x{data[i]:X2}, ");
        }

        Console.WriteLine();
    }

    public static void Write(IntPtr data, int size)
    {
        for (int i = 0; i < size; ++i)
        {
            if (i == size - 1)
                Console.Write($"0x{Marshal.ReadByte(data + i):X2}");
            else
                Console.Write($"0x{Marshal.ReadByte(data + i):X2}, ");
        }
    }

    public static void WriteLine(IntPtr data, int size)
    {
        for (int i = 0; i < size; ++i)
        {
            if (i == size - 1)
                Console.Write($"0x{Marshal.ReadByte(data + i):X2}");
            else
                Console.Write($"0x{Marshal.ReadByte(data + i):X2}, ");
        }

        Console.WriteLine();
    }
}
