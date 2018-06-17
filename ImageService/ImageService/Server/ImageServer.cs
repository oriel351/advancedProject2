using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.ImageService.Server;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using SharedData;
using System;


namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members

        // working members
        private IImageController m_controller;
        private ILoggingService m_logging;
        
        // info members
        private string outputDir;
        string[] paths;

        // communication members
        private TcpServer tcpServer;
        //private IClientHandler ch;

        #endregion

        #region Properties
        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;

        #endregion

        /*
         * Constructor.
         */
        public ImageServer(IImageController controller, ILoggingService log, ConfigData conf)
        {
            this.m_controller = controller;
            this.m_logging = log;
            this.outputDir = conf.outputDir;
            this.paths = conf.paths;

            InitializeHandlers();
        }

        private void InitializeCommunication()
        {
            IClientHandler ch = new ClientHandler();
            this.m_logging.MessageRecieved += OnLogEvent;
            this.tcpServer = new TcpServer();
        }

        private void InitializeHandlers()
        {
            foreach (string p in this.paths)
            {
                IDirectoryHandler a = new DirectoryHandler(this.m_logging, this.m_controller, p);
                a.StartHandleDirectory(p);

                a.DirectoryClose += OnHandlerCloseCommand;
                this.CommandRecieved += a.OnCommandRecieved;
            }          
        }

        private void OnHandlerCloseCommand(object sender, DirectoryCloseEventArgs e)
        {
            
            this.CommandRecieved -= ((DirectoryHandler)sender).OnCommandRecieved;
        }


        public void StopServer()
        {
            foreach (string p in this.paths)
            {
                this.CommandRecieved?.Invoke(
                    this, 
                    new CommandRecievedEventArgs((int)CommandEnum.CloseCommand,
                    new string[] { }, p));
            }
            
        }

        

        
        private void OnLogEvent(object sender, MessageRecievedEventArgs e)
        {
           
        }

        public void CloseHandler()
        {

        }
        
       
    }
}
