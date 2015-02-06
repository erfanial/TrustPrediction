using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarloOptimizerMultiThreadedCPUOnly
{
    public class UserUpdate
    {
        public int update_id;
        public int user_id;
        public int section;
        public float tag;
        public float timestamp;

        public UserUpdate(int id = -1, int uid = -1, int sec = -1, float t = -1, float time = -1)
        {
            update_id = id;
            user_id = uid;
            section = sec;
            tag = t;
            timestamp = time;
        }
    }
}
