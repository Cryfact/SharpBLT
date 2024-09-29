

namespace SharpBLT
{
    internal class BLTStringDataStore : BLTAbstractDataStore
    {
        private byte[] m_contents;

        public BLTStringDataStore(byte[] content) // std::string is used as a byte array here ?!
        {
            m_contents = content;
        }

        public BLTStringDataStore(string content)
        {
            m_contents = System.Text.Encoding.UTF8.GetBytes(content); // is UTF8 correct?!
        }

        public override bool close()
        {
            throw new NotImplementedException();
        }

        public override bool good()
        {
            return true;
        }

        public override bool is_asynchronous()
        {
            return false;
        }

        public override int read(long position_in_file, byte[] data, int offset, int length)
        {
            // If the start of the read is past the end, stop here
            if (position_in_file >= m_contents.Length)
                return 0;

            // If the end of the read is past the end, shrink it down so it'll fit
            var remaining = m_contents.Length - position_in_file;

            if (remaining < length)
                length = (int)remaining;

            Array.Copy(m_contents, data, length);
            return length;
        }

        public override long size()
        {
            return m_contents.Length;
        }
    }
}
