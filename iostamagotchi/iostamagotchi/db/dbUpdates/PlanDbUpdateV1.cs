using System;
using System.Net;
using System.Windows;
using Microsoft.Phone.Data.Linq;
using System.Linq;
using System.Data.Common;

namespace iostamagotchi
{
    public class PlanDbUpdateV1 : IDbUpdate
    {
        public const int Version = 1;

        /// <summary>
        /// Updates schema and returns result of this operation
        /// </summary>
        /// <param name="dc">Data contxet</param>
        /// <returns>Result of this operation</returns>
        public bool PerformUpdate()
        {
            using (PlansDataContext dc = new PlansDataContext())
            {
                PlanTable[] plans = new PlanTable[3];
                plans[0] = new PlanTable();
                plans[0].ClassName = "Plan1";
                plans[0].Title = "Fast";
                plans[0].Desc = "Get better quickly";
                plans[0].Ordering = 3;
                plans[0].Enabled = true;
                dc.Plans.InsertOnSubmit(plans[0]);

                plans[1] = new PlanTable();
                plans[1].ClassName = "Plan2";
                plans[1].Title = "Faster than light";
                plans[1].Desc = "Be smart as fox in no time";
                plans[1].Ordering = 2;
                plans[1].Enabled = true;
                dc.Plans.InsertOnSubmit(plans[1]);

                plans[2] = new PlanTable();
                plans[2].ClassName = "Plan3";
                plans[2].Title = "Pants on fire";
                plans[2].Desc = "Become genius over night";
                plans[2].Ordering = 1;
                plans[2].Enabled = true;
                dc.Plans.InsertOnSubmit(plans[2]);
                dc.SubmitChanges();
            }

            using (PlansDataContext dc = new PlansDataContext())
            {
                DatabaseSchemaUpdater dsu = dc.CreateDatabaseSchemaUpdater();
                dsu.DatabaseSchemaVersion = PlanDbUpdateV1.Version;
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
