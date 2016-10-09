using EntityData;
using QDMS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class ScheduledJobsViewModel : BaseViewModel
    {
        QDMSDbContext _context;
        public ObservableCollection<DataUpdateJobDetails> Jobs { get; set; }

        private DataUpdateJobDetails _originalJob;
        private DataUpdateJobDetails _selectedJob;

        public DataUpdateJobDetails SelectedJob
        {
            get { return _selectedJob; }
            set { this.RaiseAndSetIfChanged(ref _selectedJob, value); }
        }

        public ObservableCollection<Tag> Tags { get; set; }

        public ObservableCollection<Instrument> Instruments { get; set; }

        public ReactiveCommand<Unit> AddJobCommand { get; set; }

        public ReactiveCommand<object> DeleteJobCommand { get; set; }

        public ReactiveCommand<Unit> ConfirmDeleteJobCommand { get; set; }

        public ReactiveCommand<object> SaveCommand { get; set; }

        public ScheduledJobsViewModel()
        {
            _context = new QDMSDbContext();
            Jobs = new ObservableCollection<DataUpdateJobDetails>(_context.DataUpdateJobs);
            var a = _context.Tags.ToList();
            Tags = new ObservableCollection<Tag>(_context.Tags);
            Instruments = new ObservableCollection<Instrument>(new InstrumentManager().FindInstruments(_context));


            var canDelete = this.WhenAny(x => x.SelectedJob,x => x.IsBusy, (s, b) => s.Value != null && !b.Value);
            DeleteJobCommand = ReactiveCommand.Create(canDelete);

            AddJobCommand = ReactiveCommand.CreateAsyncTask(async _ =>
                {
                    var job = new DataUpdateJobDetails
                    {
                        Name = "Job " + ((int)Jobs.Count + 1).ToString(),
                        UseTag = true,
                        Frequency = BarSize.OneDay,
                        Time = new TimeSpan(8, 0, 0),
                        WeekDaysOnly = true
                    };
                    Jobs.Add(job);
                    _context.DataUpdateJobs.Add(job);
                    await _context.SaveChangesAsync();
                });

            ConfirmDeleteJobCommand =
                ReactiveCommand.CreateAsyncTask(async _ =>
                    {
                        IsBusy = true;
                        _context.DataUpdateJobs.Remove(SelectedJob);
                        Jobs.Remove(SelectedJob);
                        await _context.SaveChangesAsync();
                        IsBusy = false;
                    });

            SaveCommand = ReactiveCommand.Create();
            SaveCommand.Subscribe(_ =>
            {
                _originalJob = _context.DataUpdateJobs.Single(j => j.ID == SelectedJob.ID);
                if (SelectedJob == null) return;

                if (SelectedJob.Frequency == null)
                {
                    MessageBus.Current.SendMessage("You must select a frequency.");
                    return;
                }
                  var job = SelectedJob;

                    if (job.UseTag)
                    {
                        if (SelectedJob.Tag == null)
                        {
                            MessageBus.Current.SendMessage("You must select a tag.");
                            return;
                        }

                        job.Instrument = null;
                        job.InstrumentID = null;
                        job.Tag = SelectedJob.Tag;
                        job.TagID = job.Tag.ID;
                    }
                    else //job is for a specific instrument, not a tag
                    {
                        if (SelectedJob.Instrument == null)
                        {
                            MessageBus.Current.SendMessage("You must select an instrument.");
                            return;
                        }

                        job.Instrument = SelectedJob.Instrument;
                        job.InstrumentID = job.Instrument.ID;
                        job.Tag = null;
                        job.TagID = null;
                    }
                    _context.Entry(_originalJob).CurrentValues.SetValues(SelectedJob);
                    _context.SaveChanges();
            });
        }
        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
