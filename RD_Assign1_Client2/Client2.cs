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
                        Console.Write("Enter ID an integer value: ");
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
                    catch (KeyNotFoundException)
                    {
                        Console.WriteLine("(FindClient): Record not found");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    Console.WriteLine("MemberID:{0}", record.MemberID);
                    Console.WriteLine("First name:{0}", record.FirstName);
                    Console.WriteLine("Last name:{0}", record.LastName);
                    Console.WriteLine("DOB:{0}", record.DateOfBirth);

                    Console.WriteLine("Would you like to update this record?(y/n):");

                    string answer = Console.ReadLine();
                    try
                    {
                        if (answer == "y" || answer == "Y")
                        {
                            Console.WriteLine("Enter First Name( Blank for original):");
                            string newFirstName = Console.ReadLine();
                            Console.WriteLine("Enter Last Name( Blank for original):");
                            string newLastName = Console.ReadLine();
                            Console.WriteLine("Enter DOB( MM/DD/YYYY HH:MM:SS )(Blank for original):");
                            string sNewDOB = Console.ReadLine();

                            if (sNewDOB != "")
                            {

                                record.DateOfBirth = DateTime.Parse(sNewDOB);
                            }

                            if (newFirstName != "")
                            {
                                record.FirstName = newFirstName;
                            }
                            if (newLastName != "")
                            {
                                record.LastName = newLastName;
                            }
                            try
                            {
                                client.Update(record);
                            }
                            catch (ArgumentException)
                            {
                                Console.WriteLine("(DataClient): Error, bad response");
                            }
                            catch (OutOfMemoryException)
                            {
                                Console.WriteLine("(DataClient): Database is full");
                            }


                        }
                    }
                    catch (Exception)
                    {
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
