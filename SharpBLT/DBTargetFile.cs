using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public class DBTargetFile
    {
        public IdFile Id;

        public bool Fallback;

        public string? plain_file;

        public IdFile direct_bundle;

        public IntPtr wren_loader_obj;

        public DBTargetFile(IdFile id)
        {
            Id = id;
        }

        public void clear_sources()
        {
            plain_file = null;
            direct_bundle = new IdFile();

            if (wren_loader_obj != IntPtr.Zero)
            {
                Wren.wrenReleaseHandle(WrenLoader.GetWrenVM(), wren_loader_obj);
                wren_loader_obj = IntPtr.Zero;
            }
        }
    }
}
