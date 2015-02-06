using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSimulation
{
    class DataSimulation
    {
        public DataSimulation()
        {

        }

        public void Execute()
        {
            var simGroups = new List<GroupSimulationOptions>();
            GroupSimulationOptions dso;

            dso = new GroupSimulationOptions();
            dso.TrustInitializer = new InitialUniformTrustAssignment(0, 1);
            dso.SectionPriorities = DefaultGroupSimulationOptions.SectionPriorities;
            dso.SectionAssignmentProbabilityMap = DefaultGroupSimulationOptions.SectionAssignmentProbabilityMap;
            dso.TagOccupancies = DefaultGroupSimulationOptions.TagOccupancies;
            dso.Name = "Uniform_AllTrusts";
            simGroups.Add(dso);

            dso = new GroupSimulationOptions();
            dso.TrustInitializer = new InitialUniformTrustAssignment(0.9, 1);
            dso.SectionPriorities = DefaultGroupSimulationOptions.SectionPriorities;
            dso.SectionAssignmentProbabilityMap = DefaultGroupSimulationOptions.SectionAssignmentProbabilityMap;
            dso.TagOccupancies = DefaultGroupSimulationOptions.TagOccupancies;
            dso.Name = "Uniform_HighTrusts";
            simGroups.Add(dso);

            dso = new GroupSimulationOptions();
            dso.TrustInitializer = new InitialUniformTrustAssignment(0, 0.1);
            dso.SectionPriorities = DefaultGroupSimulationOptions.SectionPriorities;
            dso.SectionAssignmentProbabilityMap = DefaultGroupSimulationOptions.SectionAssignmentProbabilityMap;
            dso.TagOccupancies = DefaultGroupSimulationOptions.TagOccupancies;
            dso.Name = "Uniform_LowTrusts";
            simGroups.Add(dso);

            dso = new GroupSimulationOptions();
            dso.TrustInitializer = new InitialNomialTrustAssignment(new List<NormalDistributionParams>(){ new NormalDistributionParams(0.25, 0.2), new NormalDistributionParams(0.75, 0.2)}, new List<double>(){0.5,0.5} );
            dso.SectionPriorities = DefaultGroupSimulationOptions.SectionPriorities;
            dso.SectionAssignmentProbabilityMap = DefaultGroupSimulationOptions.SectionAssignmentProbabilityMap;
            dso.TagOccupancies = DefaultGroupSimulationOptions.TagOccupancies;
            dso.Name = "Binomial_1";
            simGroups.Add(dso);

            dso = new GroupSimulationOptions();
            dso.TrustInitializer = new InitialNomialTrustAssignment(new List<NormalDistributionParams>() { new NormalDistributionParams(0.05, 0.02), new NormalDistributionParams(0.95, 0.02) }, new List<double>() { 0.5, 0.5 });
            dso.SectionPriorities = DefaultGroupSimulationOptions.SectionPriorities;
            dso.SectionAssignmentProbabilityMap = DefaultGroupSimulationOptions.SectionAssignmentProbabilityMap;
            dso.TagOccupancies = DefaultGroupSimulationOptions.TagOccupancies;
            dso.Name = "Binomial_2";
            simGroups.Add(dso);

            new SqlServerConnector().ExecuteNonQuery("truncate table simulatedUserUpdates");
            for (int i = 0; i < simGroups.Count; i++)
            {
                dso = simGroups[i];
                dso.ID = i.ToString();

                var ds = new DemographySimulation(dso);
                ds.StoreGroupInDatabase();
            }
        }
    }
}
