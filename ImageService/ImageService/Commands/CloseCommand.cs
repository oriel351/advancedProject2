﻿using ImageService.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.ImageService.Commands
{
    class CloseCommand : ICommand
    {
        
        

        public string Execute(string[] args, out bool result)
        {
            result = false;
            string filepath = args[0];

        }
    }
}
