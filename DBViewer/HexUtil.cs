using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBViewer
{
    class HexUtil
    {

        private static readonly String NEWLINE = "\n";
        private static readonly char[] BYTE2CHAR = new char[256];
        private static readonly char[] HEXDUMP_TABLE = new char[256 * 4];
        private static readonly String[] HEXPADDING = new String[16];
        private static readonly String[] HEXDUMP_ROWPREFIXES = new String[65536 >> 4];
        private static readonly String[] BYTE2HEX = new String[256];
        private static readonly String[] BYTEPADDING = new String[16];
        private static readonly String[] BYTE2HEX_PAD = new String[256];

        static HexUtil()
        {
            char[] DIGITS = "0123456789abcdef".ToCharArray();
            for (int i = 0; i < 256; i++)
            {
                HEXDUMP_TABLE[i << 1] = DIGITS[i >> 4 & 0x0F];
                HEXDUMP_TABLE[(i << 1) + 1] = DIGITS[i & 0x0F];
            }

            // Generate the lookup table that converts a byte into a 2-digit hexadecimal integer.
            for (int i = 0; i < BYTE2HEX_PAD.Length; i++)
            {
                BYTE2HEX_PAD[i] = i.ToString("X2");
            }

            // Generate the lookup table for hex dump paddings
            for (int i = 0; i < HEXPADDING.Length; i++)
            {
                int padding = HEXPADDING.Length - i;
                StringBuilder buf = new StringBuilder(padding * 3);
                for (int j = 0; j < padding; j++)
                {
                    buf.Append("   ");
                }
                HEXPADDING[i] = buf.ToString();
            }

            // Generate the lookup table for the start-offset header in each row (up to 64KiB).
            for (int i = 0; i < HEXDUMP_ROWPREFIXES.Length; i++)
            {
                StringBuilder buf = new StringBuilder(12);
                buf.Append(NEWLINE);
                buf.Append((i << 4 & 0xFFFFFFFFL | 0x100000000L).ToString("X2"));
                buf[buf.Length - 9] = '|';
                buf.Append('|');
                HEXDUMP_ROWPREFIXES[i] = buf.ToString();
            }

            // Generate the lookup table for byte-to-hex-dump conversion
            for (int i = 0; i < BYTE2HEX.Length; i++)
            {
                BYTE2HEX[i] = ' ' + byteToHexStringPadded(i);
            }

            // Generate the lookup table for byte dump paddings
            for (int i = 0; i < BYTEPADDING.Length; i++)
            {
                int padding = BYTEPADDING.Length - i;
                StringBuilder buf = new StringBuilder(padding);
                for (int j = 0; j < padding; j++)
                {
                    buf.Append(' ');
                }
                BYTEPADDING[i] = buf.ToString();
            }

            // Generate the lookup table for byte-to-char conversion
            for (int i = 0; i < BYTE2CHAR.Length; i++)
            {
                if (i <= 0x1f || i >= 0x7f)
                {
                    BYTE2CHAR[i] = '.';
                }
                else
                {
                    BYTE2CHAR[i] = (char)i;
                }
            }
        }

        public static String prettyHexDump(byte[] buffer)
        {
            return prettyHexDump(buffer, 0, buffer.Length);
        }

        private static String prettyHexDump(byte[] buffer, int offset, int length)
        {
            if (length == 0)
            {
                return "";
            }
            else
            {
                int rows = length / 16 + ((length & 15) == 0 ? 0 : 1) + 4;
                StringBuilder buf = new StringBuilder(rows * 80);
                appendPrettyHexDump(buf, buffer, offset, length);
                return buf.ToString();
            }
        }

        private static void appendPrettyHexDump(StringBuilder dump, byte[] buf, int offset, int length)
        {
            if (length == 0)
            {
                return;
            }
            dump.Append(
                              "         +-------------------------------------------------+" +
                    NEWLINE + "         |  0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f |" +
                    NEWLINE + "+--------+-------------------------------------------------+----------------+");

            int fullRows = length >> 4;
            int remainder = length & 0xF;

            // Dump the rows which have 16 bytes.
            for (int row = 0; row < fullRows; row++)
            {
                int rowStartIndex = (row << 4) + offset;

                // Per-row prefix.
                appendHexDumpRowPrefix(dump, row, rowStartIndex);

                // Hex dump
                int rowEndIndex = rowStartIndex + 16;
                for (int j = rowStartIndex; j < rowEndIndex; j++)
                {
                    dump.Append(BYTE2HEX[buf[j] & 0xff]);
                }
                dump.Append(" |");

                // ASCII dump
                for (int j = rowStartIndex; j < rowEndIndex; j++)
                {
                    dump.Append(BYTE2CHAR[buf[j] & 0xff]);
                }
                dump.Append('|');
            }

            // Dump the last row which has less than 16 bytes.
            if (remainder != 0)
            {
                int rowStartIndex = (fullRows << 4) + offset;
                appendHexDumpRowPrefix(dump, fullRows, rowStartIndex);

                // Hex dump
                int rowEndIndex = rowStartIndex + remainder;
                for (int j = rowStartIndex; j < rowEndIndex; j++)
                {
                    dump.Append(BYTE2HEX[buf[j] & 0xff]);
                }
                dump.Append(HEXPADDING[remainder]);
                dump.Append(" |");

                // Ascii dump
                for (int j = rowStartIndex; j < rowEndIndex; j++)
                {
                    dump.Append(BYTE2CHAR[buf[j] & 0xff]);
                }
                dump.Append(BYTEPADDING[remainder]);
                dump.Append('|');
            }

            dump.Append(NEWLINE +
                        "+--------+-------------------------------------------------+----------------+");
        }

        private static void appendHexDumpRowPrefix(StringBuilder dump, int row, int rowStartIndex)
        {
            if (row < HEXDUMP_ROWPREFIXES.Length)
            {
                dump.Append(HEXDUMP_ROWPREFIXES[row]);
            }
            else
            {
                dump.Append(NEWLINE);
                dump.Append((rowStartIndex & 0xFFFFFFFFL | 0x100000000L).ToString("X2"));
                dump[dump.Length - 9] = '|';
                dump.Append('|');
            }
        }

        private static String byteToHexStringPadded(int value)
        {
            return BYTE2HEX_PAD[value & 0xff];
        }
    }
}
