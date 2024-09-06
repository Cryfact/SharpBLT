namespace SharpBLT;

public readonly struct IdString(ulong value) : IComparable<IdString>, IEquatable<ulong>
{
    private readonly ulong _value = value;

    public override readonly string ToString() => $"Idstring(@ID{_value:x16}@)";

    public override readonly bool Equals(object? obj)
    {
        if (obj is IdString other)
        {
            return _value == other._value;
        }
        return false;
    }

    public override readonly int GetHashCode() => _value.GetHashCode();

    public int CompareTo(IdString other)
    {
        return _value.CompareTo(other._value);
    }

    public bool Equals(ulong other)
    {
        return _value.Equals(other);
    }

    public static bool operator ==(IdString lhs, IdString rhs)
    {
        return lhs._value == rhs._value;
    }

    public static bool operator !=(IdString lhs, IdString rhs)
    {
        return !(lhs == rhs);
    }
}
