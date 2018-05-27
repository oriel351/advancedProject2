using GUI.Model;
using Microsoft.Practices.Prims.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    class SettingsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DelegateCommand<Object> RemoveCommand { get; set; }
        public ObservableCollection<string> vmHandlers
            {
                get { return settingsModel.Handlers; }
                set { throw new NotImplementedException(); }
            }

        //dont forget to create this object privetly here setting/log model as singleton
        private ISettingsModel settingsModel;
        //dont forget to implement remove command // dll


        // Ctor for the settings view model (creates a settings model)
        public SettingsVM()
            {
                this.settingsModel = new SettingsModel();
            settingsModel.PropertyChanged +=
                    delegate (Object sender, PropertyChangedEventArgs e)
                    {
                        NotifyPropertyChanged("viewModel:" + e.PropertyName);
                    };
            this.RemoveCommand = new DelegateCommand<object>(Remove, isRemoveable);
        }


            //from lecture No.5
            private void NotifyPropertyChanged(string propName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }

            public string OutputDirectory
            {
                get {return settingsModel.OutputDir; }
            }
            public string SourceName
            {
                get { return settingsModel.SrcName; }
            }
            public string LogName
            {
                get { return settingsModel.LogName; }
            }
            public string TumbnailSize
            {
                get { return settingsModel.TumbnailSize; }
            }
            
            private string selectedHandler;
        private void Remove(object obj)
        {
            //sending a tcp command if removed
            this.settingsModel.RemoveHandler(this.selectedHandler);
        }

        //asks if we can remove
        private bool isRemoveable(object obj)
        {
            if (this.selectedHandler != null) return true; 
            return false;
        }
        public string SelectedHandler
            {
                get { return this.selectedHandler; }                
                set
                {
                    selectedHandler = value;
                    var command = this.RemoveCommand as DelegateCommand<object>;
                    command.RaiseCanExecuteChanged();
                }
            
            }
    }
}
