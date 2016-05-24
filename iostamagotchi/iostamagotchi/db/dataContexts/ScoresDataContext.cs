using System;
using System.Net;
using System.Data.Linq;

namespace iostamagotchi
{
    public class ScoresDataContext : SpacechiDataContext
    {
        /// <summary>
        /// Prefix of this DataContext
        /// </summary>
        static public string Prefix = "Score";

        /// <summary>
        /// Actual version of Db schema
        /// </summary>
        static public int Version = 1;

        public ScoresDataContext()
            : base("Data Source=isostore:/scores.sdf")
        {
        }

        public Table<ScoreTable> Scores
        {
            get
            {
                return this.GetTable<ScoreTable>();
            }
        }
    }
}
