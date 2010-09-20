using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RD_SharedCode;

namespace RD_Assign1
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("(DataServer): Starting Up...");
			try
			{
				Database database = new Database();
				Console.WriteLine("(DataServer): Database Initialized.");
				Console.WriteLine("(DataServer): Database Server Starting...");
				DatabaseServer server = new DatabaseServer(database);
				Console.WriteLine("(DataServer): Database Server Started.");
				Console.WriteLine("(DataServer): Binding.");
				server.Bind();
				Console.WriteLine("(DataServer): Entering Message Loop.");
				server.MessageLoop();
				Console.WriteLine("(DataServer): Exiting Message Loop.");
			}
			catch (Exception ex)
			{
				// Crash to prevent Data Corruption
				Console.WriteLine(ex.StackTrace);
			}
			finally
			{
				Console.WriteLine("(DataServer): Shutting Down");
				Console.ReadKey();
			}
        }
    }
}
