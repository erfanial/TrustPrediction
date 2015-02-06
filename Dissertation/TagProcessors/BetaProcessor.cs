using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissertation.TagProcessors
{
    class BetaProcessor
    {        
        DataSimulation.SectionOccupancyValueToTagPolicy policy = new DataSimulation.SectionOccupancyValueToTagPolicy();
        ExperimentInputParams param = new ExperimentInputParams();


        double lamda_section = 0.2; // forgetting for parking section (forget a lot)
        double lamda_user = 0.95; // forgetting for user. 1:remember everything 0:remember nothing

        public int ProcessTags(List<UserTagReport> tags, List<User> Users, ExperimentOptions Options, int RealTag, bool isTraining, List<double> SectionRValue, List<double> SectionSValue)
        {
            int PredictedTag;
            int CurrentSection = tags[0].Section;

            double user_tag;
            int user_id;
            double user_trust;
            double rsum = 0;
            double ssum = 0;
            double mxt = -1;
            double sro, sso, r, s, E;
            double MaxVoteTag = 1;

            if (isTraining)
            {
                PredictedTag = RealTag;
            }
            else
            {
                sro = SectionRValue[CurrentSection - 1];
                sso = SectionSValue[CurrentSection - 1];

                for (int i = 0; i < tags.Count; i++)
                {
                    user_id = tags[i].UserID;
                    user_tag = tags[i].Tag;
                    user_trust = Users[user_id].PredictedTrust;

                    r = user_tag;
                    s = param.maxTagOptions - user_tag + 1;

                    if (!Options.maxTrust)
                    {
                        rsum += r * user_trust;
                        ssum += s * user_trust;
                    }
                    else
                    {
                        if (user_trust > mxt)
                        {
                            mxt = user_trust;
                            rsum = r;
                            ssum = s;
                        }
                    }
                }

                rsum += lamda_section * sro;
                ssum += lamda_section * sso;

                E = rsum / (rsum + ssum);
                PredictedTag = policy.ConvertOccupancyValueToTag(E, DataSimulation.DefaultGroupSimulationOptions.TagOccupancies);
                SectionRValue[CurrentSection - 1] = rsum;
                SectionSValue[CurrentSection - 1] = ssum;                
            }            

            return PredictedTag;
        }

        public void UpdateTrustFromObservation(List<UserTagReport> tags, List<User> Users, int PredictedTag) // workes on updates
        {
            double user_tag;
            int user_id;
            double uro, uso, v, r, s, r_new, s_new, E;
            for (int i = 0; i < tags.Count; i++)
            {
                user_id = tags[i].UserID;
                user_tag = tags[i].Tag;

                uro = Users[user_id].R;
                uso = Users[user_id].S;
                v = Math.Abs(PredictedTag - user_tag);
                r = param.maxTagOptions - v;
                s = v + 1; // mintag = 1 (always!!!)
                r_new = r + lamda_user * uro;
                s_new = s + lamda_user * uso;
                E = r_new / (r_new + s_new);
                Users[user_id].PredictedTrust = (int)Math.Ceiling(E * 99);
                Users[user_id].R = r_new;
                Users[user_id].S = s_new;
            }
        }
    }
}
