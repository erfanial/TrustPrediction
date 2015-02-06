using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Experiment
    {

        private List<List<float>> predictedSectionOccupancy;
        private List<float> predictedUsersTrust;
        private SimOptions options;
        private ResearchData data;

        public Experiment(ResearchData _data, SimOptions _options)
        {
            options = _options;
            data = _data;
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
            float sectionOccupancy;
            int user_id;
            int update_id;
            float user_tag;


            // here we have the research data, now we should proceed on fusing all these updates

            // at first we need to initialize some elements
            predictedSectionOccupancy = new List<List<float>>();
            predictedUsersTrust = new List<float>();
            List<float> predictedUsersScore = new List<float>();

            int nSections = data.nSections;
            int nUsers = data.UserTrusts.Count;
            int nIterations = data.Updates[0].Count;

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



            float currentTime;
            float interval = 5;
            for (int section = 0; section < nSections; section++)
            {
                predictedIterationOccupancy.Add(new List<float>(nIterations));

                currentTime = 7680;
                for (int iteration = 0; iteration < nIterations; iteration++)
                {
                    iterationOccupancy = options.I;
                    currentTime += interval;
                    List<UserUpdate> updates = data.Updates[section][iteration];
                    int nUpdates = updates.Count;

                    if (nUpdates == 0)
                    {

                    }
                    else
                    {
                        ///////////// get trust of Users
                        //T = [tags(:).trust];
                        trusts = new List<float>(nUpdates);
                        for (int i = 0; i < nUpdates; i++)
                        {
                            user_id = updates[i].user_id - 1;
                            trusts.Add(predictedUsersTrust[user_id]);
                        }

                        ////////////////////////////////////// make validity vector
                        //n = length(T);
                        //validity = zeros(1,n);
                        //for i=n:-1:1
                        //    validity(i)=T(i);
                        //    for j=i+1:n
                        //        validity(i) = validity(i) * (1 - T(j));
                        //    end
                        //end

                        validity = new List<float>(nUpdates);
                        for (int i = 0; i < nUpdates; i++)
                            validity.Add(0);

                        for (int i = nUpdates - 1; i >= 0; i--)
                        {
                            validity[i] = trusts[i];
                            for (int j = i + 1; j < nUpdates; j++)
                                validity[i] *= 1 - trusts[j];
                        }


                        ///////////// fuse based on validity
                        //FinalX = 0;
                        //for j=1:length(validity)
                        //    X = X_influence(tags(j),currentTime,I);
                        //    FinalX = FinalX + validity(j)*X;
                        //end
                        //newSectionOccupancy = I + FinalX; % real between 1 and 5
                        //lastUpdateTimes(section) = currentTime;

                        FinalX = 0;
                        for (int i = 0; i < nUpdates; i++)
                            //FinalX += validity[i] * X_influence(updates[i].tag, updates[i].timestamp, currentTime);
                            FinalX += validity[i] * (updates[i].tag - options.I) / ((currentTime - updates[i].timestamp) * options.decay + 1);
                        lastUpdateTime[section] = updates[updates.Count - 1].timestamp;
                        iterationOccupancy = options.I + FinalX;





                        /////////////////// update peoples trust
                        //ParkingCondition = ceil((currentSectionOccupancy-1)/4 * 5);
                        //if ParkingCondition == 0, ParkingCondition = 1; end
                        //if ~isempty(processed)
                        //    for p = find(processed == 0)
                        //        submittedTag = tags(p).value; % r
                        //        certainty = simOptions.certainty_coeff / (currentTime - lastUpdateTime); % coce

                        //        % calculate C
                        //        if ParkingCondition == submittedTag
                        //            C = simOptions.lambda_promote * certainty;
                        //        else
                        //            C = simOptions.lambda_punish * certainty * -abs(ParkingCondition - submittedTag);
                        //        end
                        //        tags(p).score = tags(p).score + C;
                        //        tags(p).trust = (tanh(tags(p).score/simOptions.score_coeff)+1)/2;

                        //        tagid = tags(p).tagID;
                        //        uid = tags(p).userID;
                        //        trust = tags(p).trust;
                        //        score = tags(p).score;
                        //% 		rank = floor(trust * 5) + 1; % rank = 1,2,3,4,5
                        //        predictedTrust(uid) = trust;
                        //        predictedScores(uid) = score;
                        //        uus(tagid,6) = 1; % flag processed
                        //    end
                        //end
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

                //iterationsPerHour = (int)(60 / interval);
                //totalHoursOfUpdates = (nIterations + 1) / iterationsPerHour;
                //predictedSectionOccupancy.Add(new List<float>());
                //for (int h = 0; h < totalHoursOfUpdates; h++)
                //{
                //    sectionOccupancy = 0;
                //    for (int iter = 0; iter < iterationsPerHour; iter++)
                //    {
                //        try
                //        {
                //            iterationOccupancy = predictedIterationOccupancy[section][h * iterationsPerHour + iter];
                //        }
                //        catch
                //        {
                //            iterationOccupancy = options.I;
                //        }
                //        sectionOccupancy += iterationOccupancy;
                //    }

                //    sectionOccupancy = sectionOccupancy / iterationsPerHour;
                //    sectionOccupancy = (sectionOccupancy - 1) / 4;
                //    predictedSectionOccupancy[section].Add(sectionOccupancy);
                //}

                iterationsPerHour = (int)(60 / interval);
                totalHoursOfUpdates = (nIterations + 1) / iterationsPerHour;
                predictedSectionOccupancy.Add(new List<float>());
                for (int iter = 0; iter < predictedIterationOccupancy[section].Count; iter += iterationsPerHour)
                    predictedSectionOccupancy[section].Add((predictedIterationOccupancy[section][iter]-1)/4);

                
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
                    temp2 = (float)Math.Floor(data.GroundTruth[section][h] * 4) + 1;
                    error1 += Math.Abs(temp1 - temp2);

                    temp1 = (float)Math.Floor(rnd.NextDouble() * 4) + 1;
                    error2 += Math.Abs(temp1 - temp2);
                }
            performance.occupancyPerformance = 1 - error1 / (4 * counter);
            performance.occupancyPerformanceRandom = 1 - error2 / (4 * counter);

            for (int u = 0; u < nUsers; u++)
            {
                error3 += Math.Abs(predictedUsersTrust[u] - data.UserTrusts[u]);
                error4 += Math.Abs((float)rnd.NextDouble() - data.UserTrusts[u]);
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
