using System;

namespace I18nConverter
{
    public class Logger
    {
        private readonly CommandLineOptions _options;

        public Logger(CommandLineOptions options)
        {
            _options = options;
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogVerbose(string message)
        {
            if (_options.Verbose)
            {
                Console.WriteLine(message);
            }
        }
    }
}