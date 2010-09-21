using System;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1
{
    public class Database : RD_Assign1.IDatabase
    {
        // Default allocation for database
        private const int kDefaultMaxCapacity = 40000;

        private SortedList<int, DataRecord> Records;
        private Mutex WriteMutex;

        public Database()
        {
            this.Records = new SortedList<int, DataRecord>(kDefaultMaxCapacity);
            this.WriteMutex = new Mutex();
        }

        public void Update(DataRecord record)
        {
            this.WriteMutex.WaitOne();
			this.Records[record.MemberID - 1] = record;
            this.WriteMutex.ReleaseMutex();
        }

        public void Insert(DataRecord record)
        {
			if (this.Records.Count + 1 > kDefaultMaxCapacity)
			{
				throw new OutOfMemoryException();
			}

            this.WriteMutex.WaitOne();
			record.MemberID = this.Records.Count + 1;
			this.Records.Add(record.MemberID - 1, record);
				
			Console.WriteLine("Record Added");
            Console.WriteLine("\tId: {0}\n\tFirstName: {1}\n\tLastName: {2}\n\tDateOfBirth: {3}\n",
				record.MemberID, record.FirstName, record.LastName, record.DateOfBirth);
            this.WriteMutex.ReleaseMutex();
        }

        public DataRecord Find(int MemberID)
        {
            return this.Records[MemberID - 1];
        }
	}
}
