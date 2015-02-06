using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeResearch
{
    class Experiment
    {
        private List<List<double>> predictedSectionOccupancy;
        private List<double> predictedUsersTrust;
        private SimOptions options;
        private ResearchData data;

        public Experiment(ResearchData _data, SimOptions _options)
        {
            options = _options;
            data = _data;
        }

        public PredictionPerformances execute()
        {
            double iterationOccupancy;
            List<double> trusts;
            List<double> validity;
            double FinalX;
            double ParkingCondition;
            double cso;
            double certainty;
            double C;
            int iterationsPerHour;
            double totalHoursOfUpdates = 0;
            double sectionOccupancy;
            int user_id;
            int update_id;
            double user_tag;


            // here we have the research data, now we should proceed on fusing all these updates

            // at first we need to initialize some elements
            predictedSectionOccupancy = new List<List<double>>();
            predictedUsersTrust = new List<double>();
            List<double> predictedUsersScore = new List<double>();

            int nSections = data.nSections;
            int nUsers = data.UserTrusts.Count;
            int nIterations = data.Updates[0].Count;

            for (int u = 0; u < nUsers; u++)
            {
                predictedUsersTrust.Add(0.5);
                predictedUsersScore.Add(0);
            }

            List<List<double>> predictedIterationOccupancy = new List<List<double>>(nSections);
            List<double> lastUpdateTime = new List<double>(nSections);
            List<double> currentSectionOccupancy = new List<double>(nSections);
            for (int s = 0; s < nSections; s++)
            {
                lastUpdateTime.Add(0);
                currentSectionOccupancy.Add(options.I);
            }
            List<Boolean> processed = new List<Boolean>(10000000);
            for (int s = 0; s < processed.Capacity; s++)
                processed.Add(false);



            double currentTime;
            double interval = 5;
            for (int section = 0; section < nSections; section++)
            {
                predictedIterationOccupancy.Add(new List<double>(nIterations));

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
                        trusts = new List<double>(nUpdates);
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

                        validity = new List<double>(nUpdates);
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
                        ParkingCondition = Math.Ceiling((iterationOccupancy - 1) / 4 * 5); // {1 2 3 4 5}
                        if (ParkingCondition == 0)
                            ParkingCondition = 1;

                        for (int i = 0; i < nUpdates; i++)
                        {
                            update_id = updates[i].update_id - 1;
                            user_id = updates[i].user_id - 1;
                            user_tag = updates[i].tag;
                            if (!processed[update_id])
                            {
                                certainty = options.certainty_coeff / (currentTime - lastUpdateTime[section]);
                                if (ParkingCondition == user_tag)
                                    C = options.lambda_promote * certainty;
                                else
                                    C = options.lambda_punish * certainty * -1 * Math.Abs(ParkingCondition - user_tag);
                                predictedUsersScore[user_id] += C;
                                predictedUsersTrust[user_id] = (Math.Tanh(predictedUsersScore[user_id] / options.score_coeff) + 1) / 2;
                                processed[update_id] = true;
                            }
                        }
                    }
                    predictedIterationOccupancy[section].Add(iterationOccupancy);
                    currentSectionOccupancy[section] = iterationOccupancy;
                }

                iterationsPerHour = (int)(60 / interval);
                totalHoursOfUpdates = (nIterations + 1) / iterationsPerHour;
                predictedSectionOccupancy.Add(new List<double>());
                for (int h = 0; h < totalHoursOfUpdates; h++)
                {
                    sectionOccupancy = 0;
                    for (int iter = 0; iter < iterationsPerHour; iter++)
                    {
                        try
                        {
                            iterationOccupancy = predictedIterationOccupancy[section][h * iterationsPerHour + iter];
                        }
                        catch
                        {
                            iterationOccupancy = options.I;
                        }
                        sectionOccupancy += iterationOccupancy;
                    }

                    sectionOccupancy = sectionOccupancy / iterationsPerHour;
                    sectionOccupancy = (sectionOccupancy - 1) / 4;
                    predictedSectionOccupancy[section].Add(sectionOccupancy);
                }
            }



            double error1 = 0, error2 = 0, error3 = 0, error4 = 0;
            int counter = 0;
            Random rnd = new Random();
            // find Performances
            PredictionPerformances performance = new PredictionPerformances();
            for (int section = 0; section < nSections; section++)
                for (int h = 0; h < totalHoursOfUpdates; h++)
                {
                    counter++;
                    error1 += Math.Abs(predictedSectionOccupancy[section][h] - data.GroundTruth[section][h]);
                    error2 += Math.Abs(rnd.NextDouble() - data.GroundTruth[section][h]);
                }
            performance.occupancyPerformance = 1 - error1 / counter;
            performance.occupancyPerformanceRandom = 1 - error2 / counter;

            for (int u = 0; u < nUsers; u++)
            {
                error3 += Math.Abs(predictedUsersTrust[u] - data.UserTrusts[u]);
                error4 += Math.Abs(rnd.NextDouble() - data.UserTrusts[u]);
            }
            performance.trustPerformance = 1 - error3 / nUsers;
            performance.trustPerformanceRandom = 1 - error4 / nUsers;
            return performance;
        }

        private double X_influence(double tag, double timestamp, double currentTime)
        {
            return (tag - options.I) / ((currentTime - timestamp) * options.decay + 1);
        }
    }
}
