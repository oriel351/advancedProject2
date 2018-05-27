using System;

namespace SharedData
{
    public class MessageRecievedEventArgs : EventArgs
    {
        //every time this class gets a message it should
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }
        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            this.Status = status;
            this.Message = message;
        }
    }
}