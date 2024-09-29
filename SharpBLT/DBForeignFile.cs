using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    internal struct DBForeignFile
    {
        public ulong Magic;
        public IntPtr filename; // std::unique_ptr<std::string> -> std::string*
        public IdFile asset;
        public IntPtr stringLiteral; // std::unique_ptr<std::string> -> std::string*

        public const ulong MAGIC_COOKIE = 0xb4cb844461d94c07; // random value

        public DBForeignFile()
        {
            Magic = 0;
            filename = IntPtr.Zero;
            stringLiteral = IntPtr.Zero;
        }
    }
}
