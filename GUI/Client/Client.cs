using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedData;

namespace GUI.Client
{
    class Client : IClient
    {
            private bool isrunning;
            // we need to create tcp client
            private TcpClient _client;
            private static Client instance;
            public delegate void Updator(CommandEnum responseObj);
            public event GUI.Client.Updator updateeventInstance;
        
             private static Mutex _mutex = new Mutex();
            // Client GUI (singleton)
            // according to Gals explenation
        private Client()
         {
                bool running = this.Start();
                this.isrunning = running;
         }

        event GUI.Client.Updator IClient.UpdateEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        // checks if the client is connected.
        public bool Running()
            {
                return this.isrunning;
            }

            // Get an instance of the client (singelton)
            public static IClient Instance
            {
                get
                {
                    if (instance != null)
                    {
                        return instance;
                    }
                    instance = new Client();
                    return instance;
                }
            }

            // Close the connection with the server (sends close command)
            public void Close()
            {
                CommandEnum commandRecievedEventArgs = new CommandEnum((int)CommandEnum.CloseClient, null, "");
                WriteCommandToServer(commandRecievedEventArgs);
                _client.Close();
                this.isrunning = false;
            }

            // Start connection with the server
            public bool Start()
            {
                try
                {
                    bool result = true;
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ConnectingData.ip), ConnectingData.port);
                    client = new TcpClient();
                    client.Connect(endPoint);
                    // connected to server now
                    isrunning = true;
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return false;

                }
            }

            // This method constantly receiving data from the server, once the client is connected to the server (running)
            // the client receives command args and invoke its event (updateEvent) where delegates from settings model
            // and log model are signed to.
            public void UpdateConstantly()
            {
                new Task(() =>
                {
                    try
                    {
                        while (this.isrunning)
                        {
                            NetworkStream stream = _client.GetStream();
                            BinaryReader reader = new BinaryReader(stream);
                            string serializedResponse = reader.ReadString();
                            CommandRecievedEventArgs deserializedResponse = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(serializedResponse);
                            if (deserializedResponse.Args != null)
                            {
                                if (deserializedResponse.Args[0].Equals("True") || deserializedResponse.Args[0].Equals("False"))
                                {
                                    continue;
                                }
                            }
                            this.UpdateEvent?.Invoke(deserializedResponse);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        String[] failArgs = new string[2];
                        failArgs[1] = MessageTypeEnum.ERROR.ToString();
                        failArgs[0] = "Can't receive messages from server";
                        CommandRecievedEventArgs failureArgs = new CommandRecievedEventArgs((int)CommandEnum.NewLogMessage, failArgs, "");
                        this.UpdateEvent?.Invoke(failureArgs);
                    };
                }).Start();
            }
            // this method is used for the view model (to set background to gray)
            public String RunningToString()
            {
                if (this.Running())
                {
                    return "True";
                }
                return "False";
            }

            // Write command to server method (writes a serialized command to the server)
            public void WriteCommandToServer(CommandRecievedEventArgs args)
            {
                new Task(() =>
                {
                    try
                    {
                        string jsonCommand = JsonConvert.SerializeObject(args);
                        NetworkStream stream = _client.GetStream();
                        BinaryWriter writer = new BinaryWriter(stream);
                        _mutex.WaitOne();
                        writer.Write(jsonCommand);
                        _mutex.ReleaseMutex();
                    }
                    catch (Exception e)
                    {
                        // If failed writing, add a fail log to this client.
                        Console.WriteLine(e.ToString());
                        String[] failArgs = new string[2];
                        failArgs[1] = MessageTypeEnum.ERROR.ToString();
                        failArgs[0] = "Can't write message to server";
                        CommandRecievedEventArgs failureArgs = new CommandRecievedEventArgs((int)CommandEnum.NewLogMessage, failArgs, "");
                        this.UpdateEvent?.Invoke(failureArgs);
                    }
                }).Start();
            }

        public void WriteCommandToServer(MessageRecievedEventArgs argsForCommand)
        {
            throw new NotImplementedException();
        }
    }
    

}
