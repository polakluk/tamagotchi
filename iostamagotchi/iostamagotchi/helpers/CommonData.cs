using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Phone.Shell;

namespace iostamagotchi
{
    /// <summary>
    /// States for flash card during studying animation
    /// </summary>
    public enum eFlashCardStateStudy
    {
        fcssNone,
        fcssBackSide,
        fcssFadingIn,
        fcssFadingOut
    };

    /// <summary>
    /// States for answer on knowledge of flash card
    /// </summary>
    public enum eFlachCardAnswerState
    {
        fcasNone,
        fcasYes,
        fcasNo
    };

    /// <summary>
    /// Types of player action
    /// </summary>
    public enum eActionType
    {
        atStudy = 0,
        atPractise,
        atTest
    }

    /// <summary>
    /// Class containg common static data across all parts of application
    /// </summary>
    public static class CommonData
    {
        /// <summary>
        /// Maximum rank or studying
        /// </summary>
        public static int MaxRank = 5;

        /// <summary>
        /// Default value for delay on displaying cheering message
        /// </summary>
        public static int CheerDefault = 3;

        /// <summary>
        /// Delays for occurance of flash cards with ranks
        /// </summary>
        public static TimeSpan[] RankTimes = {
                                                 TimeSpan.FromSeconds(0),
                                                 TimeSpan.FromMinutes(1),
                                                 TimeSpan.FromMinutes(2),
                                                 TimeSpan.FromMinutes(3),
                                                 TimeSpan.FromMinutes(4)
                                             };

        private static IStudyPlan m_studyPlan = null;

        /// <summary>
        /// Finds index of set in ViewModel
        /// </summary>
        /// <param name="SetId">Set ID</param>
        /// <returns>Index of set in list</returns>
        public static int FindIdxBySetId(int SetId)
        {
            for (int i = 0; i < App.ManageFlashCardsViewModel.ListSets.Count; i++)
            {
                if (App.ManageFlashCardsViewModel.ListSets[i].SetId == SetId)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Calculates number of cards that need to be studied
        /// </summary>
        /// <param name="Set">Set</param>
        /// <returns>Number of cards that need to be studied</returns>
        public static int CalculateHealthSet(SetTable set)
        {
            return 100 - (int)((SetHelper.GetListReadyCards(set.SetId, -1, true).Count * 100) / set.CardCount);
        }

        /// <summary>
        /// Actual study plan
        /// </summary>
        public static IStudyPlan StudyPlan
        {
            get
            {
                if (CommonData.m_studyPlan == null)
                {
                    CommonData.m_studyPlan = CommonData.getStudyPlan();
                }
                return CommonData.m_studyPlan;
            }
            set
            {
                CommonData.m_studyPlan = value;
            }
        }

        /// <summary>
        /// Gets instance of actual study plan
        /// </summary>
        /// <returns>Actual study plan</returns>
        private static IStudyPlan getStudyPlan()
        {
            int planId = App.Settings.GetValueOrDefault<int>("plan.id");
            PlanTable pt = null;

            using(PlansDataContext dc = new PlansDataContext())
            {
                pt = dc.Plans.Single( p => p.PlanId == planId );
            }
            Type act_t = App.CurrentTypes.GetType( pt.ClassName + "StudyPlan");
            return Activator.CreateInstance(act_t) as IStudyPlan;
        }

        /// <summary>
        /// Multiples score by Study Plan factor and a constant
        /// </summary>
        /// <param name="score">Actual score</param>
        /// <returns>Adjusted score</returns>
        public static int AdjustScore(int score)
        {
            return (int)(score * 100 * CommonData.StudyPlan.GetFactor());
        }

        /// <summary>
        /// Resets notification counter when necessary
        /// </summary>
        public static void ResetNotificationCounter()
        {
            App.Settings.AddOrUpdateValue("notifications.missed", 0);
            App.Settings.Save();
            ShellTile tile = ShellTile.ActiveTiles.First();
            if (tile != null)
            {
                StandardTileData data = new StandardTileData();

                data.Title = "Spacechi";
                data.Count = 0;

                tile.Update(data);
            }
        }
    }
}
