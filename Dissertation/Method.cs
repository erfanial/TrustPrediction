using Dissertation.TagProcessors;
using MachineLearningCore;
using MachineLearningCore.BinaryDecisionTrees;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Dissertation
{
    public class Method
    {
        protected List<SectionOccupancy> Occupancies;
        protected List<User> Users;
        List<UserTagReport> UserTags;
        ExperimentOptions Options;
        int StopTrainingDay;
        private Stopwatch stopwatch;
        private SqlServerConnector db;
        Random random = new Random();

        public Method(List<SectionOccupancy> occupancies, List<User> users, List<UserTagReport> userTags, ExperimentOptions options)
        {
            Occupancies = occupancies;
            Users = users;
            UserTags = userTags;
            Options = options;
            stopwatch = new Stopwatch();
            db = new SqlServerConnector();
        }

        public virtual MethodPerformance Execute()
        {

            int experimentMethodCode = Options.ExperimentMethodCode;
            int nSections = DataSimulation.DefaultGroupSimulationOptions.SectionPriorities.Length * DataSimulation.DefaultGroupSimulationOptions.SectionPriorities[0].Length;

            List<double> SectionRValue = new List<double>(nSections);
            List<double> SectionSValue = new List<double>(nSections);
            for (int i = 0; i < nSections; i++)
            {
                SectionRValue.Add(0);
                SectionSValue.Add(0);
            }


            // initialize            
            int[] PredictedTag = new int[nSections], MaxvoteTag = new int[nSections], RandomTag = new int[nSections];
            for (int i = 0; i < nSections; i++) // start off with 1
            {
                PredictedTag[i] = 1;
                MaxvoteTag[i] = 1;
                RandomTag[i] = 1;
            }

            var sectionTable = UserTags.GroupBy(u => new { u.Section, u.Day, u.Hour }, (key, group) => new DayHourSection(group.ToList(), key.Day, key.Hour, key.Section)).ToList();
            var Days = UserTags.Max(t => t.Day);
            StopTrainingDay = (int)Math.Floor(Days * Options.pot);


            // for portfolio
            DataPoint point;
            DataSet TrainingDataset = new DataSet();
            TrainingDataset.DataPoints = new List<DataPoint>();
            BinaryDecisionTree tree = null;

            foreach (DayHourSection item in sectionTable)
            {
                var tags = item.Reports;
                int CurrentOccupancyIndex = (from o in Occupancies where o.Day == item.Day && o.Hour == item.Hour && o.Section == item.Section select o.index).ToArray()[0];
                int CurrentSection = tags[0].Section;
                int RealTag = Occupancies[CurrentOccupancyIndex].OccupancyTag; // for all tags, the realtag is the same                        
                int CurrentDay = Occupancies[CurrentOccupancyIndex].Day;
                int CurrentHour = Occupancies[CurrentOccupancyIndex].Hour;
                bool IsTrainingNow = CurrentDay < StopTrainingDay;
                int ChoosenClassByPortfolioClassification = 0; ;

                // apply discounting first to all sections
                if (Options.discounted)
                {
                    //var indexes = (from o in Occupancies where o.Day >= item.Day && o.Hour >= item.Hour && o.Section == item.Section select o.index).ToArray();
                    //foreach (var i in indexes)
                    //{
                    //    Occupancies[i].MaxVoteOccupancyTag = 1;
                    //    Occupancies[i].PredictedOccupancyTag = 1;
                    //    Occupancies[i].RandomOccupancyTag = 1;
                    //}
                    PredictedTag[CurrentSection - 1] = 1;
                    MaxvoteTag[CurrentSection - 1] = 1;
                    RandomTag[CurrentSection - 1] = 1;
                }

                RandomTag[CurrentSection - 1] = random.Next(1, DataSimulation.DefaultGroupSimulationOptions.TagOccupancies.Length);

                if (tags.Count > 0)
                {
                    MaxvoteTag[CurrentSection - 1] = GetMaxVoteTag(tags);

                    stopwatch.Start();
                    
                    // let's predict the tag
                    switch (experimentMethodCode)
                    {
                        case 10:
                            var tp1 = new MLEProcessor();
                            PredictedTag[CurrentSection - 1] = tp1.ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                            tp1.UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                            break;
                        case 11:
                            var tp2 = new BayesianProcessor();
                            PredictedTag[CurrentSection - 1] = tp2.ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                            tp2.UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                            break;
                        case 20:
                            var tp3 = new GompertzProcessor();
                            PredictedTag[CurrentSection - 1] = tp3.ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                            tp3.UpdateTrustFromObservation(tags, Users);
                            break;
                        case 21:
                            var tp4 = new RobustAveragingProcessor();
                            PredictedTag[CurrentSection - 1] = tp4.ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                            tp4.UpdateTrustFromObservation(tags, Users);
                            break;
                        case 23:
                            var tp5 = new BetaProcessor();
                            PredictedTag[CurrentSection - 1] = tp5.ProcessTags(tags, Users, Options, RealTag, IsTrainingNow, SectionRValue, SectionSValue);
                            tp5.UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                            break;
                        case 32:  // portfolio classification
                        case 31: // portfolio regression
                            int FolioTag = RealTag;
                            if (!IsTrainingNow)
                            {
                                int[] tgs = new int[5];
                                tgs[0] = PredictedTag[CurrentSection - 1] = new TagProcessors.MLEProcessor().ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                                tgs[1] = PredictedTag[CurrentSection - 1] = new TagProcessors.BayesianProcessor().ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                                tgs[2] = PredictedTag[CurrentSection - 1] = new TagProcessors.GompertzProcessor().ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                                tgs[3] = PredictedTag[CurrentSection - 1] = new TagProcessors.RobustAveragingProcessor().ProcessTags(tags, Users, Options, RealTag, IsTrainingNow);
                                tgs[4] = PredictedTag[CurrentSection - 1] = new TagProcessors.BetaProcessor().ProcessTags(tags, Users, Options, RealTag, IsTrainingNow, SectionRValue, SectionSValue);


                                bool isRegression = experimentMethodCode == 31;
                                int[] algids = new int[] { 10, 11, 20, 21, 23 };

                                int featureID = -1;
                                point = new DataPoint();
                                point.Features = new List<FeatureIndex>();
                                point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Hour", Value = CurrentHour });
                                point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "MaxTrust", Value = Convert.ToDouble(Options.maxTrust) });
                                //point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Pop", Value = Options.pop });
                                //point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Pou", Value = Options.pou });
                                //point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Pot", Value = Options.pot });
                                point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Weekday", Value = Occupancies[CurrentOccupancyIndex].Weekday });
                                point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Section", Value = CurrentSection });
                                if (isRegression)
                                {
                                    point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "MLE", Value = tgs[0] });
                                    point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Bayesian", Value = tgs[1] });
                                    point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Gompertz", Value = tgs[2] });
                                    point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "RobustAveraging", Value = tgs[3] });
                                    point.Features.Add(new FeatureIndex() { ID = featureID++, Name = "Beta", Value = tgs[4] });
                                }

                                if (CurrentDay >= StopTrainingDay && CurrentDay < StopTrainingDay + 7)
                                {
                                    //gather training data                                
                                    if (isRegression)
                                    {
                                        point.Label = RealTag;
                                    }
                                    else
                                    {
                                        point.Label = 0; // maxvote
                                        for (int i = 0; i < algids.Length; i++)
                                            if (RealTag == tgs[i])
                                                point.Label = i + 1;
                                    }
                                    TrainingDataset.DataPoints.Add(point);
                                }
                                else
                                {
                                    var multiclassAdaboost = new MultiClassClassificationMethod(TrainingDataset);
                                    // train classifier
                                    if (tree == null)
                                    {
                                        //make the classifier                                    
                                        tree = multiclassAdaboost.Train(250);
                                    }
                                    // use the classifier

                                    FolioTag = MaxvoteTag[CurrentSection - 1];
                                    try
                                    {
                                        if (isRegression)
                                            FolioTag = multiclassAdaboost.ClassifyDataPoint(point, tree);
                                        else
                                        {
                                            ChoosenClassByPortfolioClassification = multiclassAdaboost.ClassifyDataPoint(point, tree);
                                            if (ChoosenClassByPortfolioClassification != 0)
                                                FolioTag = tgs[ChoosenClassByPortfolioClassification - 1];
                                        }
                                    }
                                    catch { }
                                }
                            }
                            PredictedTag[CurrentSection - 1] = FolioTag;

                            if (IsTrainingNow)
                            {
                                var tp6 = new BetaProcessor();
                                tp6.UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                            }
                            else
                            {
                                if (experimentMethodCode == 32)
                                {
                                    switch (ChoosenClassByPortfolioClassification)
                                    {
                                        case 1:
                                            new MLEProcessor().UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                                            break;
                                        case 2:
                                            new BayesianProcessor().UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                                            break;
                                        case 3:
                                            new GompertzProcessor().UpdateTrustFromObservation(tags, Users);
                                            break;
                                        case 4:
                                            new RobustAveragingProcessor().UpdateTrustFromObservation(tags, Users);
                                            break;
                                        case 5:
                                            new BetaProcessor().UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                                            break;
                                        default:
                                            new BetaProcessor().UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                                            break;
                                    }
                                }
                                if (experimentMethodCode == 31)
                                {
                                    new BetaProcessor().UpdateTrustFromObservation(tags, Users, PredictedTag[CurrentSection - 1]);
                                }
                            }
                            break;
                        default:
                            throw new Exception("experimentMethodCode is not defined");
                    }
                    stopwatch.Stop();
                    //db.ExecuteNonQuery
                }
                if (PredictedTag[CurrentSection - 1] == 0)
                    throw new Exception();
                Occupancies[CurrentOccupancyIndex].MaxVoteOccupancyTag = MaxvoteTag[CurrentSection - 1];
                Occupancies[CurrentOccupancyIndex].PredictedOccupancyTag = PredictedTag[CurrentSection - 1];
                Occupancies[CurrentOccupancyIndex].RandomOccupancyTag = RandomTag[CurrentSection - 1];
                Occupancies[CurrentOccupancyIndex].IsTrainingDay = CurrentDay < StopTrainingDay;
                Occupancies[CurrentOccupancyIndex].CountInEvaluation = true;
            }
            return new MethodPerformance(Occupancies, Users, StopTrainingDay, stopwatch.ElapsedTicks / sectionTable.Count);
        }

        private int GetMaxVoteTag(List<UserTagReport> tags)
        {
            var Query = from r in tags
                        orderby r.Tag
                        group r by r.Tag into grp
                        select new { key = grp.Key, cnt = grp.Count() };
            var List = Query.ToList();

            var MaxCount = (from r in List select r.cnt).Max();
            var MaxVote = (from r in List
                           where r.cnt == MaxCount
                           select r.key).ToList();
            return MaxVote[0];
        }
    }
}
