using System.Collections.Generic;
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
        
        [Option('l', "languages", Required = false, HelpText = "When converting to json-directory: Languages to convert. All if not specified.", Separator = ',')]
        public IEnumerable<string>? Languages { get; set; }
        
        [Option('e', "color-empty", Required = false, HelpText = "When converting to Excel-file: Color empty cells")]
        public bool ColorEmpty { get; set; }
    }
}