using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public abstract class BLTAbstractDataStore
    {
        public virtual int write(long position_in_file, byte[] data, int offset, int length) // Stubbed with an abort
        {
            throw new NotImplementedException();
        }

	    public abstract int read(long position_in_file, byte[] data, int offset, int length);
	    public abstract bool close();
	    public abstract long size();
        public abstract bool is_asynchronous();
       // public abstract void set_asynchronous_completion_callback(IntPtr _ /*dsl::LuaRef*/); // ignore this
        //public abstract ulong state(); // ignore this
        public abstract bool good();
    }
}
