using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData
{
    
    public class ConfigData
    {
        public string[] paths       { get; private set; }
        public string outputDir     { get; private set; }
        public string sourceName    { get; private set; }
        public string logName       { get; private set; }
        public int thumbnailSize    { get; private set; }

        public ConfigData(string[] paths, string outputDir, string sourceName,
            string logName, int thumbnailSize)
        {
            this.paths = paths;
            this.outputDir = outputDir;
            this.sourceName = sourceName;
            this.logName = logName;
            this.thumbnailSize = thumbnailSize;
        }



    }
}
