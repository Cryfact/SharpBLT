using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
