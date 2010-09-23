/**
 * @file
 * @author  Hekar Kahni, Samuel Lewis
 * @version 1.0
 *
 * @section DESCRIPTION
 * 
 */

using System;
using System.Xml;
using System.Collections.Generic;
using RD_SharedCode;

namespace RD_Assign1
{
    class Program
    {
        /// <summary>
        /// Runs the server allowing clients to interact with the database
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("(DataServer): Starting Up...");
            try
            {
                Database database = new Database("xmlRecords.xml");
                Console.WriteLine("(DataServer): Database Initialized.");
                Console.WriteLine("(DataServer): Database Server Starting...");
                DatabaseServer server = new DatabaseServer(database);
                Console.WriteLine("(DataServer): Database Server Started.");
                Console.WriteLine("(DataServer): Binding.");
                server.Bind();
                Console.WriteLine("(DataServer): Entering Message Loop.");
                server.ServerLoop();
                Console.WriteLine("(DataServer): Exiting Message Loop.");
            }
            catch (XmlException)
            {
                Console.WriteLine("(DataServer): Database Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message: {0}", ex.Message);
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
