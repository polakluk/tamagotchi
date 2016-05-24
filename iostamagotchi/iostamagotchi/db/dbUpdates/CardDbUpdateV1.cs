using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Data.Linq;

namespace iostamagotchi
{
    public class CardDbUpdateV1 : IDbUpdate 
    {
        /// <summary>
        /// Version of this update
        /// </summary>
        public const int Version = 1;

        /// <summary>
        /// Updates schema and returns result of this operation
        /// </summary>
        /// <returns>Result of this operation</returns>
        public bool PerformUpdate()
        {
            using (CardsDataContext dc = new CardsDataContext())
            {
                DatabaseSchemaUpdater dsu = dc.CreateDatabaseSchemaUpdater();

                SourceTable local = new SourceTable();
                local.Title = "Local"; local.ClassName = "Local";
                dc.Sources.InsertOnSubmit(local);

                SourceTable quizlet = new SourceTable();
                quizlet.Title = "Quizlet"; quizlet.ClassName = "Quizlet";
                dc.Sources.InsertOnSubmit(quizlet);

                dc.SubmitChanges();
                dsu.DatabaseSchemaVersion = CardDbUpdateV1.Version;
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
