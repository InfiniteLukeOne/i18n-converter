using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace I18nConverter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(options =>
            {
                var logger = new Logger(options);
                
                var inFormat = GetInFormat(options);

                var outFormat = GetOutFormat(options);
                logger.LogVerbose($"From {inFormat} in {options.In} to {outFormat} in {options.Out}.");

                var excelConverter = new ExcelConverter(logger);
                var jsonConverter = new JsonConverter(logger);
                
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> namespaces;
                switch (inFormat)
                {
                    case I18nConversionFormat.Excel:
                        namespaces = excelConverter.FromExcel(options.In);
                        break;
                    case I18nConversionFormat.Json:
                        namespaces = jsonConverter.FromJson(options.In);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (outFormat)
                {
                    case I18nConversionFormat.Excel:
                        excelConverter.ToExcel(namespaces, options.Out, options.ColorEmpty);
                        break;
                    case I18nConversionFormat.Json:
                        jsonConverter.ToJson(namespaces, options.Out, options.Languages);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private static I18nConversionFormat GetInFormat(CommandLineOptions options)
        {
            I18nConversionFormat fromFormat;
            if (File.GetAttributes(options.In).HasFlag(FileAttributes.Directory))
            {
                fromFormat = I18nConversionFormat.Json;
            }
            else
            {
                if (Path.GetExtension(options.In) == ".xlsx")
                {
                    fromFormat = I18nConversionFormat.Excel;
                }
                else
                {
                    throw new ArgumentException("Parameter in is a file but not .xlsx.");
                }
            }

            return fromFormat;
        }

        private static I18nConversionFormat GetOutFormat(CommandLineOptions options)
        {
            I18nConversionFormat toFormat;
            if (Path.GetExtension(options.Out) == ".xlsx")
            {
                if (Directory.Exists(options.Out))
                {
                    toFormat = I18nConversionFormat.Json;
                }
                else
                {
                    toFormat = I18nConversionFormat.Excel;
                }
            }
            else
            {
                toFormat = I18nConversionFormat.Json;
            }

            return toFormat;
        }
    }
}