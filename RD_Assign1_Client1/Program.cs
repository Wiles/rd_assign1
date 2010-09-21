using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1_Client1
{
	class Program
	{
		static void Main(string[] args)
		{
			int delayms = 0;

			int argc = args.GetUpperBound(0);
			if (argc > 2)
			{
				for (int i = 0; i < argc; i++)
				{
					if (args[i] == "-d" && (i + 1) < argc)
					{
						try
						{
							delayms = int.Parse(args[i + 1]);
						}
						catch (Exception)
						{
							Console.WriteLine("Failed to accept arguments: {0} {1}:", args[i], args[i + 1]);
							// Print usage statement...
						}
					}
				}
			}

			Console.WriteLine("(DataClient): Starting...");
			try
			{
				DatabaseClient client = new DatabaseClient();
				Console.WriteLine("(DataClient): Connecting");
				client.Connect("127.0.0.1", 8021);

				for (int i = 1; i <= 40000; i++)
				{
					Console.WriteLine("(DataClient): Inserting Record {0}", i);
					DataRecord record = new DataRecord(i, "Butt", "Cheeks", DateTime.Now);
					client.Insert(record);

					Thread.Sleep(delayms);
				}

				Console.WriteLine("(DataClient): Disconnecting");
				client.Disconnect();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine(ex.Message);
			}
			
			Console.WriteLine("(DataClient): Shutting Down");
			Console.ReadKey();
		}
	}
}
