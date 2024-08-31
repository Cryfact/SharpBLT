
namespace SharpBLT
{
    public sealed class FunctionPattern : Attribute
    {
        public string Pattern { get; }

        public FunctionPattern(string pattern) 
        {
            Pattern = pattern;
        }
    }
}
