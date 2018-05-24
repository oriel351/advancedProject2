using ImageService.Controller;
using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Server
{
    class ClientHandler
    {
        private IImageController controller;

        public void HandleClient(TcpClient client)
        {
            new Task(() => 
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string commandLine = reader.ReadLine();
                    Console.WriteLine("Got command: {0}", commandLine);
                    try
                    {
                        string result = this.controller.ExecuteCommand(commandLine, client);
                    } catch(Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                    
                    writer.Write(result);
                }
                client.Close();
            }).Start();
        }
    }
}
