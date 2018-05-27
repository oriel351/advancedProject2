using ImageService.Controller;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.ImageService.Server
{
    class ClientHandler
    {
        private Mutex mux;
        private IImageController m_controller;       

        public ClientHandler(IImageController m_controller)
        {
            this.m_controller = m_controller;
            this.mux = new Mutex()
        }
        public void HandleClient(TcpClient client)
        {
            new Task(() => 
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))                
                {
                    bool result;
                    while (client.Connected)
                    {
                        string commandLine = reader.ReadToEnd();
                        CommandRecievedEventArgs cmd = JsonConvert.DeserializeObject
                        <CommandRecievedEventArgs>(commandLine);

                        int command = cmd.CommandID;


                        switch (command)
                        {
                            case (int)CommandEnum.GetConfigCommand:
                                string data = this.m_controller.ExecuteCommand(command, null, out result);
                                SendData(client, data);
                                break;
                            case (int)CommandEnum.LogCommand:                                
                                data = this.m_controller.ExecuteCommand                                
                                (command, null, out result);
                                SendData(client, data);
                                break;
                            case (int)CommandEnum.CloseCommand:
                                this.m_controller.ExecuteCommand(command, cmd.Args, out result);
                                break;
                        }
                    }
                    

                    bool suc;                    
                    string result = this.m_controller.ExecuteCommand(Int32.Parse(commandLine), null, out suc);

                    writer.Write(result);
                }
                client.Close();
            }).Start();

        }      

        private string getConfig()
        {

        }


        private void SendData(TcpClient client, string data)
        {
            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                mux.WaitOne();
                writer.Write(data);
                mux.ReleaseMutex();
            }
        }



        private void SendData(StreamWriter writer, MessageRecievedEventArgs e) 
        {
            using (writer)
            {
                
            }
        }





        public CommandRecievedEventArgs GetData(string data)
        {
            CommandRecievedEventArgs cmd = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(data);
            JObject dt = JObject.Parse(data);
            return cmd;
        }
    }

}
