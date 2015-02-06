using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    class ExperimentIterationSpecific
    {
        private List<List<float>> predictedSectionOccupancy;
        private List<float> predictedUsersTrust;
        private SimOptions options;
        private ResearchData _researchData;

        public ExperimentIterationSpecific(ResearchData _data, SimOptions _options)
        {
            options = _options;
            _researchData = _data;
        }

        public PredictionPerformances execute()
        {
            float iterationOccupancy;
            List<float> trusts;
            List<float> validity;
            float FinalX;
            float ParkingCondition;
            float cso;
            float certainty;
            float C;
            int iterationsPerHour;
            float totalHoursOfUpdates = 0;
            int user_id;
            int update_id;
            float user_tag;


            // here we have the research data, now we should proceed on fusing all these updates

            // at first we need to initialize some elements
            predictedSectionOccupancy = new List<List<float>>();
            predictedUsersTrust = new List<float>();
            List<float> predictedUsersScore = new List<float>();

            int nSections = _researchData.nSections;
            int nUsers = _researchData.UserTrusts.Count;
            int nIterations = _researchData.Updates[0].Count;

            for (int u = 0; u < nUsers; u++)
            {
                predictedUsersTrust.Add((float)0.5);
                predictedUsersScore.Add(0);
            }

            List<List<float>> predictedIterationOccupancy = new List<List<float>>(nSections);
            List<float> lastUpdateTime = new List<float>(nSections);
            List<float> currentSectionOccupancy = new List<float>(nSections);
            for (int s = 0; s < nSections; s++)
            {
                lastUpdateTime.Add(0);
                currentSectionOccupancy.Add(options.I);
            }
            List<Boolean> processed = new List<Boolean>(10000000);
            for (int s = 0; s < processed.Capacity; s++)
                processed.Add(false);



            float currentTime,error;
            const float interval = 5; // every 5 minutes, standard
            for (int section = 0; section < nSections; section++)
            {
                predictedIterationOccupancy.Add(new List<float>(nIterations));

                currentTime = 7200;
                for (int iteration = 0; iteration < nIterations; iteration++)
                {
                    iterationOccupancy = options.I;
                    currentTime += interval;
                    List<UserUpdate> updates = _researchData.Updates[section][iteration];
                    int nUpdates = updates.Count;

                    if (nUpdates == 0)
                    {

                    }
                    else
                    {
                        trusts = new List<float>(nUpdates);
                        for (int i = 0; i < nUpdates; i++)
                        {
                            user_id = updates[i].user_id - 1;
                            trusts.Add(predictedUsersTrust[user_id]);
                        }                        

                        validity = new List<float>(nUpdates);
                        for (int i = 0; i < nUpdates; i++)
                            validity.Add(0);

                        for (int i = nUpdates - 1; i >= 0; i--)
                        {
                            validity[i] = trusts[i];
                            for (int j = i + 1; j < nUpdates; j++)
                                validity[i] *= 1 - trusts[j];
                        }                        

                        FinalX = 0;
                        for (int i = 0; i < nUpdates; i++)
                            FinalX += validity[i] * (updates[i].tag - options.I) / ((currentTime - updates[i].timestamp) * options.decayOfInteration[iteration] + 1);
                        lastUpdateTime[section] = updates[updates.Count - 1].timestamp;
                        iterationOccupancy = options.I + FinalX;

                        
                        cso = currentSectionOccupancy[section];
                        ParkingCondition = (float)(Math.Ceiling((iterationOccupancy - 1) / 4 * 5)); // {1 2 3 4 5}
                        if (ParkingCondition == 0)
                            ParkingCondition = 1;

                        for (int i = 0; i < nUpdates; i++)
                        {
                            update_id = updates[i].update_id - 1;
                            user_id = updates[i].user_id - 1;
                            user_tag = updates[i].tag;
                            if (!processed[update_id])
                            {
                                certainty = options.certainty_coeff / (currentTime - updates[i].timestamp);
                                if (ParkingCondition == user_tag)
                                    C = options.lambda_promote * certainty;
                                else
                                    C = options.lambda_punish * certainty * -1 * Math.Abs(ParkingCondition - user_tag);
                                predictedUsersScore[user_id] += C;
                                predictedUsersTrust[user_id] = (float)((Math.Tanh(predictedUsersScore[user_id] / options.score_coeff) + 1) / 2);
                                processed[update_id] = true;
                            }
                        }
                    }
                    predictedIterationOccupancy[section].Add(iterationOccupancy);
                    currentSectionOccupancy[section] = iterationOccupancy;
                }

                iterationsPerHour = (int)(60 / interval);
                totalHoursOfUpdates = (nIterations + 1) / iterationsPerHour;
                predictedSectionOccupancy.Add(new List<float>());
                for (int iter = 0; iter < predictedIterationOccupancy[section].Count; iter += iterationsPerHour)
                    predictedSectionOccupancy[section].Add((predictedIterationOccupancy[section][iter] - 1) / 4);
            }



            float error1 = 0, error2 = 0, error3 = 0, error4 = 0, temp1, temp2;
            int counter = 0;
            Random rnd = new Random();
            // find Performances
            PredictionPerformances performance = new PredictionPerformances();
            for (int section = 0; section < nSections; section++)
                for (int h = 0; h < totalHoursOfUpdates; h++)
                {
                    counter++;

                    temp1 = (float)Math.Floor(predictedSectionOccupancy[section][h] * 4) + 1;
                    temp2 = (float)Math.Floor(_researchData.GroundTruth[section][h] * 4) + 1;
                    error1 += Math.Abs(temp1 - temp2);

                    temp1 = (float)Math.Floor(rnd.NextDouble() * 4) + 1;
                    error2 += Math.Abs(temp1 - temp2);
                }
            performance.occupancyPerformance = 1 - error1 / (4 * counter);
            performance.occupancyPerformanceRandom = 1 - error2 / (4 * counter);

            for (int u = 0; u < nUsers; u++)
            {
                error3 += Math.Abs(predictedUsersTrust[u] - _researchData.UserTrusts[u]);
                error4 += Math.Abs((float)rnd.NextDouble() - _researchData.UserTrusts[u]);
            }
            performance.trustPerformance = 1 - error3 / nUsers;
            performance.trustPerformanceRandom = 1 - error4 / nUsers;
            return performance;
        }

        private float X_influence(float tag, float timestamp, float currentTime)
        {
            return (tag - options.I) / ((currentTime - timestamp) * options.decay + 1);
        }
    }
}
