using SharpBLT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public class DslFile
    {
        public IdString name;
        public IdString type;
        public uint fileId;
        public uint rawLangId;
        public IdString langId;

        /**
		 * If there are multiple of this kind of asset, but in different languages, then this
		 * points to another file with the same name/type but a different language.
		 */
        public DslFile? next;

        // These are used for reading, and are picked up from the bundle headers
        public DieselBundle? bundle;
        public uint offset = ~0u;
        public uint length = ~0u;

        public bool Found()
		{
			return bundle != null;
		}

        public bool HasLength()
		{
			return length != ~0u;
		}

        public KeyValuePair<IdString, IdString> Key()
		{
			return new KeyValuePair<IdString, IdString>(name, type);
		}

        public byte[] ReadContents(Stream fi)
        {
            uint realLength = length;
            if (!HasLength())
            {
                // This is an end-of-file asset, so it's length is it's start until the end of the file
                fi.Seek(0, SeekOrigin.End);
                realLength = (uint)fi.Position - offset;
            }

            var data = new byte[realLength];
            fi.Seek(offset, SeekOrigin.Begin);
            fi.Read(data);

            return data;
        }
    }
}
