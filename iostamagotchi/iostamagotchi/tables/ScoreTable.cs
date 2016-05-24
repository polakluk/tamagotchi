using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.ComponentModel;

namespace iostamagotchi
{
    [Table(Name="Score")]
    public class ScoreTable : BaseTable
    {
        /// <summary>
        /// Score ID
        /// </summary>
        [Column(IsPrimaryKey = true,
                IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int ScoreId { get; set; }

        /// <summary>
        /// Score that player achieved
        /// </summary>
        [Column(CanBeNull = false)]
        public int Score;

        /// <summary>
        /// When was the score acheived
        /// </summary>
        [Column(CanBeNull = false)]
        public DateTime Achieved;

        /// <summary>
        /// Was it uploaded?
        /// </summary>
        [Column(CanBeNull = false)]
        public bool IsUploaded;

        /// <summary>
        /// For which type of action player achieved this score (studying or testing) => enum eActionType
        /// </summary>
        [Column(CanBeNull = false)]
        public int ActionType;
    }
}
