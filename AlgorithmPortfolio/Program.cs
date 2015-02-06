using MachineLearningCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MachineLearningCore.BinaryDecisionTrees;

namespace AlgorithmPortfolio
{
    class Program
    {
        static void Main(string[] args)
        {

            RunClassificationTest();
            RunRegressionTest();

            Console.ReadLine();
        }

        static void RunClassificationTest()
        {
            var folio = new AlgorithmPortfolio();
            var saver = new DataOperations();

            string filename = @"ClassificationDecisionTree.bin";
            var tree = folio.TrainClassifier(folio.ClassificationDataset.TrainingDataSet, 1000);
            saver.SaveClassifierToFile(filename, tree);
            var loadedTree = saver.ReadClassifierFromFile(filename);
            var MulticlassPerformance = folio.TestClassifier(folio.ClassificationDataset.TestingDataSet, loadedTree, false);
            Console.WriteLine(MulticlassPerformance);
        }

        static void RunRegressionTest()
        {
            var folio = new AlgorithmPortfolio();
            var saver = new DataOperations();

            string filename = @"RegressionDecisionTree.bin";
            var tree = folio.TrainClassifier(folio.RegressionDataset.TrainingDataSet, 1000);
            saver.SaveClassifierToFile(filename, tree);
            var loadedTree = saver.ReadClassifierFromFile(filename);
            var MulticlassPerformance = folio.TestClassifier(folio.RegressionDataset.TestingDataSet, loadedTree);
            Console.WriteLine(MulticlassPerformance);
        }
    }
}
