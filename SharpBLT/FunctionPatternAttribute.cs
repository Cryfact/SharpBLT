
namespace SharpBLT
{
    public sealed class FunctionPatternAttribute : Attribute
    {
        public string Pattern { get; }

        public FunctionPatternAttribute(string pattern) 
        {
            Pattern = pattern;
        }
    }
}
