using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dissertation;
using DataSimulation;

namespace TrustPredictionRealData
{
    class RealDataTransfer
    {
        public RealDataTransfer()
        {

        }

        public void TransferDataFromKparkServerToLocalMsSql()
        {
            var serverDb = new MySqlConnector();
            var localDb = new Dissertation.SqlServerConnector();
            string query;

            localDb.ExecuteNonQuery("truncate table realUserUpdates");

            //query = "SELECT * FROM entries WHERE uid not in (1,44)";
            query = "SELECT entries.uid,entries.sid,entries.datetime,sectionnumber,occupancy FROM entries JOIN sections2 on entries.sid=sections2.sid JOIN parkings on parkings.pid=sections2.pid WHERE uid not in (1,44) and datetime>'2014-9-17'"; //  and parkings.pid = 21 and floor=1
            var RealDataEntries = serverDb.ExecuteSelect(query);

            // get section real tags
            query = "SELECT dbo.cardata.id, dbo.cardata.occupancy, dbo.cardata.day, dbo.cardata.weekday, dbo.cardata.hour, dbo.sectionProbabilities.section_id, dbo.sectionProbabilities.hour_id, dbo.sectionProbabilities.prob FROM dbo.cardata INNER JOIN dbo.sectionProbabilities ON dbo.cardata.id = dbo.sectionProbabilities.hour_id";
            var result2 = localDb.ExecuteQuery(query);

            Dictionary<string, int> HourSectionTagHash = HashHourSections(result2);

            Random random = new Random();
            string uid,sectionNumber,tag,day,hour,weekday, realtag;
            DateTime timeOfTag;
            TimeSpan span;
            localDb.ExecuteNonQuery("truncate table realUserUpdates");
            localDb.ExecuteNonQuery("truncate table realExperiments");
            for (int i = 0; i < RealDataEntries["uid"].Count; i++)
            {
                timeOfTag = DateTime.Parse(RealDataEntries["datetime"][i]);
                span = timeOfTag - DateTime.Parse("9/15/2014"); // first Monday before the experiment started (day 0 is monday)
                span = span.Add(new TimeSpan(2, 0, 0)); // colorado time difference

                day = span.Days.ToString();
                weekday = determinWeekDay(timeOfTag.DayOfWeek.ToString()).ToString();
                hour = span.Hours.ToString();
                sectionNumber = RealDataEntries["sectionnumber"][i];
                realtag = HourSectionTagHash[day + "_" + weekday + "_" + hour + "_" + sectionNumber].ToString();

                query = string.Format("INSERT INTO [dbo].[realUserUpdates] ([usergroup] ,[uid] ,[user_section] ,[real_section] ,[user_tag] ,[real_tag] ,[user_trust] ,[day] ,[weekday] ,[hour] ,[randcol] ,[randusercol], [date_received]) VALUES ({0} ,{1} ,{2} ,{3} ,{4} ,{5} ,{6} ,{7} ,{8} ,{9} ,{10} ,{11}, '{12}')",
                     0, RealDataEntries["uid"][i], sectionNumber, RealDataEntries["sectionnumber"][i], RealDataEntries["occupancy"][i], realtag, 50, day, weekday, hour, random.NextDouble(), random.NextDouble(), RealDataEntries["datetime"][i]);
                localDb.ExecuteNonQuery(query);
            }

            
        }

        private Dictionary<string, int> HashHourSections(Dictionary<string, List<string>> RawDatabaseData)
        {
            var occupancyValueConverter = new SectionOccupancyValueToTagPolicy(); //.ConvertOccupancyValueToTag(SectionOccupancy.OccupancyValue, DefaultGroupSimulationOptions.TagOccupancies);

            string hash_value = "";
            int occupancy_tag = 0;
            var ret = new Dictionary<string, int>();
            var policy = new double[]{0,0.33,0.66,1};
            for (int i = 0; i < RawDatabaseData["id"].Count; i++)
            {
                hash_value = RawDatabaseData["day"][i] + "_" + RawDatabaseData["weekday"][i] + "_" + RawDatabaseData["hour"][i] + "_" + RawDatabaseData["section_id"][i];// +"_" + RawDatabaseData["hour_id"][i];
                occupancy_tag = occupancyValueConverter.ConvertOccupancyValueToTag(double.Parse(RawDatabaseData["prob"][i]), /*DefaultGroupSimulationOptions.TagOccupancies*/ policy);
                ret[hash_value] = occupancy_tag;
            }
            return ret;
        }

        private int determinWeekDay(string dayOfTheWeek)
        {
            int weekday = 0;
            switch (dayOfTheWeek)
            {
                case "Monday":
                    weekday = 1;
                    break;
                case "Tuesday":
                    weekday = 2;
                    break;
                case "Wednesday":
                    weekday = 3;
                    break;
                case "Thursday":
                    weekday = 4;
                    break;
                case "Friday":
                    weekday = 5;
                    break;
                case "Saturday":
                    weekday = 6;
                    break;
                case "Sunday":
                    weekday = 7;
                    break;
            }
            return weekday;
        }
    }
}
