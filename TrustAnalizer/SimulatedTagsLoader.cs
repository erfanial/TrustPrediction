using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dissertation;
using DataSimulation;


namespace TrustAnalizer
{
    class SimulatedTagsLoader
    {
        private Random random;

        public SimulatedTagsLoader()
        {
            random = new Random();
        }

        public List<UserTagReport> simulate(int nUsers, int nTagsPerUser)
        {
            var dso = new GroupSimulationOptions();
            dso.TrustInitializer = new InitialUniformTrustAssignment(0, 1);
            dso.SectionPriorities = DefaultGroupSimulationOptions.SectionPriorities;
            dso.SectionAssignmentProbabilityMap = DefaultGroupSimulationOptions.SectionAssignmentProbabilityMap;
            dso.TagOccupancies = DefaultGroupSimulationOptions.TagOccupancies;
            dso.Name = "Uniform_AllTrusts";
            
            
            InitialTagAssigner tagAssigner = new InitialTagAssigner(dso.TagOccupancies.Length - 1);
            List<UserTagReport> UserUpdates = new List<UserTagReport>();
            int maxTags = dso.TagOccupancies.Length - 1;

            for (int u = 0; u < nUsers; u++)
            {
                int ntags = (int)Math.Ceiling(nTagsPerUser * random.NextDouble());
                int trust = (int)Math.Ceiling(99 * random.NextDouble());
                int realtag = (int)Math.Ceiling(maxTags * random.NextDouble());
                for (int tag = 0; tag < ntags; tag++)
                {
                    var update = new UserTagReport();
                    update.RealTag = realtag;
                    update.Tag = tagAssigner.AssignTag(realtag, trust);
                    update.UserTrust = trust;
                    update.Section = 1;
                    update.UserID = u;
                    UserUpdates.Add(update);
                }
            }          

            return UserUpdates;
        }
    }
}
