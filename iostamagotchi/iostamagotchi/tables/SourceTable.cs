using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.ComponentModel;

namespace iostamagotchi
{
    /// <summary>
    /// Class representing table of sources of flash cards
    /// </summary>
    [Table(Name = "Sources")]
    public class SourceTable : BaseBindingTable
    {
        private String m_title;
        private String m_class_name;
        private EntitySet<SetTable> m_sets;

        /// <summary>
        /// Constructor
        /// </summary>
        public SourceTable()
        {
            this.m_sets = new EntitySet<SetTable>(this.onSetAdded, this.onSetRemoved);
        }

        /// <summary>
        /// Source ID
        /// </summary>
        [Column(IsPrimaryKey = true,
                IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int SourceId { get; set; }

        /// <summary>
        /// Name of source
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
        /// Name of class handling manipulation with source
        /// </summary>
        [Column(CanBeNull = false)]
        public String ClassName
        {
            get { return this.m_class_name; }
            set
            {
                if (this.m_class_name == value)
                    return;
                this.NotifyPropertyChanging("ClassName");
                this.m_class_name = value;
                this.NotifyPropertyChanged("ClassName");
            }
        }

        /// <summary>
        /// List of sets created by this source
        /// </summary>
        [Association(Name = "FK_Source_Sets", Storage = "m_sets", ThisKey = "SourceId", OtherKey = "SourceId")]
        public EntitySet<SetTable> ListSets
        {
            get { return this.m_sets; }
            set { this.m_sets.Assign(value); }
        }

        private void onSetAdded(SetTable AddedSet)
        {
            AddedSet.Source = this;
        }

        private void onSetRemoved(SetTable RemovedSet)
        {
            RemovedSet.Source = null;
        }
    }
}
