using CommandLine;

namespace Reflektor.Models
{
    public class CommandLineOptions
    {
        [Option('f', "jobsfilepath", HelpText = "The path to the Reflektor jobs file.")]
        public string JobsFilePath { get; set; }
    }
}
