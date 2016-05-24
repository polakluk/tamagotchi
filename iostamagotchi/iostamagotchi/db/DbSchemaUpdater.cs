using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Phone.Data.Linq;
using System.Reflection;

namespace iostamagotchi
{
    /// <summary>
    /// This class makes sure that DB schema in device is up-to-date
    /// </summary>
    public abstract class DbSchemaUpdater
    {
        /// <summary>
        /// Initialize all DataContexts
        /// </summary>
        static public void Init()
        {
            bool onlyData = false;
            int act_version = -1;

            // initialize Cards
            using (CardsDataContext dc = new CardsDataContext())
            {
                if (dc.DatabaseExists())
                {
                    onlyData = true;
                }
                else
                {
                    dc.CreateDatabase();
                }
            }

            using (CardsDataContext dc = new CardsDataContext())
            {
                DatabaseSchemaUpdater dcUpdater = dc.CreateDatabaseSchemaUpdater();
                act_version = dcUpdater.DatabaseSchemaVersion;
            }
            DbSchemaUpdater.Update(onlyData, CardsDataContext.Prefix, act_version, CardsDataContext.Version);

            // initialize Plans
            using (PlansDataContext dc = new PlansDataContext())
            {
                if (dc.DatabaseExists())
                {
                    onlyData = true;
                }
                else
                {
                    dc.CreateDatabase();
                }
            }

            using (PlansDataContext dc = new PlansDataContext())
            {
                DatabaseSchemaUpdater dcUpdater = dc.CreateDatabaseSchemaUpdater();
                act_version = dcUpdater.DatabaseSchemaVersion;
            }
            DbSchemaUpdater.Update(onlyData, PlansDataContext.Prefix, act_version, PlansDataContext.Version);

            // initialize Scores
            using (ScoresDataContext dc = new ScoresDataContext())
            {
                if (dc.DatabaseExists())
                {
                    onlyData = true;
                }
                else
                {
                    dc.CreateDatabase();
                }
            }

            using (ScoresDataContext dc = new ScoresDataContext())
            {
                DatabaseSchemaUpdater dcUpdater = dc.CreateDatabaseSchemaUpdater();
                act_version = dcUpdater.DatabaseSchemaVersion;
            }
            DbSchemaUpdater.Update(onlyData, ScoresDataContext.Prefix, act_version, ScoresDataContext.Version);


        }

        /// <summary>
        /// Performs update on DB schema
        /// </summary>
        /// <param name="OnlyData">If only updates with data should be performed</param>
        /// <param name="ActVersion">Actual version of DataContext</param>
        /// <param name="FinalVersion">Final version of DataContext</param>
        /// <param name="Prefix">Prefix of DataContext</param>
        static public void Update(bool OnlyData, string Prefix, int ActVersion, int FinalVersion)
        {

            if (ActVersion < FinalVersion)
            {
                ActVersion++; // we need to move to the next version, otherwise we would perform update for the current version twice

                while (ActVersion <= FinalVersion)
                {
                    Type act_t = App.CurrentTypes.GetType(Prefix + "DbUpdateV" + ActVersion.ToString());
                    if (act_t == null) // we couldnt find this type -> so skip it
                    {
                        ActVersion++;
                        continue;
                    }

                    IDbUpdate update = Activator.CreateInstance(act_t) as IDbUpdate;
                    if (OnlyData)
                    {
                        if (update.OnlyData())
                            update.PerformUpdate();
                    }
                    else
                    {
                        update.PerformUpdate();
                    }
                    ActVersion++;
                }
            }
        }
    }
}
