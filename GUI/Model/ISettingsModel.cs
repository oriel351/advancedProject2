using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    interface ISettingsModel : INotifyPropertyChanged
    {
        string LogName { get; set; }
        string TumbnailSize { get; set; }
        string OutputDir { get; set; }
        string SrcName { get; set; }
        ObservableCollection<string> Handlers { get; set; }
        //remove handler from given path  
        void RemoveHandler(string selectedHandler);
    }
}
