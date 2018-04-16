using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using ImageService.Logging;
using System.Configuration;
using ImageService.Logging.Modal;






namespace WindowsImageService
{

    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };



    public partial class ImageService : ServiceBase
    {
        private ImageServer m_imageServer;          // The Image Server
        private IImageServiceModal modal;
        private IImageController controller;
        private ILoggingService logging;
        

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        

        public ImageService(string [] args)
        {
            InitializeComponent();
            string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];            
            
            eventLog1 = new System.Diagnostics.EventLog();

            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;            
        }

        private void CreateParts() {
            // now reading from configuration file:            
            string [] paths = ConfigurationManager.AppSettings["Handler"].Split(';');
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string sourceName = ConfigurationManager.AppSettings["SourceName"];
            string logName = ConfigurationManager.AppSettings["LogName"];
            int thumbnailSize = int.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);

            // model create:
            this.modal = new ImageServiceModal(outputDir, thumbnailSize);

            this.controller = new ImageController(this.modal);

            // Logger create and assign to event(in LoggingService)
            this.logging = new LoggingService();
            this.logging.MessageRecieved += eventLog1_EntryWritten;
            this.m_imageServer = new ImageServer(this.controller, this.logging, outputDir, paths);
        }

        protected override void OnStart(string[] args) {

            eventLog1.WriteEntry("Oriel is the king and sapphire also");
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // set things made by us
            CreateParts();                        

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }      

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop. seeemek");
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            this.m_imageServer.StopServer();


        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
            eventLog1.WriteEntry("Oriel and sapphire are the king ");
        }

        private void eventLog1_EntryWritten(object sender, MessageRecievedEventArgs e)
        {
            EventLogEntryType a = EventLogEntryType.Error;

            switch (e.Status)            {
                case MessageTypeEnum.FAIL:
                    a = EventLogEntryType.FailureAudit;
                    break;
                case MessageTypeEnum.INFO:
                    a = EventLogEntryType.Information;
                    break;
                case MessageTypeEnum.WARNING:
                    a = EventLogEntryType.Warning;
                    break;
            }
            this.eventLog1.WriteEntry(e.Message, a);
        }
    } // END OF CLASS
}
