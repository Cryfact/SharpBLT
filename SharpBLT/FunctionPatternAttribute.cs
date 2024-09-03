namespace SharpBLT;

[AttributeUsage(AttributeTargets.Delegate)]
public sealed class FunctionPatternAttribute(string pattern) : Attribute
{
    public string Pattern { get; } = pattern;
}
