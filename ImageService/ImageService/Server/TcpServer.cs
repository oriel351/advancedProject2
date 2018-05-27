using ImageService.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.ImageService.Server
{
    class TcpServer
    {
        private int port;
        private TcpListener listener;
        private IClientHandler ch;
        private List<TcpClient> clients;
        Mutex mux;
        

        public TcpServer(int port, IClientHandler ch, ILoggingService m_logging)
        {
            this.port = port;
            this.ch = ch;
            this.clients = new List<TcpClient>();
            this.mux = new Mutex();
            m_logging.MessageRecieved += SpreadLogEntry;
        }

        public void start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);

            listener.Start();
            Console.WriteLine("Waiting for connections...");

            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        this.clients.Add(client);
                        Console.WriteLine("Got new connection");                       
                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }


        public void SpreadLogEntry(Object sender, MessageRecievedEventArgs e)
        {
            foreach (TcpClient client in this.clients)
            {
                this.ch.SendData(client, JsonConvert.SerializeObject(e));               
            }
        }

    }
}
