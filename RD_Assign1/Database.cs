

using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1
{
    /// <summary>
    /// Used to store DataRecords that are used by the server and client
    /// </summary>
    public class Database
    {
        // Default allocation for database
        private const int kDefaultMaxCapacity = 40000;
        private Mutex WriteMutex;
        private string xmlFilePath;
        private int CurrentMemberID;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFile">Filepath to the xml file to be used to store records</param>
        public Database(string xmlFile)
        {
            this.WriteMutex = new Mutex();
            xmlFilePath = xmlFile;
            this.XMLFindHighestMemberID();
        }

        /// <summary>
        /// Updates the record that has the same id as the record given.
        /// If that record does not exist in the database an KeyNotFoundException will be thrown
        /// </summary>
        /// <param name="record">The record to the updated with the new values in it</param>
        public void Update(DataRecord record)
        {
            this.WriteMutex.WaitOne();
            if (!File.Exists(this.xmlFilePath))
            {
                throw new KeyNotFoundException();
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(this.xmlFilePath);

            string searchQuery = "/Members/Member[@ID='" + record.MemberID.ToString() + "']";

            XmlNodeList xmlNodes = xmlDoc.SelectNodes(searchQuery);

            if (xmlNodes.Count != 1)
            {
                throw new KeyNotFoundException();
            }

            xmlNodes[0].Attributes[0].Value = record.MemberID.ToString();
            xmlNodes[0].ChildNodes[0].InnerText = record.FirstName;
            xmlNodes[0].ChildNodes[1].InnerText = record.LastName;
            xmlNodes[0].ChildNodes[2].InnerText = record.DateOfBirth.ToString();
            xmlDoc.Save(this.xmlFilePath);
            this.WriteMutex.ReleaseMutex();
        }

        /// <summary>
        /// Inserts a new record into the database giving it a MemberID that is 1 over the highest currently in the database
        /// </summary>
        /// <param name="record">Record to be added</param>
        public void Insert(DataRecord record)
        {
            if (this.CurrentMemberID + 1 > kDefaultMaxCapacity)
            {
                throw new OutOfMemoryException();
            }

            this.WriteMutex.WaitOne();
            ++this.CurrentMemberID;
            record.MemberID = CurrentMemberID;
            this.XMLAppendRecord(record);

            Console.WriteLine("Record Added");
            Console.WriteLine("\tId: {0}\n\tFirstName: {1}\n\tLastName: {2}\n\tDateOfBirth: {3}\n",
                record.MemberID, record.FirstName, record.LastName, record.DateOfBirth);
            this.WriteMutex.ReleaseMutex();
        }

        /// <summary>
        /// Attempts to find an existing record in the database
        /// </summary>
        /// <param name="MemberID">ID of record to search for</param>
        /// <returns>Returns the found record</returns>
        public DataRecord Find(int MemberID)
        {
            DataRecord record = new DataRecord();
            record.MemberID = MemberID;

            this.WriteMutex.WaitOne();
            this.XMLFindRecord(ref record);
            this.WriteMutex.ReleaseMutex();

            return record;
        }

        /// <summary>
        /// Adds a record to the end of the XML document
        /// </summary>
        /// <param name="record">Record to add</param>
        private void XMLAppendRecord(DataRecord record)
        {
            if (!File.Exists(this.xmlFilePath))
            {
                this.XMLCreateFile();
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(this.xmlFilePath);

            XmlElement newElement = xmlDoc.CreateElement("Member");

            XmlAttribute newAttribut = xmlDoc.CreateAttribute("ID");
            newAttribut.Value = record.MemberID.ToString();
            newElement.SetAttributeNode(newAttribut);

            XmlElement elementFirstName = xmlDoc.CreateElement("FirstName");
            elementFirstName.InnerText = record.FirstName;
            newElement.AppendChild(elementFirstName);

            XmlElement elementLastName = xmlDoc.CreateElement("LastName");
            elementLastName.InnerText = record.LastName;
            newElement.AppendChild(elementLastName);

            XmlElement elementDOB = xmlDoc.CreateElement("DateOfBirth");
            elementDOB.InnerText = record.DateOfBirth.ToString();
            newElement.AppendChild(elementDOB);

            xmlDoc.DocumentElement.InsertAfter(newElement, xmlDoc.DocumentElement.LastChild);

            xmlDoc.Save(this.xmlFilePath);

        }

        /// <summary>
        /// Finds the record in the XML file
        /// </summary>
        /// <param name="record">Contains ID of record to find, other information will be entered from record on file</param>
        private void XMLFindRecord(ref DataRecord record)
        {
            if (!File.Exists(this.xmlFilePath))
            {
                throw new KeyNotFoundException();
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(this.xmlFilePath);

            string searchQuery = "/Members/Member[@ID='" + record.MemberID.ToString() + "']";

            XmlNodeList xmlNodes = xmlDoc.SelectNodes(searchQuery);

            if (xmlNodes.Count != 1)
            {
                throw new KeyNotFoundException();
            }

            XmlElement xmlRecord = (XmlElement)xmlNodes[0];
            record.FirstName = xmlRecord.GetElementsByTagName("FirstName")[0].InnerText;
            record.LastName = xmlRecord.GetElementsByTagName("LastName")[0].InnerText;
            record.DateOfBirth = DateTime.Parse(xmlRecord.GetElementsByTagName("DateOfBirth")[0].InnerText);

        }

        /// <summary>
        /// Sets up the XML file if it does not already exist
        /// </summary>
        private void XMLCreateFile()
        {
            XmlTextWriter newDoc = new XmlTextWriter(this.xmlFilePath, null);

            newDoc.WriteStartDocument();
            newDoc.WriteStartElement("Members");
            newDoc.WriteEndElement();
            newDoc.Close();
        }

        /// <summary>
        /// Finds the highest MemberID used in the XML file
        /// </summary>
        private void XMLFindHighestMemberID()
        {
            if (!File.Exists(this.xmlFilePath))
            {
                this.CurrentMemberID = 0;
                return;
            }
            
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(this.xmlFilePath);

            XmlNodeList nodeList = xmlDoc.SelectNodes("/Members/Member");

            foreach (XmlNode node in nodeList)
            {
                int nodeMemberID = int.Parse(node.Attributes[0].InnerText);

                if (nodeMemberID > this.CurrentMemberID)
                {
                    this.CurrentMemberID = nodeMemberID;
                }
            }
        }
    }
}
