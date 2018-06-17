using ImageService.Commands;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Commands
{
    class CloseCommand : ICommand
    {

        private ImageServer server;

        public CloseCommand(ImageServer imgServer)
        {
            imgServer.CommandRecieved +=
            this.server = imgServer;
        }
        public string Execute(string[] args, out bool result)
        {
            result = false;
            string filepath = args[0];


        }
    }
}
