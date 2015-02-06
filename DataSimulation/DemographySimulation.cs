using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSimulation
{
    public class DemographySimulation
    {
        private List<SectionOccupancy> SectionOccupancies;
        private List<User> Users;
        private List<Update> UserUpdates;
        GroupSimulationOptions Options;

        string query;
        SqlServerConnector db = new SqlServerConnector();
        Dictionary<string, List<string>> dbResult;
        Random random = new Random();

        public DemographySimulation(GroupSimulationOptions options)
        {
            Options = options;
            SectionOccupancies = ReadSectionOccupanciesFromDatabase();
            SimulateUsersAndUpdates();
        }

        private void SimulateUsersAndUpdates()
        {
            query = "SELECT [uid] ,[day] ,[weekday] ,[hour] FROM [DissertationSimDB].[dbo].[usefulUpdates] order by day,hour";
            dbResult = db.ExecuteQuery(query);
            var RawUsers = dbResult["uid"].Distinct().ToList();
            var NumberOfSections = 16;
            

            Users = new List<User>();
            UserUpdates = new List<Update>();
            foreach (var RawUserID in RawUsers)
            {
                var user = new User();
                user.ID = int.Parse(RawUserID);
                user.UserRandomColumn = random.NextDouble(); // not used in this portion of experiment
                user.Trust = InitiateRandomTrustForUser();
                Users.Add(user);
            }

            InitialSectionAssigner sectionAssigner = new InitialSectionAssigner(Options.SectionPriorities, Options.SectionAssignmentProbabilityMap);
            InitialTagAssigner tagAssigner = new InitialTagAssigner(Options.TagOccupancies.Length - 1);

            //////////////////// create a hash table for finding the section occupancy for a given day or hour
            //var totalDays = (from o in SectionOccupancies select o.Day).Distinct().ToList();
            //var totalHours = (from o in SectionOccupancies select o.Hour).Distinct().ToList();
            //var totalSections = (from o in SectionOccupancies select o.Section).Distinct().ToList();
            //var DayHourOccupancyHash = new int[totalDays.Count,totalHours.Count,totalSections.Count];
            //var array = (from o in SectionOccupancies orderby o.Day, o.Hour, o.Section select o).ToArray();
            //int counter = 0;
            //for (int i = 0; i < totalDays.Count; i++)
            //    for (int j = 0; j < totalHours.Count; j++)
            //        for (int k = 0; k < totalSections.Count; k++)
            //            {
            //                try
            //                {
            //                    DayHourOccupancyHash[i, j, k] = array[counter++].OccupancyTag;
            //                }
            //                catch { }
            //            }
            ////////////////////
            for (int i = 0; i < dbResult["uid"].Count; i++)
            {
                try
                {
                    Console.WriteLine(i);
                    var update = new Update();
                    update.Day = int.Parse(dbResult["day"][i]);
                    update.UserID = int.Parse(dbResult["uid"][i]);
                    update.Weekday = int.Parse(dbResult["weekday"][i]);
                    update.Hour = int.Parse(dbResult["hour"][i]);

                    int Section, RealSection;
                    sectionAssigner.RandomlyInitializeSectionInformation(out Section, out RealSection);
                    update.RealSection = RealSection;
                    update.Section = Section;


                    var realtag = (from o in SectionOccupancies where o.Hour == update.Hour && o.Day == update.Day && o.Section == update.RealSection select o.OccupancyTag).ToList();
                    update.RealTag = realtag[0];

                    var usertrust = (from u in Users where u.ID == update.UserID select u.Trust).ToList();
                    update.Tag = tagAssigner.AssignTag(update.RealTag, usertrust[0]);

                    UserUpdates.Add(update);
                }
                catch { }
            }            
        }

        private int InitiateRandomTrustForUser()
        {
            int trust = -1;
            trust = Options.TrustInitializer.CreateInitialTrust();

            return trust;
        }

        private List<SectionOccupancy> ReadSectionOccupanciesFromDatabase()
        {
            List<SectionOccupancy> SectionOccupancies = new List<SectionOccupancy>();
            query = "SELECT dbo.sectionProbabilities.section_id, dbo.sectionProbabilities.prob, dbo.cardata.day, dbo.cardata.weekday, dbo.cardata.hour FROM dbo.cardata INNER JOIN dbo.sectionProbabilities ON dbo.cardata.id = dbo.sectionProbabilities.hour_id ORDER BY dbo.sectionProbabilities.section_id, dbo.cardata.day, dbo.cardata.hour";
            dbResult = db.ExecuteQuery(query);
            for (int i = 0; i < dbResult["section_id"].Count; i++)
            {
                var SectionOccupancy = new SectionOccupancy();
                SectionOccupancy.Section = int.Parse(dbResult["section_id"][i]);
                SectionOccupancy.Day = int.Parse(dbResult["day"][i]);
                SectionOccupancy.Weekday = int.Parse(dbResult["weekday"][i]);
                SectionOccupancy.Hour = int.Parse(dbResult["hour"][i]);
                SectionOccupancy.OccupancyValue = double.Parse(dbResult["prob"][i]);
                SectionOccupancy.OccupancyTag = new SectionOccupancyValueToTagPolicy().ConvertOccupancyValueToTag(SectionOccupancy.OccupancyValue, Options.TagOccupancies);
                SectionOccupancies.Add(SectionOccupancy);
            }
            return SectionOccupancies;
        }

        public void StoreGroupInDatabase()
        {
            SqlServerConnector db = new SqlServerConnector();
            db.ExecuteQuery("DELETE FROM demographies WHERE ID= " + Options.ID);
            db.ExecuteQuery("INSERT INTO demographies (ID ,GroupName) VALUES (" + Options.ID + " ,'" + Options.Name + "')");
            db.ExecuteQuery("DELETE FROM simulatedUserUpdates WHERE ID= " + Options.ID);

            var joined = (from update in UserUpdates
                        join user in Users
                             on update.UserID equals user.ID
                        select new
                        {
                            update.UserID,
                            update.RealSection,
                            update.Section,
                            update.RealTag,
                            update.Tag,
                            user.Trust,
                            update.Day,
                            update.Weekday,
                            update.Hour,
                            user.UserRandomColumn
                        }).ToList();
            foreach (var item in joined)
                db.ExecuteNonQuery(string.Format("INSERT INTO [dbo].[simulatedUserUpdates] ([usergroup] ,[uid] ,[user_section] ,[real_section] ,[user_tag] ,[real_tag] ,[user_trust] ,[day] ,[weekday] ,[hour] ,[randcol] ,[randusercol]) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})", 
                    Options.ID, item.UserID, item.Section, item.RealSection, item.Tag, item.RealTag, item.Trust, item.Day, item.Weekday, item.Hour, random.NextDouble(), item.UserRandomColumn));
        }

    }
}
