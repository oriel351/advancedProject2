
using System.ServiceProcess;


namespace WindowsImageService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string [] args)
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] {
                new ImageService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
