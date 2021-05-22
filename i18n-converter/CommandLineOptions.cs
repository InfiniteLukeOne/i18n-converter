using CommandLine;
#pragma warning disable 8618

namespace I18nConverter
{
    public class CommandLineOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
        
        [Option('i', "in", Required = true, HelpText = "Input Excel-file or json-directory.")]
        public string In { get; set; }

        [Option('o', "out", Required = true, HelpText = "Output Excel-file or json-directory.")]
        public string Out { get; set; }
    }
}