using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace iostamagotchi
{

    /// <summary>
    /// Class representing table of sets of flash cards
    /// </summary>
    [Table(Name="Sets")]
    public class SetTable : BaseBindingTable
    {
        private String m_title;
        private int m_source_id;
        private int m_is_editable;
        private DateTime m_last_studied;
        private EntitySet<CardTable> m_cards;
        private EntityRef<SourceTable> m_sets;
        private bool m_isStudying;
        private float m_health;
        private int m_maxPercentage;

        /// <summary>
        /// Constructor
        /// </summary>
        public SetTable()
        {
            this.m_cards = new EntitySet<CardTable>( this.onCardAdded, this.onCardRemoved );
            this.m_sets = new EntityRef<SourceTable>();
        }

        /// <summary>
        /// Set ID
        /// </summary>
        [Column(IsPrimaryKey = true,
                IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int SetId { get; set; }

        /// <summary>
        /// Name of set of cards
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
        /// ID of this set in source repository
        /// </summary>
        [Column(CanBeNull = false)]
        public int SourceId
        {
            get { return this.m_source_id; }
            set
            {
                if (this.m_source_id == value)
                    return;
                this.NotifyPropertyChanging("SourceId");
                this.m_source_id = value;
                this.NotifyPropertyChanged("SourceId");
            }
        }

        /// <summary>
        /// Number of cards in set
        /// </summary>
        public int CardCount
        {
            get { return this.m_cards.Count; }
        }

        /// <summary>
        /// Determines, if this set can be edited from GUI (only user-created sets can be edited from GUI)
        /// </summary>
        [Column(CanBeNull = false)]
        public int IsEditable
        {
            get { return this.m_is_editable; }
            set
            {
                if (this.m_is_editable == value)
                    return;
                this.NotifyPropertyChanging("IsEditable");
                this.m_is_editable = value;
                this.NotifyPropertyChanged("IsEditable");
            }
        }

        /// <summary>
        /// Date and time when this set was studied last time
        /// </summary>
        [Column(CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTime LastStudied
        {
            get { return this.m_last_studied; }
            set
            {
                if (this.m_last_studied == value)
                    return;
                this.NotifyPropertyChanging("LastStudied");
                this.m_last_studied  = value;
                this.NotifyPropertyChanged("LastStudied");
            }
        }

        /// <summary>
        /// Whether user studies this set or not
        /// </summary>
        [Column(CanBeNull = false)]
        public bool IsStudying
        {
            get { return this.m_isStudying; }
            set
            {
                if (this.m_isStudying == value)
                    return;
                this.NotifyPropertyChanging("IsStudying");
                this.m_isStudying = value;
                this.NotifyPropertyChanged("IsStudying");
            }
        }

        /// <summary>
        /// Health represents how many cards is currently waiting to be studying in set
        /// </summary>
        public float Health
        {
            get { return this.m_health; }
            set
            {
                if (this.m_health == value)
                    return;
                this.NotifyPropertyChanging("Health");
                this.m_health = value;
                this.NotifyPropertyChanged("Health");
            }
        }

        /// <summary>
        /// Remote ID of this card in source
        /// </summary>
        [Column(CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int? RemoteId;

        /// <summary>
        /// Max percentage for this set
        /// </summary>
        [Column(CanBeNull = false, UpdateCheck = UpdateCheck.Always)]
        public int MaxPercentage
        {
            get
            {
                return this.m_maxPercentage;
            }
            set
            {
                if (this.m_maxPercentage == value)
                {
                    return;
                }
                this.NotifyPropertyChanging("MaxPercentage");
                this.m_maxPercentage = value;
                this.NotifyPropertyChanged("MaxPercentage");
            }
        }

        /// <summary>
        /// List of flash cards which belong to this set
        /// </summary>
        [Association(Name = "FK_Set_Cards", Storage = "m_cards", ThisKey = "SetId", OtherKey = "SetId")]
        public EntitySet<CardTable> ListCards
        {
            get { return this.m_cards; }
            set { this.m_cards.Assign(value); }
        }

        /// <summary>
        /// Reference to Source of this set
        /// </summary>
        [Association(Name = "FK_Source_Sets", Storage = "m_sets", ThisKey = "SourceId", OtherKey = "SourceId", IsForeignKey=true)]
        public SourceTable Source
        {
            get
            {
                return this.m_sets.Entity;
            }
            set
            {
                SourceTable previousValue = this.m_sets.Entity;
                if (((previousValue != value) || (this.m_sets.HasLoadedOrAssignedValue == false)))
                {
                    if ((previousValue != null))
                    {
                        this.m_sets.Entity = null;
                        previousValue.ListSets.Remove(this);
                    }
                    this.m_sets.Entity = value;
                    if ((value != null))
                    {
                        value.ListSets.Add(this);
                        this.m_source_id = value.SourceId;
                    }
                    else
                    {
                        this.m_source_id = default(int);
                    }
                }
            }
        }

        private void onCardAdded(CardTable AddedCard)
        {
            AddedCard.Set = this;
        }

        private void onCardRemoved(CardTable RemovedCard)
        {
            RemovedCard.Set = null;
        }
    }
}

