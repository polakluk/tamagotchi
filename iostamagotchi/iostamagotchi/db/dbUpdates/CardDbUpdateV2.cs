using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Data.Linq;
using System.Linq;
using System.Data.Common;

namespace iostamagotchi
{
    public class CardDbUpdateV2 : IDbUpdate
    {
        /// <summary>
        /// Version of this update
        /// </summary>
        public const int Version = 2;

        /// <summary>
        /// Updates schema and returns result of this operation
        /// </summary>
        /// <param name="dc">Data contxet</param>
        /// <returns>Result of this operation</returns>
        public bool PerformUpdate()
        {
            using (CardsDataContext dc = new CardsDataContext())
            {
                SourceTable st = (from s in dc.Sources where s.ClassName == "Local" select s).FirstOrDefault<SourceTable>();

                SetTable[] set = new SetTable[10];
                for (int i = 0; i < 10; i++)
                {
                    set[i] = new SetTable();
                    st.ListSets.Add(set[i]);
                    set[i].Title = "Set" + i.ToString();
                    set[i].IsEditable = 1;
                    set[i].LastStudied = DateTime.Now;
                    set[i].IsStudying = i % 2 == 0;
                    set[i].MaxPercentage = 0;
                    set[i].RemoteId = null;
                    dc.Sets.InsertOnSubmit(set[i]);
                }
                dc.SubmitChanges();
            }

            for (int i = 0; i < 10; i++)
            {
                using (CardsDataContext dc = new CardsDataContext())
                {
                    SetTable st = (from s in dc.Sets where s.Title == "Set" + i.ToString() select s).FirstOrDefault<SetTable>();
                    CardTable[] ct = new CardTable[5];
                    for (int j = 0; j < 5; j++)
                    {
                        ct[j] = new CardTable();
                        ct[j].FrontSide = "Front_" + i.ToString() + "_" + j.ToString();
                        ct[j].BackSide = "Back_" + i.ToString() + "_" + j.ToString();
                        ct[j].IsEditable = 1;
                        ct[j].LastStudied = DateTime.Now;
                        ct[j].ActRank = 0;
                        ct[j].MaxRank = 0;
                        ct[j].RemoteId = null;
                        st.ListCards.Add(ct[j]);
                        dc.Cards.InsertOnSubmit(ct[j]);
                    }
                    dc.SubmitChanges();
                }
            }

            using (CardsDataContext dc = new CardsDataContext())
            {
                DatabaseSchemaUpdater dsu = dc.CreateDatabaseSchemaUpdater();
                dsu.DatabaseSchemaVersion = CardDbUpdateV2.Version;
                dsu.Execute();
            }
            return true;
        }

        /// <summary>
        /// Returns, if this update contains only data. This is useful for filling db with initial data after installation.
        /// </summary>
        /// <returns></returns>
        public bool OnlyData()
        {
            return true;
        }
    }
}
