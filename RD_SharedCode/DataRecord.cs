using System;
using System.Text;

namespace RD_SharedCode
{
	public struct DataRecord
	{
		public const int kMemberIDLen = 4;
		// Fixed size for the First and Last name network buffers
		public const int kFirstNameMax = 64;
		public const int kLastNameMax = 64;
		public const int kDateOfBirthLen = 48;

		public const int kFullRecordLen = kMemberIDLen + kFirstNameMax + kLastNameMax + kDateOfBirthLen;

		// Offsets to binary data
		// Leave room for the message code in the data record packets
		const int kMemberIDOffset = 1;
		const int kFirstNameOffset = kMemberIDOffset + sizeof(Int32);
		const int kLastNameOffset = kFirstNameOffset + DataRecord.kFirstNameMax;
		const int kDateOfBirthOffset = kLastNameOffset + DataRecord.kLastNameMax;

		public DataRecord(int memberid, string firstname, string lastname, DateTime dateofbirth)
		{
			this.MemberID = memberid;
			this.FirstName = firstname;
			this.LastName = lastname;
			this.DateOfBirth = dateofbirth;
		}

		public static DataRecord FromBytes(byte[] buffer)
		{
			Int32 memid = Shared.ByteArrayToInt32(buffer, kMemberIDOffset);
			string firstname = Shared.ByteArrayToString(buffer, DataRecord.kFirstNameMax, kFirstNameOffset).TrimEnd();
			string lastname = Shared.ByteArrayToString(buffer, DataRecord.kLastNameMax, kLastNameOffset).TrimEnd();
			DateTime date = DateTime.Parse(Shared.ByteArrayToString(buffer, kFullRecordLen - kDateOfBirthLen, kDateOfBirthOffset));

			return new DataRecord(memid, firstname, lastname, date);
		}

		public static byte[] ToBytes(DataRecord record)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append(0);
			builder.Append(record.MemberID.ToString().PadRight(kMemberIDLen));
			builder.Append(record.FirstName.PadRight(kFirstNameMax));
			builder.Append(record.LastName.PadRight(kLastNameMax));
			builder.Append(record.DateOfBirth.ToString().PadRight(kDateOfBirthLen));

			return Shared.StringToByteArray(builder.ToString());
		}

		public int MemberID;
		public string FirstName;
		public string LastName;
		public DateTime DateOfBirth;
	}
}
