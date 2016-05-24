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
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace iostamagotchi
{
    /// <summary>
    /// Class handling work with sets
    /// </summary>
    static public class SetHelper
    {
        /// <summary>
        /// List of cards ready to be studied with specified rank (-1 means from all ranks)
        /// </summary>
        /// <param name="setId">Set ID</param>
        /// <param name="rank">Rank of cards (-1 means all cards)</param>
        /// <param name="IsAlg">If players uses algorithm</param>
        /// <returns>List of cards ready to be studied</returns>
        public static ObservableCollection<CardTable> GetListReadyCards(int setId, int rank, bool IsAlg)
        {
            ObservableCollection<CardTable> result = new ObservableCollection<CardTable>();
            using (CardsDataContext dc = new CardsDataContext())
            {
                SetTable s = dc.Sets.Single(set => set.SetId == setId);
                if (IsAlg) // use only cards ready to be studied
                {
                    for (int i = 0; i < s.CardCount; i++)
                    {
                        if ((rank == -1 || rank == s.ListCards[i].ActRank) && s.ListCards[i].Ready)
                        {
                            result.Add(s.ListCards[i]);
                        }
                    }
                }
                else // use all cards
                {
                    for (int i = 0; i < s.CardCount; i++)
                    {
                        result.Add(s.ListCards[i]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// List of sets with cards ready to be studied
        /// </summary>
        /// <param name="IsAlg">If players follows algorithm</param>
        /// <returns>List of sets with cards ready to be studied</returns>
        public static ObservableCollection<SetTable> GetListReadySets(bool IsAlg)
        {
            ObservableCollection<SetTable> result = new ObservableCollection<SetTable>();
            using (CardsDataContext dc = new CardsDataContext())
            {
                IEnumerable<SetTable> ies = dc.Sets.Where(set => set.IsStudying == true).ToList<SetTable>();
                for (int i = 0; i < ies.Count<SetTable>(); i++)
                {
                    if (SetHelper.GetListReadyCards(ies.ElementAt<SetTable>(i).SetId, -1, IsAlg).Count > 0)
                    {
                        result.Add(ies.ElementAt<SetTable>(i));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// List of sets with cards ready for testing
        /// </summary>
        /// <returns>List of sets with cards ready for testing</returns>
        public static ObservableCollection<SetTable> GetListTestSets()
        {
            ObservableCollection<SetTable> result = new ObservableCollection<SetTable>();
            using (CardsDataContext dc = new CardsDataContext())
            {
                IEnumerable<SetTable> ies = dc.Sets.Where(set => set.IsStudying == true).ToList<SetTable>();
                for (int i = 0; i < ies.Count<SetTable>(); i++)
                {
                    if (ies.ElementAt<SetTable>(i).CardCount > 3)
                    {
                        result.Add(ies.ElementAt<SetTable>(i));
                    }
                }
            }

            return result;
        }
    }
}
