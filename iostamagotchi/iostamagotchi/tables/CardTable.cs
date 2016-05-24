using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using System.ComponentModel;

namespace iostamagotchi
{
    /// <summary>
    /// Class representing table of flash cards
    /// </summary>
    [Table(Name = "Cards")]
    public class CardTable : BaseBindingTable
    {
        private String m_front;
        private String m_back;
        private String m_img_front;
        private String m_img_back;
        private int m_is_editable;
        private DateTime m_last_studied;
        private int? m_set_id;
        private EntityRef<SetTable> m_cards = new EntityRef<SetTable>();

        /// <summary>
        /// Card ID
        /// </summary>
        [Column(IsPrimaryKey = true,
            IsDbGenerated = true,
            DbType = "INT NOT NULL Identity",
            CanBeNull = false,
            AutoSync = AutoSync.OnInsert)]
        public int CardId { get; set; }

        /// <summary>
        /// Front side text of card
        /// </summary>
        [Column(CanBeNull = false)]
        public String FrontSide
        {
            get { return this.m_front; }
            set
            {
                if (this.m_front == value)
                    return;
                this.NotifyPropertyChanging("FrontSide");
                this.m_front = value;
                this.NotifyPropertyChanged("FrontSide");
            }
        }

        /// <summary>
        /// Back side text of card
        /// </summary>
        [Column(CanBeNull = false)]
        public String BackSide
        {
            get { return this.m_back; }
            set
            {
                if (this.m_back== value)
                    return;
                this.NotifyPropertyChanging("BackSide");
                this.m_back = value;
                this.NotifyPropertyChanged("BackSide");
            }
        }

        /// <summary>
        /// Front side image of card
        /// </summary>
        [Column(CanBeNull = true)]
        public String ImgFront
        {
            get { return this.m_img_front; }
            set
            {
                if (this.m_img_front == value)
                    return;

                this.NotifyPropertyChanging("ImgFront");
                this.m_img_front = value;
                this.NotifyPropertyChanged("ImgFront");
            }
        }

        /// <summary>
        /// Back side image of card
        /// </summary>
        [Column(CanBeNull = true)]
        public String ImgBack
        {
            get { return this.m_img_back; }
            set
            {
                if (this.m_img_back == value)
                    return;

                this.NotifyPropertyChanging("ImgBack");
                this.m_img_back = value;
                this.NotifyPropertyChanged("ImgBack");
            }
        }

        /// <summary>
        /// Date and time when this card was studied last time
        /// </summary>
        [Column(CanBeNull = false)]
        public DateTime LastStudied
        {
            get { return this.m_last_studied; }
            set
            {
                if (this.m_last_studied == value)
                    return;

                this.NotifyPropertyChanging("LastStudied");
                this.m_last_studied = value;
                this.NotifyPropertyChanged("LastStudied");
            }
        }

        /// <summary>
        /// Determines, if this card can be edited from GUI (only user-created cards can be edited from GUI)
        /// </summary>
        [Column(CanBeNull = false)]
        public int IsEditable
        {
            get { return this.m_is_editable; }
            set
            {
                if (this.m_is_editable == value)
                    return;

                this.NotifyPropertyChanging("LastEditable");
                this.m_is_editable = value;
                this.NotifyPropertyChanged("IsEditable");
            }
        }

        /// <summary>
        /// Set ID
        /// </summary>
        [Column(CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int? SetId
        {
            get { return this.m_set_id; }
            set
            {
                if (this.m_set_id == value)
                    return;

                this.NotifyPropertyChanging("SetId");
                this.m_set_id = value;
                this.NotifyPropertyChanged("SetId");
            }
        }

        /// <summary>
        /// Remote ID of this card in source
        /// </summary>
        [Column(CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int? RemoteId;

        /// <summary>
        /// Actual rank of this card
        /// </summary>
        [Column(CanBeNull = false)]
        public int ActRank;

        /// <summary>
        /// States, if the card is ready to be studied
        /// </summary>
        public bool Ready
        {
            get
            {
                if (this.ActRank == CommonData.MaxRank)
                {
                    return false;
                }
                else
                {
                    return (this.LastStudied + CommonData.RankTimes[this.ActRank]) < DateTime.Now;
                }
            }
        }


        /// <summary>
        /// Max rank of this card
        /// </summary>
        [Column(CanBeNull = false)]
        public int MaxRank;

        /// <summary>
        /// Reference to parent set
        /// </summary>
        [Association(Name = "FK_Set_Cards", Storage = "m_cards", ThisKey = "SetId", OtherKey = "SetId", IsForeignKey=true)]
        public SetTable Set
        {
            get
            {
                return this.m_cards.Entity;
            }
            set
            {
                SetTable previousValue = this.m_cards.Entity;
                SetTable newValue = value;

                if (newValue != previousValue)
                {

                    // remove this book from our prior category's list of books
                    this.m_cards.Entity = null;
                    if (previousValue != null)
                    {
                        previousValue.ListCards.Remove(this);
                    }

                    // set category to the new value
                    this.m_cards.Entity = newValue;

                    // add this book to the new category's list of books
                    if (newValue!= null)
                    {
                        newValue.ListCards.Add(this);
                    }
                }
                
                if (((previousValue != value) || (this.m_cards.HasLoadedOrAssignedValue == false)))
                {
                    if ((previousValue != null))
                    {
                        this.m_cards.Entity = null;
                        if (previousValue != null)
                        {
                            previousValue.ListCards.Remove(this);
                        }
                    }
                    this.m_cards.Entity = value;
                    if ((value != null))
                    {
                        value.ListCards.Add(this);
                        this.SetId = value.SetId;
                    }
                    else
                    {
                        this.SetId = default(int);
                    }
                }
            }
        }
    }
}
