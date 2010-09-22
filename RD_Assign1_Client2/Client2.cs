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

                for (; ; )
                {
                    int MemberId = 0;
                    string temp;
                    bool success = false;
                    do
                    {
                        success = true;
                        Console.Write("Enter an integer value: ");
                        temp = Console.ReadLine();
                        try
                        {
                            MemberId = Convert.ToInt32(temp);
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


                    DataRecord record = new DataRecord(MemberId, "", "", new DateTime());
                    Console.WriteLine("(FindClient): Checking for Record {0}", MemberId);

                    try
                    {
                        client.Find(ref record);
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

                    Console.WriteLine("MemberID:{0}", record.MemberID);
                    Console.WriteLine("First name:{0}", record.FirstName);
                    Console.WriteLine("Last name:{0}", record.LastName);
                    Console.WriteLine("DOB:{0}", record.DateOfBirth);



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
