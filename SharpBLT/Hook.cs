﻿using SharpDisasm.Udis86;
using System.Runtime.InteropServices;

namespace SharpBLT
{
    public sealed class Hook<TDelegate> where TDelegate : System.Delegate
    {
        private IntPtr m_address;
        private IntPtr m_targetAddress;
        private IntPtr m_trampolineAddress;
        private TDelegate m_target;
        private TDelegate m_trampoline;
        private TDelegate m_original;

        private byte[] m_origBytes;
        private byte[] m_newBytes;

        public bool IsEnabled { get; private set; }

        public Hook(IntPtr address, TDelegate target)
        { 
            m_address = address;
            m_original = Marshal.GetDelegateForFunctionPointer<TDelegate>(m_address);
            m_target = target;
            m_targetAddress = Marshal.GetFunctionPointerForDelegate(m_target);

            List<ud_type> registerInUse = [ ud_type.UD_R_RCX, ud_type.UD_R_RDX, ud_type.UD_R_R8, ud_type.UD_R_R9 ]; // all used registers in fastcall on x64 (RCX used on thiscall)

            m_newBytes = GetJumpBytes(m_address, m_targetAddress);

            int length = Analyze(registerInUse, m_newBytes.Length, 20);

            m_origBytes = new byte[length];

            Marshal.Copy(m_address, m_origBytes, 0, length);

            AllocateTrampoline(length * 3);

            m_trampoline = Marshal.GetDelegateForFunctionPointer<TDelegate>(m_trampolineAddress);

            ApplyJumps(registerInUse, length);
        }

        public TDelegate? Apply()
        {
            if (m_trampolineAddress == IntPtr.Zero)
                return null;

            if (IsEnabled)
                return m_trampoline;

            HookMemoryExecuteReadWrite(m_address, m_origBytes.Length);

            Marshal.Copy(m_newBytes, 0, m_address, m_newBytes.Length);

            HookMemoryExecuteOnly(m_address, m_origBytes.Length);

            IsEnabled = true;

            return m_trampoline;
        }

        public void Restore()
        {
            if (!IsEnabled)
                return;

            HookMemoryExecuteReadWrite(m_address, m_origBytes.Length);

            Marshal.Copy(m_origBytes, 0, m_address, m_origBytes.Length);

            HookMemoryExecuteOnly(m_address, m_origBytes.Length);

            IsEnabled = false;
        }

        public void Destroy()
        {
            if (IsEnabled)
                Restore();

            m_address = IntPtr.Zero;
            m_targetAddress = IntPtr.Zero;

            if (m_trampolineAddress != IntPtr.Zero)
            {
                HookMemoryFree(m_trampolineAddress, 0);
                m_trampolineAddress = IntPtr.Zero;
            }
        }

        private byte Analyze(List<ud_type> registerInUse, int minSize, int decodeSize)
        {
            byte length = 0;
            var disasm = new SharpDisasm.Disassembler(m_address, decodeSize, SharpDisasm.ArchitectureMode.x86_64);

            foreach (var insn in disasm.Disassemble())
            {
                length += (byte)insn.Length;

                if (length >= minSize)
                    return length;
            }

            return length;
        }

        private void AllocateTrampoline(int size)
        {
            m_trampolineAddress = AllocatePageNearMemory(m_address, size);

            if (m_trampolineAddress == IntPtr.Zero)
            {
                m_trampolineAddress = HookMemoryAlloc(IntPtr.Zero, size);

                if (m_trampolineAddress == IntPtr.Zero)
                    throw new Exception("failed to allocate memory");
            }
        }

        private void ApplyJumps(List<ud_type> registerInUse, int size)
        {
            var pBuffer = m_trampolineAddress;

            Marshal.Copy(m_origBytes, 0, pBuffer, m_origBytes.Length);
            pBuffer += m_origBytes.Length;

            var src = pBuffer.ToInt64();
            var dst = m_address.ToInt64();

            bool isNearTargetFunc = ((src + 5 + 0x7FFFFFFF) >= dst) || ((src + 5 - 0x7FFFFFFF) <= dst);

            if (!isNearTargetFunc)
            {
                // perform 64 bit jump
                var unusedRegister = GetUnusedRegister(registerInUse);

                if (unusedRegister != null)
                {
                    ApplyJump(pBuffer, m_address + size, unusedRegister.Value);
                }
                else
                {
                    throw new Exception("All registers are already in use and immediate jump is not possible.");
                }
            }
            else
            {
                ApplyJump(pBuffer, m_address + size, ud_type.UD_NONE);
            }
        }

        private static byte ApplyJump(IntPtr pSource, IntPtr pTarget, ud_type reg)
        {
            IntPtr pDst = pSource;

            if (reg == ud_type.UD_NONE)
            {
                var addressDif = (uint)(pTarget.ToInt64() - (pSource.ToInt64() + 5));

                Marshal.WriteByte(pDst++, 0xE9);
                Marshal.WriteInt32(pDst, (int)addressDif);
                pDst += sizeof(int);
            }
            else
            {
                byte jumpReg = 0xE0;
                bool hasPrefix = false;

                switch (reg)
                {
                    case ud_type.UD_R_RAX:
                    case ud_type.UD_R_EAX:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xB8);

                        jumpReg = 0xE0;
                        break;
                    case ud_type.UD_R_RCX:
                    case ud_type.UD_R_ECX:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xB9);

                        jumpReg = 0xE1;
                        break;
                    case ud_type.UD_R_RDX:
                    case ud_type.UD_R_EDX:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xBA);

                        jumpReg = 0xE2;
                        break;
                    case ud_type.UD_R_RBX:
                    case ud_type.UD_R_EBX:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xBB);

                        jumpReg = 0xE3;
                        break;
                    case ud_type.UD_R_RSP:
                    case ud_type.UD_R_ESP:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xBC);

                        jumpReg = 0xE4;
                        break;
                    case ud_type.UD_R_RBP:
                    case ud_type.UD_R_EBP:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xBD);

                        jumpReg = 0xE5;
                        break;
                    case ud_type.UD_R_RSI:
                    case ud_type.UD_R_ESI:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xBE);

                        jumpReg = 0xE6;
                        break;
                    case ud_type.UD_R_RDI:
                    case ud_type.UD_R_EDI:
                        Marshal.WriteByte(pDst++, 0x48);
                        Marshal.WriteByte(pDst++, 0xBF);

                        jumpReg = 0xE7;
                        break;
                    case ud_type.UD_R_R8:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xB8);

                        jumpReg = 0xE0;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R9:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xB9);

                        jumpReg = 0xE1;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R10:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xBA);

                        jumpReg = 0xE2;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R11:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xBB);

                        jumpReg = 0xE3;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R12:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xBC);

                        jumpReg = 0xE4;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R13:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xBD);

                        jumpReg = 0xE5;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R14:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xBE);

                        jumpReg = 0xE6;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R15:
                        Marshal.WriteByte(pDst++, 0x49);
                        Marshal.WriteByte(pDst++, 0xBF);

                        jumpReg = 0xE7;
                        hasPrefix = true;
                        break;
                }

                Marshal.WriteInt64(pDst, pTarget.ToInt64());
                pDst += sizeof(long);

                if (hasPrefix)
                    Marshal.WriteByte(pDst++, 0x41);

                Marshal.WriteByte(pDst++, 0xFF);
                Marshal.WriteByte(pDst++, jumpReg);
            }

            return (byte)(pDst.ToInt64() - pSource.ToInt64());
        }

        private static IntPtr HookMemoryAlloc(IntPtr address, int size)
        {
            return Kernel32.VirtualAlloc(address, (uint)size, Kernel32.AllocationType.Commit | Kernel32.AllocationType.Reserve, Kernel32.MemoryProtection.ReadWrite);
        }

        private static void HookMemoryFree(IntPtr address, int size)
        {
            Kernel32.VirtualFree(address, 0, Kernel32.FreeType.Release);
        }

        private static IntPtr AllocatePageNearMemory(IntPtr address, int size)
        {
            var sysInfo = new Kernel32.SYSTEM_INFO();
            Kernel32.GetSystemInfo(ref sysInfo);

            ulong pageSize = sysInfo.dwPageSize;

            ulong startAddr = (ulong)address.ToInt64() & ~(pageSize - 1);
            ulong minAddr = Math.Min(startAddr - 0x7FFFFF00, (ulong)sysInfo.lpMinimumApplicationAddress.ToInt64());
            ulong maxAddr = Math.Max(startAddr + 0x7FFFFF00, (ulong)sysInfo.lpMaximumApplicationAddress.ToInt64());

            ulong startPage = (startAddr - (startAddr % pageSize));
            uint pageOffset = 0;
            ulong highAddr;
            ulong lowAddr;

            do
            {
                pageOffset++;
                ulong byteOffset = pageOffset * pageSize;
                highAddr = startPage + byteOffset;
                lowAddr = (startPage > byteOffset) ? startPage - byteOffset : 0;

                if (highAddr < maxAddr)
                {
                    IntPtr outAddr = HookMemoryAlloc(new IntPtr((long)highAddr), size);

                    if (outAddr != IntPtr.Zero)
                        return outAddr;
                }

                if (lowAddr > minAddr)
                {
                    IntPtr outAddr = HookMemoryAlloc(new IntPtr((long)lowAddr), size);

                    if (outAddr != IntPtr.Zero)
                        return outAddr;
                }

            } while (highAddr <= maxAddr || lowAddr >= minAddr);

            return IntPtr.Zero;
        }

        private static byte[] GetJumpBytes(IntPtr address, IntPtr target)
        {
            List<ud_type> registerInUse = [ ud_type.UD_R_RCX, ud_type.UD_R_RDX, ud_type.UD_R_R8, ud_type.UD_R_R9 ]; // all used registers in fastcall on x64 (RCX used on thiscall)
            long src = address.ToInt64();
            long dst = target.ToInt64();

            bool isNearTargetFunc = ((src + 5 + 0x7FFFFFFF) >= dst) || ((src + 5 - 0x7FFFFFFF) <= dst);

            if (!isNearTargetFunc)
            {
                // perform 64 bit jump
                var unusedRegister = GetUnusedRegister(registerInUse);

                if (unusedRegister != null)
                {
                    return CreateJump(address, target, unusedRegister.Value);
                }
                else
                {
                    throw new Exception("All registers are already in use and immediate jump is not possible.");
                }
            }
            else
            {
                return CreateJump(address, target, ud_type.UD_NONE);
            }
        }

        private static ud_type? GetUnusedRegister(List<ud_type> registers)
        {
            List<ud_type> allRegisters = [
                ud_type.UD_R_RAX,
                ud_type.UD_R_RCX,
                ud_type.UD_R_RDX,
                ud_type.UD_R_RBX,
                ud_type.UD_R_RSP,
                ud_type.UD_R_RBP,
                ud_type.UD_R_RSI,
                ud_type.UD_R_RDI,
                ud_type.UD_R_R8,
                ud_type.UD_R_R9,
                ud_type.UD_R_R10,
                ud_type.UD_R_R11,
                ud_type.UD_R_R12,
                ud_type.UD_R_R13,
                ud_type.UD_R_R14,
                ud_type.UD_R_R15,
            ];

            foreach (var it in registers)
            {
                if (!allRegisters.Contains(it))
                {
                    switch (it)
                    {
                        case ud_type.UD_R_RAX:
                        case ud_type.UD_R_EAX:
                        case ud_type.UD_R_AX:
                        case ud_type.UD_R_AL:
                        case ud_type.UD_R_AH:
                            allRegisters.Remove(ud_type.UD_R_RAX);
                            break;
                        case ud_type.UD_R_RCX:
                        case ud_type.UD_R_ECX:
                        case ud_type.UD_R_CX:
                        case ud_type.UD_R_CL:
                        case ud_type.UD_R_CH:
                            allRegisters.Remove(ud_type.UD_R_RCX);
                            break;
                        case ud_type.UD_R_RDX:
                        case ud_type.UD_R_EDX:
                        case ud_type.UD_R_DX:
                        case ud_type.UD_R_DL:
                        case ud_type.UD_R_DH:
                            allRegisters.Remove(ud_type.UD_R_RDX);
                            break;
                        case ud_type.UD_R_RBX:
                        case ud_type.UD_R_EBX:
                        case ud_type.UD_R_BX:
                        case ud_type.UD_R_BL:
                        case ud_type.UD_R_BH:
                            allRegisters.Remove(ud_type.UD_R_RBX);
                            break;
                        case ud_type.UD_R_RSP:
                        case ud_type.UD_R_ESP:
                        case ud_type.UD_R_SP:
                        case ud_type.UD_R_SPL:
                            allRegisters.Remove(ud_type.UD_R_RSP);
                            break;
                        case ud_type.UD_R_RBP:
                        case ud_type.UD_R_EBP:
                        case ud_type.UD_R_BP:
                        case ud_type.UD_R_BPL:
                            allRegisters.Remove(ud_type.UD_R_RBP);
                            break;
                        case ud_type.UD_R_RSI:
                        case ud_type.UD_R_ESI:
                        case ud_type.UD_R_SI:
                        case ud_type.UD_R_SIL:
                            allRegisters.Remove(ud_type.UD_R_RSI);
                            break;
                        case ud_type.UD_R_RDI:
                        case ud_type.UD_R_EDI:
                        case ud_type.UD_R_DI:
                        case ud_type.UD_R_DIL:
                            allRegisters.Remove(ud_type.UD_R_RDI);
                            break;
                        case ud_type.UD_R_R8:
                        case ud_type.UD_R_R8D:
                        case ud_type.UD_R_R8W:
                        case ud_type.UD_R_R8B:
                            allRegisters.Remove(ud_type.UD_R_R8);
                            break;
                        case ud_type.UD_R_R9:
                        case ud_type.UD_R_R9D:
                        case ud_type.UD_R_R9W:
                        case ud_type.UD_R_R9B:
                            allRegisters.Remove(ud_type.UD_R_R9);
                            break;
                        case ud_type.UD_R_R10:
                        case ud_type.UD_R_R10D:
                        case ud_type.UD_R_R10W:
                        case ud_type.UD_R_R10B:
                            allRegisters.Remove(ud_type.UD_R_R10);
                            break;
                        case ud_type.UD_R_R11:
                        case ud_type.UD_R_R11D:
                        case ud_type.UD_R_R11W:
                        case ud_type.UD_R_R11B:
                            allRegisters.Remove(ud_type.UD_R_R11);
                            break;
                        case ud_type.UD_R_R12:
                        case ud_type.UD_R_R12D:
                        case ud_type.UD_R_R12W:
                        case ud_type.UD_R_R12B:
                            allRegisters.Remove(ud_type.UD_R_R12);
                            break;
                        case ud_type.UD_R_R13:
                        case ud_type.UD_R_R13D:
                        case ud_type.UD_R_R13W:
                        case ud_type.UD_R_R13B:
                            allRegisters.Remove(ud_type.UD_R_R13);
                            break;
                        case ud_type.UD_R_R14:
                        case ud_type.UD_R_R14D:
                        case ud_type.UD_R_R14W:
                        case ud_type.UD_R_R14B:
                            allRegisters.Remove(ud_type.UD_R_R14);
                            break;
                        case ud_type.UD_R_R15:
                        case ud_type.UD_R_R15D:
                        case ud_type.UD_R_R15W:
                        case ud_type.UD_R_R15B:
                            allRegisters.Remove(ud_type.UD_R_R15);
                            break;
                    }
                }
            }

            if (allRegisters.Count == 0)
                return null;

            return allRegisters[0];
        }

        private static byte[] CreateJump(IntPtr pSource, IntPtr pTarget, ud_type register)
        {
            byte[] result;

            if (register == ud_type.UD_NONE)
            {
                result = new byte[5];

                var addressDif = (uint)(pTarget.ToInt64() - (pSource.ToInt64() + 5));

                result[0] = 0xE9;
                result[1] = (byte)(addressDif >> 24);
                result[2] = (byte)(addressDif >> 16);
                result[3] = (byte)(addressDif >> 8);
                result[4] = (byte)(addressDif);
            }
            else
            {
                byte jumpReg = 0xE0;
                bool hasPrefix = false;

                switch (register)
                {
                    case ud_type.UD_R_R8:
                    case ud_type.UD_R_R9:
                    case ud_type.UD_R_R10:
                    case ud_type.UD_R_R11:
                    case ud_type.UD_R_R12:
                    case ud_type.UD_R_R13:
                    case ud_type.UD_R_R14:
                    case ud_type.UD_R_R15:
                        result = new byte[13];
                        break;
                    default:
                        result = new byte[12];
                        break;
                }

                switch (register)
                {
                    case ud_type.UD_R_RAX:
                        result[0] = 0x48;
                        result[1] = 0xB8;

                        jumpReg = 0xE0;
                        break;
                    case ud_type.UD_R_RCX:
                        result[0] = 0x48;
                        result[1] = 0xB9;

                        jumpReg = 0xE1;
                        break;
                    case ud_type.UD_R_RDX:
                        result[0] = 0x48;
                        result[1] = 0xBA;

                        jumpReg = 0xE2;
                        break;
                    case ud_type.UD_R_RBX:
                        result[0] = 0x48;
                        result[1] = 0xBB;

                        jumpReg = 0xE3;
                        break;
                    case ud_type.UD_R_RSP:
                        result[0] = 0x48;
                        result[1] = 0xBC;

                        jumpReg = 0xE4;
                        break;
                    case ud_type.UD_R_RBP:
                        result[0] = 0x48;
                        result[1] = 0xBD;

                        jumpReg = 0xE5;
                        break;
                    case ud_type.UD_R_RSI:
                        result[0] = 0x48;
                        result[1] = 0xBE;

                        jumpReg = 0xE6;
                        break;
                    case ud_type.UD_R_RDI:
                        result[0] = 0x48;
                        result[1] = 0xBF;

                        jumpReg = 0xE7;
                        break;
                    case ud_type.UD_R_R8:
                        result[0] = 0x49;
                        result[1] = 0xB8;

                        jumpReg = 0xE0;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R9:
                        result[0] = 0x49;
                        result[1] = 0xB9;

                        jumpReg = 0xE1;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R10:
                        result[0] = 0x49;
                        result[1] = 0xBA;

                        jumpReg = 0xE2;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R11:
                        result[0] = 0x49;
                        result[1] = 0xBB;

                        jumpReg = 0xE3;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R12:
                        result[0] = 0x49;
                        result[1] = 0xBC;

                        jumpReg = 0xE4;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R13:
                        result[0] = 0x49;
                        result[1] = 0xBD;

                        jumpReg = 0xE5;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R14:
                        result[0] = 0x49;
                        result[1] = 0xBE;

                        jumpReg = 0xE6;
                        hasPrefix = true;
                        break;
                    case ud_type.UD_R_R15:
                        result[0] = 0x49;
                        result[1] = 0xBF;

                        jumpReg = 0xE7;
                        hasPrefix = true;
                        break;
                }

                var targetAddr = pTarget.ToInt64();

                result[sizeof(byte) * 2] = (byte)(targetAddr >> 56);
                result[sizeof(byte) * 2 + 1] = (byte)(targetAddr >> 48);
                result[sizeof(byte) * 2 + 2] = (byte)(targetAddr >> 40);
                result[sizeof(byte) * 2 + 3] = (byte)(targetAddr >> 32);
                result[sizeof(byte) * 2 + 4] = (byte)(targetAddr >> 24);
                result[sizeof(byte) * 2 + 5] = (byte)(targetAddr >> 16);
                result[sizeof(byte) * 2 + 6] = (byte)(targetAddr >> 8);
                result[sizeof(byte) * 2 + 7] = (byte)(targetAddr);

                int offset = 2 + sizeof(long);

                if (hasPrefix)
                    result[offset++] = 0x41;

                result[offset++] = 0xFF;
                result[offset++] = jumpReg;
            }

            return result;
        }

        private static void HookMemoryExecuteOnly(IntPtr pAddr, int size)
        {
	        if (!Kernel32.VirtualProtect(pAddr, new UIntPtr((uint)size), Kernel32.PAGE_EXECUTE, out uint oldProtect))
                throw new Exception("Failed to protect memory");
        }

        private static void HookMemoryExecuteReadWrite(IntPtr pAddr, int size)
        {
            if (!Kernel32.VirtualProtect(pAddr, new UIntPtr((uint)size), Kernel32.PAGE_EXECUTE_READWRITE, out uint oldProtect))
                throw new Exception("Failed to protect memory");
        }
    }
}
