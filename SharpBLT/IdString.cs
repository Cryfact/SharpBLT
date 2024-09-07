namespace SharpBLT;

using System.Runtime.CompilerServices;

public readonly struct IdString(ulong value) : IComparable<IdString>, IEquatable<ulong>
{
    private readonly ulong _value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToHexString() => $"{_value: x16}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString() => $"Idstring(@ID{ToHexString()}@)";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        if (obj is IdString other)
        {
            return _value == other._value;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode() => _value.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(IdString other)
    {
        return _value.CompareTo(other._value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ulong other)
    {
        return _value.Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IdString lhs, IdString rhs)
    {
        return lhs._value == rhs._value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IdString lhs, IdString rhs)
    {
        return !(lhs == rhs);
    }
}
