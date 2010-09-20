using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RD_SharedCode;

namespace RD_Assign1_Client1
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("(DataClient): Starting...");
			try
			{
				DatabaseClient client = new DatabaseClient();
				client.Connect("127.0.0.1", 8021);

				DataRecord record = new DataRecord(1, "hekar", "khani", DateTime.Now);
				client.Insert(record);
			}
			catch (Exception)
			{	
			}

			Console.WriteLine("(DataClient): Shutting Down");
			Console.ReadKey();
		}
	}
}
