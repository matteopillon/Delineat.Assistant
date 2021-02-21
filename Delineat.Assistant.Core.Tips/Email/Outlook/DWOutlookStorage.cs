/*
 * OutlookStorage - Reads outlook msg file without Outlook object model - http://www.iwantedue.com
 * Copyright (C) 2008 David Ewen
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of following license:
 *
 *  - The Code Project Open License 1.02 or later (the "CPOL")
 *    http://www.codeproject.com/info/cpol10.aspx
 *
 * == END LICENSE ==
 *
 * This file defines the OutlookStorage class used to read an outlook msg file.
 */

using Delineat.Assistant.Core.Tips.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Delineat.Assistant.Core.Tips.Email.Outlook
{
    public class DWOutlookStorage : IDisposable
    {
        #region CLZF (This Region Has A Seperate Licence)

        /*
         * Copyright (c) 2005 Oren J. Maurice <oymaurice@hazorea.org.il>
         * 
         * Redistribution and use in source and binary forms, with or without modifica-
         * tion, are permitted provided that the following conditions are met:
         * 
         *   1.  Redistributions of source code must retain the above copyright notice,
         *       this list of conditions and the following disclaimer.
         * 
         *   2.  Redistributions in binary form must reproduce the above copyright
         *       notice, this list of conditions and the following disclaimer in the
         *       documentation and/or other materials provided with the distribution.
         * 
         *   3.  The name of the author may not be used to endorse or promote products
         *       derived from this software without specific prior written permission.
         * 
         * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
         * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MER-
         * CHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO
         * EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPE-
         * CIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
         * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
         * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
         * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTH-
         * ERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
         * OF THE POSSIBILITY OF SUCH DAMAGE.
         *
         * Alternatively, the contents of this file may be used under the terms of
         * the GNU General Public License version 2 (the "GPL"), in which case the
         * provisions of the GPL are applicable instead of the above. If you wish to
         * allow the use of your version of this file only under the terms of the
         * GPL and not to allow others to use your version of this file under the
         * BSD license, indicate your decision by deleting the provisions above and
         * replace them with the notice and other provisions required by the GPL. If
         * you do not delete the provisions above, a recipient may use your version
         * of this file under either the BSD or the GPL.
         */

        /// <summary>
        /// Summary description for CLZF.
        /// </summary>
        public class CLZF
        {
            /*
             This program is free software; you can redistribute it and/or modify
             it under the terms of the GNU General Public License as published by
             the Free Software Foundation; either version 2 of the License, or
             (at your option) any later version.

             You should have received a copy of the GNU General Public License
             along with this program; if not, write to the Free Software Foundation,
             Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA
            */

            /*
             * Prebuffered bytes used in RTF-compressed format (found them in RTFLIB32.LIB)
            */
            static byte[] COMPRESSED_RTF_PREBUF;
            static string prebuf = "{\\rtf1\\ansi\\mac\\deff0\\deftab720{\\fonttbl;}" +
                "{\\f0\\fnil \\froman \\fswiss \\fmodern \\fscript " +
                "\\fdecor MS Sans SerifSymbolArialTimes New RomanCourier" +
                "{\\colortbl\\red0\\green0\\blue0\n\r\\par " +
                "\\pard\\plain\\f0\\fs20\\b\\i\\u\\tab\\tx";

            /* The lookup table used in the CRC32 calculation */
            static uint[] CRC32_TABLE =
            {
                0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
                0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
                0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
                0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
                0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
                0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
                0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
                0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
                0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
                0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
                0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
                0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
                0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
                0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
                0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
                0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
                0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
                0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
                0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
                0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
                0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
                0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
                0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
                0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
                0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
                0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
                0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
                0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
                0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
                0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
                0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
                0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
                0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
                0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
                0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
                0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
                0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
                0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
                0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
                0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
                0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
                0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
                0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
                0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
                0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
                0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
                0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
                0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
                0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
                0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
                0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
                0x2D02EF8D
            };

            /*
             * Calculates the CRC32 of the given bytes.
             * The CRC32 calculation is similar to the standard one as demonstrated
             * in RFC 1952, but with the inversion (before and after the calculation)
             * ommited.
             * 
             * @param buf the byte array to calculate CRC32 on
             * @param off the offset within buf at which the CRC32 calculation will start
             * @param len the number of bytes on which to calculate the CRC32
             * @return the CRC32 value.
             */
            static public int calculateCRC32(byte[] buf, int off, int len)
            {
                uint c = 0;
                int end = off + len;
                for (int i = off; i < end; i++)
                {
                    //!!!!        c = CRC32_TABLE[(c ^ buf[i]) & 0xFF] ^ (c >>> 8);
                    c = CRC32_TABLE[(c ^ buf[i]) & 0xFF] ^ (c >> 8);
                }
                return (int)c;
            }

            /*
                 * Returns an unsigned 32-bit value from little-endian ordered bytes.
                 *
                 * @param   buf a byte array from which byte values are taken
                 * @param   offset the offset within buf from which byte values are taken
                 * @return  an unsigned 32-bit value as a long.
            */
            public static long getU32(byte[] buf, int offset)
            {
                return ((buf[offset] & 0xFF) | ((buf[offset + 1] & 0xFF) << 8) | ((buf[offset + 2] & 0xFF) << 16) | ((buf[offset + 3] & 0xFF) << 24)) & 0x00000000FFFFFFFFL;
            }

            /*
             * Returns an unsigned 8-bit value from a byte array.
             *
             * @param   buf a byte array from which byte value is taken
             * @param   offset the offset within buf from which byte value is taken
             * @return  an unsigned 8-bit value as an int.
             */
            public static int getU8(byte[] buf, int offset)
            {
                return buf[offset] & 0xFF;
            }

            /*
              * Decompresses compressed-RTF data.
              *
              * @param   src the compressed-RTF data bytes
              * @return  an array containing the decompressed bytes.
              * @throws  IllegalArgumentException if src does not contain valid                                                                                                                                            *          compressed-RTF bytes.
            */
            public static byte[] decompressRTF(byte[] src)
            {
                byte[] dst; // destination for uncompressed bytes
                int inPos = 0; // current position in src array
                int outPos = 0; // current position in dst array

                COMPRESSED_RTF_PREBUF = System.Text.Encoding.UTF8.GetBytes(prebuf);

                // get header fields (as defined in RTFLIB.H)
                if (src == null || src.Length < 16)
                    throw new Exception("Invalid compressed-RTF header");

                int compressedSize = (int)getU32(src, inPos);
                inPos += 4;
                int uncompressedSize = (int)getU32(src, inPos);
                inPos += 4;
                int magic = (int)getU32(src, inPos);
                inPos += 4;
                int crc32 = (int)getU32(src, inPos);
                inPos += 4;

                if (compressedSize != src.Length - 4) // check size excluding the size field itself
                    throw new Exception("compressed-RTF data size mismatch");

                if (crc32 != calculateCRC32(src, 16, src.Length - 16))
                    throw new Exception("compressed-RTF CRC32 failed");

                // process the data
                if (magic == 0x414c454d)
                { // magic number that identifies the stream as a uncompressed stream
                    dst = new byte[uncompressedSize];
                    Array.Copy(src, inPos, dst, outPos, uncompressedSize); // just copy it as it is
                }
                else if (magic == 0x75465a4c)
                { // magic number that identifies the stream as a compressed stream
                    dst = new byte[COMPRESSED_RTF_PREBUF.Length + uncompressedSize];
                    Array.Copy(COMPRESSED_RTF_PREBUF, 0, dst, 0, COMPRESSED_RTF_PREBUF.Length);
                    outPos = COMPRESSED_RTF_PREBUF.Length;
                    int flagCount = 0;
                    int flags = 0;
                    while (outPos < dst.Length)
                    {
                        // each flag byte flags 8 literals/references, 1 per bit
                        flags = (flagCount++ % 8 == 0) ? getU8(src, inPos++) : flags >> 1;
                        if ((flags & 1) == 1)
                        { // each flag bit is 1 for reference, 0 for literal
                            int offset = getU8(src, inPos++);
                            int length = getU8(src, inPos++);
                            //!!!!!!!!!            offset = (offset << 4) | (length >>> 4); // the offset relative to block start
                            offset = (offset << 4) | (length >> 4); // the offset relative to block start
                            length = (length & 0xF) + 2; // the number of bytes to copy
                            // the decompression buffer is supposed to wrap around back
                            // to the beginning when the end is reached. we save the
                            // need for such a buffer by pointing straight into the data
                            // buffer, and simulating this behaviour by modifying the
                            // pointers appropriately.
                            offset = (outPos / 4096) * 4096 + offset;
                            if (offset >= outPos) // take from previous block
                                offset -= 4096;
                            // note: can't use System.arraycopy, because the referenced
                            // bytes can cross through the current out position.
                            int end = offset + length;
                            while (offset < end)
                                dst[outPos++] = dst[offset++];
                        }
                        else
                        { // literal
                            dst[outPos++] = src[inPos++];
                        }
                    }
                    // copy it back without the prebuffered data
                    src = dst;
                    dst = new byte[uncompressedSize];
                    Array.Copy(src, COMPRESSED_RTF_PREBUF.Length, dst, 0, uncompressedSize);
                }
                else
                { // unknown magic number
                    throw new Exception("Unknown compression type (magic number " + magic + ")");
                }

                return dst;
            }
        }

        #endregion

        #region NativeMethods

        protected class NativeMethods
        {
            [DllImport("kernel32.dll")]
            static extern IntPtr GlobalLock(IntPtr hMem);

            [DllImport("ole32.DLL")]
            public static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out ILockBytes ppLkbyt);

            //[DllImport("ole32.DLL", CharSet = CharSet.Auto, PreserveSig = false)]
            //public static extern IntPtr GetHGlobalFromILockBytes(ILockBytes pLockBytes);

            [DllImport("ole32.DLL")]
            public static extern int StgIsStorageILockBytes(ILockBytes plkbyt);

            [DllImport("ole32.DLL")]
            public static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, STGM grfMode, uint reserved, out IStorage ppstgOpen);

            [DllImport("ole32.DLL")]
            public static extern void StgOpenStorageOnILockBytes(ILockBytes plkbyt, IStorage pstgPriority, STGM grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);

            [DllImport("ole32.DLL")]
            public static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)] string wcsName);

            [DllImport("ole32.DLL")]
            public static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IStorage pstgPriority, STGM grfMode, IntPtr snbExclude, int reserved, out IStorage ppstgOpen);

            [ComImport, Guid("0000000A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface ILockBytes
            {
                void ReadAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcbRead);
                void WriteAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcbWritten);
                void Flush();
                void SetSize([In, MarshalAs(UnmanagedType.U8)] long cb);
                void LockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);
                void UnlockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);
                void Stat([Out] out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In, MarshalAs(UnmanagedType.U4)] int grfStatFlag);
            }

            [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
            public interface IStorage
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                ComTypes.IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);
                [return: MarshalAs(UnmanagedType.Interface)]
                ComTypes.IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);
                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);
                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);
                void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);
                void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);
                void Commit(int grfCommitFlags);
                void Revert();
                void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out IEnumSTATSTG ppVal);
                void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);
                void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);
                void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
                void SetClass([In] ref Guid clsid);
                void SetStateBits(int grfStateBits, int grfMask);
                void Stat([Out] out System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
            }

            [ComImport, Guid("0000000D-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IEnumSTATSTG
            {
                void Next(uint celt, [MarshalAs(UnmanagedType.LPArray), Out] System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt, out uint pceltFetched);
                void Skip(uint celt);
                void Reset();
                [return: MarshalAs(UnmanagedType.Interface)]
                IEnumSTATSTG Clone();
            }

            public enum STGM : int
            {
                DIRECT = 0x00000000,
                TRANSACTED = 0x00010000,
                SIMPLE = 0x08000000,
                READ = 0x00000000,
                WRITE = 0x00000001,
                READWRITE = 0x00000002,
                SHARE_DENY_NONE = 0x00000040,
                SHARE_DENY_READ = 0x00000030,
                SHARE_DENY_WRITE = 0x00000020,
                SHARE_EXCLUSIVE = 0x00000010,
                PRIORITY = 0x00040000,
                DELETEONRELEASE = 0x04000000,
                NOSCRATCH = 0x00100000,
                CREATE = 0x00001000,
                CONVERT = 0x00020000,
                FAILIFTHERE = 0x00000000,
                NOSNAPSHOT = 0x00200000,
                DIRECT_SWMR = 0x00400000
            }

            public const ushort PT_UNSPECIFIED = 0; /* (Reserved for interface use) type doesn't matter to caller */
            public const ushort PT_NULL = 1;        /* NULL property value */
            public const ushort PT_I2 = 2;          /* Signed 16-bit value */
            public const ushort PT_LONG = 3;        /* Signed 32-bit value */
            public const ushort PT_R4 = 4;          /* 4-byte floating point */
            public const ushort PT_DOUBLE = 5;      /* Floating point double */
            public const ushort PT_CURRENCY = 6;    /* Signed 64-bit int (decimal w/    4 digits right of decimal pt) */
            public const ushort PT_APPTIME = 7;     /* Application time */
            public const ushort PT_ERROR = 10;      /* 32-bit error value */
            public const ushort PT_BOOLEAN = 11;    /* 16-bit boolean (non-zero true) */
            public const ushort PT_OBJECT = 13;     /* Embedded object in a property */
            public const ushort PT_I8 = 20;         /* 8-byte signed integer */
            public const ushort PT_STRING8 = 30;    /* Null terminated 8-bit character string */
            public const ushort PT_UNICODE = 31;    /* Null terminated Unicode string */
            public const ushort PT_SYSTIME = 64;    /* FILETIME 64-bit int w/ number of 100ns periods since Jan 1,1601 */
            public const ushort PT_CLSID = 72;      /* OLE GUID */
            public const ushort PT_BINARY = 258;    /* Uninterpreted (counted byte array) */

            public static IStorage CloneStorage(IStorage source, bool closeSource)
            {
                NativeMethods.IStorage memoryStorage = null;
                NativeMethods.ILockBytes memoryStorageBytes = null;
                try
                {
                    //create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
                    NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    NativeMethods.StgCreateDocfileOnILockBytes(memoryStorageBytes, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    //copy the source storage into the new storage
                    source.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    //ensure memory is released
                    ReferenceManager.AddItem(memoryStorage);
                }
                catch
                {
                    if (memoryStorage != null)
                    {
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(memoryStorage);
                    }
                }
                finally
                {
                    if (memoryStorageBytes != null)
                    {
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(memoryStorageBytes);
                    }

                    if (closeSource)
                    {
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(source);
                    }
                }

                return memoryStorage;
            }
        }

        #endregion

        #region ReferenceManager

        private class ReferenceManager
        {
            public static void AddItem(object track)
            {
                lock (instance)
                {
                    if (!instance.trackingObjects.Contains(track))
                    {
                        instance.trackingObjects.Add(track);
                    }
                }
            }

            public static void RemoveItem(object track)
            {
                lock (instance)
                {
                    if (instance.trackingObjects.Contains(track))
                    {
                        instance.trackingObjects.Remove(track);
                    }
                }
            }

            private static ReferenceManager instance = new ReferenceManager();

            private List<object> trackingObjects = new List<object>();

            private ReferenceManager() { }

            ~ReferenceManager()
            {
                foreach (object trackingObject in trackingObjects)
                {
                    if (OperatingSystem.IsWindows())
                        Marshal.ReleaseComObject(trackingObject);
                }
            }
        }

        #endregion

        #region Nested Classes

        public enum RecipientType
        {
            To,
            CC,
            Unknown
        }

        public class Recipient : DWOutlookStorage
        {
            #region Property(s)

            /// <summary>
            /// Gets the display name.
            /// </summary>
            /// <value>The display name.</value>
            public string DisplayName
            {
                get { return this.GetMapiPropertyString(DWOutlookStorage.PR_DISPLAY_NAME); }
            }

            /// <summary>
            /// Gets the recipient email.
            /// </summary>
            /// <value>The recipient email.</value>
            public string Email
            {
                get
                {
                    string email = this.GetMapiPropertyString(DWOutlookStorage.PR_EMAIL);
                    if (String.IsNullOrEmpty(email))
                    {
                        email = this.GetMapiPropertyString(DWOutlookStorage.PR_EMAIL_2);
                    }
                    return email;
                }
            }

            /// <summary>
            /// Gets the recipient type.
            /// </summary>
            /// <value>The recipient type.</value>
            public RecipientType Type
            {
                get
                {
                    int recipientType = this.GetMapiPropertyInt32(DWOutlookStorage.PR_RECIPIENT_TYPE);
                    switch (recipientType)
                    {
                        case DWOutlookStorage.MAPI_TO:
                            return RecipientType.To;

                        case DWOutlookStorage.MAPI_CC:
                            return RecipientType.CC;
                    }
                    return RecipientType.Unknown;
                }
            }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="Recipient"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public Recipient(DWOutlookStorage message)
                : base(message.storage)
            {
                GC.SuppressFinalize(message);
                this.propHeaderSize = DWOutlookStorage.PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP;
            }

            #endregion
        }

        public class Attachment : DWOutlookStorage
        {
            #region Property(s)

            /// <summary>
            /// Gets the filename.
            /// </summary>
            /// <value>The filename.</value>
            public string Filename
            {
                get
                {
                    string filename = this.GetMapiPropertyString(DWOutlookStorage.PR_ATTACH_LONG_FILENAME);
                    if (String.IsNullOrEmpty(filename))
                    {
                        filename = this.GetMapiPropertyString(DWOutlookStorage.PR_ATTACH_FILENAME);
                    }
                    if (String.IsNullOrEmpty(filename))
                    {
                        filename = this.GetMapiPropertyString(DWOutlookStorage.PR_DISPLAY_NAME);
                    }
                    return filename;
                }
            }

            /// <summary>
            /// Gets the data.
            /// </summary>
            /// <value>The data.</value>
            public byte[] Data
            {
                get { return this.GetMapiPropertyBytes(DWOutlookStorage.PR_ATTACH_DATA); }
            }

            /// <summary>
            /// Gets the content id.
            /// </summary>
            /// <value>The content id.</value>
            public string ContentId
            {
                get { return this.GetMapiPropertyString(DWOutlookStorage.PR_ATTACH_CONTENT_ID); }
            }

            /// <summary>
            /// Gets the rendering posisiton.
            /// </summary>
            /// <value>The rendering posisiton.</value>
            public int RenderingPosisiton
            {
                get { return this.GetMapiPropertyInt32(DWOutlookStorage.PR_RENDERING_POSITION); }
            }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="Attachment"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public Attachment(DWOutlookStorage message)
                : base(message.storage)
            {
                GC.SuppressFinalize(message);
                this.propHeaderSize = DWOutlookStorage.PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP;
            }

            #endregion
        }

        public class Message : DWOutlookStorage
        {
            #region Property(s)

            /// <summary>
            /// Gets the list of recipients in the outlook message.
            /// </summary>
            /// <value>The list of recipients in the outlook message.</value>
            public List<Recipient> Recipients
            {
                get { return this.recipients; }
            }
            private List<Recipient> recipients = new List<Recipient>();

            /// <summary>
            /// Gets the list of attachments in the outlook message.
            /// </summary>
            /// <value>The list of attachments in the outlook message.</value>
            public List<Attachment> Attachments
            {
                get { return this.attachments; }
            }
            private List<Attachment> attachments = new List<Attachment>();

            /// <summary>
            /// Gets the list of sub messages in the outlook message.
            /// </summary>
            /// <value>The list of sub messages in the outlook message.</value>
            public List<Message> Messages
            {
                get { return this.messages; }
            }
            private List<Message> messages = new List<Message>();


            /// <summary>
            /// Gets the display value of the contact that sent the email.
            /// </summary>
            /// <value>The display value of the contact that sent the email.</value>
            public String From
            {
                get
                {
                    var email = this.GetMapiPropertyString("0C1F");

                    if (!email.IsEmail())
                        email = this.GetMapiPropertyString("5D0A");
                    if (!email.IsEmail())
                        email = this.GetMapiPropertyString("3FFA");
                    return email ?? string.Empty;

                }
            }

            /// <summary>
            /// Gets the deliver time.
            /// </summary>
            /// <value>The display value of the contact that sent the email.</value>
            public DateTime? DeliverDateUtc
            {
                get
                {
                    try
                    {
                        var dateTime = this.GetMapiPropertyInt64(DWOutlookStorage.PR_CLIENT_SUBMIT_TIME);
                        return new DateTime(1601, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(dateTime);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// Gets the subject of the outlook message.
            /// </summary>
            /// <value>The subject of the outlook message.</value>
            public String Subject
            {
                get { return this.GetMapiPropertyString(DWOutlookStorage.PR_SUBJECT); }
            }

            /// <summary>
            /// Gets the body of the outlook message in plain text format.
            /// </summary>
            /// <value>The body of the outlook message in plain text format.</value>
            public String BodyText
            {
                get { return this.GetMapiPropertyString(DWOutlookStorage.PR_BODY); }
            }

            /// <summary>
            /// Gets the body of the outlook message in RTF format.
            /// </summary>
            /// <value>The body of the outlook message in RTF format.</value>
            public String BodyRTF
            {
                get
                {
                    //get value for the RTF compressed MAPI property
                    byte[] rtfBytes = this.GetMapiPropertyBytes(DWOutlookStorage.PR_RTF_COMPRESSED);

                    //return null if no property value exists
                    if (rtfBytes == null || rtfBytes.Length == 0)
                    {
                        return null;
                    }

                    //decompress the rtf value
                    rtfBytes = CLZF.decompressRTF(rtfBytes);

                    //encode the rtf value as an ascii string and return
                    return Encoding.UTF8.GetString(rtfBytes, 0, rtfBytes.Length);
                }
            }

            public void Save(Stream stream)
            {
                // Get statistics for stream 
                var saveMsg = this;

                NativeMethods.IStorage memoryStorage = null;
                NativeMethods.IStorage nameIdSourceStorage = null;
                NativeMethods.ILockBytes memoryStorageBytes = null;

                try
                {
                    // Create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
                    NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    NativeMethods.StgCreateDocfileOnILockBytes(memoryStorageBytes, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    // Copy the save storage into the new storage
                    saveMsg.storage.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    // If not the top parent then the name id mapping needs to be copied from top parent to this message and the property stream header needs to be padded by 8 bytes
                    if (!IsTopParent)
                    {
                        // Create a new name id storage and get the source name id storage to copy from
                        var nameIdStorage = memoryStorage.CreateStorage(MapiTags.NameIdStorage, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        nameIdSourceStorage = TopParent.storage.OpenStorage(MapiTags.NameIdStorage, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE,
                            IntPtr.Zero, 0);

                        // Copy the name id storage from the parent to the new name id storage
                        nameIdSourceStorage.CopyTo(0, null, IntPtr.Zero, nameIdStorage);

                        // Get the property bytes for the storage being copied
                        var props = saveMsg.GetStreamBytes(MapiTags.PropertiesStream);

                        // Create new array to store a copy of the properties that is 8 bytes larger than the old so the header can be padded
                        var newProps = new byte[props.Length + 8];

                        // Insert 8 null bytes from index 24 to 32. this is because a top level object property header requires a 32 byte header
                        Buffer.BlockCopy(props, 0, newProps, 0, 24);
                        Buffer.BlockCopy(props, 24, newProps, 32, props.Length - 24);

                        // Remove the copied prop bytes so it can be replaced with the padded version
                        memoryStorage.DestroyElement(MapiTags.PropertiesStream);

                        // Create the property stream again and write in the padded version
                        var propStream = memoryStorage.CreateStream(MapiTags.PropertiesStream, NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        propStream.Write(newProps, newProps.Length, IntPtr.Zero);
                    }

                    // Commit changes to the storage
                    memoryStorage.Commit(0);
                    memoryStorageBytes.Flush();

                    // Get the STATSTG of the ILockBytes to determine how many bytes were written to it
                    ComTypes.STATSTG memoryStorageBytesStat;
                    memoryStorageBytes.Stat(out memoryStorageBytesStat, 1);

                    // Read the bytes into a managed byte array
                    var memoryStorageContent = new byte[memoryStorageBytesStat.cbSize];
                    memoryStorageBytes.ReadAt(0, memoryStorageContent, memoryStorageContent.Length, null);

                    // Write storage bytes to stream
                    stream.Write(memoryStorageContent, 0, memoryStorageContent.Length);
                }
                finally
                {
                    if (nameIdSourceStorage != null)
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(nameIdSourceStorage);


                    if (memoryStorage != null)
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(memoryStorage);

                    if (memoryStorageBytes != null)
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(memoryStorageBytes);
                }
            }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class from a msg file.
            /// </summary>
            /// <param name="filename">The msg file to load.</param>
            public Message(string msgfile) : base(msgfile) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class from a <see cref="Stream"/> containing an IStorage.
            /// </summary>
            /// <param name="storageStream">The <see cref="Stream"/> containing an IStorage.</param>
            public Message(Stream storageStream) : base(storageStream) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class on the specified <see cref="NativeMethods.IStorage"/>.
            /// </summary>
            /// <param name="storage">The storage to create the <see cref="Message"/> on.</param>
            private Message(NativeMethods.IStorage storage)
                : base(storage)
            {
                this.propHeaderSize = DWOutlookStorage.PROPERTIES_STREAM_HEADER_TOP;
            }

            #endregion

            #region Methods(LoadStorage)

            /// <summary>
            /// Processes sub storages on the specified storage to capture attachment and recipient data.
            /// </summary>
            /// <param name="storage">The storage to check for attachment and recipient data.</param>
            protected override void LoadStorage(NativeMethods.IStorage storage)
            {
                base.LoadStorage(storage);

                foreach (ComTypes.STATSTG storageStat in this.subStorageStatistics.Values)
                {
                    //element is a storage. get it and add its statistics object to the sub storage dictionary
                    NativeMethods.IStorage subStorage = this.storage.OpenStorage(storageStat.pwcsName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0);

                    //run specific load method depending on sub storage name prefix
                    if (storageStat.pwcsName.StartsWith(DWOutlookStorage.RECIP_STORAGE_PREFIX))
                    {
                        Recipient recipient = new Recipient(new DWOutlookStorage(subStorage));
                        this.recipients.Add(recipient);
                    }
                    else if (storageStat.pwcsName.StartsWith(DWOutlookStorage.ATTACH_STORAGE_PREFIX))
                    {
                        this.LoadAttachmentStorage(subStorage);
                    }
                    else
                    {
                        //release sub storage
                        if (OperatingSystem.IsWindows())
                            Marshal.ReleaseComObject(subStorage);
                    }
                }
            }

            /// <summary>
            /// Loads the attachment data out of the specified storage.
            /// </summary>
            /// <param name="storage">The attachment storage.</param>
            private void LoadAttachmentStorage(NativeMethods.IStorage storage)
            {
                //create attachment from attachment storage
                Attachment attachment = new Attachment(new DWOutlookStorage(storage));

                //if attachment is a embeded msg handle differently than an normal attachment
                int attachMethod = attachment.GetMapiPropertyInt32(DWOutlookStorage.PR_ATTACH_METHOD);
                if (attachMethod == DWOutlookStorage.ATTACH_EMBEDDED_MSG)
                {
                    //create new Message and set parent and header size
                    Message subMsg = new Message(attachment.GetMapiProperty(DWOutlookStorage.PR_ATTACH_DATA) as NativeMethods.IStorage);
                    subMsg.parentMessage = this;
                    subMsg.propHeaderSize = DWOutlookStorage.PROPERTIES_STREAM_HEADER_EMBEDED;


                    //add to messages list
                    this.messages.Add(subMsg);
                }
                else
                {
                    //add attachment to attachment list
                    this.attachments.Add(attachment);
                }
            }

            #endregion

            #region Methods(Save)
            /*
            /// <summary>
            /// Saves this <see cref="Message"/> to the specified file name.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>

            public void Save(string fileName)
            {
                FileStream saveFileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
                this.Save(saveFileStream);
                saveFileStream.Close();
            }

            /// <summary>
            /// Saves this <see cref="Message"/> to the specified stream.
            /// </summary>
            /// <param name="stream">The stream to save to.</param>
            public void Save(Stream stream)
            {
                //get statistics for stream 
                OutlookStorage saveMsg = this;

                byte[] memoryStorageContent;
                NativeMethods.IStorage memoryStorage = null;
                NativeMethods.IStorage nameIdStorage = null;
                NativeMethods.IStorage nameIdSourceStorage = null;
                NativeMethods.ILockBytes memoryStorageBytes = null;
                try
                {
                    //create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
                    NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    NativeMethods.StgCreateDocfileOnILockBytes(memoryStorageBytes, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    //copy the save storage into the new storage
                    saveMsg.storage.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    //if not the top parent then the name id mapping needs to be copied from top parent to this message and the property stream header needs to be padded by 8 bytes
                    if (!this.IsTopParent)
                    {
                        //create a new name id storage and get the source name id storage to copy from
                        nameIdStorage = memoryStorage.CreateStorage(OutlookStorage.NAMEID_STORAGE, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        nameIdSourceStorage = this.TopParent.storage.OpenStorage(OutlookStorage.NAMEID_STORAGE, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0);

                        //copy the name id storage from the parent to the new name id storage
                        nameIdSourceStorage.CopyTo(0, null, IntPtr.Zero, nameIdStorage);

                        //get the property bytes for the storage being copied
                        byte[] props = saveMsg.GetStreamBytes(OutlookStorage.PROPERTIES_STREAM);

                        //create new array to store a copy of the properties that is 8 bytes larger than the old so the header can be padded
                        byte[] newProps = new byte[props.Length + 8];

                        //insert 8 null bytes from index 24 to 32. this is because a top level object property header requires a 32 byte header
                        Buffer.BlockCopy(props, 0, newProps, 0, 24);
                        Buffer.BlockCopy(props, 24, newProps, 32, props.Length - 24);

                        //remove the copied prop bytes so it can be replaced with the padded version
                        memoryStorage.DestroyElement(OutlookStorage.PROPERTIES_STREAM);

                        //create the property stream again and write in the padded version
                        ComTypes.IStream propStream = memoryStorage.CreateStream(OutlookStorage.PROPERTIES_STREAM, NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        propStream.Write(newProps, newProps.Length, IntPtr.Zero);
                    }

                    //commit changes to the storage
                    memoryStorage.Commit(0);
                    memoryStorageBytes.Flush();

                    //get the STATSTG of the ILockBytes to determine how many bytes were written to it
                    ComTypes.STATSTG memoryStorageBytesStat;
                    memoryStorageBytes.Stat(out memoryStorageBytesStat, 1);

                    //read the bytes into a managed byte array
                    memoryStorageContent = new byte[memoryStorageBytesStat.cbSize];
                    memoryStorageBytes.ReadAt(0, memoryStorageContent, memoryStorageContent.Length, null);

                    //write storage bytes to stream
                    stream.Write(memoryStorageContent, 0, memoryStorageContent.Length);
                }
                finally
                {
                    if (nameIdSourceStorage != null)
                    {
                        Marshal.ReleaseComObject(nameIdSourceStorage);
                    }

                    if (memoryStorage != null)
                    {
                        Marshal.ReleaseComObject(memoryStorage);
                    }

                    if (memoryStorageBytes != null)
                    {
                        Marshal.ReleaseComObject(memoryStorageBytes);
                    }
                }
            }
            */
            #endregion

            #region Methods(Disposing)

            protected override void Disposing()
            {
                //dispose sub storages
                foreach (DWOutlookStorage subMsg in this.messages)
                {
                    subMsg.Dispose();
                }

                //dispose sub storages
                foreach (DWOutlookStorage recip in this.recipients)
                {
                    recip.Dispose();
                }

                //dispose sub storages
                foreach (DWOutlookStorage attach in this.attachments)
                {
                    attach.Dispose();
                }
            }

            #endregion
        }

        #endregion

        #region Constants

        //attachment constants
        private const string ATTACH_STORAGE_PREFIX = "__attach_version1.0_#";
        private const string PR_ATTACH_FILENAME = "3704";
        private const string PR_ATTACH_LONG_FILENAME = "3707";
        private const string PR_ATTACH_DATA = "3701";
        private const string PR_ATTACH_METHOD = "3705";
        private const string PR_RENDERING_POSITION = "370B";
        private const string PR_ATTACH_CONTENT_ID = "3712";
        private const int ATTACH_BY_VALUE = 1;
        private const int ATTACH_EMBEDDED_MSG = 5;

        //recipient constants
        private const string RECIP_STORAGE_PREFIX = "__recip_version1.0_#";
        private const string PR_DISPLAY_NAME = "3001";
        private const string PR_EMAIL = "39FE";
        private const string PR_EMAIL_2 = "403E"; //not sure why but email address is in this property sometimes cant find any documentation on it
        private const string PR_RECIPIENT_TYPE = "0C15";
        private const int MAPI_TO = 1;
        private const int MAPI_CC = 2;

        //msg constants
        private const string PR_SUBJECT = "0037";
        private const string PR_BODY = "1000";
        private const string PR_RTF_COMPRESSED = "1009";
        private const string PR_SENDER_NAME = "0C1A";
        private const string PR_DELIVER_TIME = "0010";
        private const string PR_CLIENT_SUBMIT_TIME = "0039";
        //property stream constants
        public const string PROPERTIES_STREAM = "__properties_version1.0";
        private const int PROPERTIES_STREAM_HEADER_TOP = 32;
        private const int PROPERTIES_STREAM_HEADER_EMBEDED = 24;
        private const int PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP = 8;

        //name id storage name in root storage
        private const string NAMEID_STORAGE = "__nameid_version1.0";

        #endregion

        #region Property(s)

        /// <summary>
        /// Gets the top level outlook message from a sub message at any level.
        /// </summary>
        /// <value>The top level outlook message.</value>
        private DWOutlookStorage TopParent
        {
            get
            {
                if (this.parentMessage != null)
                {
                    return this.parentMessage.TopParent;
                }
                return this;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the top level outlook message.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is the top level outlook message; otherwise, <c>false</c>.
        /// </value>
        private bool IsTopParent
        {
            get
            {
                if (this.parentMessage != null)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// The IStorage associated with this instance.
        /// </summary>
        private NativeMethods.IStorage storage;

        /// <summary>
        /// Header size of the property stream in the IStorage associated with this instance.
        /// </summary>
        private int propHeaderSize = DWOutlookStorage.PROPERTIES_STREAM_HEADER_TOP;

        /// <summary>
        /// A reference to the parent message that this message may belong to.
        /// </summary>
        private DWOutlookStorage parentMessage = null;

        /// <summary>
        /// The statistics for all streams in the IStorage associated with this instance.
        /// </summary>
        public Dictionary<string, ComTypes.STATSTG> streamStatistics = new Dictionary<string, ComTypes.STATSTG>();

        /// <summary>
        /// The statistics for all storgages in the IStorage associated with this instance.
        /// </summary>
        public Dictionary<string, ComTypes.STATSTG> subStorageStatistics = new Dictionary<string, ComTypes.STATSTG>();

        /// <summary>
        /// Indicates wether this instance has been disposed.
        /// </summary>
        private bool disposed = false;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="DWOutlookStorage"/> class from a file.
        /// </summary>
        /// <param name="storageFilePath">The file to load.</param>
        private DWOutlookStorage(string storageFilePath)
        {
            //ensure provided file is an IStorage
            if (NativeMethods.StgIsStorageFile(storageFilePath) != 0)
            {
                throw new ArgumentException("The provided file is not a valid IStorage", "storageFilePath");
            }

            //open and load IStorage from file
            NativeMethods.IStorage fileStorage;
            NativeMethods.StgOpenStorage(storageFilePath, null, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out fileStorage);
            this.LoadStorage(fileStorage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DWOutlookStorage"/> class from a <see cref="Stream"/> containing an IStorage.
        /// </summary>
        /// <param name="storageStream">The <see cref="Stream"/> containing an IStorage.</param>
        private DWOutlookStorage(Stream storageStream)
        {
            NativeMethods.IStorage memoryStorage = null;
            NativeMethods.ILockBytes memoryStorageBytes = null;
            try
            {
                //read stream into buffer
                byte[] buffer = new byte[storageStream.Length];
                storageStream.Read(buffer, 0, buffer.Length);

                //create a ILockBytes (unmanaged byte array) and write buffer into it
                NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                memoryStorageBytes.WriteAt(0, buffer, buffer.Length, null);

                //ensure provided stream data is an IStorage
                if (NativeMethods.StgIsStorageILockBytes(memoryStorageBytes) != 0)
                {
                    throw new ArgumentException("The provided stream is not a valid IStorage", "storageStream");
                }

                //open and load IStorage on the ILockBytes
                NativeMethods.StgOpenStorageOnILockBytes(memoryStorageBytes, null, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out memoryStorage);
                this.LoadStorage(memoryStorage);
            }
            catch
            {
                if (memoryStorage != null)
                {
                    if (OperatingSystem.IsWindows())
                        Marshal.ReleaseComObject(memoryStorage);
                }
            }
            finally
            {
                if (memoryStorageBytes != null)
                {
                    if (OperatingSystem.IsWindows())
                        Marshal.ReleaseComObject(memoryStorageBytes);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DWOutlookStorage"/> class on the specified <see cref="NativeMethods.IStorage"/>.
        /// </summary>
        /// <param name="storage">The storage to create the <see cref="DWOutlookStorage"/> on.</param>
        private DWOutlookStorage(NativeMethods.IStorage storage)
        {
            this.LoadStorage(storage);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DWOutlookStorage"/> is reclaimed by garbage collection.
        /// </summary>
        ~DWOutlookStorage()
        {
            this.Dispose();
        }

        #endregion

        #region Methods(LoadStorage)

        /// <summary>
        /// Processes sub streams and storages on the specified storage.
        /// </summary>
        /// <param name="storage">The storage to get sub streams and storages for.</param>
        protected virtual void LoadStorage(NativeMethods.IStorage storage)
        {
            this.storage = storage;

            //ensures memory is released
            ReferenceManager.AddItem(this.storage);

            NativeMethods.IEnumSTATSTG storageElementEnum = null;
            try
            {
                //enum all elements of the storage
                storage.EnumElements(0, IntPtr.Zero, 0, out storageElementEnum);

                //iterate elements
                while (true)
                {
                    //get 1 element out of the com enumerator
                    uint elementStatCount;
                    ComTypes.STATSTG[] elementStats = new ComTypes.STATSTG[1];
                    storageElementEnum.Next(1, elementStats, out elementStatCount);

                    //break loop if element not retrieved
                    if (elementStatCount != 1)
                    {
                        break;
                    }

                    ComTypes.STATSTG elementStat = elementStats[0];
                    switch (elementStat.type)
                    {
                        case 1:
                            //element is a storage. add its statistics object to the storage dictionary
                            subStorageStatistics.Add(elementStat.pwcsName, elementStat);
                            break;

                        case 2:
                            //element is a stream. add its statistics object to the stream dictionary
                            streamStatistics.Add(elementStat.pwcsName, elementStat);
                            break;
                        default:
                            break;
                    }
                }
            }
            finally
            {
                //free memory
                if (storageElementEnum != null)
                {
                    if (OperatingSystem.IsWindows())
                        Marshal.ReleaseComObject(storageElementEnum);
                }
            }
        }

        #endregion

        #region Methods(GetStreamBytes, GetStreamAsString)

        /// <summary>
        /// Gets the data in the specified stream as a byte array.
        /// </summary>
        /// <param name="streamName">Name of the stream to get data for.</param>
        /// <returns>A byte array containg the stream data.</returns>
        public byte[] GetStreamBytes(string streamName)
        {
            //get statistics for stream 
            ComTypes.STATSTG streamStatStg = this.streamStatistics[streamName];

            byte[] iStreamContent;
            ComTypes.IStream stream = null;
            try
            {
                //open stream from the storage
                stream = this.storage.OpenStream(streamStatStg.pwcsName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, 0);

                //read the stream into a managed byte array
                iStreamContent = new byte[streamStatStg.cbSize];
                stream.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);
            }
            finally
            {
                if (stream != null)
                {
                    if (OperatingSystem.IsWindows())
                        Marshal.ReleaseComObject(stream);
                }
            }

            //return the stream bytes
            return iStreamContent;
        }

        /// <summary>
        /// Gets the data in the specified stream as a string using the specifed encoding to decode the stream data.
        /// </summary>
        /// <param name="streamName">Name of the stream to get string data for.</param>
        /// <param name="streamEncoding">The encoding to decode the stream data with.</param>
        /// <returns>The data in the specified stream as a string.</returns>
        public string GetStreamAsString(string streamName, Encoding streamEncoding)
        {
            string streamContent = string.Empty;
            using (StreamReader streamReader = new StreamReader(new MemoryStream(this.GetStreamBytes(streamName)), streamEncoding))
            {
                streamContent = streamReader.ReadToEnd();
            }
            return streamContent;
        }

        #endregion

        #region Methods(GetMapiProperty)

        /// <summary>
        /// Gets the raw value of the MAPI property.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The raw value of the MAPI property.</returns>
        public object GetMapiProperty(string propIdentifier)
        {
            //try get prop value from stream or storage
            object propValue = this.GetMapiPropertyFromStreamOrStorage(propIdentifier);

            //if not found in stream or storage try get prop value from property stream
            if (propValue == null)
            {
                propValue = this.GetMapiPropertyFromPropertyStream(propIdentifier);
            }

            return propValue;
        }

        /// <summary>
        /// Gets the MAPI property value from a stream or storage in this storage.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property or null if not found.</returns>
        private object GetMapiPropertyFromStreamOrStorage(string propIdentifier)
        {
            //get list of stream and storage identifiers which map to properties
            List<string> propKeys = new List<string>();
            propKeys.AddRange(this.streamStatistics.Keys);
            propKeys.AddRange(this.subStorageStatistics.Keys);

            //determine if the property identifier is in a stream or sub storage
            string propTag = null;
            ushort propType = NativeMethods.PT_UNSPECIFIED;
            foreach (string propKey in propKeys)
            {
                if (propKey.StartsWith("__substg1.0_" + propIdentifier))
                {
                    propTag = propKey.Substring(12, 8);
                    propType = ushort.Parse(propKey.Substring(16, 4), System.Globalization.NumberStyles.HexNumber);
                    break;
                }
            }

            //depending on prop type use method to get property value
            string containerName = "__substg1.0_" + propTag;
            switch (propType)
            {
                case NativeMethods.PT_UNSPECIFIED:
                    return null;

                case NativeMethods.PT_STRING8:
                    return this.GetStreamAsString(containerName, Encoding.UTF8);

                case NativeMethods.PT_UNICODE:
                    return this.GetStreamAsString(containerName, Encoding.Unicode);

                case NativeMethods.PT_BINARY:
                    return this.GetStreamBytes(containerName);

                case NativeMethods.PT_OBJECT:
                    return NativeMethods.CloneStorage(this.storage.OpenStorage(containerName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0), true);

                default:
                    throw new Exception("MAPI property has an unsupported type and can not be retrieved.");
            }
        }

        /// <summary>
        /// Gets the MAPI property value from the property stream in this storage.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property or null if not found.</returns>
        private object GetMapiPropertyFromPropertyStream(string propIdentifier)
        {
            //if no property stream return null
            if (!this.streamStatistics.ContainsKey(DWOutlookStorage.PROPERTIES_STREAM))
            {
                return null;
            }

            //get the raw bytes for the property stream
            byte[] propBytes = this.GetStreamBytes(DWOutlookStorage.PROPERTIES_STREAM);

            //iterate over property stream in 16 byte chunks starting from end of header
            for (int i = this.propHeaderSize; i < propBytes.Length; i = i + 16)
            {
                //get property type located in the 1st and 2nd bytes as a unsigned short value
                ushort propType = BitConverter.ToUInt16(propBytes, i);

                //get property identifer located in 3nd and 4th bytes as a hexdecimal string
                byte[] propIdent = new byte[] { propBytes[i + 3], propBytes[i + 2] };
                string propIdentString = BitConverter.ToString(propIdent).Replace("-", "");

                //if this is not the property being gotten continue to next property
                if (propIdentString != propIdentifier)
                {
                    continue;
                }

                //depending on prop type use method to get property value
                switch (propType)
                {
                    case NativeMethods.PT_I2:
                        return BitConverter.ToInt16(propBytes, i + 8);

                    case NativeMethods.PT_LONG:
                        return BitConverter.ToInt32(propBytes, i + 8);

                    case NativeMethods.PT_SYSTIME:
                        return BitConverter.ToInt64(propBytes, i + 8);
                    default:
                        throw new Exception("MAPI property has an unsupported type and can not be retrieved.");
                }
            }

            //property not found return null
            return null;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a string.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a string.</returns>
        public string GetMapiPropertyString(string propIdentifier)
        {
            return this.GetMapiProperty(propIdentifier) as string;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a short.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a short.</returns>
        public Int16 GetMapiPropertyInt16(string propIdentifier)
        {
            return (Int16)this.GetMapiProperty(propIdentifier);
        }

        /// <summary>
        /// Gets the value of the MAPI property as a integer.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a integer.</returns>
        public int GetMapiPropertyInt32(string propIdentifier)
        {
            return (int)this.GetMapiProperty(propIdentifier);
        }

        /// <summary>
        /// Gets the value of the MAPI property as a integer.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a integer.</returns>
        public Int64 GetMapiPropertyInt64(string propIdentifier)
        {
            return (Int64)this.GetMapiProperty(propIdentifier);
        }

        /// <summary>
        /// Gets the value of the MAPI property as a byte array.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a byte array.</returns>
        public byte[] GetMapiPropertyBytes(string propIdentifier)
        {
            return (byte[])this.GetMapiProperty(propIdentifier);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                //ensure only disposed once
                this.disposed = true;

                //call virtual disposing method to let sub classes clean up
                this.Disposing();

                //release COM storage object and suppress finalizer
                ReferenceManager.RemoveItem(this.storage);
                if (OperatingSystem.IsWindows())
                    Marshal.ReleaseComObject(this.storage);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Gives sub classes the chance to free resources during object disposal.
        /// </summary>
        protected virtual void Disposing() { }

        #endregion

    } //End OutlookStorage

    internal static class MapiTags
    {
        #region Mapi standard tags
        // ReSharper disable InconsistentNaming
        /*
         *	M A P I T A G S . H
         *
         *	Property tag definitions for standard properties of MAPI
         *	objects.
         *
         *	The following ranges should be used for all property IDs. Note that
         *	property IDs for objects other than messages and recipients should
         *	all fall in the range "3000 to "3FFF:
         *
         *	From	To		Kind of property
         *	--------------------------------
         *	0001	0BFF	MAPI_defined envelope property
         *	0C00	0DFF	MAPI_defined per-recipient property
         *	0E00	0FFF	MAPI_defined non-transmittable property
         *	1000	2FFF	MAPI_defined message content property
         *
         *	3000	3FFF	MAPI_defined property (usually not message or recipient)
         *
         *	4000	57FF	Transport-defined envelope property
         *	5800	5FFF	Transport-defined per-recipient property
         *	6000	65FF	User-defined non-transmittable property
         *	6600	67FF	Provider-defined internal non-transmittable property
         *	6800	7BFF	Message class-defined content property
         *	7C00	7FFF	Message class-defined non-transmittable
         *					property
         *
         *	8000	FFFE	User-defined Name-to-id mapped property
         *
         *	The 3000-3FFF range is further subdivided as follows:
         *
         *	From	To		Kind of property
         *	--------------------------------
         *	3000	33FF	Common property such as display name, entry ID
         *	3400	35FF	Message store object
         *	3600	36FF	Folder or AB container
         *	3700	38FF	Attachment
         *	3900	39FF	Address book object
         *	3A00	3BFF	Mail user
         *	3C00	3CFF	Distribution list
         *	3D00	3DFF	Profile section
         *	3E00	3FFF	Status object
         *
         *  Copyright (c) 2009 Microsoft Corporation. All Rights Reserved.
         */
        public const string PR_ACKNOWLEDGEMENT_MODE = "0001";
        public const string PR_ALTERNATE_RECIPIENT_ALLOWED = "0002";
        public const string PR_AUTHORIZING_USERS = "0003";
        public const string PR_AUTO_FORWARD_COMMENT = "0004";
        public const string PR_AUTO_FORWARD_COMMENT_W = "0004";
        public const string PR_AUTO_FORWARD_COMMENT_A = "0004";
        public const string PR_AUTO_FORWARDED = "0005";
        public const string PR_CONTENT_CONFIDENTIALITY_ALGORITHM_ID = "0006";
        public const string PR_CONTENT_CORRELATOR = "0007";
        public const string PR_CONTENT_IDENTIFIER = "0008";
        public const string PR_CONTENT_IDENTIFIER_W = "0008";
        public const string PR_CONTENT_IDENTIFIER_A = "0008";
        public const string PR_CONTENT_LENGTH = "0009";
        public const string PR_CONTENT_RETURN_REQUESTED = "000A";
        public const string PR_CONVERSATION_KEY = "000B";
        public const string PR_CONVERSION_EITS = "000C";
        public const string PR_CONVERSION_WITH_LOSS_PROHIBITED = "000D";
        public const string PR_CONVERTED_EITS = "000E";
        public const string PR_DEFERRED_DELIVERY_TIME = "000F";
        public const string PR_DELIVER_TIME = "0010";
        public const string PR_DISCARD_REASON = "0011";
        public const string PR_DISCLOSURE_OF_RECIPIENTS = "0012";
        public const string PR_DL_EXPANSION_HISTORY = "0013";
        public const string PR_DL_EXPANSION_PROHIBITED = "0014";
        public const string PR_EXPIRY_TIME = "0015";
        public const string PR_IMPLICIT_CONVERSION_PROHIBITED = "0016";
        public const string PR_IMPORTANCE = "0017";
        public const string PR_IPM_ID = "0018";
        public const string PR_LATEST_DELIVERY_TIME = "0019";
        public const string PR_MESSAGE_CLASS = "001A";
        public const string PR_MESSAGE_CLASS_W = "001A";
        public const string PR_MESSAGE_CLASS_A = "001A";
        public const string PR_MESSAGE_DELIVERY_ID = "001B";
        public const string PR_MESSAGE_SECURITY_LABEL = "001E";
        public const string PR_OBSOLETED_IPMS = "001F";
        public const string PR_ORIGINALLY_INTENDED_RECIPIENT_NAME = "0020";
        public const string PR_ORIGINAL_EITS = "0021";
        public const string PR_ORIGINATOR_CERTIFICATE = "0022";
        public const string PR_ORIGINATOR_DELIVERY_REPORT_REQUESTED = "0023";
        public const string PR_ORIGINATOR_RETURN_ADDRESS = "0024";
        public const string PR_PARENT_KEY = "0025";
        public const string PR_PRIORITY = "0026";
        public const string PR_ORIGIN_CHECK = "0027";
        public const string PR_PROOF_OF_SUBMISSION_REQUESTED = "0028";
        public const string PR_READ_RECEIPT_REQUESTED = "0029";
        public const string PR_RECEIPT_TIME = "002A";
        public const string PR_RECIPIENT_REASSIGNMENT_PROHIBITED = "002B";
        public const string PR_REDIRECTION_HISTORY = "002C";
        public const string PR_RELATED_IPMS = "002D";
        public const string PR_ORIGINAL_SENSITIVITY = "002E";
        public const string PR_LANGUAGES = "002F";
        public const string PR_LANGUAGES_W = "002F";
        public const string PR_LANGUAGES_A = "002F";
        public const string PR_REPLY_TIME = "0030";
        public const string PR_REPORT_TAG = "0031";
        public const string PR_REPORT_TIME = "0032";
        public const string PR_RETURNED_IPM = "0033";
        public const string PR_SECURITY = "0034";
        public const string PR_INCOMPLETE_COPY = "0035";
        public const string PR_SENSITIVITY = "0036";
        public const string PR_SUBJECT = "0037";
        public const string PR_SUBJECT_W = "0037";
        public const string PR_SUBJECT_A = "0037";
        public const string PR_SUBJECT_IPM = "0038";
        public const string PR_CLIENT_SUBMIT_TIME = "0039";
        public const string PR_REPORT_NAME = "003A";
        public const string PR_REPORT_NAME_W = "003A";
        public const string PR_REPORT_NAME_A = "003A";
        public const string PR_SENT_REPRESENTING_SEARCH_KEY = "003B";
        public const string PR_X400_CONTENT_TYPE = "003C";
        public const string PR_SUBJECT_PREFIX = "003D";
        public const string PR_SUBJECT_PREFIX_W = "003D";
        public const string PR_SUBJECT_PREFIX_A = "003D";
        public const string PR_NON_RECEIPT_REASON = "003E";
        public const string PR_RECEIVED_BY_ENTRYID = "003F";
        public const string PR_RECEIVED_BY_NAME = "0040";
        public const string PR_RECEIVED_BY_NAME_W = "0040";
        public const string PR_RECEIVED_BY_NAME_A = "0040";
        public const string PR_SENT_REPRESENTING_ENTRYID = "0041";
        public const string PR_SENT_REPRESENTING_NAME = "0042";
        public const string PR_SENT_REPRESENTING_NAME_W = "0042";
        public const string PR_SENT_REPRESENTING_NAME_A = "0042";
        public const string PR_RCVD_REPRESENTING_ENTRYID = "0043";
        public const string PR_RCVD_REPRESENTING_NAME = "0044";
        public const string PR_RCVD_REPRESENTING_NAME_W = "0044";
        public const string PR_RCVD_REPRESENTING_NAME_A = "0044";
        public const string PR_REPORT_ENTRYID = "0045";
        public const string PR_READ_RECEIPT_ENTRYID = "0046";
        public const string PR_MESSAGE_SUBMISSION_ID = "0047";
        public const string PR_PROVIDER_SUBMIT_TIME = "0048";
        public const string PR_ORIGINAL_SUBJECT = "0049";
        public const string PR_ORIGINAL_SUBJECT_W = "0049";
        public const string PR_ORIGINAL_SUBJECT_A = "0049";
        public const string PR_DISC_VAL = "004A";
        public const string PR_ORIG_MESSAGE_CLASS = "004B";
        public const string PR_ORIG_MESSAGE_CLASS_W = "004B";
        public const string PR_ORIG_MESSAGE_CLASS_A = "004B";
        public const string PR_ORIGINAL_AUTHOR_ENTRYID = "004C";
        public const string PR_ORIGINAL_AUTHOR_NAME = "004D";
        public const string PR_ORIGINAL_AUTHOR_NAME_W = "004D";
        public const string PR_ORIGINAL_AUTHOR_NAME_A = "004D";
        public const string PR_ORIGINAL_SUBMIT_TIME = "004E";
        public const string PR_REPLY_RECIPIENT_ENTRIES = "004F";
        public const string PR_REPLY_RECIPIENT_NAMES = "0050";
        public const string PR_REPLY_RECIPIENT_NAMES_W = "0050";
        public const string PR_REPLY_RECIPIENT_NAMES_A = "0050";

        public const string PR_RECEIVED_BY_SEARCH_KEY = "0051";
        public const string PR_RCVD_REPRESENTING_SEARCH_KEY = "0052";
        public const string PR_READ_RECEIPT_SEARCH_KEY = "0053";
        public const string PR_REPORT_SEARCH_KEY = "0054";
        public const string PR_ORIGINAL_DELIVERY_TIME = "0055";
        public const string PR_ORIGINAL_AUTHOR_SEARCH_KEY = "0056";

        public const string PR_MESSAGE_TO_ME = "0057";
        public const string PR_MESSAGE_CC_ME = "0058";
        public const string PR_MESSAGE_RECIP_ME = "0059";

        public const string PR_ORIGINAL_SENDER_NAME = "005A";
        public const string PR_ORIGINAL_SENDER_NAME_W = "005A";
        public const string PR_ORIGINAL_SENDER_NAME_A = "005A";
        public const string PR_ORIGINAL_SENDER_ENTRYID = "005B";
        public const string PR_ORIGINAL_SENDER_SEARCH_KEY = "005C";
        public const string PR_ORIGINAL_SENT_REPRESENTING_NAME = "005D";
        public const string PR_ORIGINAL_SENT_REPRESENTING_NAME_W = "005D";
        public const string PR_ORIGINAL_SENT_REPRESENTING_NAME_A = "005D";
        public const string PR_ORIGINAL_SENT_REPRESENTING_ENTRYID = "005E";
        public const string PR_ORIGINAL_SENT_REPRESENTING_SEARCH_KEY = "005F";

        public const string PR_START_DATE = "0060";
        public const string PR_END_DATE = "0061";
        public const string PR_OWNER_APPT_ID = "0062";
        public const string PR_RESPONSE_REQUESTED = "0063";

        public const string PR_SENT_REPRESENTING_ADDRTYPE = "0064";
        public const string PR_SENT_REPRESENTING_ADDRTYPE_W = "0064";
        public const string PR_SENT_REPRESENTING_ADDRTYPE_A = "0064";
        public const string PR_SENT_REPRESENTING_EMAIL_ADDRESS = "0065";
        public const string PR_SENT_REPRESENTING_EMAIL_ADDRESS_W = "0065";
        public const string PR_SENT_REPRESENTING_EMAIL_ADDRESS_A = "0065";

        public const string PR_ORIGINAL_SENDER_ADDRTYPE = "0066";
        public const string PR_ORIGINAL_SENDER_ADDRTYPE_W = "0066";
        public const string PR_ORIGINAL_SENDER_ADDRTYPE_A = "0066";
        public const string PR_ORIGINAL_SENDER_EMAIL_ADDRESS = "0067";
        public const string PR_ORIGINAL_SENDER_EMAIL_ADDRESS_W = "0067";
        public const string PR_ORIGINAL_SENDER_EMAIL_ADDRESS_A = "0067";

        public const string PR_ORIGINAL_SENT_REPRESENTING_ADDRTYPE = "0068";
        public const string PR_ORIGINAL_SENT_REPRESENTING_ADDRTYPE_W = "0068";
        public const string PR_ORIGINAL_SENT_REPRESENTING_ADDRTYPE_A = "0068";
        public const string PR_ORIGINAL_SENT_REPRESENTING_EMAIL_ADDRESS = "0069";
        public const string PR_ORIGINAL_SENT_REPRESENTING_EMAIL_ADDRESS_W = "0069";
        public const string PR_ORIGINAL_SENT_REPRESENTING_EMAIL_ADDRESS_A = "0069";

        public const string PR_CONVERSATION_TOPIC = "0070";
        public const string PR_CONVERSATION_TOPIC_W = "0070";
        public const string PR_CONVERSATION_TOPIC_A = "0070";
        public const string PR_CONVERSATION_INDEX = "0071";

        public const string PR_ORIGINAL_DISPLAY_BCC = "0072";
        public const string PR_ORIGINAL_DISPLAY_BCC_W = "0072";
        public const string PR_ORIGINAL_DISPLAY_BCC_A = "0072";
        public const string PR_ORIGINAL_DISPLAY_CC = "0073";
        public const string PR_ORIGINAL_DISPLAY_CC_W = "0073";
        public const string PR_ORIGINAL_DISPLAY_CC_A = "0073";
        public const string PR_ORIGINAL_DISPLAY_TO = "0074";
        public const string PR_ORIGINAL_DISPLAY_TO_W = "0074";
        public const string PR_ORIGINAL_DISPLAY_TO_A = "0074";

        public const string PR_RECEIVED_BY_ADDRTYPE = "0075";
        public const string PR_RECEIVED_BY_ADDRTYPE_W = "0075";
        public const string PR_RECEIVED_BY_ADDRTYPE_A = "0075";
        public const string PR_RECEIVED_BY_EMAIL_ADDRESS = "0076";
        public const string PR_RECEIVED_BY_EMAIL_ADDRESS_W = "0076";
        public const string PR_RECEIVED_BY_EMAIL_ADDRESS_A = "0076";

        public const string PR_RCVD_REPRESENTING_ADDRTYPE = "0077";
        public const string PR_RCVD_REPRESENTING_ADDRTYPE_W = "0077";
        public const string PR_RCVD_REPRESENTING_ADDRTYPE_A = "0077";
        public const string PR_RCVD_REPRESENTING_EMAIL_ADDRESS = "0078";
        public const string PR_RCVD_REPRESENTING_EMAIL_ADDRESS_W = "0078";
        public const string PR_RCVD_REPRESENTING_EMAIL_ADDRESS_A = "0078";

        public const string PR_ORIGINAL_AUTHOR_ADDRTYPE = "0079";
        public const string PR_ORIGINAL_AUTHOR_ADDRTYPE_W = "0079";
        public const string PR_ORIGINAL_AUTHOR_ADDRTYPE_A = "0079";
        public const string PR_ORIGINAL_AUTHOR_EMAIL_ADDRESS = "007A";
        public const string PR_ORIGINAL_AUTHOR_EMAIL_ADDRESS_W = "007A";
        public const string PR_ORIGINAL_AUTHOR_EMAIL_ADDRESS_A = "007A";

        public const string PR_ORIGINALLY_INTENDED_RECIP_ADDRTYPE = "007B";
        public const string PR_ORIGINALLY_INTENDED_RECIP_ADDRTYPE_W = "007B";
        public const string PR_ORIGINALLY_INTENDED_RECIP_ADDRTYPE_A = "007B";
        public const string PR_ORIGINALLY_INTENDED_RECIP_EMAIL_ADDRESS = "007C";
        public const string PR_ORIGINALLY_INTENDED_RECIP_EMAIL_ADDRESS_W = "007C";
        public const string PR_ORIGINALLY_INTENDED_RECIP_EMAIL_ADDRESS_A = "007C";

        public const string PR_TRANSPORT_MESSAGE_HEADERS = "007D";
        public const string PR_TRANSPORT_MESSAGE_HEADERS_W = "007D";
        public const string PR_TRANSPORT_MESSAGE_HEADERS_A = "007D";
        public const string PR_DELEGATION = "007E";
        public const string PR_TNEF_CORRELATION_KEY = "007F";

        /*
         *	Message content properties
         */

        public const string PR_BODY = "1000";
        public const string PR_BODY_W = "1000";
        public const string PR_BODY_A = "1000";
        public const string PR_BODY_HTML = "1013";
        public const string PR_REPORT_TEXT = "1001";
        public const string PR_REPORT_TEXT_W = "1001";
        public const string PR_REPORT_TEXT_A = "1001";
        public const string PR_ORIGINATOR_AND_DL_EXPANSION_HISTORY = "1002";
        public const string PR_REPORTING_DL_NAME = "1003";
        public const string PR_REPORTING_MTA_CERTIFICATE = "1004";

        /*  Removed PR_REPORT_ORIGIN_AUTHENTICATION_CHECK with DCR 3865, use PR_ORIGIN_CHECK */

        public const string PR_RTF_SYNC_BODY_CRC = "1006";
        public const string PR_RTF_SYNC_BODY_COUNT = "1007";
        public const string PR_RTF_SYNC_BODY_TAG = "1008";
        public const string PR_RTF_SYNC_BODY_TAG_W = "1008";
        public const string PR_RTF_SYNC_BODY_TAG_A = "1008";
        public const string PR_RTF_COMPRESSED = "1009";
        public const string PR_RTF_SYNC_PREFIX_COUNT = "1010";
        public const string PR_RTF_SYNC_TRAILING_COUNT = "1011";
        public const string PR_ORIGINALLY_INTENDED_RECIP_ENTRYID = "1012";

        /*
         *  Reserved "1100-"1200
         */


        /*
         *	Message recipient properties
         */

        public const string PR_CONTENT_INTEGRITY_CHECK = "0C00";
        public const string PR_EXPLICIT_CONVERSION = "0C01";
        public const string PR_IPM_RETURN_REQUESTED = "0C02";
        public const string PR_MESSAGE_TOKEN = "0C03";
        public const string PR_NDR_REASON_CODE = "0C04";
        public const string PR_NDR_DIAG_CODE = "0C05";
        public const string PR_NON_RECEIPT_NOTIFICATION_REQUESTED = "0C06";
        public const string PR_DELIVERY_POINT = "0C07";

        public const string PR_ORIGINATOR_NON_DELIVERY_REPORT_REQUESTED = "0C08";
        public const string PR_ORIGINATOR_REQUESTED_ALTERNATE_RECIPIENT = "0C09";
        public const string PR_PHYSICAL_DELIVERY_BUREAU_FAX_DELIVERY = "0C0A";
        public const string PR_PHYSICAL_DELIVERY_MODE = "0C0B";
        public const string PR_PHYSICAL_DELIVERY_REPORT_REQUEST = "0C0C";
        public const string PR_PHYSICAL_FORWARDING_ADDRESS = "0C0D";
        public const string PR_PHYSICAL_FORWARDING_ADDRESS_REQUESTED = "0C0E";
        public const string PR_PHYSICAL_FORWARDING_PROHIBITED = "0C0F";
        public const string PR_PHYSICAL_RENDITION_ATTRIBUTES = "0C10";
        public const string PR_PROOF_OF_DELIVERY = "0C11";
        public const string PR_PROOF_OF_DELIVERY_REQUESTED = "0C12";
        public const string PR_RECIPIENT_CERTIFICATE = "0C13";
        public const string PR_RECIPIENT_NUMBER_FOR_ADVICE = "0C14";
        public const string PR_RECIPIENT_NUMBER_FOR_ADVICE_W = "0C14";
        public const string PR_RECIPIENT_NUMBER_FOR_ADVICE_A = "0C14";
        public const string PR_RECIPIENT_TYPE = "0C15";
        public const string PR_REGISTERED_MAIL_TYPE = "0C16";
        public const string PR_REPLY_REQUESTED = "0C17";
        public const string PR_REQUESTED_DELIVERY_METHOD = "0C18";
        public const string PR_SENDER_ENTRYID = "0C19";
        public const string PR_SENDER_NAME = "0C1A";
        public const string PR_SENDER_NAME_W = "0C1A";
        public const string PR_SENDER_NAME_A = "0C1A";
        public const string PR_SUPPLEMENTARY_INFO = "0C1B";
        public const string PR_SUPPLEMENTARY_INFO_W = "0C1B";
        public const string PR_SUPPLEMENTARY_INFO_A = "0C1B";
        public const string PR_TYPE_OF_MTS_USER = "0C1C";
        public const string PR_SENDER_SEARCH_KEY = "0C1D";
        public const string PR_SENDER_ADDRTYPE = "0C1E";
        public const string PR_SENDER_ADDRTYPE_W = "0C1E";
        public const string PR_SENDER_ADDRTYPE_A = "0C1E";
        public const string PR_SENDER_EMAIL_ADDRESS = "0C1F";
        public const string PR_SENDER_EMAIL_ADDRESS_W = "0C1F";
        public const string PR_SENDER_EMAIL_ADDRESS_A = "0C1F";

        /*
         *	Message non-transmittable properties
         */

        /*
         * The two tags, PR_MESSAGE_RECIPIENTS and PR_MESSAGE_ATTACHMENTS,
         * are to be used in the exclude list passed to
         * IMessage::CopyTo when the caller wants either the recipients or attachments
         * of the message to not get copied.  It is also used in the ProblemArray
         * return from IMessage::CopyTo when an error is encountered copying them
         */

        public const string PR_CURRENT_VERSION = "0E00";
        public const string PR_DELETE_AFTER_SUBMIT = "0E01";
        public const string PR_DISPLAY_BCC = "0E02";
        public const string PR_DISPLAY_BCC_W = "0E02";
        public const string PR_DISPLAY_BCC_A = "0E02";
        public const string PR_DISPLAY_CC = "0E03";
        public const string PR_DISPLAY_CC_W = "0E03";
        public const string PR_DISPLAY_CC_A = "0E03";
        public const string PR_DISPLAY_TO = "0E04";
        public const string PR_DISPLAY_TO_W = "0E04";
        public const string PR_DISPLAY_TO_A = "0E04";
        public const string PR_PARENT_DISPLAY = "0E05";
        public const string PR_PARENT_DISPLAY_W = "0E05";
        public const string PR_PARENT_DISPLAY_A = "0E05";
        public const string PR_MESSAGE_DELIVERY_TIME = "0E06";
        public const string PR_MESSAGE_FLAGS = "0E07";
        public const string PR_MESSAGE_SIZE = "0E08";
        public const string PR_PARENT_ENTRYID = "0E09";
        public const string PR_SENTMAIL_ENTRYID = "0E0A";
        public const string PR_CORRELATE = "0E0C";
        public const string PR_CORRELATE_MTSID = "0E0D";
        public const string PR_DISCRETE_VALUES = "0E0E";
        public const string PR_RESPONSIBILITY = "0E0F";
        public const string PR_SPOOLER_STATUS = "0E10";
        public const string PR_TRANSPORT_STATUS = "0E11";
        public const string PR_MESSAGE_RECIPIENTS = "0E12";
        public const string PR_MESSAGE_ATTACHMENTS = "0E13";
        public const string PR_SUBMIT_FLAGS = "0E14";
        public const string PR_RECIPIENT_STATUS = "0E15";
        public const string PR_TRANSPORT_KEY = "0E16";
        public const string PR_MSG_STATUS = "0E17";
        public const string PR_MESSAGE_DOWNLOAD_TIME = "0E18";
        public const string PR_CREATION_VERSION = "0E19";
        public const string PR_MODIFY_VERSION = "0E1A";
        public const string PR_HASATTACH = "0E1B";
        public const string PR_BODY_CRC = "0E1C";
        public const string PR_NORMALIZED_SUBJECT = "0E1D";
        public const string PR_NORMALIZED_SUBJECT_W = "0E1D";
        public const string PR_NORMALIZED_SUBJECT_A = "0E1D";
        public const string PR_RTF_IN_SYNC = "0E1F";
        public const string PR_ATTACH_SIZE = "0E20";
        public const string PR_ATTACH_NUM = "0E21";
        public const string PR_PREPROCESS = "0E22";

        /* PR_ORIGINAL_DISPLAY_TO, _CC, and _BCC moved to transmittible range 03/09/95 */

        public const string PR_ORIGINATING_MTA_CERTIFICATE = "0E25";
        public const string PR_PROOF_OF_SUBMISSION = "0E26";

        /*
         * The range of non-message and non-recipient property IDs ("3000 - "3FFF" is
         * further broken down into ranges to make assigning new property IDs easier.
         *
         *	From	To		Kind of property
         *	--------------------------------
         *	3000	32FF	MAPI_defined common property
         *	3200	33FF	MAPI_defined form property
         *	3400	35FF	MAPI_defined message store property
         *	3600	36FF	MAPI_defined Folder or AB Container property
         *	3700	38FF	MAPI_defined attachment property
         *	3900	39FF	MAPI_defined address book property
         *	3A00	3BFF	MAPI_defined mailuser property
         *	3C00	3CFF	MAPI_defined DistList property
         *	3D00	3DFF	MAPI_defined Profile Section property
         *	3E00	3EFF	MAPI_defined Status property
         *	3F00	3FFF	MAPI_defined display table property
         */

        /*
         *	Properties common to numerous MAPI objects.
         *
         *	Those properties that can appear on messages are in the
         *	non-transmittable range for messages. They start at the high
         *	end of that range and work down.
         *
         *	Properties that never appear on messages are defined in the common
         *	property range (see above".
         */

        /*
         * properties that are common to multiple objects (including message objects";
         * -- these ids are in the non-transmittable range
         */

        public const string PR_ENTRYID = "0FFF";
        public const string PR_OBJECT_TYPE = "0FFE";
        public const string PR_ICON = "0FFD";
        public const string PR_MINI_ICON = "0FFC";
        public const string PR_STORE_ENTRYID = "0FFB";
        public const string PR_STORE_RECORD_KEY = "0FFA";
        public const string PR_RECORD_KEY = "0FF9";
        public const string PR_MAPPING_SIGNATURE = "0FF8";
        public const string PR_ACCESS_LEVEL = "0FF7";
        public const string PR_INSTANCE_KEY = "0FF6";
        public const string PR_ROW_TYPE = "0FF5";
        public const string PR_ACCESS = "0FF4";

        /*
         * properties that are common to multiple objects (usually not including message objects";
         * -- these ids are in the transmittable range
         */

        public const string PR_ROWID = "3000";
        public const string PR_DISPLAY_NAME = "3001";
        public const string PR_DISPLAY_NAME_W = "3001";
        public const string PR_DISPLAY_NAME_A = "3001";
        public const string PR_ADDRTYPE = "3002";
        public const string PR_ADDRTYPE_W = "3002";
        public const string PR_ADDRTYPE_A = "3002";
        public const string PR_EMAIL_ADDRESS = "3003";
        public const string PR_EMAIL_ADDRESS_W = "3003";
        public const string PR_EMAIL_ADDRESS_A = "3003";
        public const string PR_COMMENT = "3004";
        public const string PR_COMMENT_W = "3004";
        public const string PR_COMMENT_A = "3004";
        public const string PR_DEPTH = "3005";
        public const string PR_PROVIDER_DISPLAY = "3006";
        public const string PR_PROVIDER_DISPLAY_W = "3006";
        public const string PR_PROVIDER_DISPLAY_A = "3006";
        public const string PR_CREATION_TIME = "3007";
        public const string PR_LAST_MODIFICATION_TIME = "3008";
        public const string PR_RESOURCE_FLAGS = "3009";
        public const string PR_PROVIDER_DLL_NAME = "300A";
        public const string PR_PROVIDER_DLL_NAME_W = "300A";
        public const string PR_PROVIDER_DLL_NAME_A = "300A";
        public const string PR_SEARCH_KEY = "300B";
        public const string PR_PROVIDER_UID = "300C";
        public const string PR_PROVIDER_ORDINAL = "300D";

        /*
         *  MAPI Form properties
         */
        public const string PR_FORM_VERSION = "3301";
        public const string PR_FORM_VERSION_W = "3301";
        public const string PR_FORM_VERSION_A = "3301";
        public const string PR_FORM_CLSID = "3302";
        public const string PR_FORM_CONTACT_NAME = "3303";
        public const string PR_FORM_CONTACT_NAME_W = "3303";
        public const string PR_FORM_CONTACT_NAME_A = "3303";
        public const string PR_FORM_CATEGORY = "3304";
        public const string PR_FORM_CATEGORY_W = "3304";
        public const string PR_FORM_CATEGORY_A = "3304";
        public const string PR_FORM_CATEGORY_SUB = "3305";
        public const string PR_FORM_CATEGORY_SUB_W = "3305";
        public const string PR_FORM_CATEGORY_SUB_A = "3305";
        public const string PR_FORM_HOST_MAP = "3306";
        public const string PR_FORM_HIDDEN = "3307";
        public const string PR_FORM_DESIGNER_NAME = "3308";
        public const string PR_FORM_DESIGNER_NAME_W = "3308";
        public const string PR_FORM_DESIGNER_NAME_A = "3308";
        public const string PR_FORM_DESIGNER_GUID = "3309";
        public const string PR_FORM_MESSAGE_BEHAVIOR = "330A";

        /*
         *	Message store properties
         */

        public const string PR_DEFAULT_STORE = "3400";
        public const string PR_STORE_SUPPORT_MASK = "340D";
        public const string PR_STORE_STATE = "340E";

        public const string PR_IPM_SUBTREE_SEARCH_KEY = "3410";
        public const string PR_IPM_OUTBOX_SEARCH_KEY = "3411";
        public const string PR_IPM_WASTEBASKET_SEARCH_KEY = "3412";
        public const string PR_IPM_SENTMAIL_SEARCH_KEY = "3413";
        public const string PR_MDB_PROVIDER = "3414";
        public const string PR_RECEIVE_FOLDER_SETTINGS = "3415";

        public const string PR_VALID_FOLDER_MASK = "35DF";
        public const string PR_IPM_SUBTREE_ENTRYID = "35E0";

        public const string PR_IPM_OUTBOX_ENTRYID = "35E2";
        public const string PR_IPM_WASTEBASKET_ENTRYID = "35E3";
        public const string PR_IPM_SENTMAIL_ENTRYID = "35E4";
        public const string PR_VIEWS_ENTRYID = "35E5";
        public const string PR_COMMON_VIEWS_ENTRYID = "35E6";
        public const string PR_FINDER_ENTRYID = "35E7";

        /* Proptags "35E8-"35FF reserved for folders "guaranteed" by PR_VALID_FOLDER_MASK */


        /*
         *	Folder and AB Container properties
         */

        public const string PR_CONTAINER_FLAGS = "3600";
        public const string PR_FOLDER_TYPE = "3601";
        public const string PR_CONTENT_COUNT = "3602";
        public const string PR_CONTENT_UNREAD = "3603";
        public const string PR_CREATE_TEMPLATES = "3604";
        public const string PR_DETAILS_TABLE = "3605";
        public const string PR_SEARCH = "3607";
        public const string PR_SELECTABLE = "3609";
        public const string PR_SUBFOLDERS = "360A";
        public const string PR_STATUS = "360B";
        public const string PR_ANR = "360C";
        public const string PR_ANR_W = "360C";
        public const string PR_ANR_A = "360C";
        public const string PR_CONTENTS_SORT_ORDER = "360D";
        public const string PR_CONTAINER_HIERARCHY = "360E";
        public const string PR_CONTAINER_CONTENTS = "360F";
        public const string PR_FOLDER_ASSOCIATED_CONTENTS = "3610";
        public const string PR_DEF_CREATE_DL = "3611";
        public const string PR_DEF_CREATE_MAILUSER = "3612";
        public const string PR_CONTAINER_CLASS = "3613";
        public const string PR_CONTAINER_CLASS_W = "3613";
        public const string PR_CONTAINER_CLASS_A = "3613";
        public const string PR_CONTAINER_MODIFY_VERSION = "3614";
        public const string PR_AB_PROVIDER_ID = "3615";
        public const string PR_DEFAULT_VIEW_ENTRYID = "3616";
        public const string PR_ASSOC_CONTENT_COUNT = "3617";

        /* Reserved "36C0-"36FF */

        /*
         *	Attachment properties
         */

        public const string PR_ATTACHMENT_X400_PARAMETERS = "3700";
        public const string PR_ATTACH_DATA_OBJ = "3701";
        public const string PR_ATTACH_DATA_BIN = "3701";
        public const string PR_ATTACH_ENCODING = "3702";
        public const string PR_ATTACH_EXTENSION = "3703";
        public const string PR_ATTACH_EXTENSION_W = "3703";
        public const string PR_ATTACH_EXTENSION_A = "3703";
        public const string PR_ATTACH_FILENAME = "3704";
        public const string PR_ATTACH_FILENAME_W = "3704";
        public const string PR_ATTACH_FILENAME_A = "3704";
        public const string PR_ATTACH_METHOD = "3705";
        public const string PR_ATTACH_LONG_FILENAME = "3707";
        public const string PR_ATTACH_LONG_FILENAME_W = "3707";
        public const string PR_ATTACH_LONG_FILENAME_A = "3707";
        public const string PR_ATTACH_PATHNAME = "3708";
        public const string PR_ATTACH_PATHNAME_W = "3708";
        public const string PR_ATTACH_PATHNAME_A = "3708";
        public const string PR_ATTACH_RENDERING = "3709";
        public const string PR_ATTACH_CONTENTID = "3712";
        public const string PR_ATTACH_TAG = "370A";
        public const string PR_RENDERING_POSITION = "370B";
        public const string PR_ATTACH_TRANSPORT_NAME = "370C";
        public const string PR_ATTACH_TRANSPORT_NAME_W = "370C";
        public const string PR_ATTACH_TRANSPORT_NAME_A = "370C";
        public const string PR_ATTACH_LONG_PATHNAME = "370D";
        public const string PR_ATTACH_LONG_PATHNAME_W = "370D";
        public const string PR_ATTACH_LONG_PATHNAME_A = "370D";
        public const string PR_ATTACH_MIME_TAG = "370E";
        public const string PR_ATTACH_MIME_TAG_W = "370E";
        public const string PR_ATTACH_MIME_TAG_A = "370E";
        public const string PR_ATTACH_ADDITIONAL_INFO = "370F";
        public const string PR_ATTACHMENT_CONTACTPHOTO = "7FFF";

        /*
         *  AB Object properties
         */

        public const string PR_DISPLAY_TYPE = "3900";
        public const string PR_DISPLAY_TYPE_EX = "3905";
        public const string PR_TEMPLATEID = "3902";
        public const string PR_PRIMARY_CAPABILITY = "3904";


        /*
         *	Mail user properties
         */

        public const string PR_7BIT_DISPLAY_NAME = "39FF";
        public const string PR_ACCOUNT = "3A00";
        public const string PR_ACCOUNT_W = "3A00";

        /// <summary>
        /// E-mail address e.g. PeterPan@neverland.com
        /// </summary>
        public const string PR_EMAIL_1 = "39FE";

        /// <summary>
        /// Second place to search for an E-mail address
        /// </summary>
        public const string PR_EMAIL_2 = "403E";
        public const string PR_ACCOUNT_A = "3A00";
        public const string PR_ALTERNATE_RECIPIENT = "3A01";
        public const string PR_CALLBACK_TELEPHONE_NUMBER = "3A02";
        public const string PR_CALLBACK_TELEPHONE_NUMBER_W = "3A02";
        public const string PR_CALLBACK_TELEPHONE_NUMBER_A = "3A02";
        public const string PR_CONVERSION_PROHIBITED = "3A03";
        public const string PR_DISCLOSE_RECIPIENTS = "3A04";
        public const string PR_GENERATION = "3A05";
        public const string PR_GENERATION_W = "3A05";
        public const string PR_GENERATION_A = "3A05";
        public const string PR_GIVEN_NAME = "3A06";
        public const string PR_GIVEN_NAME_W = "3A06";
        public const string PR_GIVEN_NAME_A = "3A06";
        public const string PR_GOVERNMENT_ID_NUMBER = "3A07";
        public const string PR_GOVERNMENT_ID_NUMBER_W = "3A07";
        public const string PR_GOVERNMENT_ID_NUMBER_A = "3A07";
        public const string PR_BUSINESS_TELEPHONE_NUMBER = "3A08";
        public const string PR_BUSINESS_TELEPHONE_NUMBER_W = "3A08";
        public const string PR_BUSINESS_TELEPHONE_NUMBER_A = "3A08";
        public const string PR_OFFICE_TELEPHONE_NUMBER = "3A08";
        public const string PR_OFFICE_TELEPHONE_NUMBER_W = "3A08";
        public const string PR_OFFICE_TELEPHONE_NUMBER_A = "3A08";
        public const string PR_HOME_TELEPHONE_NUMBER = "3A09";
        public const string PR_HOME_TELEPHONE_NUMBER_W = "3A09";
        public const string PR_HOME_TELEPHONE_NUMBER_A = "3A09";
        public const string PR_INITIALS = "3A0A";
        public const string PR_INITIALS_W = "3A0A";
        public const string PR_INITIALS_A = "3A0A";
        public const string PR_KEYWORD = "3A0B";
        public const string PR_KEYWORD_W = "3A0B";
        public const string PR_KEYWORD_A = "3A0B";
        public const string PR_LANGUAGE = "3A0C";
        public const string PR_LANGUAGE_W = "3A0C";
        public const string PR_LANGUAGE_A = "3A0C";
        public const string PR_LOCATION = "3A0D";
        public const string PR_LOCATION_W = "3A0D";
        public const string PR_LOCATION_A = "3A0D";
        public const string PR_MAIL_PERMISSION = "3A0E";
        public const string PR_MHS_COMMON_NAME = "3A0F";
        public const string PR_MHS_COMMON_NAME_W = "3A0F";
        public const string PR_MHS_COMMON_NAME_A = "3A0F";
        public const string PR_ORGANIZATIONAL_ID_NUMBER = "3A10";
        public const string PR_ORGANIZATIONAL_ID_NUMBER_W = "3A10";
        public const string PR_ORGANIZATIONAL_ID_NUMBER_A = "3A10";
        public const string PR_SURNAME = "3A11";
        public const string PR_SURNAME_W = "3A11";
        public const string PR_SURNAME_A = "3A11";
        public const string PR_ORIGINAL_ENTRYID = "3A12";
        public const string PR_ORIGINAL_DISPLAY_NAME = "3A13";
        public const string PR_ORIGINAL_DISPLAY_NAME_W = "3A13";
        public const string PR_ORIGINAL_DISPLAY_NAME_A = "3A13";
        public const string PR_ORIGINAL_SEARCH_KEY = "3A14";
        public const string PR_POSTAL_ADDRESS = "3A15";
        public const string PR_POSTAL_ADDRESS_W = "3A15";
        public const string PR_POSTAL_ADDRESS_A = "3A15";
        public const string PR_COMPANY_NAME = "3A16";
        public const string PR_COMPANY_NAME_W = "3A16";
        public const string PR_COMPANY_NAME_A = "3A16";
        public const string PR_TITLE = "3A17";
        public const string PR_TITLE_W = "3A17";
        public const string PR_TITLE_A = "3A17";
        public const string PR_DEPARTMENT_NAME = "3A18";
        public const string PR_DEPARTMENT_NAME_W = "3A18";
        public const string PR_DEPARTMENT_NAME_A = "3A18";
        public const string PR_OFFICE_LOCATION = "3A19";
        public const string PR_OFFICE_LOCATION_W = "3A19";
        public const string PR_OFFICE_LOCATION_A = "3A19";
        public const string PR_PRIMARY_TELEPHONE_NUMBER = "3A1A";
        public const string PR_PRIMARY_TELEPHONE_NUMBER_W = "3A1A";
        public const string PR_PRIMARY_TELEPHONE_NUMBER_A = "3A1A";
        public const string PR_BUSINESS2_TELEPHONE_NUMBER = "3A1B";
        public const string PR_BUSINESS2_TELEPHONE_NUMBER_W = "3A1B";
        public const string PR_BUSINESS2_TELEPHONE_NUMBER_A = "3A1B";
        public const string PR_OFFICE2_TELEPHONE_NUMBER = "3A1B";
        public const string PR_OFFICE2_TELEPHONE_NUMBER_W = "3A1B";
        public const string PR_OFFICE2_TELEPHONE_NUMBER_A = "3A1B";
        public const string PR_MOBILE_TELEPHONE_NUMBER = "3A1C";
        public const string PR_MOBILE_TELEPHONE_NUMBER_W = "3A1C";
        public const string PR_MOBILE_TELEPHONE_NUMBER_A = "3A1C";
        public const string PR_CELLULAR_TELEPHONE_NUMBER = "3A1C";
        public const string PR_CELLULAR_TELEPHONE_NUMBER_W = "3A1C";
        public const string PR_CELLULAR_TELEPHONE_NUMBER_A = "3A1C";
        public const string PR_RADIO_TELEPHONE_NUMBER = "3A1D";
        public const string PR_RADIO_TELEPHONE_NUMBER_W = "3A1D";
        public const string PR_RADIO_TELEPHONE_NUMBER_A = "3A1D";
        public const string PR_CAR_TELEPHONE_NUMBER = "3A1E";
        public const string PR_CAR_TELEPHONE_NUMBER_W = "3A1E";
        public const string PR_CAR_TELEPHONE_NUMBER_A = "3A1E";
        public const string PR_OTHER_TELEPHONE_NUMBER = "3A1F";
        public const string PR_OTHER_TELEPHONE_NUMBER_W = "3A1F";
        public const string PR_OTHER_TELEPHONE_NUMBER_A = "3A1F";
        public const string PR_TRANSMITABLE_DISPLAY_NAME = "3A20";
        public const string PR_TRANSMITABLE_DISPLAY_NAME_W = "3A20";
        public const string PR_TRANSMITABLE_DISPLAY_NAME_A = "3A20";
        public const string PR_PAGER_TELEPHONE_NUMBER = "3A21";
        public const string PR_PAGER_TELEPHONE_NUMBER_W = "3A21";
        public const string PR_PAGER_TELEPHONE_NUMBER_A = "3A21";
        public const string PR_BEEPER_TELEPHONE_NUMBER = "3A21";
        public const string PR_BEEPER_TELEPHONE_NUMBER_W = "3A21";
        public const string PR_BEEPER_TELEPHONE_NUMBER_A = "3A21";
        public const string PR_USER_CERTIFICATE = "3A22";
        public const string PR_PRIMARY_FAX_NUMBER = "3A23";
        public const string PR_PRIMARY_FAX_NUMBER_W = "3A23";
        public const string PR_PRIMARY_FAX_NUMBER_A = "3A23";
        public const string PR_BUSINESS_FAX_NUMBER = "3A24";
        public const string PR_BUSINESS_FAX_NUMBER_W = "3A24";
        public const string PR_BUSINESS_FAX_NUMBER_A = "3A24";
        public const string PR_HOME_FAX_NUMBER = "3A25";
        public const string PR_HOME_FAX_NUMBER_W = "3A25";
        public const string PR_HOME_FAX_NUMBER_A = "3A25";
        public const string PR_COUNTRY = "3A26";
        public const string PR_COUNTRY_W = "3A26";
        public const string PR_COUNTRY_A = "3A26";
        public const string PR_BUSINESS_ADDRESS_COUNTRY = "3A26";
        public const string PR_BUSINESS_ADDRESS_COUNTRY_W = "3A26";
        public const string PR_BUSINESS_ADDRESS_COUNTRY_A = "3A26";
        public const string PR_LOCALITY = "3A27";
        public const string PR_LOCALITY_W = "3A27";
        public const string PR_LOCALITY_A = "3A27";
        public const string PR_BUSINESS_ADDRESS_CITY = "3A27";
        public const string PR_BUSINESS_ADDRESS_CITY_W = "3A27";
        public const string PR_BUSINESS_ADDRESS_CITY_A = "3A27";
        public const string PR_STATE_OR_PROVINCE = "3A28";
        public const string PR_STATE_OR_PROVINCE_W = "3A28";
        public const string PR_STATE_OR_PROVINCE_A = "3A28";
        public const string PR_BUSINESS_ADDRESS_STATE_OR_PROVINCE = "3A28";
        public const string PR_BUSINESS_ADDRESS_STATE_OR_PROVINCE_W = "3A28";
        public const string PR_BUSINESS_ADDRESS_STATE_OR_PROVINCE_A = "3A28";
        public const string PR_STREET_ADDRESS = "3A29";
        public const string PR_STREET_ADDRESS_W = "3A29";
        public const string PR_STREET_ADDRESS_A = "3A29";
        public const string PR_BUSINESS_ADDRESS_STREET = "3A29";
        public const string PR_BUSINESS_ADDRESS_STREET_W = "3A29";
        public const string PR_BUSINESS_ADDRESS_STREET_A = "3A29";
        public const string PR_POSTAL_CODE = "3A2A";
        public const string PR_POSTAL_CODE_W = "3A2A";
        public const string PR_POSTAL_CODE_A = "3A2A";
        public const string PR_BUSINESS_ADDRESS_POSTAL_CODE = "3A2A";
        public const string PR_BUSINESS_ADDRESS_POSTAL_CODE_W = "3A2A";
        public const string PR_BUSINESS_ADDRESS_POSTAL_CODE_A = "3A2A";
        public const string PR_POST_OFFICE_BOX = "3A2B";
        public const string PR_POST_OFFICE_BOX_W = "3A2B";
        public const string PR_POST_OFFICE_BOX_A = "3A2B";
        public const string PR_BUSINESS_ADDRESS_POST_OFFICE_BOX = "3A2B";
        public const string PR_BUSINESS_ADDRESS_POST_OFFICE_BOX_W = "3A2B";
        public const string PR_BUSINESS_ADDRESS_POST_OFFICE_BOX_A = "3A2B";
        public const string PR_TELEX_NUMBER = "3A2C";
        public const string PR_TELEX_NUMBER_W = "3A2C";
        public const string PR_TELEX_NUMBER_A = "3A2C";
        public const string PR_ISDN_NUMBER = "3A2D";
        public const string PR_ISDN_NUMBER_W = "3A2D";
        public const string PR_ISDN_NUMBER_A = "3A2D";
        public const string PR_ASSISTANT_TELEPHONE_NUMBER = "3A2E";
        public const string PR_ASSISTANT_TELEPHONE_NUMBER_W = "3A2E";
        public const string PR_ASSISTANT_TELEPHONE_NUMBER_A = "3A2E";
        public const string PR_HOME2_TELEPHONE_NUMBER = "3A2F";
        public const string PR_HOME2_TELEPHONE_NUMBER_W = "3A2F";
        public const string PR_HOME2_TELEPHONE_NUMBER_A = "3A2F";
        public const string PR_ASSISTANT = "3A30";
        public const string PR_ASSISTANT_W = "3A30";
        public const string PR_ASSISTANT_A = "3A30";
        public const string PR_SEND_RICH_INFO = "3A40";
        public const string PR_WEDDING_ANNIVERSARY = "3A41";
        public const string PR_BIRTHDAY = "3A42";
        public const string PR_HOBBIES = "3A43";
        public const string PR_HOBBIES_W = "3A43";
        public const string PR_HOBBIES_A = "3A43";
        public const string PR_MIDDLE_NAME = "3A44";
        public const string PR_MIDDLE_NAME_W = "3A44";
        public const string PR_MIDDLE_NAME_A = "3A44";
        public const string PR_DISPLAY_NAME_PREFIX = "3A45";
        public const string PR_DISPLAY_NAME_PREFIX_W = "3A45";
        public const string PR_DISPLAY_NAME_PREFIX_A = "3A45";
        public const string PR_PROFESSION = "3A46";
        public const string PR_PROFESSION_W = "3A46";
        public const string PR_PROFESSION_A = "3A46";

        public const string PR_PREFERRED_BY_NAME = "3A47";
        public const string PR_PREFERRED_BY_NAME_W = "3A47";
        public const string PR_PREFERRED_BY_NAME_A = "3A47";

        public const string PR_SPOUSE_NAME = "3A48";
        public const string PR_SPOUSE_NAME_W = "3A48";
        public const string PR_SPOUSE_NAME_A = "3A48";

        public const string PR_COMPUTER_NETWORK_NAME = "3A49";
        public const string PR_COMPUTER_NETWORK_NAME_W = "3A49";
        public const string PR_COMPUTER_NETWORK_NAME_A = "3A49";

        public const string PR_CUSTOMER_ID = "3A4A";
        public const string PR_CUSTOMER_ID_W = "3A4A";
        public const string PR_CUSTOMER_ID_A = "3A4A";

        public const string PR_TTYTDD_PHONE_NUMBER = "3A4B";
        public const string PR_TTYTDD_PHONE_NUMBER_W = "3A4B";
        public const string PR_TTYTDD_PHONE_NUMBER_A = "3A4B";

        public const string PR_FTP_SITE = "3A4C";
        public const string PR_FTP_SITE_W = "3A4C";
        public const string PR_FTP_SITE_A = "3A4C";

        public const string PR_GENDER = "3A4D";

        public const string PR_MANAGER_NAME = "3A4E";
        public const string PR_MANAGER_NAME_W = "3A4E";
        public const string PR_MANAGER_NAME_A = "3A4E";

        public const string PR_NICKNAME = "3A4F";
        public const string PR_NICKNAME_W = "3A4F";
        public const string PR_NICKNAME_A = "3A4F";

        public const string PR_PERSONAL_HOME_PAGE = "3A50";
        public const string PR_PERSONAL_HOME_PAGE_W = "3A50";
        public const string PR_PERSONAL_HOME_PAGE_A = "3A50";

        public const string PR_BUSINESS_HOME_PAGE = "3A51";
        public const string PR_BUSINESS_HOME_PAGE_W = "3A51";
        public const string PR_BUSINESS_HOME_PAGE_A = "3A51";

        public const string PR_CONTACT_VERSION = "3A52";
        public const string PR_CONTACT_ENTRYIDS = "3A53";

        public const string PR_CONTACT_ADDRTYPES = "3A54";
        public const string PR_CONTACT_ADDRTYPES_W = "3A54";
        public const string PR_CONTACT_ADDRTYPES_A = "3A54";

        public const string PR_CONTACT_DEFAULT_ADDRESS_INDEX = "3A55";

        public const string PR_CONTACT_EMAIL_ADDRESSES = "3A56";
        public const string PR_CONTACT_EMAIL_ADDRESSES_W = "3A56";
        public const string PR_CONTACT_EMAIL_ADDRESSES_A = "3A56";

        public const string PR_COMPANY_MAIN_PHONE_NUMBER = "3A57";
        public const string PR_COMPANY_MAIN_PHONE_NUMBER_W = "3A57";
        public const string PR_COMPANY_MAIN_PHONE_NUMBER_A = "3A57";

        public const string PR_CHILDRENS_NAMES = "3A58";
        public const string PR_CHILDRENS_NAMES_W = "3A58";
        public const string PR_CHILDRENS_NAMES_A = "3A58";

        public const string PR_HOME_ADDRESS_CITY = "3A59";
        public const string PR_HOME_ADDRESS_CITY_W = "3A59";
        public const string PR_HOME_ADDRESS_CITY_A = "3A59";

        public const string PR_HOME_ADDRESS_COUNTRY = "3A5A";
        public const string PR_HOME_ADDRESS_COUNTRY_W = "3A5A";
        public const string PR_HOME_ADDRESS_COUNTRY_A = "3A5A";

        public const string PR_HOME_ADDRESS_POSTAL_CODE = "3A5B";
        public const string PR_HOME_ADDRESS_POSTAL_CODE_W = "3A5B";
        public const string PR_HOME_ADDRESS_POSTAL_CODE_A = "3A5B";

        public const string PR_HOME_ADDRESS_STATE_OR_PROVINCE = "3A5C";
        public const string PR_HOME_ADDRESS_STATE_OR_PROVINCE_W = "3A5C";
        public const string PR_HOME_ADDRESS_STATE_OR_PROVINCE_A = "3A5C";

        public const string PR_HOME_ADDRESS_STREET = "3A5D";
        public const string PR_HOME_ADDRESS_STREET_W = "3A5D";
        public const string PR_HOME_ADDRESS_STREET_A = "3A5D";

        public const string PR_HOME_ADDRESS_POST_OFFICE_BOX = "3A5E";
        public const string PR_HOME_ADDRESS_POST_OFFICE_BOX_W = "3A5E";
        public const string PR_HOME_ADDRESS_POST_OFFICE_BOX_A = "3A5E";

        public const string PR_OTHER_ADDRESS_CITY = "3A5F";
        public const string PR_OTHER_ADDRESS_CITY_W = "3A5F";
        public const string PR_OTHER_ADDRESS_CITY_A = "3A5F";

        public const string PR_OTHER_ADDRESS_COUNTRY = "3A60";
        public const string PR_OTHER_ADDRESS_COUNTRY_W = "3A60";
        public const string PR_OTHER_ADDRESS_COUNTRY_A = "3A60";

        public const string PR_OTHER_ADDRESS_POSTAL_CODE = "3A61";
        public const string PR_OTHER_ADDRESS_POSTAL_CODE_W = "3A61";
        public const string PR_OTHER_ADDRESS_POSTAL_CODE_A = "3A61";

        public const string PR_OTHER_ADDRESS_STATE_OR_PROVINCE = "3A62";
        public const string PR_OTHER_ADDRESS_STATE_OR_PROVINCE_W = "3A62";
        public const string PR_OTHER_ADDRESS_STATE_OR_PROVINCE_A = "3A62";

        public const string PR_OTHER_ADDRESS_STREET = "3A63";
        public const string PR_OTHER_ADDRESS_STREET_W = "3A63";
        public const string PR_OTHER_ADDRESS_STREET_A = "3A63";

        public const string PR_OTHER_ADDRESS_POST_OFFICE_BOX = "3A64";
        public const string PR_OTHER_ADDRESS_POST_OFFICE_BOX_W = "3A64";
        public const string PR_OTHER_ADDRESS_POST_OFFICE_BOX_A = "3A64";


        /*
         *	Profile section properties
         */

        public const string PR_STORE_PROVIDERS = "3D00";
        public const string PR_AB_PROVIDERS = "3D01";
        public const string PR_TRANSPORT_PROVIDERS = "3D02";

        public const string PR_DEFAULT_PROFILE = "3D04";
        public const string PR_AB_SEARCH_PATH = "3D05";
        public const string PR_AB_DEFAULT_DIR = "3D06";
        public const string PR_AB_DEFAULT_PAB = "3D07";

        public const string PR_FILTERING_HOOKS = "3D08";
        public const string PR_SERVICE_NAME = "3D09";
        public const string PR_SERVICE_NAME_W = "3D09";
        public const string PR_SERVICE_NAME_A = "3D09";
        public const string PR_SERVICE_DLL_NAME = "3D0A";
        public const string PR_SERVICE_DLL_NAME_W = "3D0A";
        public const string PR_SERVICE_DLL_NAME_A = "3D0A";
        public const string PR_SERVICE_ENTRY_NAME = "3D0B";
        public const string PR_SERVICE_UID = "3D0C";
        public const string PR_SERVICE_EXTRA_UIDS = "3D0D";
        public const string PR_SERVICES = "3D0E";
        public const string PR_SERVICE_SUPPORT_FILES = "3D0F";
        public const string PR_SERVICE_SUPPORT_FILES_W = "3D0F";
        public const string PR_SERVICE_SUPPORT_FILES_A = "3D0F";
        public const string PR_SERVICE_DELETE_FILES = "3D10";
        public const string PR_SERVICE_DELETE_FILES_W = "3D10";
        public const string PR_SERVICE_DELETE_FILES_A = "3D10";
        public const string PR_AB_SEARCH_PATH_UPDATE = "3D11";
        public const string PR_PROFILE_NAME = "3D12";
        public const string PR_PROFILE_NAME_A = "3D12";
        public const string PR_PROFILE_NAME_W = "3D12";

        /*
         *	Status object properties
         */

        public const string PR_IDENTITY_DISPLAY = "3E00";
        public const string PR_IDENTITY_DISPLAY_W = "3E00";
        public const string PR_IDENTITY_DISPLAY_A = "3E00";
        public const string PR_IDENTITY_ENTRYID = "3E01";
        public const string PR_RESOURCE_METHODS = "3E02";
        public const string PR_RESOURCE_TYPE = "3E03";
        public const string PR_STATUS_CODE = "3E04";
        public const string PR_IDENTITY_SEARCH_KEY = "3E05";
        public const string PR_OWN_STORE_ENTRYID = "3E06";
        public const string PR_RESOURCE_PATH = "3E07";
        public const string PR_RESOURCE_PATH_W = "3E07";
        public const string PR_RESOURCE_PATH_A = "3E07";
        public const string PR_STATUS_STRING = "3E08";
        public const string PR_STATUS_STRING_W = "3E08";
        public const string PR_STATUS_STRING_A = "3E08";
        public const string PR_X400_DEFERRED_DELIVERY_CANCEL = "3E09";
        public const string PR_HEADER_FOLDER_ENTRYID = "3E0A";
        public const string PR_REMOTE_PROGRESS = "3E0B";
        public const string PR_REMOTE_PROGRESS_TEXT = "3E0C";
        public const string PR_REMOTE_PROGRESS_TEXT_W = "3E0C";
        public const string PR_REMOTE_PROGRESS_TEXT_A = "3E0C";
        public const string PR_REMOTE_VALIDATE_OK = "3E0D";

        /*
         * Display table properties
         */

        public const string PR_CONTROL_FLAGS = "3F00";
        public const string PR_CONTROL_STRUCTURE = "3F01";
        public const string PR_CONTROL_TYPE = "3F02";
        public const string PR_DELTAX = "3F03";
        public const string PR_DELTAY = "3F04";
        public const string PR_XPOS = "3F05";
        public const string PR_YPOS = "3F06";
        public const string PR_CONTROL_ID = "3F07";
        public const string PR_INITIAL_DETAILS_PANE = "3F08";

        /*
         * Secure property id range
         */

        public const string PROP_ID_SECURE_MIN = "67F0";
        public const string PROP_ID_SECURE_MAX = "67FF";

        /* MAPITAGS_H */
        #endregion

        #region Mapi tag types
        /// <summary>
        ///     (Reserved for interface use) type doesn't matter to caller
        /// </summary>
        public const ushort PT_UNSPECIFIED = 0;

        /// <summary>
        ///     NULL property value
        /// </summary>
        public const ushort PT_NULL = 1;

        /// <summary>
        ///     Signed 16-bit value (0x02)
        /// </summary>
        public const ushort PT_I2 = 2;

        /// <summary>
        ///     Signed 32-bit value (0x03)
        /// </summary>
        public const ushort PT_LONG = 3;

        /// <summary>
        ///     4-byte floating point (0x04)
        /// </summary>
        public const ushort PT_R4 = 4;

        /// <summary>
        ///     Floating point double (0x05)
        /// </summary>
        public const ushort PT_DOUBLE = 5;

        /// <summary>
        ///     Signed 64-bit int (decimal w/4 digits right of decimal pt) (0x06)
        /// </summary>
        public const ushort PT_CURRENCY = 6;

        /// <summary>
        ///     Application time (0x07)
        /// </summary>
        public const ushort PT_APPTIME = 7;

        /// <summary>
        ///     32-bit error value (0x0A)
        /// </summary>
        public const ushort PT_ERROR = 10;

        /// <summary>
        ///     16-bit boolean (non-zero true) (0x0B)
        /// </summary>
        public const ushort PT_BOOLEAN = 11;

        /// <summary>
        ///     Embedded object in a property (0x0D)
        /// </summary>
        public const ushort PT_OBJECT = 13;

        /// <summary>
        ///     8-byte signed integer (0x14)
        /// </summary>
        public const ushort PT_I8 = 20;

        /// <summary>
        ///     Null terminated 8-bit character string (0x1E)
        /// </summary>
        public const ushort PT_STRING8 = 30;

        /// <summary>
        ///     Null terminated Unicode string (0x1F)
        /// </summary>
        public const ushort PT_UNICODE = 31;

        /// <summary>
        ///     FILETIME 64-bit int w/ number of 100ns periods since Jan 1,1601 (0x40)
        /// </summary>
        public const ushort PT_SYSTIME = 64;

        /// <summary>
        ///     OLE GUID (0x48)
        /// </summary>
        public const ushort PT_CLSID = 72;

        /// <summary>
        ///     Uninterpreted (counted byte array) (0x102)
        /// </summary>
        public const ushort PT_BINARY = 258;

        /// <summary>
        ///     Multi-view Null terminated 8-bit character string (0x101E)
        /// </summary>
        public const ushort PT_MV_STRING8 = 4126;

        /// <summary>
        ///     Multi-view unicode string (0x101F)
        /// </summary>
        public const ushort PT_MV_UNICODE = 4127;
        #endregion

        #region Stream constants
        /// <summary>
        ///     Storage prefix tag
        /// </summary>
        public const string RecipStoragePrefix = "__recip_version1.0_#";

        /// <summary>
        ///     Prefix that is placed before an attachment tag
        /// </summary>
        public const string AttachStoragePrefix = "__attach_version1.0_#";

        /// <summary>
        ///     Sub storage version 1.0 streams
        /// </summary>
        public const string SubStgVersion1 = "__substg1.0";

        /// <summary>
        ///     Stream that contains the internet E-mail headers
        /// </summary>
        public const string HeaderStreamName = "__substg1.0_007D001F";

        /// <summary>
        ///     The stream that contains all the MAPI properties
        /// </summary>
        public const string PropertiesStream = "__properties_version1.0";

        /// <summary>
        ///     Contains the streams needed to perform named property mapping
        /// </summary>
        public const string NameIdStorage = "__nameid_version1.0";

        /// <summary>
        ///     The GUID stream
        /// </summary>
        public const string GuidStream = "__substg1.0_00020102";

        /// <summary>
        ///     The property stream
        /// </summary>
        public const string EntryStream = "__substg1.0_00030102";

        /// <summary>
        ///     The string stream
        ///     http://msdn.microsoft.com/en-us/library/ee202480%28v=exchg.80%29.aspx
        /// </summary>
        public const string StringStream = "__substg1.0_00040102";

        /// <summary>
        ///     Stream properties begin for header or top
        /// </summary>
        public const int PropertiesStreamHeaderTop = 32;

        // Stream properties begin for embeded
        public const int PropertiesStreamHeaderEmbeded = 24;

        /// <summary>
        ///     Stream properties begin for attachments or recipients
        /// </summary>
        public const int PropertiesStreamHeaderAttachOrRecip = 8;
        #endregion

        #region Attachment type constants
        /// <summary>
        ///     There is no attachment
        /// </summary>
        public const int NO_ATTACHMENT = 0;

        /// <summary>
        ///     The PR_ATTACH_DATA_BIN property contains the attachment data
        /// </summary>
        public const int ATTACH_BY_VALUE = 1;

        /// <summary>
        ///     The PR_ATTACH_PATHNAME or PR_ATTACH_LONG_PATHNAME property contains a fully qualified path 
        ///     identifying the attachment to recipients with access to a common file server
        /// </summary>
        public const int ATTACH_BY_REFERENCE = 2;

        /// <summary>
        ///     The PR_ATTACH_PATHNAME or PR_ATTACH_LONG_PATHNAME property contains a fully qualified path identifying the attachment
        /// </summary>
        public const int ATTACH_BY_REF_RESOLVE = 3;

        /// <summary>
        ///     The PR_ATTACH_PATHNAME or PR_ATTACH_LONG_PATHNAME property contains a fully qualified path identifying the attachment
        /// </summary>
        public const int ATTACH_BY_REF_ONLY = 4;

        /// <summary>
        ///     The attachment is a msg file
        /// </summary>
        public const int ATTACH_EMBEDDED_MSG = 5;

        /// <summary>
        ///     The attachment in an OLE object
        /// </summary>
        public const int ATTACH_OLE = 6;
        #endregion

        #region RecipientType contstants
        /// <summary>
        ///     The recipient is an TO E-mail address
        /// </summary>
        public const int RecipientTo = 1;

        /// <summary>
        ///     The recipient is a CC E-mail address
        /// </summary>
        public const int RecipientCC = 2;

        /// <summary>
        ///     The recipient is a BCC E-mail address
        /// </summary>
        public const int RecipientBCC = 3;

        /// <summary>
        ///     The recipient is a resource (e.g. a room)
        /// </summary>
        public const int RecipientResource = 4;

        /// <summary>
        ///     The recipient is a room (uses PR_RECIPIENT_TYPE_EXE) needs Exchange 2007 or higher
        /// </summary>
        public const int RecipientRoom = 7;
        #endregion

        #region Flag constants
        /// <summary>
        ///     E-mail follow up flag (named property)
        /// </summary>
        public const string FlagRequest = "8530";

        /// <summary>
        ///     Specifies the flag state of the message object; Not present, 1 = Completed, 2 = Flagged.
        ///     Only available from Outlook 2007 and up.
        /// </summary>
        public const string PR_FLAG_STATUS = "1090";

        /// <summary>
        ///     Contains the date when the task was completed. Only filled when <see cref="TaskComplete" /> is true.
        ///     Only available from Outlook 2007 and up.
        /// </summary>
        public const string PR_FLAG_COMPLETE_TIME = "1091";
        #endregion

        #region Task constants
        /// <summary>
        ///     <see cref="TaskStatus" /> of the task (named property)
        /// </summary>
        public const string TaskStatus = "8101";

        /// <summary>
        ///     Start date of the task (named property)
        /// </summary>
        public const string TaskStartDate = "8104";

        /// <summary>
        ///     Due date of the task (named property)
        /// </summary>
        public const string TaskDueDate = "8105";

        /// <summary>
        ///     True when the task is complete (named property)
        /// </summary>
        public const string TaskComplete = "811C";

        /// <summary>
        ///     The actual task effort in minutes (named property)
        /// </summary>
        public const string TaskActualEffort = "8110";

        /// <summary>
        ///     The estimated task effort in minutes (named property)
        /// </summary>
        public const string TaskEstimatedEffort = "8111";

        /// <summary>
        ///     The complete percentage of the task (named property)
        /// </summary>
        public const string PercentComplete = "8102";

        /// <summary>
        ///     The contacts of the task (named property)
        /// </summary>
        public const string Contacts = "853A";

        /// <summary>
        ///     The companies for the task (named property)
        /// </summary>
        public const string Companies = "8539";

        /// <summary>
        ///     The task billing information (named property)
        /// </summary>
        public const string Billing = "8535";

        /// <summary>
        ///     The task mileage information (named property)
        /// </summary>
        public const string Mileage = "8534";

        /// <summary>
        ///     The task owner (named property)
        /// </summary>
        public const string Owner = "811F";
        #endregion

        #region Appointment constants
        /// <summary>
        ///     Appointment location (named property)
        /// </summary>
        public const string Location = "8208";

        /// <summary>
        ///     Appointment reccurence type (named property)
        /// </summary>
        public const string ReccurrenceType = "8231";

        /// <summary>
        ///     Appointment reccurence pattern (named property)
        /// </summary>
        public const string ReccurrencePattern = "8232";

        /// <summary>
        ///     Appointment start time (greenwich time) (named property)
        /// </summary>
        public const string AppointmentStartWhole = "820D";

        /// <summary>
        ///     Appointment end time (greenwich time) (named property)
        /// </summary>
        public const string AppointmentEndWhole = "820E";

        /// <summary>
        ///     Appointment all attendees string (named property)
        /// </summary>
        public const string AppointmentAllAttendees = "8238";

        /// <summary>
        ///     Appointment to attendees string (named property)
        /// </summary>
        public const string AppointmentToAttendees = "823B";

        /// <summary>
        ///     Appointment cc attendees string (named property)
        /// </summary>
        public const string AppointmentCCAttendees = "823C";

        /// <summary>
        ///     The PidLidClientIntent property ([MS-OXPROPS] section 2.58) indicates what actions a user has taken on a Meeting object
        /// </summary>
        public const string PidLidClientIntent = "15";
        #endregion

        #region Contact constants
        /// <summary>
        /// Instant messaging address (named property)    
        /// </summary>
        public const string InstantMessagingAddress = "8062";

        /// <summary>
        /// Home address (named property)
        /// </summary>
        public const string HomeAddress = "801A";

        /// <summary>
        /// Work address (named property)
        /// </summary>
        public const string WorkAddress = "801B";

        /// <summary>
        /// Other address (named property)
        /// </summary>
        public const string OtherAddress = "801C";

        /// <summary>
        /// E-mail 1 address (named property)
        /// </summary>
        public const string Email1EmailAddress = "8083";

        /// <summary>
        /// E-mail 1 display name (named property
        /// </summary>
        public const string Email1DisplayName = "8080";

        /// <summary>
        /// E-mail 2 address (named property)
        /// </summary>
        public const string Email2EmailAddress = "8093";

        /// <summary>
        /// E-mail 2 display name (named property
        /// </summary>
        public const string Email2DisplayName = "8090";

        /// <summary>
        /// E-mail 3 address (named property)
        /// </summary>
        public const string Email3EmailAddress = "80A3";

        /// <summary>
        /// E-mail 3 display name (named property)
        /// </summary>
        public const string Email3DisplayName = "80A0";

        /// <summary>
        /// Html (named property)
        /// </summary>
        public const string Html = "802B";
        #endregion

        /// <summary>
        /// Categories (named property)
        /// </summary>
        public const string Keywords = "Keywords";

        /// <summary>
        /// E-mail address of the sender e.g. PeterPan@neverland.com (named property)
        /// </summary>
        public const string PR_SENDER_EMAIL_ADDRESS_2 = "8012";

        /// <summary>
        /// Can contain the E-mail address of the sender (named property)
        /// </summary>
        public const string InternetAccountName = "8580";

        /// <summary>
        /// Contains the code page that is used in HTML when this is added in binary format
        /// </summary>
        public const string PR_CODE_PAGE_ID = "66C3";

        /// <summary>
        /// Contains the code page that is used in the body
        /// </summary>
        public const string PR_MESSAGE_CODEPAGE = "3FFD";

        /// <summary>
        /// Indicates the code page used for PR_BODY (PidTagBody) or PR_BODY_HTML (PidTagBodyHtml) properties.
        /// </summary>
        public const string PR_INTERNET_CPID = "3FDE";

        /// <summary>
        /// Corresponds to the message ID field as specified in [RFC2822].
        /// </summary>
        public const string PR_INTERNET_MESSAGE_ID = "1035";
        // ReSharper restore InconsistentNaming
    }
} //End Namespace