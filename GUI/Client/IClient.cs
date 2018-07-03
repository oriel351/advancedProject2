using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData;

namespace GUI.Client
{
    // delegate of update command (settings and log models has this delegate which is assigned to update event.   
    public delegate void Updator(MessageRecievedEventArgs args);
    interface IClient
    {
            bool Running();   
            void Close();
            void WriteCommandToServer(MessageRecievedEventArgs argsForCommand);
            void UpdateConstantly();
            event Updator UpdateEvent;
            String RunningToString();
    }
}
