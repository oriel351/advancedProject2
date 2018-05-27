using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GUI.Model
{
    class SettingsModel : ISettingsModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string name_source;
        private string dir_output;
        private string name_log;
        private int size_thumb;
        private ObservableCollection<string> Handlers;
        private string handler;
        public string LogName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string TumbnailSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string OutputDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SrcName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void RemoveHandler(string selectedHandler)
        {
            throw new NotImplementedException();
        }

    }
}

