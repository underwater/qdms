// -----------------------------------------------------------------------
// <copyright file="DataUpdateJob.cs" company="">
// Copyright 2014 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

// Holds instructions on how a data update job is to be performed.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using QDMS.Annotations;

namespace QDMS
{
    public class DataUpdateJobDetails : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        [MaxLength(255)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _name;

        private bool _useTag;
        /// <summary>
        /// If true, all instruments with the given tag are matched. If false, a specific instrument is matched.
        /// </summary>
        public bool UseTag
        {
            get { return _useTag; }
            set { _useTag = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// If UseTag = false, this instrument's data gets updated.
        /// </summary>
        public int? InstrumentID { get; set; }

        private Instrument _instrument;
        /// <summary>
        /// Instrument.
        /// </summary>
        public virtual Instrument Instrument
        {
            get { return _instrument; }
            set { _instrument = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// If UseTag = true, instruments having this tag are updated.
        /// </summary>
        public int? TagID { get; set; }

        private Tag _tag;
        /// <summary>
        /// Tag.
        /// </summary>
        public virtual Tag Tag { get { return _tag; } set { _tag = value; OnPropertyChanged(); } }

        private bool _weekDaysOnly;
        /// <summary>
        /// If true, updates will only happen monday through friday.
        /// </summary>
        public bool WeekDaysOnly { get { return _weekDaysOnly; } set { _weekDaysOnly = value; OnPropertyChanged(); } }

        private TimeSpan _time;
        /// <summary>
        /// The time when the job runs.
        /// </summary>
        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        private BarSize _frequence;
        /// <summary>
        /// The data frequency to be updated.
        /// </summary>
        public BarSize Frequency { get { return _frequence; } set { _frequence = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
