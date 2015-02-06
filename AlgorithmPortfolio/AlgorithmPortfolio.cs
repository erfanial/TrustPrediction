using MachineLearningCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MachineLearningCore.BinaryDecisionTrees;

namespace AlgorithmPortfolio
{
    public class AlgorithmPortfolio
    {
        public TrainingTestingDataSet ClassificationDataset;
        public TrainingTestingDataSet RegressionDataset;
        private List<DatabaseRecord> dbdata;
        private double[][] PenaltyMatrix;


        public AlgorithmPortfolio()
        {
            PenaltyMatrix = new double[][] { new double[] { 0, 5, 10 }, new double[] { 1, 0, 4 }, new double[] { 20, 10, 0 } }; // [realTag][predictedTag]
            init();
        }

        private void init()
        {
            var db = new SqlServerConnector();
            var rand = new Random();
            var Occupancies = db.ExecuteQuery("SELECT DISTINCT TOP (100) PERCENT dbo.experiments.algorithm_id, dbo.experiments.population_group, dbo.experiments.max_trust, dbo.experiments.pop, dbo.experiments.pou, dbo.experiments.averagePenalties_maxvote - dbo.experiments.averagePenalties_pred AS LessPenalties, dbo.SimulatedOccupancies.weekday, dbo.SimulatedOccupancies.hour, dbo.SimulatedOccupancies.real_tag, dbo.SimulatedOccupancies.predicted_tag, dbo.SimulatedOccupancies.maxvote_tag, dbo.SimulatedOccupancies.section FROM dbo.experiments INNER JOIN dbo.SimulatedOccupancies ON dbo.experiments.id = dbo.SimulatedOccupancies.experiment_id WHERE (dbo.experiments.discounted = 1) AND (dbo.SimulatedOccupancies.experiment_id IN (SELECT id FROM dbo.experiments AS experiments_1 WHERE (pot = 0))) ORDER BY dbo.experiments.population_group, dbo.experiments.max_trust, dbo.experiments.pop, dbo.experiments.pou, dbo.SimulatedOccupancies.weekday, dbo.SimulatedOccupancies.hour, dbo.experiments.algorithm_id");

            dbdata = new List<DatabaseRecord>(Occupancies.Rows.Count);
            for (int i = 0; i < Occupancies.Rows.Count; i++)
            {
                dbdata.Add(new DatabaseRecord
                {
                    AlgorithmID = Convert.ToInt32(Occupancies.Rows[i][0]),
                    PopulationGroup = Convert.ToInt32(Occupancies.Rows[i][1]),
                    MaxTrust = Convert.ToInt32(Occupancies.Rows[i][1]) == 1,
                    Pop = Convert.ToDouble(Occupancies.Rows[i][3]),
                    Pou = Convert.ToDouble(Occupancies.Rows[i][4]),
                    //LessPenalties = Convert.ToDouble(Occupancies.Rows[i][5]),

                    WeekDay = Convert.ToInt32((byte)Occupancies.Rows[i][6]),
                    Hour = Convert.ToInt32((byte)Occupancies.Rows[i][7]),
                    RealTag = Convert.ToInt32((byte)Occupancies.Rows[i][8]),
                    PredictedTag = Convert.ToInt32((byte)Occupancies.Rows[i][9]),
                    MaxvoteTag = Convert.ToInt32((byte)Occupancies.Rows[i][10]),
                    Section = Convert.ToInt32((byte)Occupancies.Rows[i][11])
                });
            }

            var set = (from x in dbdata group x by new { x.Hour, x.Pop, x.PopulationGroup, x.Pou, x.WeekDay } into g select new { key = g.Key, gr = g }).ToList();
            var b = set[0];

            /////////////////////////// create dataset for Classification
            ClassificationDataset = new TrainingTestingDataSet();
            ClassificationDataset.TrainingDataSet = new DataSet();
            ClassificationDataset.TrainingDataSet.DataPoints = new List<DataPoint>();
            ClassificationDataset.TestingDataSet = new DataSet();
            ClassificationDataset.TestingDataSet.DataPoints = new List<DataPoint>();

            foreach (DatabaseRecord record in dbdata)
            {
                int label = 0; // maxvote
                if (record.PredictedTag == record.RealTag)
                    label = record.AlgorithmID;


                var point = new PortfolioClassificationDataPoint();
                point.Label = label;
                point.Features = new List<FeatureIndex>();

                point.Features.Add(new FeatureIndex() { ID = 0, Name = "Hour", Value = record.Hour });
                point.Features.Add(new FeatureIndex() { ID = 1, Name = "MaxTrust", Value = Convert.ToDouble(record.MaxTrust) });
                point.Features.Add(new FeatureIndex() { ID = 2, Name = "Pop", Value = record.Pop });
                point.Features.Add(new FeatureIndex() { ID = 3, Name = "Pou", Value = record.Pou });
                point.Features.Add(new FeatureIndex() { ID = 4, Name = "PopulationGroup", Value = record.PopulationGroup });
                point.Features.Add(new FeatureIndex() { ID = 5, Name = "Weekday", Value = record.WeekDay });
                point.Features.Add(new FeatureIndex() { ID = 6, Name = "Section", Value = record.Section });


                point.DatabaseRecord = record;

                if (rand.NextDouble() < 0.99)
                    ClassificationDataset.TrainingDataSet.DataPoints.Add(point);
                else
                    ClassificationDataset.TestingDataSet.DataPoints.Add(point);

            }
            //ClassificationDataset.TrainingDataSet.DataPoints = ClassificationDataset.TrainingDataSet.DataPoints.Distinct().ToList();
            //ClassificationDataset.TestingDataSet.DataPoints = ClassificationDataset.TestingDataSet.DataPoints.Distinct().ToList();
            ///////////////////////////////////////////////////////////////

            /////////////////////////// create dataset for Regression
            RegressionDataset = new TrainingTestingDataSet();
            RegressionDataset.TrainingDataSet = new DataSet();
            RegressionDataset.TrainingDataSet.DataPoints = new List<DataPoint>();
            RegressionDataset.TestingDataSet = new DataSet();
            RegressionDataset.TestingDataSet.DataPoints = new List<DataPoint>();

            //var set = dbdata.GroupBy(x => new { x.Hour, x.Pop, x.PopulationGroup, x.Pou, x.WeekDay }).ToList();
            //var a = set[0];

            //foreach (var g in set)
            //{
            //    var record = g.ToList()[0];

            //    //int MleTag = (from r in g where r.AlgorithmID == 10 select r.PredictedTag).First();
            //    //int BayesianTag = (from r in g where r.AlgorithmID == 11 select r.PredictedTag).First();
            //    //int BetaTag = (from r in g where r.AlgorithmID == 23 select r.PredictedTag).First();
            //    //int GompertzTag = (from r in g where r.AlgorithmID == 20 select r.PredictedTag).First();
            //    //int RATag = (from r in g where r.AlgorithmID == 21 select r.PredictedTag).First();


            //    //var point = new PortfolioClassificationDataPoint();
            //    //point.Label = record.RealTag;
            //    //point.Features = new List<FeatureIndex>();

            //    //point.Features.Add(new FeatureIndex() { ID = 1, Name = "Hour", Value = record.Hour });
            //    //point.Features.Add(new FeatureIndex() { ID = 2, Name = "MaxTrust", Value = Convert.ToDouble(record.MaxTrust) });
            //    //point.Features.Add(new FeatureIndex() { ID = 3, Name = "Pop", Value = record.Pop });
            //    //point.Features.Add(new FeatureIndex() { ID = 4, Name = "Pou", Value = record.Pou });
            //    //point.Features.Add(new FeatureIndex() { ID = 5, Name = "PopulationGroup", Value = record.PopulationGroup });
            //    //point.Features.Add(new FeatureIndex() { ID = 6, Name = "MLETag", Value = MleTag });
            //    //point.Features.Add(new FeatureIndex() { ID = 7, Name = "BayesianTag", Value = BayesianTag });
            //    //point.Features.Add(new FeatureIndex() { ID = 8, Name = "BetaTag", Value = BetaTag });
            //    //point.Features.Add(new FeatureIndex() { ID = 9, Name = "GompertTag", Value = GompertzTag });
            //    //point.Features.Add(new FeatureIndex() { ID = 10, Name = "RATag", Value = RATag });
            //    //point.Features.Add(new FeatureIndex() { ID = 11, Name = "MaxvoteTag", Value = record.MaxvoteTag });

            //    //point.DatabaseRecord = record;

            //    //if (rand.NextDouble() < 0.8)
            //    //    RegressionDataset.TrainingDataSet.DataPoints.Add(point);
            //    //else
            //    //    RegressionDataset.TestingDataSet.DataPoints.Add(point);           
            //}

            foreach (DatabaseRecord record in dbdata)
            {
                var point = new PortfolioClassificationDataPoint();
                point.Label = record.RealTag;
                point.Features = new List<FeatureIndex>();

                point.Features.Add(new FeatureIndex() { ID = 1, Name = "Hour", Value = record.Hour });
                point.Features.Add(new FeatureIndex() { ID = 2, Name = "MaxTrust", Value = Convert.ToDouble(record.MaxTrust) });
                point.Features.Add(new FeatureIndex() { ID = 3, Name = "Pop", Value = record.Pop });
                point.Features.Add(new FeatureIndex() { ID = 4, Name = "Pou", Value = record.Pou });
                point.Features.Add(new FeatureIndex() { ID = 5, Name = "PopulationGroup", Value = record.PopulationGroup });
                point.Features.Add(new FeatureIndex() { ID = 6, Name = "Algorithm", Value = record.AlgorithmID });
                point.Features.Add(new FeatureIndex() { ID = 7, Name = "PredictedTag", Value = record.PredictedTag });
                point.Features.Add(new FeatureIndex() { ID = 8, Name = "MaxvoteTag", Value = record.MaxvoteTag });
                point.Features.Add(new FeatureIndex() { ID = 9, Name = "Section", Value = record.Section });
                point.Features.Add(new FeatureIndex() { ID = 10, Name = "Weekday", Value = record.WeekDay });
                //point.Features.Add(new FeatureIndex() { ID = 9, Name = "UseMaxvote", Value = record.LessPenalties });

                point.DatabaseRecord = record;

                if (rand.NextDouble() < 0.99)
                    RegressionDataset.TrainingDataSet.DataPoints.Add(point);
                else
                    RegressionDataset.TestingDataSet.DataPoints.Add(point);

            }
            RegressionDataset.TrainingDataSet.DataPoints = RegressionDataset.TrainingDataSet.DataPoints.Distinct().ToList();
            RegressionDataset.TestingDataSet.DataPoints = RegressionDataset.TestingDataSet.DataPoints.Distinct().ToList();
            ///////////////////////////////////////////////////////////////
        }

        public BinaryDecisionTree TrainClassifier(DataSet Dataset, int trainingRounds = 250)
        {
            var multiclassAdaboost = new MultiClassClassificationMethod(Dataset);
            var tree = multiclassAdaboost.Train(trainingRounds);
            //var p = multiclassAdaboost.Test(Dataset, tree);
            //Console.WriteLine(p.MultiClassVectorDifference + " " + p.MultiClassPerformance);
            return tree;
        }

        public double TestClassifier(DataSet Dataset, BinaryDecisionTree tree, bool isRegression = true)
        {
            double improvement = 0;

            var multiclassAdaboost = new MultiClassClassificationMethod();

            int FolioTag;
            double FolioPenalties = 0, PredictedPenalties = 0, MaxvotePenalties = 0;
            foreach (PortfolioClassificationDataPoint datapoint in Dataset.DataPoints)
            {
                if (isRegression)
                    FolioTag = multiclassAdaboost.ClassifyDataPoint(datapoint, tree);
                else
                {
                    int t = multiclassAdaboost.ClassifyDataPoint(datapoint, tree);
                    if (t == 0)
                        FolioTag = datapoint.DatabaseRecord.MaxvoteTag;
                    else
                        FolioTag = datapoint.DatabaseRecord.PredictedTag;
                }

                FolioPenalties += GetPenalty(datapoint.DatabaseRecord.RealTag, FolioTag);

                MaxvotePenalties += GetPenalty(datapoint.DatabaseRecord.RealTag, datapoint.DatabaseRecord.MaxvoteTag);

                PredictedPenalties += GetPenalty(datapoint.DatabaseRecord.RealTag, datapoint.DatabaseRecord.PredictedTag);
            }

            improvement = (PredictedPenalties - FolioPenalties) / PredictedPenalties;

            return improvement;
        }

        private double GetPenalty(int realtag, int tag)
        {
            return PenaltyMatrix[realtag - 1][tag - 1];
        }
    }
}
