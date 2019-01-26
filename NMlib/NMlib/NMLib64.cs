using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


/* SADECE 64 BIT İŞLETİM SİSTEMİNİ DESTEKLER!
 * 
 * 
 * Bu kütüphaneyi kullandığınız için teşekkürler. :)
 * 
 * https://www.turkhackteam.org/members/832873.html
 * - ilyn
 */

namespace NMlib64
{
    public static class NeutronMemoryLibrary
    {
        public static bool ProcessRunning = false;
        #region DllImports
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        static extern bool WriteMemory64(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int IpNumberOfBytesWritten);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "ReadProcessMemory")]
        static extern bool ReadMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        #endregion
        #region İçAyarlar
        static private IntPtr Handle = IntPtr.Zero;
        static long Base = 0;
        static bool hasBase = false;

        private static byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        private static byte[] ToOpcode(string hex)
        {
            return ToByteArray(hex.Replace(" ", ""));
        }

        private static void pc(string processname)
        {
            Process[] procs = Process.GetProcessesByName(processname);
            if (processname != null)
            {
                if (procs.Length == 0)
                {
                    hasBase = false;
                    ProcessRunning = false;
                    Handle = IntPtr.Zero;
                }
                else
                {
                    foreach (Process proc in procs)
                    {
                        int num = 256;
                        StringBuilder builder = new StringBuilder(num);

                        ProcessRunning = true;
                        Handle = OpenProcess(0x1F0FFF, false, proc.Id);
                        if (!hasBase)
                        {
                            hasBase = true;
                            Base = proc.MainModule.BaseAddress.ToInt64();
                        }
                    }
                }
            }
        }
        private static void WM(long opcode, byte[] bytes)
        {
            if (ProcessRunning && Handle != IntPtr.Zero)
            {
                int written = 0;
                WriteMemory64((int)Handle, (Base + opcode), bytes, bytes.Length, ref written);
            }
        }

        private static long ReadInt64(IntPtr process, IntPtr baseAddress)
        {
            var buffer = new byte[16];
            IntPtr bytesRead;
            ReadMemory(process, baseAddress, buffer, 8, out bytesRead);
            return BitConverter.ToInt64(buffer, 0);
        }

        private static long GetRealAddress(IntPtr process, IntPtr baseAddress, int[] offsets)
        {
            var address = baseAddress.ToInt64();
            foreach (var offset in offsets)
            {
                address = ReadInt64(process, (IntPtr)address) + offset;
            }
            return address;
        }

        private static long GetPointer(Int64 base_, int[] offsets)
        {
            if (Handle != IntPtr.Zero && ProcessRunning)
            {
                long realAddress = GetRealAddress(Handle, (IntPtr)(Base + base_), offsets);
                return realAddress;
            }
            else
            {
                return 0xFFFFFFFFFFF;
            }
        }
        #endregion


        /// <summary>
        /// Hedef işlemi belirlemek için kullanılır. Bir timer'ın içine yerleştiriniz.
        /// </summary>
        /// <param name="processname">Hedef işlem ismi. Ör: notepad, iebrowser vb...</param>
        public static void IslemEkle(string processname)
        {
            pc(processname);
        }
        /// <summary>
        /// Seçtiğiniz opcode'u işleme yazdırmanızı sağlar.
        /// </summary>
        /// <param name="opcode">Örnek: notepad.exe+0F570C, notepad.exe+'dan sonrakini yazınız.</param>
        /// <param name="bytes">Bulduğunuz opcode'un değiştirmek istediğiniz bit değerleri. Örnek: 74 0F 80</param>
        public static void OpcodeKullan(long opcode, string bytes)
        {
            WM(opcode, ToOpcode(bytes));
        }

        /// <summary>
        /// Kalıcı pointeri kullanmanızı sağlar.
        /// </summary>
        /// <param name="offset1">Adresin ilk offseti.</param>
        /// <param name="offset2">Adresin 2. offseti.</param>
        /// <param name="offset3">Adresin 3. offseti.</param>
        /// <param name="value">Pointerdaki değiştirmek istediğiniz değer.</param>
        public static void PointerKullan(long address, int offset1, int offset2, int offset3, int value)
        {
            long Pointer = GetPointer(address, new int[] { offset1, offset2, offset3 });
            byte[] val = new byte[sizeof(int)];
            val = BitConverter.GetBytes(value);
            int written = 0;
            WriteMemory64((int)Handle, Pointer, val, val.Length, ref written);
        }
    }
}
