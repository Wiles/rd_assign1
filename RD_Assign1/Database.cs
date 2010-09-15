using System;
using System.Collections.Generic;
using System.Threading;

namespace RD_Assign1
{
    public class Database : IDisposable
    {
        // Default allocation for database
        private const int kDefaultCapacity = 40000;

        private SortedList<int, DataRecord> Records;
        private Mutex WriteMutex;

        public Database()
        {
            this.Records = new SortedList<int, DataRecord>(kDefaultCapacity);
            this.WriteMutex = new Mutex();
        }

        public void Update(int MemberID, string FirstName, string LastName, DateTime DateOfBirth)
        {
            this.WriteMutex.WaitOne();
            try
            {
                var data = new DataRecord(MemberID, FirstName, LastName, DateOfBirth);
                this.Records[MemberID] = data;
            }
            catch (KeyNotFoundException ex)
            {
            }
            finally
            {
                this.WriteMutex.ReleaseMutex();
            }
        }

        public void Insert(int MemberID, string FirstName, string LastName, DateTime DateOfBirth)
        {
            this.WriteMutex.WaitOne();
            try
            {
                var data = new DataRecord(MemberID, FirstName, LastName, DateOfBirth);
                this.Records.Add(MemberID, data);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.WriteMutex.ReleaseMutex();
            }
        }

        public DataRecord Find(int MemberID)
        {
            return this.Records[MemberID];
        }
    }
}
