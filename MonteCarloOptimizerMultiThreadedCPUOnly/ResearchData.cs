using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class ResearchData
    {
        public List<List<float>> GroundTruth;
        public List<float> UserTrusts;
        public List<List<List<UserUpdate>>> Updates; // [section][interval][updateNumber]
        public int nSections;

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
            using (StreamReader stream = new StreamReader(jsonFolder + "groundTruth.json"))
            {
                string json = stream.ReadToEnd();
                dynamic arr = JsonConvert.DeserializeObject(json);
                for (int section = 0; section < arr.Count; section++)
                {
                    groud_truth.Add(new List<float>());
                    for (int hour = 0; hour < arr[section].Count; hour++)
                    {
                        element = (float)arr[section][hour].Value;
                        groud_truth[section].Add(element);
                    }
                }
                GroundTruth = groud_truth;
            }


            // this part populates data.UserTrusts
            List<float> _usertrusts = new List<float>();
            using (StreamReader stream = new StreamReader(jsonFolder + "userTrusts.json"))
            {
                string json = stream.ReadToEnd();
                dynamic arr = JsonConvert.DeserializeObject(json);
                for (int user = 0; user < arr.Count; user++)
                {
                    element = (float)arr[user].Value;
                    _usertrusts.Add(element);
                }
                UserTrusts = _usertrusts;
            }

            // this part populates data.Updates
            List<List<List<UserUpdate>>> _updates = new List<List<List<UserUpdate>>>();
            using (StreamReader stream = new StreamReader(jsonFolder + "updates_section0.json"))
            {
                string json = stream.ReadToEnd();
                dynamic arr = JsonConvert.DeserializeObject(json);

                //json = JsonConvert.SerializeObject(arr[0]);
                //using(StreamWriter writer = new StreamWriter(@"jsons/updates_section0.json"))
                //{
                //    writer.Write(json);
                //}

                nSections = arr.Count;
                for (int section = 0; section < arr.Count; section++)
                {
                    _updates.Add(new List<List<UserUpdate>>());
                    for (int minute = 0; minute < arr[section].Count; minute++)
                    {
                        //Console.WriteLine("processing section {0} minutes {1}", section, minute);
                        _updates[section].Add(new List<UserUpdate>());
                        int NumberOfElements = arr[section][minute].Count;
                        if (NumberOfElements == 0)
                        {
                            // do nothing
                        }
                        else
                        {
                            int updatesForThatMin;
                            try
                            {
                                updatesForThatMin = arr[section][minute][0].Count;
                            }
                            catch
                            {
                                updatesForThatMin = 1;
                            }
                            for (int updateNumber = 0; updateNumber < updatesForThatMin; updateNumber++)
                            {
                                _updates[section][minute].Add(new UserUpdate());

                                if (updatesForThatMin > 1)
                                {
                                    _updates[section][minute][updateNumber].update_id = arr[section][minute][0][updateNumber];
                                    _updates[section][minute][updateNumber].user_id = arr[section][minute][1][updateNumber];
                                    _updates[section][minute][updateNumber].section = arr[section][minute][2][updateNumber];
                                    _updates[section][minute][updateNumber].tag = arr[section][minute][3][updateNumber];
                                    _updates[section][minute][updateNumber].timestamp = arr[section][minute][4][updateNumber];
                                    //_updates[section][minute][updateNumber].processed = arr[section][minute][5][updateNumber];
                                }
                                else
                                {
                                    _updates[section][minute][updateNumber].update_id = arr[section][minute][0];
                                    _updates[section][minute][updateNumber].user_id = arr[section][minute][1];
                                    _updates[section][minute][updateNumber].section = arr[section][minute][2];
                                    _updates[section][minute][updateNumber].tag = arr[section][minute][3];
                                    _updates[section][minute][updateNumber].timestamp = arr[section][minute][4];
                                    //Updates[section][minute][updateNumber].processed = arr[section][minute][5];
                                }
                            }
                        }

                    }
                }
                Updates = _updates;
            }
            return;
        }
    }
}
