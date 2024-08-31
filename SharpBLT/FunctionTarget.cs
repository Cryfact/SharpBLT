
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
