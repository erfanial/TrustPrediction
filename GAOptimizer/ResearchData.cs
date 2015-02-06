using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cudafy;

namespace GeneticAlgorithm
{
    public class ResearchData
    {
        public float[,] GroundTruth;
        public float[] UserTrusts;
        public UserUpdate[,,] Updates; // [section][interval][updateNumber]
        public int nSections;

        public ResearchData(string param = "")
        {
            int i, j, k, l;

            // this part populates data.GroundTruth
            List<List<double>> groud_truth = new List<List<double>>();
            using (StreamReader stream = new StreamReader(@"C:\Users\Student Admin\Documents\Visual Studio 2012\Projects\TrustPrediction\jsons\groundTruth.json"))
            {
                string json = stream.ReadToEnd();
                dynamic arr = JsonConvert.DeserializeObject(json);
                for (int section = 0; section < arr.Count; section++)
                {
                    groud_truth.Add(new List<double>());
                    for (int hour = 0; hour < arr[section].Count; hour++)
                    {
                        double element = arr[section][hour].Value;
                        groud_truth[section].Add(element);
                    }
                }
                GroundTruth = new float[groud_truth.Count,groud_truth[0].Count];
                for (i = 0; i < groud_truth.Count; i++)
                    for (j = 0; j < groud_truth[i].Count; j++)
                        GroundTruth[i,j] = (float)groud_truth[i][j];
            }


            // this part populates data.UserTrusts
            List<double> _usertrusts = new List<double>();
            using (StreamReader stream = new StreamReader(@"C:\Users\Student Admin\Documents\Visual Studio 2012\Projects\TrustPrediction\jsons\userTrusts.json"))
            {
                string json = stream.ReadToEnd();
                dynamic arr = JsonConvert.DeserializeObject(json);
                for (int user = 0; user < arr.Count; user++)
                {
                    double element = arr[user].Value;
                    _usertrusts.Add(element);
                }
                UserTrusts = new float[_usertrusts.Count];
                for (i = 0; i < _usertrusts.Count; i++)
                    UserTrusts[i] = (float)_usertrusts[i];
            }

            // this part populates data.Updates
            List<List<List<UserUpdate>>> _updates = new List<List<List<UserUpdate>>>();
            using (StreamReader stream = new StreamReader(@"C:\Users\Student Admin\Documents\Visual Studio 2012\Projects\TrustPrediction\jsons\updates_section0.json"))
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
                    for (int minute = 0; minute < 20146 /*arr[section].Count*/; minute++)
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
                                if (updatesForThatMin > 1)
                                {
                                    _updates[section][minute].Add(new UserUpdate(
                                        (int)arr[section][minute][0][updateNumber],
                                        (int)arr[section][minute][1][updateNumber],
                                        (int)arr[section][minute][2][updateNumber],
                                        (float)arr[section][minute][3][updateNumber],
                                        (float)arr[section][minute][4][updateNumber]));
                                }
                                else
                                {
                                    _updates[section][minute].Add(new UserUpdate(
                                        (int)arr[section][minute][0],
                                        (int)arr[section][minute][1],
                                        (int)arr[section][minute][2],
                                        (float)arr[section][minute][3],
                                        (float)arr[section][minute][4]));
                                }

                                //_updates[section][minute].Add(new UserUpdate());

                                //if (updatesForThatMin > 1)
                                //{
                                //    _updates[section][minute][updateNumber].update_id = arr[section][minute][0][updateNumber];
                                //    _updates[section][minute][updateNumber].user_id = arr[section][minute][1][updateNumber];
                                //    _updates[section][minute][updateNumber].section = arr[section][minute][2][updateNumber];
                                //    _updates[section][minute][updateNumber].tag = arr[section][minute][3][updateNumber];
                                //    _updates[section][minute][updateNumber].timestamp = arr[section][minute][4][updateNumber];
                                //    //_updates[section][minute][updateNumber].processed = arr[section][minute][5][updateNumber];
                                //}
                                //else
                                //{
                                //    _updates[section][minute][updateNumber].update_id = arr[section][minute][0];
                                //    _updates[section][minute][updateNumber].user_id = arr[section][minute][1];
                                //    _updates[section][minute][updateNumber].section = arr[section][minute][2];
                                //    _updates[section][minute][updateNumber].tag = arr[section][minute][3];
                                //    _updates[section][minute][updateNumber].timestamp = arr[section][minute][4];
                                //    //Updates[section][minute][updateNumber].processed = arr[section][minute][5];
                                //}
                            }
                        }

                    }
                }


                int maxUpdates = 0;
                for (i = 0; i < _updates.Count; i++)
                    for (j = 0; j < _updates[i].Count; j++)
                        if (maxUpdates < _updates[i][j].Count)
                            maxUpdates = _updates[i][j].Count;


                Updates = new UserUpdate[_updates.Count, _updates[0].Count, maxUpdates];
                for (i = 0; i < _updates.Count; i++)
                    for (j = 0; j < _updates[i].Count; j++)
                        for (k = 0; k < _updates[i][j].Count; k++)
                            Updates[i, j, k] = _updates[i][j][k];
                            //try {
                            //    Updates[i, j, k] = _updates[i][j][k];
                            //}
                            //catch (Exception e) {
                            //    Console.WriteLine(e.Message);
                            //}
                    
                    
            }
        }
    }
}
