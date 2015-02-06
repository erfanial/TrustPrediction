using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Dissertation
{
    public class DissertationMain
    {
        private ExperimentExecutor _experimentExecutor;
        bool runParallel = true;

        public DissertationMain(bool RunInParallel, bool useRealData = false)
        {
            runParallel = RunInParallel;

            if (useRealData)
            {
                Constants.UserUpdatesTable = "realUserUpdates";
                Constants.ExperimentsTable = "realExperiments";
            }
            else
            {
                Constants.UserUpdatesTable = "simulatedUserUpdates";
                Constants.ExperimentsTable = "experiments";
            }
            init();
        }

        private void init()
        {
            // you must first configure SQL Server, create a database called 'DissertationSimDB' and run the DissertationSimDB.sql
            // on it to make the tables and insert all the data

            int[] experimentIDsBase = new int[] { 10,11,20,21,23,31,32 };
            int n = experimentIDsBase.Length;

            SqlServerConnector db = new SqlServerConnector();
            string query;

            query = "select count(distinct usergroup) as NumberOfGroups from " + Constants.UserUpdatesTable;
            
            var result = db.ExecuteQuery(query);
            int nGroups = int.Parse(result["NumberOfGroups"][0]);
            //nGroups = 1;
            int[] experimentIDs = new int[nGroups * n];
            for (int i = 0; i < n; i++)
                for (int g = 0; g < nGroups; g++)
                    experimentIDs[i + n * g] = 100 * g + experimentIDsBase[i];

            //experimentIDs = new int[] { 10, 11, 20, 21, 23, 31, 32 };

            _experimentExecutor = new ExperimentExecutor(experimentIDs);

        }

        public void run()
        {
            ////// running experiments in parallel will result in inaccurate processing times for each algorithm
            ////// only run when you already have the times ready
            
            if(!runParallel)
                _experimentExecutor.execute();
            else
                _experimentExecutor.executeParallel();
        }

    }    
}
