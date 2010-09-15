using System;

namespace RD_Assign1
{
    public struct DataRecord
    {
        public DataRecord(int memberid, string firstname, string lastname, DateTime dateofbirth)
        {
            this.MemberID = memberid;
            this.FirstName = firstname;
            this.LastName = lastname;
            this.DateOfBirth = dateofbirth;
        }

        public int MemberID;
        public string FirstName;
        public string LastName;
        public DateTime DateOfBirth;
    }
}
