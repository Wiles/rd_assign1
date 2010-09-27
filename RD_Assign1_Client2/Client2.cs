/**
 * @file
 * @author  Hekar Kahni, Samuel Lewis
 * @version 1.0
 *
 * @section DESCRIPTION
 * A client that allows the user to search and update the database
 *
 */

using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1_Client1
{
    class Program
    {
        private const int kQuitValue = -1;

        /// <summary>
        /// Runs a client that allows the user to search the database for records and update found records
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8021;

            //Parse command line arguments
            int argc = args.GetUpperBound(0);
            if (argc > 2)
            {
                for (int i = 0; i < argc; i++)
                {
                    try
                    {
                        //Port
                        if (args[i] == "-p" && (i + 1) < argc)
                        {
                            port = int.Parse(args[i + 1]);
                        }
                        //Ip address
                        else if (args[i] == "-i" && (i + 1) < argc)
                        {
                            ipAddress = IPAddress.Parse(args[i + 1]);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to accept arguments: {0} {1}:", args[i], args[i + 1]);
                        // Print usage statement...
                        Console.WriteLine("Usage: Client1 [-p port] [-i ipaddress]");
                    }
                }
            }

            Console.WriteLine("(FindClient): Starting...");
            try
            {
                DatabaseClient client = new DatabaseClient();
                Console.WriteLine("(FindClient): Connecting");
                client.Connect(ipAddress.ToString(), port);

                //Find loop
                while (true)
                {
                    int memid = 0;
                    string memid_input;
                    bool success = false;

                    do
                    {
                        success = true;
                        Console.Write("Enter an integer value ({0} to quit): ", kQuitValue);
                        memid_input = Console.ReadLine();

                        try
                        {
                            memid = int.Parse(memid_input);
                        }
                        catch (FormatException)
                        {
                            success = false;
                        }
                        catch (OverflowException)
                        {
                            success = false;
                        }
                    }
                    while (!success);

                    if (memid == kQuitValue)
                    {
                        break;
                    }

                    try
                    {
                        //Query server for record
                        Console.WriteLine("(FindClient): Checking for Record {0}", memid);
                        DataRecord record = client.Find(memid);

                        Console.WriteLine("MemberID:{0}", record.MemberID);
                        Console.WriteLine("First name:{0}", record.FirstName);
                        Console.WriteLine("Last name:{0}", record.LastName);
                        Console.WriteLine("DOB:{0}", record.DateOfBirth);

                        Console.WriteLine("Update record?(y/n):");
                        string answer = Console.ReadLine();

                        //Update record
                        if (answer == "y" || answer == "Y")
                        {
                            try
                            {
                                Console.WriteLine("New first name(Blank for original):");
                                string newFirstName = Console.ReadLine();
                                Console.WriteLine("New lastfirst name(Blank for original):");
                                string newLastName = Console.ReadLine();
                                Console.WriteLine("New DOB( MM/DD/YYYY HH:MM:SS )(Blank for original):");
                                string newDOB = Console.ReadLine();
                                if (newDOB != "")
                                {
                                    record.DateOfBirth = DateTime.Parse(newDOB);
                                }
                                if (newFirstName != "")
                                {
                                    record.FirstName = newFirstName;
                                }
                                if (newLastName != "")
                                {
                                    record.LastName = newLastName;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write("(FindClient): Error, ");
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                            client.Update(record);
                        }
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("(FindClient): Error, bad response");
                        continue;
                    }
                    catch (OutOfMemoryException)
                    {
                        Console.WriteLine("(FindClient): Database is full");
                        continue;
                    }
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("(FindClient): Record with MemberID {0} does not exist", memid);
                        continue;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("(FindClient): Failure to recieve valid response");
                        continue;
                    }

                    catch (DatabaseException)
                    {
                        Console.WriteLine("(DataClient): Database Error");
                    }
                }

                Console.WriteLine("(FindClient): Disconnecting");
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("(FindClient): Shutting Down");
            Console.ReadKey();
        }
    }
}
