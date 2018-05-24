using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.ImageService.Server;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using System;


namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private TcpServer broad;
        private string outputDir;
        string[] paths;




        #endregion

        #region Properties
        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        #endregion

        /*
         * Constructor.
         */
        public ImageServer(IImageController controller, ILoggingService log, string outDir, string [] path)
        {
            this.m_controller = controller;
            this.m_logging = log;
            this.outputDir = outDir;
            this.paths = path;
            this.broad = new TcpServer();
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach (string p in this.paths)
            {
                IDirectoryHandler a = new DirectoryHandler(this.m_logging, this.m_controller, p);
                a.StartHandleDirectory(p);

                a.DirectoryClose += HandlerSaysIWantToCloseMyself;
                CommandRecieved += a.OnCommandRecieved;                
            }          
        }


        private void HandlerSaysIWantToCloseMyself(object sender, DirectoryCloseEventArgs e)
        {
            this.CommandRecieved -= ((DirectoryHandler)sender).OnCommandRecieved;
        }


        public void StopServer()
        {
            foreach (string p in this.paths)
            {
                this.CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, new string[] { }, p));
            }
            
        }



        
       
    }
}
