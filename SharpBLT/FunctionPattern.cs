using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
