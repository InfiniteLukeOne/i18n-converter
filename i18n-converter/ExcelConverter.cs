using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace I18nConverter
{
    public class ExcelConverter
    {
        private readonly Logger _logger;

        public ExcelConverter(Logger logger)
        {
            _logger = logger;
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> FromExcel(string inPath)
        {
            _logger.LogVerbose("Reading Excel...");
            IWorkbook wb = new XSSFWorkbook(inPath);

            var namespaces = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            for (int sheetIndex = 0; sheetIndex < wb.NumberOfSheets; sheetIndex++)
            {
                var sheet = wb.GetSheetAt(sheetIndex);

                var namespaceDictionary = ReadSheet(sheet);

                namespaces.Add(sheet.SheetName, namespaceDictionary);
            }

            return namespaces;
        }

        private Dictionary<string, Dictionary<string, string>> ReadSheet(ISheet sheet)
        {
            var namespaceDictionary = new Dictionary<string, Dictionary<string, string>>();
            _logger.LogVerbose(sheet.SheetName);

            var languagesRow = sheet.GetRow(sheet.FirstRowNum);
            foreach (var languageCell in languagesRow.Cells)
            {
                var language = languageCell.StringCellValue;
                _logger.LogVerbose(language);

                var languageDictionary = new Dictionary<string, string>();
                for (int rowIndex = sheet.FirstRowNum + 1; rowIndex < sheet.LastRowNum + 1; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    var key = row.GetCell(0).StringCellValue;
                    
                    var cell = row.GetCell(languageCell.ColumnIndex);
                    if (cell == null)
                    {
                        continue;
                    }

                    var value = cell.StringCellValue;
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    _logger.LogVerbose(key + ": " + value);
                    languageDictionary.Add(key, value);
                }

                namespaceDictionary.Add(language, languageDictionary);
            }

            return namespaceDictionary;
        }

        public void ToExcel(Dictionary<string, Dictionary<string, Dictionary<string, string>>> namespaces,
            string outPath)
        {
            _logger.LogVerbose("Generating Excel...");

            var i18NNamespaces = namespaces.Keys;
            var languages = namespaces.Values.SelectMany(n => n.Keys).Distinct().ToList();

            IWorkbook wb = new XSSFWorkbook();
            
            var font = wb.CreateFont();
            font.IsBold = true; 
            var cellStyle = wb.CreateCellStyle();
            cellStyle.SetFont(font);
            
            // ReSharper disable once InconsistentNaming
            foreach (var i18nNamespace in i18NNamespaces)
            {
                ISheet s1 = wb.CreateSheet(i18nNamespace);
                IRow headerRow = s1.CreateRow(0);
                
                var keyHeaderCell = headerRow.CreateCell(0);
                keyHeaderCell.SetCellValue("i18n-key");
                keyHeaderCell.CellStyle = cellStyle;
                
                for (var languageIndex = 0; languageIndex < languages.Count; languageIndex++)
                {
                    var language = languages[languageIndex];
                    var languageCell = headerRow.CreateCell(languageIndex + 1);
                    languageCell.SetCellValue(language);
                    languageCell.CellStyle = cellStyle;
                }

                if (namespaces.TryGetValue(i18nNamespace, out var namespaceDictionary))
                {
                    var keys = namespaceDictionary
                        .Values
                        .SelectMany(v => v.Keys)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToList();

                    for (var rowIndex = 0; rowIndex < keys.Count; rowIndex++)
                    {
                        var key = keys[rowIndex];
                        var row = s1.CreateRow(rowIndex + 1);

                        var keyCell = row.CreateCell(0);
                        keyCell.SetCellValue(key);
                        keyCell.CellStyle = cellStyle;

                        var languageValues = languages.Select(l =>
                            namespaceDictionary.GetValueOrDefault(l)?.GetValueOrDefault(key)).ToList();
                        for (int languageIndex = 0; languageIndex < languageValues.Count; languageIndex++)
                        {
                            var value = languageValues[languageIndex];
                            if (value != null)
                            {
                                row.CreateCell(languageIndex + 1).SetCellValue(value);
                            }
                        }
                    }
                }
            }

            _logger.LogVerbose("Writing output...");
            using (FileStream fs = File.Create(outPath))
            {
                wb.Write(fs);
            }
        }
    }
}