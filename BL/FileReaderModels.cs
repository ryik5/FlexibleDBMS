using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public class FileReaderModels
    {
        public static AutoResetEvent evntWaitHandle = new AutoResetEvent(false);
        public int importedRows = 0;
        public IList<IModels> listCommonModels;
        public IModels columnNames;
        IParserRowTo parsedModel;

        public delegate void Message<TextEventArgs>(object sender, TextEventArgs e);
        public event Message<TextEventArgs> EvntInfoMessage;

        public delegate void CollectionFull<BoolEventArgs>(object sender, BoolEventArgs e);
        public event CollectionFull<BoolEventArgs> EvntCollectionFull;
        public event CollectionFull<BoolEventArgs> EvntHeaderReady;


        public async Task SelectImportingMethod(string filePath, ImportedFileType typeFile, int maxElementsInDictionary)
        {
            Encoding _encoding = Encoding.GetEncoding(1251);
            switch (typeFile)
            {
                case ImportedFileType.Text:
                    await ImportTextFile(filePath, _encoding, maxElementsInDictionary);
                    break;
                case ImportedFileType.Excel:
                    ImportExcelFile(filePath, maxElementsInDictionary);
                    break;
            }
        }

        async Task ImportTextFile(string filePath, Encoding encoding, int maxElementsInDictionary)
        {
            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            string nameColumns = null;
            string currentRow;
            IModels models;
            columnNames = new ModelCommonStore();
            listCommonModels = new List<IModels>(maxElementsInDictionary);

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    while ((currentRow = await reader.ReadLineAsync())?.Trim()?.Length > 10)
                    {
                        if (nameColumns == null)
                        {
                            importedRows = 0;

                            parsedModel = new ParserRowModelCommon(currentRow);
                            columnNames = (parsedModel as ParserRowModelCommon).MatchColumnToAlias();
                            nameColumns = (parsedModel as ParserRowModelCommon).ImportedColumnName;

                            EvntHeaderReady?.Invoke(this, new BoolEventArgs(true));//cHeader is ready  
                        } //first found not_empty_line containes name columns
                        else
                        {
                            parsedModel = new ParserRowModelCommon(currentRow, nameColumns);

                            models = parsedModel?.ConvertRowToModels();

                            if (models?.list?.Count > 0)
                            {
                                importedRows++;
                                listCommonModels.Add(models);

                                if (importedRows > 0 && importedRows % maxElementsInDictionary == 0)
                                {
                                    EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full
                                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"lastRow: {currentRow}" +
                                        $"{Environment.NewLine}parsed: {models.AsString()}" +
                                        $"{Environment.NewLine}Ожидаю пока запишутся данные(до 5 сек.)..."));

                                    FileReaderModels.evntWaitHandle.WaitOne(5000);

                                    listCommonModels = new List<IModels>(maxElementsInDictionary);
                                }
                            }
                        }
                    }
                }
            }

            if (listCommonModels?.Count > 0)
            {
                EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//last part of the collection
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ожидаю пока запишется последняя часть данных(до 2 сек.)..."));
                FileReaderModels.evntWaitHandle.WaitOne(2000);
            }
        }

        void ImportExcelFile(string filePath, int maxElementsInDictionary)
        {
            string nameColumns = null;
            string currentRow;
            IModels models;
            columnNames = new ModelCommonStore();
            listCommonModels = new List<IModels>(maxElementsInDictionary);
            var fi = new FileInfo(filePath);

            using (var package = new ExcelPackage(fi))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();

                //get the first worksheet in the workbook
                int colCount = worksheet.Dimension.End.Column;  //get Column Count
                int rowCount = worksheet.Dimension.End.Row;     //get row count

                EvntInfoMessage?.Invoke(this, new TextEventArgs($"File contains: {colCount} columns{Environment.NewLine}and {rowCount} rows"));
                for (int row = 1; row <= rowCount; row++)
                {
                    currentRow = string.Empty;
                    for (int col = 1; col <= colCount; col++)
                    {
                        currentRow += $"{worksheet.Cells[row, col]?.Value?.ToString()?.Trim()}|";
                    }

                    currentRow = currentRow?.TrimEnd('|');

                    if (currentRow.Contains("|"))
                    {
                        if (nameColumns == null)
                        {
                            importedRows = 0;

                            parsedModel = new ParserRowModelCommon(currentRow);
                            columnNames = (parsedModel as ParserRowModelCommon).MatchColumnToAlias();
                            nameColumns = (parsedModel as ParserRowModelCommon).ImportedColumnName;

                            EvntHeaderReady?.Invoke(this, new BoolEventArgs(true));//cHeader is ready                         
                        } //first found not_empty_line containes name columns
                        else
                        {
                            parsedModel = new ParserRowModelCommon(currentRow, nameColumns);

                            models = parsedModel?.ConvertRowToModels();

                            if (models?.list?.Count > 0)
                            {
                                importedRows++;
                                listCommonModels.Add(models);

                                if (importedRows > 0 && importedRows % maxElementsInDictionary == 0)
                                {
                                    EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full
                                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"lastRow: {currentRow}" +
                                        $"{Environment.NewLine}parsed: {models.AsString()}" +
                                        $"{Environment.NewLine}Ожидаю пока запишутся данные(до 5 сек.)..."));

                                    FileReaderModels.evntWaitHandle.WaitOne(5000);
                                    listCommonModels = new List<IModels>(maxElementsInDictionary);
                                }
                            }
                        }
                    }
                }
            }
            if (listCommonModels?.Count > 0)
            {
                EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//last part of the collection
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ожидаю пока запишется последняя часть данных(до 2 сек.)..."));
                FileReaderModels.evntWaitHandle.WaitOne(2000);
            }
        }
    }

    public enum ImportedFileType
    {
        Text = 0,
        Excel = 2
    }
}
