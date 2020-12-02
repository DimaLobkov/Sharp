using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigManager;

namespace FileWatcherService
{
    public class Options
    {
        public PathToDirectory PathToDirectory { get; set; }
        public Archive Archive { get; set; }
    }

    public class PathToDirectory
    {
        public string SourceDirectory { get; set; }
        public string TargetDirectory { get; set; }
    }

    public class Archive
    {
        public string Name { get; set; }
    }
}
