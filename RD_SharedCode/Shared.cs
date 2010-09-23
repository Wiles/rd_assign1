using System;
using System.Collections.Generic;
using System.Text;

namespace RD_SharedCode
{
	// Maximum 255 entries
	public enum DatabaseMessage
	{
		Success = 0,

		Error_InvalidArgs,
		Error_ItemNotFound,
		Error_OutOfMemory,
        Error_DatabaseError,

		Comm_Insert,
		Comm_Update,
		Comm_Find_MemberID,

		// Return a finding to the client
		Client_Found_Record,

		// Tell the server to close our connection
		Server_Close
	}

    public class DatabaseException : Exception
    {
        public DatabaseException() : base("Database is corrupt") { }
    };

	public class Shared
	{
		public const int kMaxNetBuffer = 2048;

		public static Int32 ByteArrayToInt32(byte[] array, int offset = 0)
		{
			Int32 converted =
						((int)array[0 + offset]) |
						(8 << (int)array[1 + offset]) |
						(16 << (int)array[2 + offset]) |
						(24 << (int)array[3 + offset]);

			return converted;
		}

		public static byte[] Int32ToByteArray(Int32 data)
		{
			byte[] converted = new byte[4];

            converted[0] = (byte)(data &            0x000000FF);
			converted[1] = (byte)((data >> 8) &     0x0000FF00);
			converted[2] = (byte)((data >> 16) &    0x00FF0000);
			converted[3] = (byte)((data >> 24) &    0xFF000000);

			return converted;
		}

		static UTF8Encoding utf8encoder = new UTF8Encoding();
		public static string ByteArrayToString(byte[] buffer, int size, int offset)
		{
			return utf8encoder.GetString(buffer, offset, size);
		}

		public static byte[] StringToByteArray(string data)
		{
			return utf8encoder.GetBytes(data.ToCharArray());
		}
	}
}
