/**
 * @file
 * @author  Hekar Kahni, Samuel Lewis
 * @version 1.0
 *
 * @section DESCRIPTION
 * A client that automatically sends records to be added to the database
 * 
 */

using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1_Client1
{
    class Client1
    {
        /// <summary>
        /// Runs a client that connects to the server and automatically feeds in random records to add to the database
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8021;
            int delayms = 100;

            //Parse Command Line arguments
            int argc = args.GetUpperBound(0);
            if (argc > 2)
            {
                for (int i = 0; i < argc; i++)
                {
                    try
                    {
                        //delay
                        if (args[i] == "-d" && (i + 1) < argc)
                        {
                            delayms = int.Parse(args[i + 1]);
                        }
                        //Port
                        else if (args[i] == "-p" && (i + 1) < argc)
                        {
                            port = int.Parse(args[i + 1]);
                        }
                        //ip address
                        else if (args[i] == "-i" && (i + 1) < argc)
                        {
                            ipAddress = IPAddress.Parse(args[i + 1]);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to accept arguments: {0} {1}:", args[i], args[i + 1]);
                        // Print usage statement...
                        Console.WriteLine("Usage: Client1 [-d delay][-p port][-i ipaddress]");
                    }
                }
            }

            Console.WriteLine("(DataClient): Starting...");
            try
            {
                Random rand = new Random();
                DatabaseClient client = new DatabaseClient();
                Console.WriteLine("(DataClient): Connecting");
                client.Connect(ipAddress.ToString(), port);

                for (int i = 1; i <= 40000; i++)
                {
                    int namemin = kRandomNames.GetLowerBound(0);
                    int namemax = kRandomNames.GetUpperBound(0);

                    string firstname = kRandomNames[rand.Next(namemin, namemax)];
                    string lastname = kRandomNames[rand.Next(namemin, namemax)];

                    string datestr = rand.Next(int.MaxValue / 4).ToString() + rand.Next(int.MaxValue / 4).ToString();
                    DateTime date = DateTime.FromFileTime(long.Parse(datestr));

                    Console.WriteLine("(DataClient): Inserting Record {0}", i);
                    Console.WriteLine("(DataClient):\tFirstName: {0}\tLastName: {1}\tDate: {2}", firstname, lastname, date);

                    try
                    {
                        DataRecord record = new DataRecord(0, firstname, lastname, date);
                        client.Insert(record);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("(DataClient): Error, bad response");
                    }
                    catch (OutOfMemoryException)
                    {
                        Console.WriteLine("(DataClient): Database is full");
                    }
                    catch (DatabaseException)
                    {
                        Console.WriteLine("(DataClient): Database Error");
                    }

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

        /// <summary>
        /// List of random names used to create records
        /// </summary>
        public static string[] kRandomNames = 
        {
            "Jim",
            "Bob",
            "John",
            "Abbot",
            "Abbotson",
            "Addison",
            "Adie",
            "Airlie",
            "Airth",
            "Aitcheson",
            "Aitken",
            "Alexander",
            "Alistair",
            "Allan",
            "Allanson",
            "Allison",
            "Armstrong",
            "Arrol",
            "Arthur",
            "Askey",
            "Austin",
            "Ayson",
            "Bain",
            "Balloch",
            "Barrie",
            "Barron",
            "Bartholomew",
            "Bean",
            "Beath",
            "Beattie",
            "Begg",
            "Berry",
            "Beton",
            "Binnie",
            "Black",
            "Blake",
            "Bonar",
            "Bontein",
            "Bontine",
            "Bowers",
            "Bowie",
            "Bowmaker",
            "Bowman",
            "Boyes",
            "Brebner",
            "Brewer",
            "Brieve",
            "Brodie",
            "Brown",
            "Bruce",
            "Bryce",
            "Bryde",
            "Buchanan",
            "Buntain",
            "Bunten",
            "Buntine",
            "Burdon",
            "Burk",
            "Burnes",
            "Burns",
            "Caddell",
            "Caird",
            "Cameron",
            "Campbell",
            "Cariston",
            "Carlyle",
            "Carr",
            "Carrick",
            "Carson",
            "Carstarphen",
            "Cassels",
            "Cattanach",
            "Caw",
            "Cessford",
            "Charles",
            "Christie",
            "Clanachan",
            "Clark",
            "Clarke",
            "Clarkson",
            "Clement",
            "Clerk",
            "Cluny",
            "Clyne",
            "Cobb",
            "Collier",
            "Colman",
            "Colquhoun",
            "Colson",
            "Colyear",
            "Combie",
            "Comine",
            "Comrie",
            "Conacher",
            "Connall",
            "Connell",
            "Conochie",
            "Constable",
            "Cook",
            "Corbet",
            "Cormack",
            "Corstorphine",
            "Coull",
            "Coulson",
            "Cousland",
            "Coutts",
            "Cowan",
            "Cowie",
            "Crerar",
            "Crombie",
            "Crookshanks",
            "Cruickshanks",
            "Crum",
            "Cullen",
            "Cumin",
            "Cumming",
            "Dallas",
            "Daniels",
            "Davidson",
            "Davie",
            "Davis",
            "Davison",
            "Dawson",
            "Day",
            "Dean",
            "Denoon",
            "Denune",
            "Deuchar",
            "Dickson",
            "Dingwall",
            "Dinnes",
            "Dis",
            "Dixon",
            "Dobbie",
            "Dobson",
            "Dochart",
            "Docharty",
            "Doig",
            "Doles",
            "Donachie",
            "Donaldson",
            "Donillson",
            "Donleavy",
            "Donlevy",
            "Donnellson",
            "Douglas",
            "Dove",
            "Dow",
            "Dowe",
            "Downie",
            "Drummond",
            "Drysdale",
            "Duff",
            "Duffie",
            "Dufus",
            "Duffy",
            "Duilach",
            "Duncanson",
            "Dunnachie",
            "Duthie",
            "Dyce",
            "Eadie",
            "Eaton",
            "Edie",
            "Elder",
            "Ennis",
            "Enrick",
            "Esson",
            "Ewing",
            "Fair",
            "Fairbairn",
            "Farquharson",
            "Federith",
            "Forbes",
            "Fraser",
            "Gordon",
            "Graham",
            "Gunn",
            "Hay",
            "Home",
            "Innes",
            "Keith",
            "Kennedy",
            "Kerr",
            "Lamont",
            "Lindsay",
            "Macalister",
            "Macarthur",
            "Macbain",
            "Macdonald",
            "Macdonnell",
            "Macdougall",
            "Macduff",
            "Macfarlane",
            "Macfie",
            "Macgregor",
            "Mackenzie",
            "Mackintosh",
            "Maclachlan",
            "Maclean",
            "Macleod",
            "Macnab",
            "Macpherson",
            "Macthomas",
            "Morrison",
            "Munro",
            "Ogilvy",
            "Robertson",
            "Rose",
            "Ross",
            "Sinclair",
            "Skene",
            "Stewart",
            "Sutherland"
        };
    }
}
