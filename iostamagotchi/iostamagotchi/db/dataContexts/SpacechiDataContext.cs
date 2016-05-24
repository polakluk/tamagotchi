using System;
using System.Net;
using System.Data.Linq;

namespace iostamagotchi
{
    /// <summary>
    /// DataContext working with local SQL CE DB
    /// </summary>
    public class SpacechiDataContext : DataContext
    {
        /// <summary>
        /// Data context prefix for update files
        /// </summary>
        static public string Prefix = "";

        /// <summary>
        /// Actual version of Db schema
        /// </summary>
        static public int Version = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connect">Connection string</param>
        public SpacechiDataContext(string connect)
            : base(connect)
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public SpacechiDataContext()
            : base("Data Source=isostore:/spacechi.sdf")
        {
        }
    }
}
