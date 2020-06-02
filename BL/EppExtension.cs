using OfficeOpenXml;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FlexibleDBMS
{
   public static class EppExtension
    {
        /// <summary>
        /// Import excel table as object's List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> ConvertTableToObjects<T>(this ExcelTable table) where T : new()
        {
            //Using:
            // public class ExcelData
            //{
            //    public string Id { get; set; }
            //    public string Name { get; set; }
            //    public string Gender { get; set; }
            //}
            //var fi = new FileInfo(@"c:\temp\Table_To_Object.xlsx");

            //using (var package = new ExcelPackage(fi))
            //{
            //    var workbook = package.Workbook;
            //    var worksheet = workbook.Worksheets.First();
            //    var ThatList = worksheet.Tables.First().ConvertTableToObjects<ExcelData>();
            //    foreach (var data in ThatList)
            //    {
            //        Console.WriteLine(data.Id + data.Name + data.Gender);
            //    }

            //    package.Save();
            //}

            //DateTime Conversion
            var convertDateTime = new Func<double, DateTime>(excelDate =>
            {
                if (excelDate < 1)
                    throw new ArgumentException("Excel dates cannot be smaller than 0.");

                var dateOfReference = new DateTime(1900, 1, 1);

                if (excelDate > 60d)
                    excelDate = excelDate - 2;
                else
                    excelDate = excelDate - 1;
                return dateOfReference.AddDays(excelDate);
            });

            //Get the properties of T
            var tprops = (new T())
                .GetType()
                .GetProperties()
                .ToList();

            //Get the cells based on the table address
            var start = table.Address.Start;
            var end = table.Address.End;
            var cells = new List<ExcelRangeBase>();

            //Have to use for loops insteadof worksheet.Cells to protect against empties
            for (var r = start.Row; r <= end.Row; r++)
                for (var c = start.Column; c <= end.Column; c++)
                    cells.Add(table.WorkSheet.Cells[r, c]);

            var groups = cells
                .GroupBy(cell => cell.Start.Row)
                .ToList();

            //Assume the second row represents column data types (big assumption!)
            var types = groups
                .Skip(1)
                .First()
                .Select(rcell => rcell.Value.GetType())
                .ToList();

            //Assume first row has the column names
            var colnames = groups
                .First()
                .Select((hcell, idx) => new { Name = hcell.Value.ToString(), index = idx })
                .Where(o => tprops.Select(p => p.Name).Contains(o.Name))
                .ToList();

            //Everything after the header is data
            var rowvalues = groups
                .Skip(1) //Exclude header
                .Select(cg => cg.Select(c => c.Value).ToList());

            //Create the collection container
            var collection = rowvalues
                .Select(row =>
                {
                    var tnew = new T();
                    colnames.ForEach(colname =>
                    {
                //This is the real wrinkle to using reflection - Excel stores all numbers as double including int
                var val = row[colname.index];
                        var type = types[colname.index];
                        var prop = tprops.First(p => p.Name == colname.Name);

                //If it is numeric it is a double since that is how excel stores all numbers
                if (type == typeof(double))
                        {
                            if (!string.IsNullOrWhiteSpace(val?.ToString()))
                            {
                        //Unbox it
                        var unboxedVal = (double)val;

                        //FAR FROM A COMPLETE LIST!!!
                        if (prop.PropertyType == typeof(Int32))
                                    prop.SetValue(tnew, (int)unboxedVal);
                                else if (prop.PropertyType == typeof(double))
                                    prop.SetValue(tnew, unboxedVal);
                                else if (prop.PropertyType == typeof(DateTime))
                                    prop.SetValue(tnew, convertDateTime(unboxedVal));
                                else
                                    throw new NotImplementedException(String.Format("Type '{0}' not implemented yet!", prop.PropertyType.Name));
                            }
                        }
                        else
                        {
                    //Its a string
                    prop.SetValue(tnew, val);
                        }
                    });

                    return tnew;
                });


            //Send it back
            return collection;
        }


        /// <summary>
        /// Used EPPlus
        /// https://stackoverrun.com/ru/q/3109752
        /// </summary>
        /// <param name="pathToFile">path to exported file</param>
        /// <param name="nameSheet">name of the sheet</param>
        /// <param name="columnsRedColor">caption columns which data backgroud will be filled red color</param>
        /// <param name="columnsGreenColor">caption columns which data backgroud will be filled green color</param>
        /// <param name="tabular">do pivot table like Tabular</param>
        public static void ExportToExcel(
            this DataTable source,
            string pathToFile,
            string nameSheet,
            TypeOfPivot selector,
            string[] columnsRedColor = null,
            string[] columnsGreenColor = null,
            bool tabular = false)
        {
            DataTable table = source;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(pathToFile);

            if (fileInfo.Exists) fileInfo.Delete();

            //https://riptutorial.com/epplus/example/26056/number-formatting
            using (ExcelPackage excel = new ExcelPackage(fileInfo))
            {
                var wsData = excel.Workbook.Worksheets.Add(nameSheet);
                wsData.Cells["A2"].LoadFromDataTable(table, true, TableStyles.Medium6);

                if (table.Rows.Count != 0)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        // format all dates in german format (adjust accordingly)
                        if (col.DataType == typeof(System.DateTime))
                        {
                            var colNumber = col.Ordinal + 1;
                            var range = wsData.Cells[2, colNumber, table.Rows.Count + 2, colNumber];
                            range.Style.Numberformat.Format = "yyyy.MM.dd"; //"dd.MM.yyyy"
                        }
                        else if (col.DataType == typeof(decimal) || col.DataType == typeof(int) || col.DataType == typeof(long) || col.DataType == typeof(double))
                        {
                            var colNumber = col.Ordinal + 1;
                            var range = wsData.Cells[2, colNumber, table.Rows.Count + 2, colNumber];
                            range.Style.Numberformat.Format = "0.00"; //"dd.MM.yyyy"
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            var colNumber = col.Ordinal + 1;
                            var range = wsData.Cells[2, colNumber, table.Rows.Count + 2, colNumber];
                            //  range.Style.Numberformat.Format = "@"; //"dd.MM.yyyy"
                            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }
                    }

                    //Set special color at pointed columns
                    for (int c = 1; c < 1 + table.Columns.Count; c++)
                    {
                        if (columnsRedColor?.Length > 0)
                        {
                            foreach (var col in columnsRedColor)
                            {
                                if (col?.Trim().Length > 0 && c == table.Columns.IndexOf(col))
                                {
                                    for (int r = 3; r < table.Rows.Count + 3; r++)
                                    {
                                        if (wsData.Cells[r, c + 1]?.ToString()?.Length > 0)
                                        {
                                            wsData.Cells[r, c + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            wsData.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SandyBrown);
                                        }
                                    }
                                }
                            }
                        }
                        if (columnsGreenColor?.Length > 0)
                        {
                            foreach (var col in columnsGreenColor)
                            {
                                if (col?.Trim().Length > 0 && c == table.Columns.IndexOf(col))
                                {
                                    for (int r = 3; r < table.Rows.Count + 3; r++)
                                    {
                                        if (wsData.Cells[r, c + 1]?.ToString()?.Length > 0)
                                        {
                                            wsData.Cells[r, c + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                            wsData.Cells[r, c + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.PaleGreen);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Set format of header of table
                    var headerRange = wsData.Cells[2, 1, 2, table.Columns.Count];
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerRange.Style.WrapText = true;
                    headerRange.Style.Font.Size = 9;
                    headerRange.Style.Font.Bold = true;

                    //Set format of body of table
                    var bodyRange = wsData.Cells[3, 1, table.Rows.Count + 2, table.Columns.Count];
                    bodyRange.Style.WrapText = false;
                    bodyRange.Style.Font.Size = 8;
                    bodyRange.Style.Font.Bold = false;

                    //var dataRange1 = wsData.Cells[wsData.Dimension.Address];
                    var dataRange = wsData.Cells[wsData.Dimension.Address.ToString()];
                    dataRange.Style.Font.Name = "Tahoma";
                    dataRange.AutoFitColumns();

                    switch (selector)
                    {
                        case TypeOfPivot.Accountant:  //Сводная из бухгалтерских данных
                            {
                                var wsPivot = excel.Workbook.Worksheets.Add("Сводная");
                                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A3"], dataRange, "Сводная");
                                pivotTable.AddPivotTable(
                                    "Сводная",
                                    new string[] { "ФИО сотрудника", "ТАРИФНАЯ МОДЕЛЬ", "Номер телефона абонента" },
                                    new string[] { "Итого по контракту, грн", "К оплате владельцем номера, грн" },
                                    new string[] { },
                                    new string[] { "Подразделение" },
                                    tabular
                                    );
                                break;
                            }
                        case TypeOfPivot.Market: // Сводная из данных для маркетинга
                            {
                                var wsPivot = excel.Workbook.Worksheets.Add("Сводная");
                                var pivotTable = wsPivot.PivotTables.Add(wsPivot.Cells["A3"], dataRange, "Сводная");
                                pivotTable.AddPivotTable(
                                    "Интернет",
                                    new string[] { },
                                    new string[] { "Суммарно, МБ", "Количество" },
                                    new string[] { },
                                    new string[] { "Подразделение", "ФИО", "Номер телефона" },
                                    tabular
                                    );
                                break;
                            }
                        case TypeOfPivot.MarketWithChart: // Графики
                        case TypeOfPivot.AccountantWithChart: // Графики
                            {
                                var wsChart = excel.Workbook.Worksheets.Add("Графики");
                                //      var chart = wsChart.Drawings.AddChart("Графики", OfficeOpenXml.Drawing.Chart.eChartType.BarOfPie, pivotTable);
                                //      chart.SetPosition(1, 0, 1, 0);
                                //       chart.SetSize(300, 300);
                                //      pivotTable.DataOnRows = true; //don't show table
                                break;
                            }
                        case TypeOfPivot.NonePivot:
                        default:
                            break;
                    }
                }
                excel.Save();
            }


        }


        /// <summary>
        /// Формирует ExcelPivotTable согласно переданным данным
        /// </summary>
        /// <param name="table">Обрабатываемая ExcelPivotTable таблица</param>
        /// <param name="nameTable">Имя сводной таблицы</param>
        /// <param name="pageFields">список колонок из DataTable для формирования фильтра сводной таблицы</param>
        /// <param name="dataFileds">список колонок из DataTable для рассчета вычисляемых данных (их названия идут в список колонок)</param>
        /// <param name="columnFileds">список колонок из DataTable для колонок сводной таблицы</param>
        /// <param name="rowFileds">список колонок из DataTable для формирования строк сводной таблицы</param>
        /// <param name="tabular">do pivot table like Tabular</param>
        /// <returns></returns>
        public static ExcelPivotTable AddPivotTable(this ExcelPivotTable table,
            string nameTable,
            string[] pageFields,
            string[] dataFileds,
            string[] columnFileds,
            string[] rowFileds,
            bool tabular = false
            )
        {
            var pivotTable = table;

            if (tabular)
            {
                pivotTable.Compact = false;
                pivotTable.CompactData = false;
                pivotTable.Indent = 0;
                pivotTable.RowGrandTotals = false;
                pivotTable.ShowMemberPropertyTips = false;
                pivotTable.Outline = false;
                pivotTable.OutlineData = false;
            }
            else
            {
                pivotTable.Compact = true;
                pivotTable.CompactData = true;
                pivotTable.RowGrandTotals = true;
                pivotTable.Outline = true;
                pivotTable.OutlineData = true;
            }


            pivotTable.MultipleFieldFilters = true;
            pivotTable.ColumnGrandTotals = true;
            pivotTable.GridDropZones = false;
            pivotTable.ShowError = true;
            pivotTable.ErrorCaption = "[error]";
            pivotTable.ShowHeaders = true;
            pivotTable.UseAutoFormatting = true;
            pivotTable.ApplyWidthHeightFormats = true;
            pivotTable.ShowDrill = true;
            pivotTable.FirstHeaderRow = 1;// first row has headers
            pivotTable.FirstDataCol = 1;// first col of data
            pivotTable.FirstDataRow = 2;// first row of data
            pivotTable.RowHeaderCaption = nameTable; //Имя таблицы

            //Filter(PageFields)
            if (pageFields?.Length > 0)
            {
                foreach (var columnName in pageFields)
                {
                    if (columnName.Trim()?.Length > 0)
                    {
                        var field = pivotTable.Fields[columnName];//Дата счета
                        field.Sort = eSortType.Ascending;
                        if (tabular)
                        {
                            field.Outline = false;
                            field.Compact = false;
                            field.ShowAll = false;
                            field.SubtotalTop = false;
                        }

                        pivotTable.PageFields.Add(field);
                    }
                }
            }

            //Вычисляемые данные(DataFields)
            if (dataFileds?.Length > 0)
            {
                foreach (var columnName in dataFileds)
                {
                    if (columnName.Trim()?.Length > 0)
                    {
                        var field = pivotTable.Fields[columnName];
                        if (tabular)
                        {
                            field.Outline = false;
                            field.Compact = false;
                            field.ShowAll = false;
                            field.SubtotalTop = false;
                        }
                        else
                        {
                            field.Compact = true;
                            field.Outline = true;                     //Отображать результат в колонках 
                            field.ShowInFieldList = true;                 //Отображать результат в колонках
                        }

                        var dataField = pivotTable.DataFields.Add(field); //"Суммарно, МБ" - имя вычисляемой колонки
                        dataField.Name = columnName;
                        dataField.Function = DataFieldFunctions.Sum; //результат - сумма данных в ячейках
                        dataField.Field.SubTotalFunctions = eSubTotalFunctions.Sum; //результат - сумма данных в ячейках
                        dataField.Format = "0.00";
                    }
                }
            }

            //Columns(ColumnFields)
            if (columnFileds?.Length > 0)
            {
                foreach (var columnName in columnFileds)
                {
                    if (columnName.Trim()?.Length > 0)
                    {
                        var field = pivotTable.Fields[columnName];
                        if (tabular)
                        {
                            field.Outline = false;
                            field.Compact = false;
                            field.ShowAll = false;
                            field.SubtotalTop = false;
                        }
                        pivotTable.ColumnFields.Add(field);
                    }
                }
            }

            //Rows(Caption)
            if (rowFileds?.Length > 0)
            {
                foreach (var columnName in rowFileds)
                {
                    if (columnName.Trim()?.Length > 0)
                    {
                        var field = pivotTable.Fields[columnName];
                        var rowField = pivotTable.RowFields.Add(field);
                        rowField.Sort = eSortType.Ascending;

                        if (tabular)
                        {
                            field.Outline = false;
                            field.Compact = false;
                            field.ShowAll = false;
                            field.SubtotalTop = false;
                        }
                    }
                }
            }

            pivotTable.DataOnRows = false;

            return pivotTable;
        }
    }

    /// <summary>
    /// Selector The type of PivotTable when do Export DataTable To Excel
    /// </summary>
    public enum TypeOfPivot
    {
        NonePivot = 0,
        Accountant = 4,
        AccountantWithChart = 5,
        Market = 8,
        MarketWithChart = 9,
    }
}
