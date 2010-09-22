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

        public static DataRecord FromString(string data)
        {
            // Only take up to the null terminator
            int nullindex = data.IndexOf('\0');
            if (nullindex > 0)
            {                        
                data = data.Substring(0, nullindex);
            }

            string[] tokens = data.Split(kSeparator);

            Int32 memid = int.Parse(tokens[1]);
            string firstname = tokens[2];
            string lastname = tokens[3];
            DateTime date = DateTime.Parse(tokens[4]);

            return new DataRecord(memid, firstname, lastname, date);
        }

		public static DataRecord FromBytes(byte[] buffer)
		{
			// Extract the elements from the delimited csv
			string data = Shared.ByteArrayToString(buffer, buffer.Length, 0);

			// Only take up to the null terminator
			data = data.Substring(0, data.IndexOf('\0'));

            return DataRecord.FromString(data);
		}

        public override string ToString()
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
            builder.Append(kSeparator);

            return builder.ToString();
        }

		public byte[] ToBytes()
		{
            return Shared.StringToByteArray(ToString());
		}

		public int MemberID;
		public string FirstName;
		public string LastName;
		public DateTime DateOfBirth;
	}
}
