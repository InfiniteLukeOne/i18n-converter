# i18n-converter
Cross-platform console application for converting a json structure (as used by aurelia-i18n) to Excel and back.

## Parameters
```
  -v, --verbose        Set output to verbose messages.

  -i, --in             Required. Input Excel-file or json-directory.

  -o, --out            Required. Output Excel-file or json-directory.
  
  -l, --languages      When converting to json-directory: Languages to convert. All if not specified.

  -e, --color-empty    When converting to Excel-file: Color empty cells
```

## Excel format
Is used if --in parameter has .xlsx extension and is a file. Is used if --out parameter has .xlsx extension and is not a directory.

The application reads or creates an Excel file with one sheet for each namespace. The top row specifies the language, the left row specifies the key. Sub keys are seperated by dots.

### Example sheet
| i18n-key     | de    | en    |
| ------------ | ----- | ----- |
| value1       | test  | test  |
| value2.test1 | asdf  | asdf  |
| value2.test2 |       | hallo |
| value2.test3 | test3 |       |	

## JSON format
Is used if --in parameter is a directory. Is used in any other case for --out parameter.

Inside the specified folder the application reads or creates folders for each language inside which are json files for each namespace. Namespace files without translation or keys without translation will not be created.

### Example file structure
```
locales/
├── de
│   ├── test.json
│   └── translation.json
└── en
    └── test.json
```

### Example JSON file
```json
{
    "value1": "test",
    "value2": {
        "test1": "asdf",
        "test3": "test3"
    }
}
```
