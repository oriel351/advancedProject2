

using ImageService.Commands;
using Newtonsoft.Json;
using SharedData;

namespace ImageService.ImageService.Commands
{
    class ConfigCommand : ICommand
    {
        private ConfigData config;
        
        public ConfigCommand(ConfigData config)
        {
            this.config = config;
        }
        public string Execute(string[] args, out bool result)
        {
            result = true;
            string data = JsonConvert.SerializeObject(this.config);
            return data;
        }
    }
}
