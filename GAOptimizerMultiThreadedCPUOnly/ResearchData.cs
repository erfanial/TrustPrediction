using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace GeneticAlgorithm
{
    class ResearchData
    {
        public List<List<float>> GroundTruth;
        public List<float> UserTrusts;
        public List<List<List<UserUpdate>>> Updates;
        public int nSections;
        public int nUsers;

        public ResearchData()
        {
            readData();
        }

        private void readData()
        {
            string jsonFolder = @"../../../jsons/";
            float element;
            
            // this part populates data.GroundTruth
            List<List<float>> groud_truth = new List<List<float>>();
            XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
            xmlDoc.Load(jsonFolder + "groundTruth.xml"); // Load the XML document from the specified file
            XmlNodeList sections = xmlDoc.GetElementsByTagName("section");
            int count1 = 0;
            foreach(XmlNode section in sections)
            {
                groud_truth.Add(new List<float>());
                XmlNodeList hours = section.ChildNodes;
                foreach (XmlNode hour in hours)
                {
                    element = float.Parse(hour.InnerText);
                    groud_truth[count1].Add(element);
                }
                count1++;
            }
            GroundTruth = groud_truth;


            // this part populates data.UserTrusts
            List<float> _usertrusts = new List<float>();
            XmlNodeList trusts = xmlDoc.GetElementsByTagName("trust");
            xmlDoc.Load(jsonFolder + "userTrusts.xml"); // Load the XML document from the specified file
            foreach (XmlNode trust in trusts)
            {
                element = float.Parse(trust.InnerText);
                _usertrusts.Add(element);
            }
            UserTrusts = _usertrusts;
            
            
            // this part populates data.Updates
            List<List<List<UserUpdate>>> _updates = new List<List<List<UserUpdate>>>();
            xmlDoc.Load(jsonFolder + "updates_section0.xml"); // Load the XML document from the specified file, use updates_small.xml or updates_section0 for faster speeds
            sections = xmlDoc.GetElementsByTagName("section");
            for (int s = 0; s < sections.Count; s++)
            {
                _updates.Add(new List<List<UserUpdate>>());
                XmlNodeList iterations = sections[s].ChildNodes;
                for (int iter = 0; iter < iterations.Count; iter++)
                {
                    _updates[s].Add(new List<UserUpdate>());
                    XmlNodeList updates = iterations[iter].ChildNodes;
                    for (int u = 0; u < updates.Count; u++)
                    {
                        _updates[s][iter].Add(new UserUpdate());
                        _updates[s][iter][u].update_id = int.Parse(updates[u].Attributes[0].Value);
                        _updates[s][iter][u].user_id = int.Parse(updates[u].Attributes[1].Value);
                        _updates[s][iter][u].section = int.Parse(updates[u].Attributes[2].Value);
                        _updates[s][iter][u].tag = float.Parse(updates[u].Attributes[3].Value);
                        _updates[s][iter][u].timestamp = float.Parse(updates[u].Attributes[4].Value);
                    }
                }
            }
            Updates = _updates;

            nSections = Updates.Count;
            nUsers = UserTrusts.Count;

            return;
        }
    }


    public class UpdatesClass
    {
        public List<List<List<UserUpdate>>> storedUpdates;
    }
}
