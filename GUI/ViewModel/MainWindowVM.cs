using GUI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GUI.ViewModel
{
    class MainWindowVM : INotifyPropertyChanged
    {
            MainWindowModel mainWindow = new MainWindowModel();
            public event PropertyChangedEventHandler PropertyChanged;
            private bool CloseAble(object obj) => true;
            public ICommand CloseCommand { get; set; }

        // Property if the singleton client has connection.
        public String vmAvailableConnection
            {
                get
                {
                    return mainWindow.HasConnection;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            private void NotifyPropertyChanged(string propName)
            {
                PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propName);
                this.PropertyChanged?.Invoke(this, propertyChangedEventArgs);
            }
            public String HasConnection
            {
                get
                {
                    return mainWindow.HasConnection;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            // ctor for the main window
            public MainWindowVM()
            {
                this.mainWindow = new MainWindowModel();
                this.mainWindow.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("mainwindowVM:" + e.PropertyName);
                };

                this.CloseCommand = new DelegateCommand<object>(this.OnClose, this.CloseAble);
            }

        private void OnClose(object obj)
        {
            this.mainWindow.logClient.Close();
        }
    }
}
