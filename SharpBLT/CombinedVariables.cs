﻿namespace SharpBLT;

using System.Runtime.InteropServices;

// Author: Chris Eelmaa

#region VariableCombiner

class CombinedVariables : IDisposable
{
    readonly IntPtr _ptr;
    readonly IList<IDisposable> _disposables;

    bool _disposed;

    public CombinedVariables(VariableArgument[] args)
    {
        _disposables = [];

        _ptr = Marshal.AllocHGlobal(args.Sum(arg => arg.GetSize()));
        IntPtr curPtr = _ptr;

        foreach (VariableArgument arg in args)
        {
            _disposables.Add(arg.Write(curPtr));
            curPtr += arg.GetSize();
        }
    }

    public IntPtr GetPtr()
    {
        if (_disposed)
            throw new InvalidOperationException("Disposed already.");

        return _ptr;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();

            Marshal.FreeHGlobal(_ptr);
        }
    }
}

#endregion

#region VariableArgument

public abstract class VariableArgument
{
    #region SentinelDispose

    protected static readonly IDisposable SentinelDisposable =
          new SentinelDispose();

    class SentinelDispose : IDisposable
    {
        public void Dispose()
        {

        }
    }

    #endregion

    public abstract IDisposable Write(IntPtr buffer);

    public virtual int GetSize()
    {
        return IntPtr.Size;
    }

    public static implicit operator VariableArgument(int input)
    {
        return new VariableIntegerArgument(input);
    }

    public static implicit operator VariableArgument(string input)
    {
        return new VariableStringArgument(input);
    }

    public static implicit operator VariableArgument(double input)
    {
        return new VariableDoubleArgument(input);
    }
}

#endregion

#region VariableIntegerArgument

sealed class VariableIntegerArgument(int value) : VariableArgument
{
    readonly int _value = value;

    public override IDisposable Write(IntPtr buffer)
    {
        Marshal.Copy(new[] { _value }, 0, buffer, 1);
        return SentinelDisposable;
    }
}

#endregion

#region VariableDoubleArgument

sealed class VariableDoubleArgument(double value) : VariableArgument
{
    readonly double _value = value;

    public override int GetSize()
    {
        return 8;
    }

    public override IDisposable Write(IntPtr buffer)
    {
        Marshal.Copy(new[] { _value }, 0, buffer, 1);
        return SentinelDisposable;
    }
}

#endregion

#region VariableStringArgument

sealed class VariableStringArgument(string value) : VariableArgument
{
    readonly string _value = value;

    public override IDisposable Write(IntPtr buffer)
    {
        IntPtr ptr = Marshal.StringToHGlobalAnsi(_value);

        Marshal.Copy(new[] { ptr }, 0, buffer, 1);

        return new StringArgumentDisposable(ptr);
    }

    #region StringArgumentDisposable

    class StringArgumentDisposable(IntPtr ptr) : IDisposable
    {
        IntPtr _ptr = ptr;

        public void Dispose()
        {
            if (_ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_ptr);
                _ptr = IntPtr.Zero;
            }
        }
    }

    #endregion
}
#endregion
