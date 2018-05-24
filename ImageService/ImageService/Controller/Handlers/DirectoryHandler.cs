using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;




namespace ImageService.Controller.Handlers
{
    public class DirectoryHandler : IDirectoryHandler
    {
        #region Members
        private string[] m_imageSuffix =
            { ".bmp", ".jpg", ".gif", ".png", ".jpeg" };             // Image Suffixes
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;                  //The log
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        // The Event That Notifies that the Directory is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;  

        //Constructor
        public DirectoryHandler(ILoggingService log, IImageController imgController, string dirPath)
        {
            this.m_logging = log;
            this.m_controller = imgController;
            this.m_path = dirPath;
            //the member that listen to the files in folder
            this.m_dirWatcher = new FileSystemWatcher(this.m_path);
            // oriel is the king
        }

        public void StartHandleDirectory(string dirPath)
        {
            //getting the info of the message about the path
            //m_logging.Log("Start Handeling Directory-" + dirPath, MessageTypeEnum.INFO);
            //extract the files in our directory in order to watch them
            string[] files = Directory.GetFiles(m_path);

            this.m_dirWatcher.Created += new FileSystemEventHandler(OnFileArrive);
            this.m_dirWatcher.NotifyFilter = (NotifyFilters.Size | NotifyFilters.FileName);
            this.m_dirWatcher.Filter = "*.*";
            //Set the listener to listen           
            this.m_dirWatcher.EnableRaisingEvents = true;
            this.m_logging.Log("Start handeling dir: " + dirPath, MessageTypeEnum.INFO);

        }

        // The Event that will be activated upon new Command
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if(e.RequestDirPath == this.m_path)
            {
                if(e.CommandID == (int)CommandEnum.CloseCommand) 
                {
                    SelfKill();
                    DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(this.m_path,"Closing Handler"));
                    this.m_logging.Log("Closing handler for folder:" + this.m_path, MessageTypeEnum.INFO);
                }
            }

        } 
       
        void SelfKill()
        {
            this.m_logging.Log("SELF KILL:" + this.m_path, MessageTypeEnum.INFO);
            this.m_dirWatcher.Created -= new FileSystemEventHandler(OnFileArrive);
            this.m_dirWatcher.EnableRaisingEvents = false;
        }

        void OnFileArrive(object sender, FileSystemEventArgs e)
        {
            // writing to log 
            this.m_logging.Log("File Arrived: " + e.FullPath, MessageTypeEnum.INFO);
            bool succeeded;
            string [] ar = { e.FullPath };

            if (this.m_imageSuffix.Contains(Path.GetExtension(e.FullPath))) // check legal suffix
            {
                string returnMsg = this.m_controller.ExecuteCommand((int)(CommandEnum.NewFileCommand), ar, out succeeded);
                if (succeeded)
                {
                    this.m_logging.Log(returnMsg + e.FullPath, MessageTypeEnum.INFO);
                }
                else
                {
                    this.m_logging.Log(returnMsg + e.FullPath, MessageTypeEnum.FAIL);
                }
            }
        }

      

    }
}
