
using System;
using System.Collections;
using System.Threading;

namespace HekarisAwesome
{


	public class Database:IDisposable
	{

		public Database ()
		{
		}
		
		public void Update( int MemberID, string FirstName, string LastName, DateTime DateOfBirth ){
		
		}
		
		public void Insert( int MemberID, string FirstName, string LastName, DateTime DateOfBirth ){
		}
		
		public DBRecord Find( int MemberID ){
		}
		
		private SortedList< DBRecord > _Records;
		private Mutex _WriteMutex;
		
	}
}
