using System;
using System.Text;

namespace RD_SharedCode
{
	public struct DataRecord
	{
		private const char kSeparator = ',';

		public DataRecord(int memberid, string firstname, string lastname, DateTime dateofbirth)
		{
			this.MemberID = memberid;
			this.FirstName = firstname;
			this.LastName = lastname;
			this.DateOfBirth = dateofbirth;
		}

		public static DataRecord FromBytes(byte[] buffer)
		{
			// Extract the elements from the delimited csv
			string data = Shared.ByteArrayToString(buffer, buffer.Length, 0);

			string[] tokens = data.Split(kSeparator);

			Int32 memid = int.Parse(tokens[1]);
			string firstname = tokens[2];
			string lastname = tokens[3];
			DateTime date = DateTime.Parse(tokens[4]);

			return new DataRecord(memid, firstname, lastname, date);
		}

		public byte[] ToBytes()
		{
			// Create a delimited string for network transfer
			StringBuilder builder = new StringBuilder();

			builder.Append(0);
			builder.Append(kSeparator);
			builder.Append(this.MemberID.ToString());
			builder.Append(kSeparator);
			builder.Append(this.FirstName);
			builder.Append(kSeparator);
			builder.Append(this.LastName);
			builder.Append(kSeparator);
			builder.Append(this.DateOfBirth.ToString());

			return Shared.StringToByteArray(builder.ToString());
		}

		public int MemberID;
		public string FirstName;
		public string LastName;
		public DateTime DateOfBirth;
	}
}
