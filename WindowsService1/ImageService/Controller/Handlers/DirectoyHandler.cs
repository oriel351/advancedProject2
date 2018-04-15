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
using System.Text.RegularExpressions;


namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private string[] m_imageSuffix =
            { ".bmp", ".jpg", ".gif", ".png" };             // Image Suffixes
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;                  //The log
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;  // The Event That Notifies that the Directory is being closed

        //Constructor
        public DirectoyHandler(ILoggingService log, IImageController imgController, string dirPath)
        {
            this.m_logging = log;
            this.m_controller = imgController;
            this.m_path = dirPath;
            //the member that listen to the files in folder
            this.m_dirWatcher = new FileSystemWatcher(this.m_path);
        }
        void StartHandleDirectory(string dirPath)
        {
            //getting the info of the message about the path
            m_logging.Log("Start Handeling Directory-" + dirPath, MessageTypeEnum.INFO);
            //extract the files in our directory in order to watch them
            string[] files = Directory.GetFiles(m_path);
            //we want to add all the files in the directory to the output dir folder
            //so we move on each path in the current directory
            //now we will filter the relevant files by their exstentions
            //we finaly want to add all the relevant files to the outputdir
            foreach (string filePathInDirectory in files)
            {
                //document to the logger the handeling of the directory
                m_logging.Log("Start handeling directory:" + filePathInDirectory, MessageTypeEnum.INFO);
                //filtering the files by their suffix
                if (this.m_imageSuffix.Contains(Path.GetExtension(filePathInDirectory)))
                {
                    string[] relevantfiles = {filePathInDirectory};
                    CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand,
                        relevantfiles, filePathInDirectory);
                    OnCommandRecieved(this, e);
                }
            }
            //Add new file system event handler
            this.m_dirWatcher.Created += new FileSystemEventHandler(onCreate);
            this.m_dirWatcher.Changed += new FileSystemEventHandler(onCreate);

            //Set the listener to listen
            this.m_logging.Log("Start handeling dir: " + dirPath, MessageTypeEnum.INFO);
            this.m_dirWatcher.EnableRaisingEvents = true;

        }

        void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {     // The Event that will be activated upon new Command

        }

        // Implement Here!
    }
}
