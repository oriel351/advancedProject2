using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    class MainWindowModel : INotifyPropertyChanged
    {
        public IClientGUI logClient { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private String isConnected;
        // constractor for main window model
        public MainWindowModel()
        {
            this.logClient = ClientGUI.Instance;
            HasConnection = logClient.RunningToString();
        }
 
        // binding between the view and this field
        public String HasConnection
        {
            get
            {
                if (logClient.Running())
                {
                    return "True";
                }
                return "False";
            }
            set
            {
                isConnected = value;
                OnPropertyChanged("Available");

            }
        }
        // when property changed notify the main view that the client is off
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}