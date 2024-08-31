using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public class FunctionTarget : Attribute
    {
        public string FunctionName { get; }

        public Type DelegateType { get; }

        public FunctionTarget(string funcName, Type delegateType) 
        {
            FunctionName = funcName;
            DelegateType = delegateType;
        }
    }
}
