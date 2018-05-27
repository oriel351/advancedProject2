﻿using GlobalClasses.Modal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    interface ILogsModel : INotifyPropertyChanged
    {
        ObservableCollection<LogObject> Logs { get; set; }
    }
}
