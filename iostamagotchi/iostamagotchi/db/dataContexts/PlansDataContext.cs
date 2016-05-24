using System;
using System.Net;
using System.Data.Linq;

namespace iostamagotchi
{
    public class PlansDataContext : SpacechiDataContext
    {
        /// <summary>
        /// Prefix of this DataContext
        /// </summary>
        static public string Prefix = "Plan";

        /// <summary>
        /// Actual version of Db schema
        /// </summary>
        static  public int Version = 1;

        public PlansDataContext()
            : base("Data Source=isostore:/plans.sdf")
        {
        }

        public Table<PlanTable> Plans
        {
            get
            {
                return this.GetTable<PlanTable>();
            }
        }
    }
}
