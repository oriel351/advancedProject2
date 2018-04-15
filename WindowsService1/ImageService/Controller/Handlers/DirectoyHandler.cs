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
            //now



            // e will filter the relevant files by their exstentions
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

            //addind watcher 
            this.m_dirWatcher.Created += new FileSystemEventHandler(onCreate);
            this.m_dirWatcher.Changed += new FileSystemEventHandler(onCreate);

            //Set the listener to listen
            this.m_logging.Log("Start handeling dir: " + dirPath, MessageTypeEnum.INFO);
            this.m_dirWatcher.EnableRaisingEvents = true;

        }

        public void OnCloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            try
            {
                // Don't listen to the dir
                this.m_dirWatcher.EnableRaisingEvents = false;
                // Remove from the event OnCommandReceived.
                ((ImageServer)sender).CommandRecieved -= this.OnCommandRecieved;
                this.m_logging.Log("Path Handler Closed - " + this.m_path, MessageTypeEnum.INFO);
            }
            catch (Exception e)
            {
                this.m_logging.Log("Failed To Close Path Handler - " + this.m_path + "-"
                    + e.ToString(), MessageTypeEnum.FAIL);
            }
        }


        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            bool currentResult;
            string message = this.m_controller.ExecuteCommand(e.CommandID, e.Args, out currentResult);
            // According to the current result - INFO if true, FAIL if not
            if (currentResult)
            {
                //Log INFO
                this.m_logging.Log(message, MessageTypeEnum.INFO);
            }
            else
            {

                //Log FAIL
                this.m_logging.Log(message, MessageTypeEnum.FAIL);
            }
        }

        private void onCreate(object sender, FileSystemEventArgs e)
        {
            this.m_logging.Log("onCreat - " + e.FullPath, MessageTypeEnum.INFO);
            string extension = Path.GetExtension(e.FullPath);
            // check that the file is an image.
            if (this.m_imageSuffix.Contains(extension))
            {
                //Gets current arguments of the path
                string[] args = { e.FullPath };
                CommandRecievedEventArgs cmd = new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand, args, "");
                this.OnCommandRecieved(this, cmd);
            }


        }
    }
}
