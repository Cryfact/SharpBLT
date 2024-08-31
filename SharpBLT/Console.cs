
namespace SharpBLT
{
    public sealed class Console
    {
        public Console() 
        {
            if (!Kernel32.AllocConsole())
                throw new Exception("Failed to alloc console");
        }
    }
}
