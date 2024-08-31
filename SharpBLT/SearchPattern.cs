using System.Runtime.InteropServices;

namespace SharpBLT
{
    public sealed class SearchPattern
    {
        struct NibblePattern
        {
            public sbyte Nibble;
        };

        private static readonly NibblePattern Wildcard = new NibblePattern { Nibble = -16 };

        private readonly NibblePattern[] m_nibbles;
        private readonly int m_nibbleCount;

        public SearchPattern(string pattern)
        {
            m_nibbles = new NibblePattern[GetNibbleCount(pattern)];

            int i = 0;

            foreach (var c in pattern)
            {
                if (c >= '0' && c <= '9')
                    m_nibbles[i++] = new NibblePattern() { Nibble = (sbyte)(c - '0') };
                else if (c >= 'A' && c <= 'F')
                    m_nibbles[i++] = new NibblePattern() { Nibble = (sbyte)(c - 'A' + 0x0A) };

                else if (c >= 'a' && c <= 'f')
                    m_nibbles[i++] = new NibblePattern() { Nibble = (sbyte)(c - 'a' + 0x0A) };

                else if (c == '?')
                    m_nibbles[i++] = Wildcard;
            }

            m_nibbleCount = i;
        }

        public IntPtr Match(IntPtr startAddress, int size)
        {
            IntPtr pCurrent = startAddress;
            IntPtr pEnd = pCurrent + size;

            while (pCurrent < pEnd)
            {
                var pStart = pCurrent;

                sbyte rShift = 4;
                bool bFound = true;

                foreach (var it in m_nibbles)
                {
                    if (it.Nibble != Wildcard.Nibble)
                    {
                        sbyte nibble = (sbyte)((Marshal.ReadByte(pStart) >> rShift) & 0x0F);

                        if (nibble != it.Nibble)
                        {
                            bFound = false;
                            break;
                        }
                    }

                    rShift ^= 4;
                    pStart += (rShift >> 2);
                }

                if (bFound)
                    return pCurrent;

                ++pCurrent;
            }

            return IntPtr.Zero;
        }

        private int GetNibbleCount(string pattern)
        {
            int space = 0;

            foreach (var c in pattern)
            {
                if (c == ' ')
                    ++space;
            }

            return pattern.Length - space;
        }
    }
}
