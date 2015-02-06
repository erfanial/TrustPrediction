using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation.TagProcessors
{
    class MLEProcessor
    {
        DataSimulation.SectionOccupancyValueToTagPolicy policy = new DataSimulation.SectionOccupancyValueToTagPolicy();
        ExperimentInputParams param = new ExperimentInputParams();

        ///// see Docs\Creating Bayesian Lookup Tables\createMLE_DeltaPerTrust.m
        //////// 5 tags
        //////private double[] continiousReferenceDeltaPerTrust = new double[] { 1.95524114583566, 1.95457365930755, 1.95343465781191, 1.95180147675407, 1.94965058759322, 1.94695758919216, 1.94369720673151, 1.93984330012091, 1.93536888416158, 1.93024616306868, 1.92444658234492, 1.91794090140323, 1.91069929075578, 1.90269145800447, 1.89388680726215, 1.88425463697529, 1.87376438136776, 1.86238590083152, 1.85008982649071, 1.83684796378417, 1.82263375916076, 1.80742283276189, 1.79119357817350, 1.77392782786280, 1.75561157968571, 1.73623577580257, 1.71579712046585, 1.69429891751743, 1.67175190222821, 1.64817503563828, 1.62359622326746, 1.59805291458072, 1.57159253567174, 1.54427270613389, 1.51616119290966, 1.48733555984584, 1.45788248229277, 1.42789671153307, 1.39747969370765, 1.36673787114883, 1.33578071884419, 1.30471859271870, 1.27366048670613, 1.24271180930623, 1.21197229503901, 1.18153416038698, 1.15148059729423, 1.12188467148733, 1.09280866073306, 1.06430383368486, 1.03641063761483, 1.00915923703366, 0.982570327672468, 0.956656142507803, 0.931421567629082, 0.906865293541914, 0.882980939023879, 0.859758097032036, 0.837183263281598, 0.815240616970072, 0.793912629858917, 0.773180485574180, 0.753024297007127, 0.733423117477457, 0.714354751768246, 0.695795386393214, 0.677719073793256, 0.660097121072891, 0.642897448296477, 0.626083991930751, 0.609616233535105, 0.593448930588425, 0.577532114629505, 0.561811402072406, 0.546228636788358, 0.530722853587922, 0.515231521609836, 0.499692000071786, 0.484043119260407, 0.468226789588518, 0.452189542426157, 0.435883918354940, 0.419269640433274, 0.402314539852304, 0.384995235939631, 0.367297607974156, 0.349217128290296, 0.330759150082753, 0.311939255065445, 0.292783762879161, 0.273330485109962, 0.253629773517149, 0.233745867900797, 0.213758497660017, 0.193764634655661, 0.173880232509106, 0.154241714476781, 0.135006882171031, 0.116354805918634 };
        
        // 3 tags
        private double[] continiousReferenceDeltaPerTrust = new double[] { 1.351, 1.32467, 1.37233, 1.32433, 1.361, 1.331, 1.292, 1.30767, 1.32367, 1.29867, 1.33633, 1.298, 1.31567, 1.26167, 1.30333, 1.302, 1.294, 1.30067, 1.26767, 1.302, 1.252, 1.25733, 1.257, 1.292, 1.33033, 1.215, 1.204, 1.239, 1.201, 1.174, 1.207, 1.17167, 1.14467, 1.14233, 1.15667, 1.04533, 1.08, 1.06067, 1.02067, 1.03233, 1.05, 0.971, 0.961333, 0.950667, 0.917667, 0.925667, 0.849, 0.832333, 0.822667, 0.765333, 0.768667, 0.758333, 0.702667, 0.667667, 0.634, 0.662, 0.62, 0.588333, 0.570667, 0.515333, 0.520333, 0.524667, 0.484, 0.453, 0.425667, 0.410667, 0.403, 0.393333, 0.373667, 0.344333, 0.315333, 0.309, 0.300333, 0.275333, 0.268333, 0.249667, 0.232, 0.209333, 0.201, 0.185, 0.187333, 0.170667, 0.153333, 0.140667, 0.133667, 0.111667, 0.104667, 0.0956667, 0.0823333, 0.0723333, 0.0586667, 0.0496667, 0.0576667, 0.0363333, 0.0253333, 0.0273333, 0.0183333, 0.0163333, 0.0123333 };

        public int ProcessTags(List<UserTagReport> tags, List<User> Users, ExperimentOptions Options, int RealTag, bool isTraining)
        {
            int PredictedTag;
            if (isTraining)
            {
                PredictedTag = RealTag;
            }
            else
            {
                if (Options.maxTrust)
                {
                    PredictedTag = (from report in tags orderby Users[report.UserID].PredictedTrust descending,report.RandomColumnValue select report.Tag).ToArray()[0];
                }
                else
                {
                    double sum1 = 0, sum2 = 0;
                    int t;
                    foreach (var report in tags)
                    {
                        t = Users[report.UserID].PredictedTrust;
                        sum1 += t * report.Tag;
                        sum2 += t;
                    }
                    double a1 = (sum1 / sum2 - 1) / (DataSimulation.DefaultGroupSimulationOptions.TagOccupancies.Length - 1 - 1);
                    PredictedTag = policy.ConvertOccupancyValueToTag(a1, DataSimulation.DefaultGroupSimulationOptions.TagOccupancies);
                }
            }

            return PredictedTag;
        }

        public void UpdateTrustFromObservation(List<UserTagReport> tags, List<User> Users, int PredictedTag) // workes on updates
        {
            double min_dist, dist, delta_star;
            int selected_id, user_id;
            foreach (var report in tags)
            {
                min_dist = double.PositiveInfinity;
                selected_id = -1;
                user_id = report.UserID;
                Users[user_id].MLE_SUM += Math.Pow(report.Tag - PredictedTag, 2);
                Users[user_id].NTags++;

                delta_star = Math.Sqrt(Users[user_id].MLE_SUM / (double)Users[user_id].NTags);

                for (int j = 0; j < continiousReferenceDeltaPerTrust.Length; j++)
                {
                    dist = Math.Pow(continiousReferenceDeltaPerTrust[j] - delta_star, 2);
                    if (dist < min_dist)
                    {
                        min_dist = dist;
                        selected_id = j;
                    }
                }
                Users[user_id].PredictedTrust = (int)(param.possibleTrusts[selected_id] * 100);
            }
        }
    }
}
