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
	start:
			Console.WriteLine("(DataClient): Starting...");
			try
			{
				for (int i = 1; i <= 40000; i++)
				{
					DatabaseClient client = new DatabaseClient();
					Console.WriteLine("(DataClient): Connecting");
					client.Connect("127.0.0.1", 8021);

					Console.WriteLine("(DataClient): Inserting Record {0}", i);
					DataRecord record = new DataRecord(i, "Butt", "Cheeks", DateTime.Now);
					client.Insert(record);

					Console.WriteLine("(DataClient): Disconnecting");
					client.Disconnect();
				}
			}
			catch (Exception)
			{	
			}
			
			Console.WriteLine("(DataClient): Shutting Down");
			Console.ReadKey();

			goto start;
		}
	}
}
