using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;

namespace GeneticAlgorithm
{
    class Experiment
    {
        
        [Cudafy]
        public static unsafe float execute(float[,] GroundTruth, float[] UserTrusts, UserUpdate[, ,] Updates, FitnessParameter fitnessParam)
        {
            SimOptions options = new SimOptions(1, fitnessParam.var1, fitnessParam.var2, fitnessParam.var3, fitnessParam.var4, fitnessParam.var5);



            float iterationOccupancy;
            float[] trusts;
            float[] validity;
            float FinalX;
            float ParkingCondition;
            float cso;
            float certainty;
            float C;
            int iterationsPerHour;
            int totalHoursOfUpdates = 0;
            float sectionOccupancy;
            int user_id;
            int update_id;
            float user_tag;


            // here we have the research data, now we should proceed on fusing all these updates

            // at first we need to initialize some elements
            //float* predictedSectionOccupancy = fitnessParam.predictedSectionOccupancy;
            //float* predictedUsersTrust = fitnessParam.predictedUsersTrust;
            //float* predictedUsersScore = fitnessParam.predictedUsersScore;
            int nSections = GroundTruth.GetLength(0);
            int nUsers = UserTrusts.Length;
            int nIterations = Updates.GetLength(1);

            //for (int u = 0; u < nUsers; u++)
            //    predictedUsersTrust[u] = (float)0.5;

            //float[,] predictedIterationOccupancy = new float[nSections,nIterations];
            //float[] lastUpdateTime = new float[nSections];
            //float[] currentSectionOccupancy = new float[nSections];
            //for (int s = 0; s < nSections; s++)
            //{
            //    lastUpdateTime[s] = 0;
            //    currentSectionOccupancy[s] = (float)options.I;
            //}
            //bool[] processed = new bool[10000000];
            //for (int s = 0; s < processed.Length; s++)
            //    processed[s] = false;



            //float currentTime;
            //float interval = 5;
            //for (int section = 0; section < nSections; section++)
            //{
            //    currentTime = 7680;
            //    for (int iteration = 0; iteration < nIterations; iteration++)
            //    {
            //        iterationOccupancy = (float)options.I;
            //        currentTime += interval;
            //        UserUpdate[] updates = new UserUpdate[Updates.GetLength(2)];
            //        int nUpdates = 0;
            //        for (int tempCounter = 0; tempCounter < updates.Length; tempCounter++)
            //            if(Updates[section, iteration, tempCounter].section > 0)
            //                updates[nUpdates++] = Updates[section, iteration, tempCounter];
            //        if (nUpdates == 0)
            //        {

            //        }
            //        else
            //        {
            //            ///////////// get trust of Users
            //            //T = [tags(:).trust];
            //            trusts = new float[nUpdates];
            //            for (int i = 0; i < nUpdates; i++)
            //            {
            //                user_id = updates[i].user_id - 1;
            //                trusts[i] = predictedUsersTrust[user_id];
            //            }

            //            ////////////////////////////////////// make validity vector
            //            //n = length(T);
            //            //validity = zeros(1,n);
            //            //for i=n:-1:1
            //            //    validity(i)=T(i);
            //            //    for j=i+1:n
            //            //        validity(i) = validity(i) * (1 - T(j));
            //            //    end
            //            //end

            //            validity = new float[nUpdates];

            //            for (int i = nUpdates - 1; i >= 0; i--)
            //            {
            //                validity[i] = trusts[i];
            //                for (int j = i + 1; j < nUpdates; j++)
            //                    validity[i] *= 1 - trusts[j];
            //            }


            //            ///////////// fuse based on validity
            //            //FinalX = 0;
            //            //for j=1:length(validity)
            //            //    X = X_influence(tags(j),currentTime,I);
            //            //    FinalX = FinalX + validity(j)*X;
            //            //end
            //            //newSectionOccupancy = I + FinalX; % real between 1 and 5
            //            //lastUpdateTimes(section) = currentTime;

            //            FinalX = 0;
            //            for (int i = 0; i < nUpdates; i++)
            //                //FinalX += validity[i] * X_influence(updates[i].tag, updates[i].timestamp, currentTime);
            //                FinalX += validity[i] * (updates[i].tag - options.I) / ((currentTime - updates[i].timestamp) * options.decay + 1);
            //            lastUpdateTime[section] = (float)updates[updates.Length - 1].timestamp;
            //            iterationOccupancy = (float)options.I + FinalX;





            //            /////////////////// update peoples trust
            //            //ParkingCondition = ceil((currentSectionOccupancy-1)/4 * 5);
            //            //if ParkingCondition == 0, ParkingCondition = 1; end
            //            //if ~isempty(processed)
            //            //    for p = find(processed == 0)
            //            //        submittedTag = tags(p).value; % r
            //            //        certainty = simOptions.certainty_coeff / (currentTime - lastUpdateTime); % coce

            //            //        % calculate C
            //            //        if ParkingCondition == submittedTag
            //            //            C = simOptions.lambda_promote * certainty;
            //            //        else
            //            //            C = simOptions.lambda_punish * certainty * -abs(ParkingCondition - submittedTag);
            //            //        end
            //            //        tags(p).score = tags(p).score + C;
            //            //        tags(p).trust = (tanh(tags(p).score/simOptions.score_coeff)+1)/2;

            //            //        tagid = tags(p).tagID;
            //            //        uid = tags(p).userID;
            //            //        trust = tags(p).trust;
            //            //        score = tags(p).score;
            //            //% 		rank = floor(trust * 5) + 1; % rank = 1,2,3,4,5
            //            //        predictedTrust(uid) = trust;
            //            //        predictedScores(uid) = score;
            //            //        uus(tagid,6) = 1; % flag processed
            //            //    end
            //            //end
            //            cso = currentSectionOccupancy[section];
            //            ParkingCondition = (float)Math.Ceiling((iterationOccupancy - 1) / 4 * 5); // {1 2 3 4 5}
            //            if (ParkingCondition == 0)
            //                ParkingCondition = 1;

            //            for (int i = 0; i < nUpdates; i++)
            //            {
            //                update_id = updates[i].update_id - 1;
            //                user_id = updates[i].user_id - 1;
            //                user_tag = updates[i].tag;
            //                if (!processed[update_id])
            //                {
            //                    certainty = options.certainty_coeff / (currentTime - lastUpdateTime[section]);
            //                    if (ParkingCondition == user_tag)
            //                        C = options.lambda_promote * certainty;
            //                    else
            //                        C = options.lambda_punish * certainty * -1 * Math.Abs(ParkingCondition - user_tag);
            //                    predictedUsersScore[user_id] += C;
            //                    predictedUsersTrust[user_id] = (float)(Math.Tanh(predictedUsersScore[user_id] / options.score_coeff) + 1) / 2;
            //                    processed[update_id] = true;
            //                }
            //            }
            //        }
            //        predictedIterationOccupancy[section,iteration] = iterationOccupancy;
            //        currentSectionOccupancy[section] = iterationOccupancy;
            //    }

            //    iterationsPerHour = (int)(60 / interval);
            //    totalHoursOfUpdates = (int)((nIterations + 1) / iterationsPerHour);
            //    for (int h = 0; h < totalHoursOfUpdates; h++)
            //    {
            //        sectionOccupancy = 0;
            //        for (int iter = 0; iter < iterationsPerHour; iter++)
            //        {
            //            try
            //            {
            //                iterationOccupancy = predictedIterationOccupancy[section,h * iterationsPerHour + iter];
            //            }
            //            catch
            //            {
            //                iterationOccupancy = options.I;
            //            }
            //            sectionOccupancy += iterationOccupancy;
            //        }

            //        sectionOccupancy = sectionOccupancy / iterationsPerHour;
            //        sectionOccupancy = (sectionOccupancy - 1) / 4;
            //        predictedSectionOccupancy[section,h] = sectionOccupancy;
            //    }
            //}



            //float error1 = 0, error2 = 0, error3 = 0, error4 = 0;
            //int counter = 0;
            //Random rnd = new Random();
            //// find Performances
            //PredictionPerformances performance = new PredictionPerformances();
            //for (int section = 0; section < nSections; section++)
            //    for (int h = 0; h < totalHoursOfUpdates; h++)
            //    {
            //        counter++;
            //        error1 += (float)(Math.Abs(predictedSectionOccupancy[section,h] - GroundTruth[section,h]));
            //        error2 += (float)(Math.Abs(rnd.NextDouble() - GroundTruth[section,h]));
            //    }
            //performance.occupancyPerformance = 1 - error1 / counter;
            //performance.occupancyPerformanceRandom = 1 - error2 / counter;

            //for (int u = 0; u < nUsers; u++)
            //{
            //    error3 += (float)(Math.Abs(predictedUsersTrust[u] - UserTrusts[u]));
            //    error4 += (float)(Math.Abs(rnd.NextDouble() - UserTrusts[u]));
            //}
            //performance.trustPerformance = 1 - error3 / nUsers;
            //performance.trustPerformanceRandom = 1 - error4 / nUsers;
            //return performance.occupancyPerformance;


            return (float)-1.1;
        }

    }
}
