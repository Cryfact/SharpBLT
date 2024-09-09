using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBLT
{
    public class BLTFileDataStore : BLTAbstractDataStore
    {
        private long file_size;
        private FileStream m_fileStream;

        public override bool close()
        {
            throw new NotImplementedException();
        }

        public override bool good()
        {
            throw new NotImplementedException();
        }

        public override bool is_asynchronous()
        {
            return false;
        }

        public override int read(long position_in_file, byte[] data, int offset, int length)
        {
            m_fileStream.Seek(position_in_file, SeekOrigin.Begin);

            var result = m_fileStream.Read(data, offset, length);

            return result;
        }

        public override long size()
        {
            return file_size;
        }

        public static BLTFileDataStore Open(string filename)
        {
            var result = new BLTFileDataStore();
            result.m_fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            result.file_size = result.m_fileStream.Length;
            return result;
        }
    }
}
