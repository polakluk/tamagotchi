using System;
using System.Net;
using System.Data.Linq;

namespace iostamagotchi
{
    public class CardsDataContext : SpacechiDataContext
    {
        /// <summary>
        /// Prefix of this DataContext
        /// </summary>
        static public string Prefix = "Card";

        /// <summary>
        /// Actual version of Db schema
        /// </summary>
        static public int Version = 2;

        public CardsDataContext()
            : base("Data Source=isostore:/cards.sdf")
        {
        }

        public Table<CardTable> Cards
        {
            get
            {
                return this.GetTable<CardTable>();
            }
        }

        public Table<SetTable> Sets
        {
            get
            {
                return this.GetTable<SetTable>();
            }
        }

        public Table<SourceTable> Sources
        {
            get
            {
                return this.GetTable<SourceTable>();
            }
        }
    }
}
