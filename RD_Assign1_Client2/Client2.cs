using System;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1_Client1
{
    class Program
    {
        static void Main(string[] args)
        {
            int delayms = 1500;

            Console.WriteLine("(FindClient): Starting...");
            try
            {
                DatabaseClient client = new DatabaseClient();
                Console.WriteLine("(FindClient): Connecting");
                client.Connect("127.0.0.1", 8021);

                while(true)
                {
                    int memid = 0;
                    string memid_input;
                    bool success = false;

                    do
                    {
                        success = true;
                        Console.Write("Enter an integer value (-1 to quit): ");
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
                    while ( !success );

                    if (memid == -1)
                    {
                        break;
                    }

                    try
                    {
                        Console.WriteLine("(FindClient): Checking for Record {0}", memid);
                        DataRecord record = client.Find(memid);

                        Console.WriteLine("MemberID:{0}", record.MemberID);
                        Console.WriteLine("First name:{0}", record.FirstName);
                        Console.WriteLine("Last name:{0}", record.LastName);
                        Console.WriteLine("DOB:{0}", record.DateOfBirth);
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

                    Thread.Sleep(delayms);
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
