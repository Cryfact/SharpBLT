namespace SharpBLT;

using System;

public class IdFile : IComparable<IdFile>
{
    public readonly IdString IDSTRING_NONE = new(0);

    public IdString Name { get; set; }
    public IdString Ext { get; set; }

    public IdFile()
    {
        Name = IDSTRING_NONE;
        Ext = IDSTRING_NONE;
    }

    public IdFile(IdString name, IdString ext)
    {
        Name = name;
        Ext = ext;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IdFile other)
        {
            return other?.Name == Name && other.Ext == Ext;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Ext);
    }

    public int CompareTo(IdFile? other)
    {
        if (Name != other!.Name)
        {
            return Name.CompareTo(other.Name);
        }
        return Ext.CompareTo(other!.Ext);
    }

    public bool IsEmpty()
    {
        return Name == IDSTRING_NONE && Ext == IDSTRING_NONE;
    }

    public override string ToString()
    {
        return $"{Name}.{Ext}";
    }
}
