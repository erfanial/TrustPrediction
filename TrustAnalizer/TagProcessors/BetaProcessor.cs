using Dissertation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustAnalizer.TagProcessors
{
    class BetaProcessor : TrustPredictionMethod
    {
        private List<UserTagReport> _tags;


        private User _user;
        private ExperimentInputParams inputParams = new ExperimentInputParams();
        double lamda_section = 0.2; // forgetting for parking section (forget a lot)
        double lamda_user = 0.95; // forgetting for user. 1:remember everything 0:remember nothing
        private DataSimulation.SectionOccupancyValueToTagPolicy policy = new DataSimulation.SectionOccupancyValueToTagPolicy();

        public override int ProcessTags(List<UserTagReport> tags)
        {
            _tags = tags;

            int RealTag;
            RealTag = tags[0].RealTag; // for all tags, the realtag is the same                        

            _user = new User(tags[0].UserID, tags[0].UserTrust, 0);

            //updateTrustFromObservation   
            updateTrustFromObservation(RealTag);

            int performance = 100 - Math.Abs(_user.PredictedTrust - _user.RealTrust);

            return performance;
        }

        public override void updateTrustFromObservation(int PredictedTag) // workes on updates
        {
            double user_tag;
            int user_id;
            double uro, uso, v, r, s, r_new, s_new, E;
            for (int i = 0; i < _tags.Count; i++)
            {
                user_id = _tags[i].UserID;
                user_tag = _tags[i].Tag;

                uro = _user.R;
                uso = _user.S;
                v = Math.Abs(PredictedTag - user_tag);
                r = inputParams.maxTagOptions - v;
                s = v + 1; // mintag = 1 (always!!!)
                r_new = r + lamda_user * uro;
                s_new = s + lamda_user * uso;
                E = r_new / (r_new + s_new);
                _user.PredictedTrust = (int)Math.Ceiling(E * 99);
                _user.R = r_new;
                _user.S = s_new;
            }
        }
    }
}
