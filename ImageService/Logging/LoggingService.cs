
using SharedData;
using System;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;    

        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecieved?.Invoke(this, new MessageRecievedEventArgs(type, message));
        }

    }
}
