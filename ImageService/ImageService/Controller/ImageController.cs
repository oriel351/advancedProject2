﻿using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>()
            {                
                { (int)CommandEnum.NewFileCommand, new NewFileCommand(this.m_modal) },
                { (int)CommandEnum.CloseCommand, new ImageService.Commands.CloseCommand() }
                
            };
        }
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {           
            resultSuccesful = false;
            ICommand a = this.commands[commandID];
            return a.Execute(args, out resultSuccesful);            
        }
    }
}
