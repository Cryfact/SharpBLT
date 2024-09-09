namespace SharpBLT;

using System;
using System.Runtime.CompilerServices;

public struct IdFile : IComparable<IdFile>
{
    public readonly IdString IDSTRING_NONE = new(0);

    public IdString Name;
    public IdString Ext;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IdFile()
    {
        Name = IDSTRING_NONE;
        Ext = IDSTRING_NONE;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IdFile(IdString name, IdString ext)
    {
        Name = name;
        Ext = ext;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj)
    {
        if (obj is IdFile other)
        {
            return other.Name == Name && other.Ext == Ext;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Ext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IdFile other)
    {
        if (Name != other!.Name)
        {
            return Name.CompareTo(other.Name);
        }
        return Ext.CompareTo(other!.Ext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEmpty()
    {
        return Name == IDSTRING_NONE && Ext == IDSTRING_NONE;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return $"{Name}.{Ext}";
    }
}
