using NLog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IDisposable
    {
        protected BaseViewModel()
        {
            CloseCommand = ReactiveCommand.Create();
        }

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public Logger Logger { get { return _logger; } }

        public ReactiveCommand<object> CloseCommand { get; set; }

        protected bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~BaseViewModel()
        {
            Dispose();
        }
    }
}
