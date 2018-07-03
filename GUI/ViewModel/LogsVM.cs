using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData;

namespace GUI.Model
{
    class LogsVM : INotifyPropertyChanged
    {
        //implementing the interface
        public event PropertyChangedEventHandler PropertyChanged;
        private LogsModel logsModel = new LogsModel();
        // Observeble list of logs
        public ObservableCollection<LogObject> Logs
        {
            get { return this.logsModel.Logs; }
            set { throw new NotImplementedException(); }
        }
    }
}
