using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.ComponentModel;

namespace iostamagotchi
{
    /// <summary>
    /// Class representing table of a study program
    /// </summary>
    [Table(Name = "Plan")]
    public class PlanTable : BaseBindingTable
    {
        private string m_title;
        private string m_desc;
        private string m_className;
        private int m_ordering;
        private bool m_enabled;


        /// <summary>
        /// Plan ID
        /// </summary>
        [Column(IsPrimaryKey = true,
                IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int PlanId { get; set; }

        /// <summary>
        /// Name of plan
        /// </summary>
        [Column(CanBeNull = false)]
        public String Title
        {
            get { return this.m_title; }
            set
            {
                if (this.m_title == value)
                    return;
                this.NotifyPropertyChanging("Title");
                this.m_title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Description of plan
        /// </summary>
        [Column(CanBeNull = false)]
        public String Desc
        {
            get { return this.m_desc; }
            set
            {
                if (this.m_desc == value)
                    return;
                this.NotifyPropertyChanging("Desc");
                this.m_desc= value;
                this.NotifyPropertyChanged("Desc");
            }
        }

        /// <summary>
        /// Class name for plan
        /// </summary>
        [Column(CanBeNull = false)]
        public String ClassName
        {
            get { return this.m_className; }
            set
            {
                if (this.m_className == value)
                    return;
                this.NotifyPropertyChanging("ClassName");
                this.m_className = value;
                this.NotifyPropertyChanged("ClassName");
            }
        }

        /// <summary>
        /// Ordering in ListPicker
        /// </summary>
        [Column(CanBeNull = false)]
        public int Ordering
        {
            get { return this.m_ordering; }
            set
            {
                if (this.m_ordering == value)
                    return;
                this.NotifyPropertyChanging("Ordering");
                this.m_ordering = value;
                this.NotifyPropertyChanged("Ordering");
            }
        }

        /// <summary>
        /// If plan is enabled
        /// </summary>
        [Column(CanBeNull = false)]
        public bool Enabled
        {
            get { return this.m_enabled; }
            set
            {
                if (this.m_enabled == value)
                    return;
                this.NotifyPropertyChanging("Enabled");
                this.m_enabled = value;
                this.NotifyPropertyChanged("Enabled");
            }
        }

    }
}
