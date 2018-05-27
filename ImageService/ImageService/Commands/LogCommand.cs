using ImageService.Commands;
using ImageService.Logging;
using Newtonsoft.Json;
using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Commands
{
    class LogCommand : ICommand
    {
        private ILoggingService m_logging;
        private List<MessageRecievedEventArgs> log;

        public LogCommand(ILoggingService m_logging)
        {
            this.m_logging = m_logging;
            this.m_logging.MessageRecieved += LogEntryEnter;
            this.log = new List<MessageRecievedEventArgs>();
        }

        private void LogEntryEnter(object sender, MessageRecievedEventArgs e)
        {
            this.log.Add(e);          
        }

        public string Execute(string[] args, out bool result)
        {
            result = true;
            string res = JsonConvert.SerializeObject(this.log);
            return res;
        }

        

    }
}
