/**
 * @file
 * @author  Hekar Kahni, Samuel Lewis
 * @version 1.0
 *
 * @section DESCRIPTION
 * Simple Database class that contains Insert, Update, and Find.
 * XML is used to save and retrieve information from the database
 * 
 */


using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1
{
    /// <summary>
    /// Basic Database that contains Insert, Update, and Find.
    /// </summary>
    public class Database
    {
        // Default allocation for database
        private const int kDefaultMaxCapacity = 40000;

        //private SortedList<int, DataRecord> Records;
        private Mutex WriteMutex;
        private string xmlFilePath;
        private int CurrentMemberID;

        /// <summary>
        /// Database Class
        /// </summary>
        /// <param name="xmlFile">Filename of XML storage</param>
        public Database(string xmlFile)
        {
            //this.Records = new SortedList<int, DataRecord>(kDefaultMaxCapacity);
            this.WriteMutex = new Mutex();
            xmlFilePath = xmlFile;
            this.XMLFindHighestMemberID();
        }

        /// <summary>
        /// Update the values of a record in the database.
        /// </summary>
        /// <param name="record">Record who's corresponding memberid will be updated.</param>
        public void Update(DataRecord record)
        {
            //this.WriteMutex.WaitOne();
            //this.Records[record.MemberID - 1] = record;
            //this.WriteMutex.ReleaseMutex();
            this.XMLRemoveRecord(record.MemberID);
            this.XMLAppendRecord(record);
        }

        /// <summary>
        /// Inserts a new record into the database
        /// </summary>
        /// <param name="record">Record to insert</param>
        public void Insert(DataRecord record)
        {
            if (this.CurrentMemberID + 1 > kDefaultMaxCapacity)
            {
                throw new OutOfMemoryException();
            }

            this.WriteMutex.WaitOne();
            ++this.CurrentMemberID;
            record.MemberID = CurrentMemberID;
            //this.Records.Add(record.MemberID - 1, record);
            this.XMLAppendRecord(record);

            Console.WriteLine("Record Added");
            Console.WriteLine("\tId: {0}\n\tFirstName: {1}\n\tLastName: {2}\n\tDateOfBirth: {3}\n",
                record.MemberID, record.FirstName, record.LastName, record.DateOfBirth);
            this.WriteMutex.ReleaseMutex();
        }

        /// <summary>
        /// Return a record based on its memberid
        /// </summary>
        /// <param name="MemberID">memberid of the desired record to find</param>
        /// <returns>Record corresponding to memberid</returns>
        public DataRecord Find(int MemberID)
        {
            DataRecord record = new DataRecord();
            record.MemberID = MemberID;
            this.XMLFindRecord(ref record);
            return record;
            //return this.Records[MemberID - 1];
        }

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

        private void XMLRemoveRecord(int MemberID)
        {
            if (!File.Exists(this.xmlFilePath))
            {
                throw new KeyNotFoundException();
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(this.xmlFilePath);

            string searchQuery = "/Members/Member[@ID='" + MemberID.ToString() + "']";

            XmlNodeList xmlNodes = xmlDoc.SelectNodes(searchQuery);

            if (xmlNodes.Count != 1)
            {
                throw new KeyNotFoundException();
            }

            xmlNodes[0].ParentNode.RemoveChild((XmlNode)xmlNodes[0]);
        }

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

        private void XMLCreateFile()
        {
            XmlTextWriter newDoc = new XmlTextWriter(this.xmlFilePath, null);

            newDoc.WriteStartDocument();
            newDoc.WriteStartElement("Members");
            newDoc.WriteEndElement();
            newDoc.Close();
        }

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
