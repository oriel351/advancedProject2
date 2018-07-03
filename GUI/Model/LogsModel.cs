//using GlobalClasses.Modal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SharedData;
namespace GUI.Model
{
    class LogsModel : ILogsModel
    {
       // class LogModel : ILogModel
       // {
            private ObservableCollection<LogObject> logs { get; set; }
            //we need to create a client that will talk to the server - oriel
            private IClientGUI logClient;
            public event PropertyChangedEventHandler PropertyChanged;


            public ObservableCollection<LogObject> Logs
            {
                get { return this.logs; }
                set { throw new NotImplementedException(); }
            }

           
            public LogsModel()
            {
                logs = new ObservableCollection<LogObject>();
              //  this._logClient = ClientGUI.Instance;
                if (this.logClient.Running())
                {
                    this.logClient.UpdateEvent += this.Updater;
                    this.GetPrevLogs();
                }
                else
                {
                    Console.WriteLine("client of the log is not connected");
                }
            }
            
            private void GetPrevLogs()
            {
                this.logs = new ObservableCollection<LogObject>();
                Object thisLock = new Object();
                BindingOpeations.EnableCollectionSynchronization(logs, thisLock);
                CommandRecievedEventArgs commandRecievedEventArgs = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, null, "");
                this.logClient.WriteCommandToServer(commandRecievedEventArgs);
            }
            /// This method is a delegate signed to the ClientGUI updatorEvent. every time the client gets a command
            /// from the server it invokes his event and therefore this delegate.
            private void Updater(CommandRecievedEventArgs args)
            {
                if (args != null)
                {
                    switch (args.CommandID)
                    {
                        case (int)CommandEnum.LogCommand:
                            SetupPreviousLogs(args);
                            break;
                        case (int)CommandEnum.NewLogMessage:
                            InsertLog(args);
                            break;
                        default:
                            break;
                    }
                }
            }
            // Set up previous logs (invoked from updater)
            private void SetupPreviousLogs(CommandRecievedEventArgs args)
            {
                try
                {
                    ObservableCollection<LogObject> previousLogs = JsonConvert.DeserializeObject<ObservableCollection<LogTuple>>(args.Args[0]);
                    for (int i = 1; i < previousLogs.Count(); i++)
                    {
                        this.logs.Add(previousLogs[i]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            // insert a new log
            private void InsertLog(CommandRecievedEventArgs args)
            {
                try
                {
                    this.logs.Insert(0, new LogObject { EnumType = args.Args[1], Data = args.Args[0] });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
        }
    //}

}
