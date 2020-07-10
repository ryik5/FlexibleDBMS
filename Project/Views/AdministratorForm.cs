using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public partial class AdministratorForm : Form
    {
        SelectDBForm getNewConnectionForm;
        public Bitmap bmpLogo;

        internal SQLConnectionStore currentSQLConnectionStore { get; set; }// = new SQLConnectionStore();
        SqlAbstractConnector dBOperations { get; set; }

        ModelCommonStringStore tableStore = new ModelCommonStringStore();
        BindingList<string> lstColumn = new BindingList<string>();
        ModelCommonStringStore sampleStore = new ModelCommonStringStore();

        ModelCommonStringStore columnDeleteStore = new ModelCommonStringStore();

        TextBox txtbResultShow;


        //import a txt file in DB
        FileReaderModels readerModelCommon;
        const int MAX_ELEMENTS_COLLECTION = 100000;


        public AdministratorForm()
        {
            InitializeComponent();

            bmpLogo = Properties.Resources.LogoRYIK;
            AdminStatusLabel.Text = $"Текущая БД не установлена";
            currentTableToolStripMenuItem.Text = "Текущая база";
            currentTableToolStripMenuItem.ToolTipText = "Обслуживание ранее подключенной БД";
            clearSelectedDbToolStripMenuItem.Text = "Сжать базу";
            clearSelectedDbToolStripMenuItem.ToolTipText = "Удалить пустые колонки в выбранной локальной SQLite БД";
            updatePlateRegionToolStripMenuItem.Text = "Обновить регионы регистрации номеров а/м";
            updatePlateRegionToolStripMenuItem.ToolTipText = "Проанализировать в текущей БД номера автомашин и добавить колонку, указывающую регион их первичной регистрации. Поддерживаются только базы сформированные во время импорта текстового файла";
            deleteSelectedColumnsToolStripMenuItem.Text = "Удалить столбцы из таблицы";
            deleteSelectedColumnsToolStripMenuItem.ToolTipText = "Удалить безвозвратно выделенные столбцы из таблицы";
            getNewConnectionMenuItem.Text = "Подключить новую базу";
            getNewConnectionMenuItem.ToolTipText = "Подключить локальную БД или настроить подключение к внешней БД ";
            importMenu.Text = "Импорт файла";
            importMenu.ToolTipText = "Импорт файла в сгенерированную приложением БД";
            fromTextFileToolStripMenuItem.Text = "Импортировать текстовый файл";
            fromTextFileToolStripMenuItem.ToolTipText = "Создать новую локальную БД SQLite и импортировать в нее текстовый файл";
            fromExcelFileToolStripMenuItem.Text = "Импортировать Excel файл";
            fromExcelFileToolStripMenuItem.ToolTipText = "Создать новую локальную БД SQLite и импортировать в нее Excel файл";
        }


        private void AdministratorForm_Load(object sender, EventArgs e)
        {
            foreach (var m in menuStrip1.Items)
            {
                if (m is ToolStripMenuItem)
                { (m as ToolStripMenuItem).MouseHover += new EventHandler(ToolStripMenuItem_MouseHover); }
                else if (m is ToolStripItem)
                { (m as ToolStripItem).MouseHover += new EventHandler(ToolStripMenuItem_MouseHover); }
            }

            clearSelectedDbToolStripMenuItem.Click += new EventHandler(ClearSelectedDbToolStripMenuItem_Click);

            updatePlateRegionToolStripMenuItem.Click += new EventHandler(UpdatePlateRegionToolStripMenuItem_Click);

            deleteSelectedColumnsToolStripMenuItem.Click += new EventHandler(DeleteSelectedColumnsToolStripMenuItem_Click);

            getNewConnectionMenuItem.Click += new EventHandler(GetNewConnectionMenuItem_Click);

            fromTextFileToolStripMenuItem.Click += new EventHandler(fromTextFileToolStripMenuItem_Click);

            fromExcelFileToolStripMenuItem.Click += new EventHandler(FromExcelFileToolStripMenuItem_Click);

            currentSQLConnectionStore.EvntConfigChanged += new SQLConnectionStore.ConfigChanged<BoolEventArgs>(CurrentSQLConnectionStore_EvntConfigChanged);

            tableStore.EvntCollectionChanged += new ModelCommonStringStore.ItemAddedInCollection(TableStore_EvntCollectionChanged);

            sampleStore.EvntCollectionChanged += new ModelCommonStringStore.ItemAddedInCollection(SampleStore_EvntCollectionChanged);

            columnDeleteStore.EvntCollectionChanged += new ModelCommonStringStore.ItemAddedInCollection(ColumnDeleteStore_EvntCollectionChanged);

            comboBoxTable.SelectedIndexChanged += new EventHandler(ComboBoxTable_SelectedIndexChanged);

            comboBoxColumn.DataSource = lstColumn;

            comboBoxColumn.SelectedIndexChanged += new EventHandler(comboBoxColumn_SelectedIndexChanged);

            listBoxColumn.MouseDown += new MouseEventHandler(listBoxColumn_MouseDown);

            currentSQLConnectionStore.Refresh();
        }


        private void ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            string text = null;
            if (sender is ToolStripMenuItem)
                text = (sender as ToolStripMenuItem).ToolTipText;
            else if (sender is ToolStripItem)
                text = (sender as ToolStripItem).ToolTipText;

            SetAdminStatusLabelText(text);
        }

        private void ColumnDeleteStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            listBoxColumn.DataSource = columnDeleteStore.ToList();
            listBoxColumn.Sorted = true;
        }

        private void SampleStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            listBoxSample.DataSource = sampleStore?.ToList();
            listBoxSample.Refresh();
        }


        private void TableStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            comboBoxTable.DataSource = null;
            comboBoxTable.Items.Clear();

            comboBoxTable.DataSource = tableStore?.ToList();

            if (tableStore?.ToList()?.Count > 0)
            {
                comboBoxTable.SelectedIndex = 0;
            }
        }

        private void CurrentSQLConnectionStore_EvntConfigChanged(object sender, BoolEventArgs args)
        {
            ISQLConnectionSettings oldSettings = currentSQLConnectionStore?.GetPrevious();
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();

            if (!(string.IsNullOrWhiteSpace(newSettings?.Database)) && File.Exists(newSettings?.Database))
            {
                try { dBOperations.EvntInfoMessage -= AddLineAtTextBoxResultShow; } catch { }
                dBOperations = SQLSelector.SetConnector(newSettings);
                dBOperations.EvntInfoMessage += new SqlAbstractConnector.Message<TextEventArgs>(AddLineAtTextBoxResultShow);
            }

            if (oldSettings == null)
            {
                tableStore.Set(SQLSelector.GetTables(newSettings));
            }
            else
            {
                if (oldSettings.Name != newSettings.Name && oldSettings.Database != newSettings.Database)
                {
                    tableStore.Set(SQLSelector.GetTables(newSettings));
                }
            }

            if (!string.IsNullOrWhiteSpace(newSettings?.Database) && !string.IsNullOrWhiteSpace(newSettings?.Table))
                SetAdminStatusLabelText($"Текущая база: {newSettings?.Database}, таблица: {newSettings?.Table}");
        }


        #region Import 
        private void FromExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        { _ = GetFile(ImportedFileType.Excel); }


        private void fromTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        { _ = GetFile(ImportedFileType.Text); }

        async Task GetFile(ImportedFileType typeFile)
        {
            MakeTextBoxResultShow();
            txtbResultShow.ReadOnly = true;
            EnabledInterface(false);

            string filePath = null;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (typeFile == ImportedFileType.Text)
                    filePath = ofd.OpenFileDialogReturnPath(Properties.Resources.DialogTextFile, "Выберите для импорта текстовый файл с данными и резделителями:");
                else if (typeFile == ImportedFileType.Excel)
                    filePath = ofd.OpenFileDialogReturnPath(Properties.Resources.DialogExcelFile, "Выберите для импорта Excel файл с данными. Первая строчка должна содержать названия колонок:");
            }

            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                await ImportFileIntoLocalDB(filePath, typeFile);
                await Task.Run(() => ShowMessage("Готово!"));
            }
            else { AddLineAtTextBoxResultShow($"Файл '{filePath}' пустой или не выбран"); }

            HideTextBoxResultShow();
            EnabledInterface(true);
        }


        async Task ImportFileIntoLocalDB(string filePath, ImportedFileType fileType)
        {
            readerModelCommon = new FileReaderModels();

            ISQLConnectionSettings newConnection = new SQLConnectionSettings
            {
                Database = $"{Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath))}.db"
            };

            dBOperations = new SQLiteModelDBOperations(newConnection);
            dBOperations.EvntInfoMessage += new SqlAbstractConnector.Message<TextEventArgs>(AddLineAtTextBoxResultShow);
            
            SetAdminStatusLabelText($"Импорт файла '{filePath}' в базу данных...");
            AddLineAtTextBoxResultShow();

            readerModelCommon.EvntInfoMessage += new FileReaderModels.Message<TextEventArgs>(AddLineAtTextBoxResultShow);
            readerModelCommon.EvntHeaderReady += new FileReaderModels.CollectionFull<BoolEventArgs>(ModelCommon_EvntHeaderReady);
            readerModelCommon.EvntCollectionFull += new FileReaderModels.CollectionFull<BoolEventArgs>(ModelCommon_collectionFull);

            await readerModelCommon.SelectImportingMethod(filePath, fileType, MAX_ELEMENTS_COLLECTION);

            readerModelCommon.EvntCollectionFull -= ModelCommon_collectionFull;
            readerModelCommon.EvntHeaderReady -= ModelCommon_EvntHeaderReady;
            readerModelCommon.EvntInfoMessage -= AddLineAtTextBoxResultShow;

            SetAdminStatusLabelText($"Всего в БД '{newConnection.Database}' импортировано {readerModelCommon.importedRows} строк.");
            AddLineAtTextBoxResultShow("newConnection: " + newConnection.ToString());
            dBOperations.EvntInfoMessage -= AddLineAtTextBoxResultShow;

            readerModelCommon = null;
            AddLineAtTextBoxResultShow();

            currentSQLConnectionStore.Set(newConnection);
            currentSQLConnectionStore.Refresh();
        }

        private void ModelCommon_EvntHeaderReady(object sender, BoolEventArgs e)
        {
            IModels header = readerModelCommon.columnNames;

            //make DB and tables by header of text table
            (dBOperations as SQLiteModelDBOperations).PrepareTablesForCommonModel(header);

            //write into Log status info
            AddLineAtTextBoxResultShow($"Количество столбцов в заголовке: {header.ToList().Count}");
            AddLineAtTextBoxResultShow(header.ToString());
        }

        private async void ModelCommon_collectionFull(object sender, BoolEventArgs e)
        {
            if (e.Status)
            {
                IList<IModels> listModels = readerModelCommon.listCommonModels.ToList();
                int readRows = readerModelCommon.importedRows;
                await Task.Run(() => (dBOperations as SQLiteModelDBOperations).WriteModelInTable(listModels));

                AddLineAtTextBoxResultShow($"Эта часть записана:");
                await Task.Run(() => WriteFewModelsInLog(listModels, readRows));
            }
        }

        /// <summary>
        /// Print information about the current status the importing stage
        /// </summary>
        /// <param name="listModels">Imported data' list</param>
        /// <param name="totalReadRow">read total rows summary till the current moment</param>
        public void WriteFewModelsInLog(IList<IModels> listModels, int totalReadRow)
        {
            SetAdminStatusLabelText($"Всего импортировано строк: {totalReadRow}");
            AddLineAtTextBoxResultShow("");
            int numberModel = 0;
            IModels model = listModels.ElementAt(numberModel);
            AddLineAtTextBoxResultShow($"Элемент {numberModel}: {model.ToString()}");

            if (listModels?.Count > 100)
            {
                numberModel = 100;
                model = listModels.ElementAt(numberModel);
                AddLineAtTextBoxResultShow($"Элемент {numberModel}: {model.ToString()}");

                if (listModels?.Count > 1000)
                {
                    numberModel = 1000;
                    model = listModels.ElementAt(numberModel);
                    AddLineAtTextBoxResultShow($"Элемент {numberModel}: {model.ToString()}");
                }
            }
        }
        #endregion


        #region GetNew Connection
        private void GetNewConnectionMenuItem_Click(object sender, EventArgs e)
        {
            GetNewConnection();
        }

        private void GetNewConnection()
        {
            ISQLConnectionSettings tmpConnection = currentSQLConnectionStore?.GetCurrent();

            getNewConnectionForm = new SelectDBForm
            {
                Owner = this,
                Icon = Icon.FromHandle(bmpLogo.GetHicon()),
                Text = "Форма для подключения новой базы",
                currentSQLConnectionStore =
                string.IsNullOrWhiteSpace(tmpConnection?.Name) == true ?
                new SQLConnectionStore() :
                new SQLConnectionStore(tmpConnection)

            };

            getNewConnectionForm.FormClosing += GetNewConnection_FormClosing;
            getNewConnectionForm.Show();
        }

        private void GetNewConnection_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(getNewConnectionForm?.currentSQLConnectionStore.GetCurrent()?.Name) &&
                getNewConnectionForm?.currentSQLConnectionStore.GetCurrent().Database != null &&
                getNewConnectionForm?.currentSQLConnectionStore.GetCurrent().Name != null)
            {
                currentSQLConnectionStore.Set(getNewConnectionForm?.currentSQLConnectionStore.GetCurrent());
            }

            getNewConnectionForm?.Dispose();

            Show();
        }
        #endregion


        #region Clear DB       
        private void EnabledInterface(bool enabled)
        {
            currentTableToolStripMenuItem.Enabled = enabled;
            importMenu.Enabled = enabled;
            getNewConnectionMenuItem.Enabled = enabled;
            comboBoxTable.Enabled = enabled;
            comboBoxColumn.Enabled = enabled;
            listBoxColumn.Enabled = enabled;
        }

        private async void ClearSelectedDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBoxSample.Hide();
            EnabledInterface(false);

            dBOperations.EvntInfoMessage -= DBOperations_EvntInfoMessage;
            dBOperations.EvntInfoMessage += new SqlAbstractConnector.Message<TextEventArgs>(DBOperations_EvntInfoMessage);

            MakeTextBoxResultShow();

            sampleStore.Reset();
            await DeleteColumnsInDB(currentSQLConnectionStore.GetCurrent());

            ShowMessage("Готово!");

            listBoxSample.Show();
            dBOperations.EvntInfoMessage -= DBOperations_EvntInfoMessage;
            HideTextBoxResultShow();
            EnabledInterface(true);
        }

        private async void DeleteSelectedColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBoxSample.Hide();
            EnabledInterface(false);

            dBOperations.EvntInfoMessage -= DBOperations_EvntInfoMessage;
            dBOperations.EvntInfoMessage += new SqlAbstractConnector.Message<TextEventArgs>(DBOperations_EvntInfoMessage);

            MakeTextBoxResultShow();

            sampleStore.Reset();
            await DeleteColumnsInDB(currentSQLConnectionStore.GetCurrent(), columnDeleteStore.ToList());

            ShowMessage("Готово!");

            listBoxSample.Show();
            dBOperations.EvntInfoMessage -= DBOperations_EvntInfoMessage;
            HideTextBoxResultShow();
            EnabledInterface(true);
        }

        private async Task DeleteColumnsInDB(ISQLConnectionSettings settings, IList<string> columns = null)
        {
            if (settings.ProviderName == SQLProvider.SQLite)
            {
                DbSchema schemaDB = DbSchema.LoadDB(settings.Database);

                IList<string> tablesDB = new List<string>();
                DataTable dtForStore = null;
                string queryCheck;

                AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
                AddLineAtTextBoxResultShow($"- Selected DB: '{settings.Database}'");
                IList<string> columnsLeave = new List<string>();
                IList<string> columnsDelete = new List<string>();

                bool isImportedDB = SQLiteCheckImportedDB.Check(settings.Database);
                if (isImportedDB)
                {
                    DbTable db = schemaDB.Tables.Values.First(x => x.TableName.Equals("MainData"));
                    var cancellationToken = new System.Threading.CancellationTokenSource(10000).Token;
                    if (columns?.Count > 0)
                    {
                        columnsDelete = columns;
                        columnsLeave = db.Columns.Where(x => columns.Contains(x.ColumnName) == false).Select(x => x.ColumnName).ToList();
                    }
                    else
                    {
                        foreach (var column in db.Columns)
                        {
                            queryCheck = $"SELECT {column.ColumnName} FROM {db.TableName} WHERE LENGTH(TRIM({column.ColumnName})) > 0  LIMIT 10;";
                            AddLineAtTextBoxResultShow(queryCheck);
                            cancellationToken = new System.Threading.CancellationTokenSource(10000).Token;
                            dtForStore = (dBOperations as SQLiteModelDBOperations).GetTable( queryCheck);

                            if (dtForStore.Rows.Count > 0) { columnsLeave.Add($"{column.ColumnName}"); }
                            else { columnsDelete.Add($"{column.ColumnName}"); }
                        }
                    }
                    AddLineAtTextBoxResultShow(Properties.Resources.SymbolsSos);
                    AddLineAtTextBoxResultShow(Properties.Resources.SymbolsSos);

                    (dBOperations as SQLiteModelDBOperations).ClearDataTable("MainData", columnsLeave); // delete columns in the table 'MainData' except columns - columnsLeave
                    (dBOperations as SQLiteModelDBOperations).ClearDataTable("ColumnNameAndAlias", "ColumnName", columnsDelete); // delete from 'ColumnNameAndAlias' where ColumnName contained columnsDelete
                }
                else
                {
                    DbTable db = schemaDB.Tables.Values.First(x => x.TableName.Equals(settings.Table));

                    if (columns?.Count > 0)
                    {
                        columnsDelete = columns;
                        columnsLeave = db.Columns.Where(x => columns.Contains(x.ColumnName) == false).Select(x => x.ColumnName).ToList();
                        (dBOperations as SQLiteModelDBOperations).ClearDataTable(settings.Table, columnsLeave); // delete columns in the table 'MainData' except columns - columnsLeave
                    }

                }

                AddLineAtTextBoxResultShow($"-  The End  -:");
                AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
                AddLineAtTextBoxResultShow();
            }
            else
            {
                AddLineAtTextBoxResultShow($"{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}");
                AddLineAtTextBoxResultShow($"На данный момент настроена очистка только локальных SQLite БД");
                AddLineAtTextBoxResultShow($"{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}");
            }
        }
        #endregion



        #region Update date in DB
        //Some columns store date as 5 digits string as - "39456"
        //  "39456", // (1/9/2008)
        //  "36557", // (2/1/2000)
        //  "39270"  // (7//7/2007)

        //  //We must have a double to convert the OA date to a real date.   
        //double d = double.Parse(b);

        //  //Get the converted date from the OLE automation date.   
        //DateTime conv = DateTime.FromOADate(d);
        // use: string result = date.FromOQDateToRealDate();

        #region Update Plate' District
        private async void UpdatePlateRegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnabledInterface(false);
            //добавить задачу в конце 
            //импорт =>сжатие базы (выбор отдельных дополнительных колонок для удаления)=> добавление региона принадлежности
            await CheckTableForPlates(currentSQLConnectionStore.GetCurrent().Database);
            EnabledInterface(true);
        }

        //добавить задачу в конце операции 
        // импорт =>сжатие базы (выбор отдельных дополнительных колонок для удаления)=> добавление региона принадлежности
        private async Task CheckTableForPlates(string pathToSqliteDB)
        {
            MakeTextBoxResultShow();
            txtbResultShow.ReadOnly = true;

            if (SQLiteCheckImportedDB.Check(pathToSqliteDB))
            {
                DbSchema schemaDB = DbSchema.LoadDB(pathToSqliteDB);
                IList<string> tablesDB = new List<string>();
                IList<string> columns = new List<string>();
                string queryCheck = null;

                AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
                AddLineAtTextBoxResultShow($"- Selected DB: '{pathToSqliteDB}'");
                string columnsLeave = string.Empty;
                string columnsDelete = string.Empty;

                int totalRows, rowsWithDataInColumn, rowsWithCorrectDataInColumn;

                //check if it is existed 'ColumnPlateDistrict' in the table of the localDB
                bool isColumnDistrictPlate = false; //Column 'ColumnPlateDistrict' is not existed in the table 'MainData'
                bool isExistedColumnDistrictPlate = false; //row 'ColumnPlateDistrict is not existed in the table 'ColumnNameAndAlias'
                string columnWithPlate = string.Empty;

                bool isImportedDB = SQLiteCheckImportedDB.Check(pathToSqliteDB);

                if (isImportedDB)
                {
                    foreach (var table in schemaDB.Tables)
                    {
                        rowsWithDataInColumn = 0;
                        totalRows = 0;
                        if (table.Value.TableName.Equals("MainData"))
                        {
                            foreach (var column in table.Value.Columns)
                            {
                                if (column.ColumnName == CommonConst.ColumnPlateDistrict)
                                {
                                    isColumnDistrictPlate = true;
                                }

                                rowsWithCorrectDataInColumn = 0;

                                if (totalRows == 0)
                                {
                                    queryCheck = $"SELECT COUNT(*) FROM '{table.Value.TableName}'";
                                    await Task.Run(() => totalRows = CountRowsInTable(queryCheck));
                                }

                                if (rowsWithDataInColumn == 0)
                                {
                                    queryCheck = $"SELECT COUNT(*) FROM {table.Value.TableName} WHERE LENGTH(TRIM({column.ColumnName})) > 0;";
                                    await Task.Run(() => rowsWithDataInColumn = CountRowsInTable(queryCheck));
                                }

                                queryCheck =
                                    $"SELECT COUNT(*) " +
                                    $"FROM {table.Value.TableName} " +
                                    $"WHERE LENGTH(TRIM({column.ColumnName})) > 6 AND  LENGTH(TRIM({column.ColumnName})) < 9 " +
                                    $"AND ( " +
                                    $"{column.ColumnName} LIKE 'А%' OR " +
                                    $"{column.ColumnName} LIKE 'В%' OR " +
                                    $"{column.ColumnName} LIKE 'С%' OR " +
                                    $"{column.ColumnName} LIKE 'І%' OR " +
                                    $"{column.ColumnName} LIKE 'К%' OR " +
                                    $"{column.ColumnName} LIKE 'Н%' OR " +
                                    $"{column.ColumnName} LIKE 'A%' OR " +
                                    $"{column.ColumnName} LIKE 'B%' OR " +
                                    $"{column.ColumnName} LIKE 'C%' OR " +
                                    $"{column.ColumnName} LIKE 'I%' OR " +
                                    $"{column.ColumnName} LIKE 'K%' OR " +
                                    $"{column.ColumnName} LIKE 'H%' " +
                                    $");";


                                //////////////////
                                //do correct in DB
                                //PRAGMA case_sensitive_like = false;
                                //////////////////


                                await Task.Run(() => rowsWithCorrectDataInColumn = CountRowsInTable(queryCheck));

                                bool isPlateColumn = false;
                                string results = string.Empty;

                                if (rowsWithCorrectDataInColumn > 0 && rowsWithDataInColumn > 0)
                                {
                                    isPlateColumn =
                                        Math.Round((decimal)rowsWithDataInColumn / (decimal)rowsWithCorrectDataInColumn, 1) > (decimal)0.5 &&
                                        Math.Round((decimal)rowsWithDataInColumn / (decimal)rowsWithCorrectDataInColumn, 1) < (decimal)1.5;
                                }

                                if (isPlateColumn)
                                {
                                    columns.Add(
                                      $"Column '{column.ColumnName}' has auto plates: {isPlateColumn}, {Environment.NewLine}" +
                                      $"Full total rows: {totalRows}, {Environment.NewLine}" +
                                      $"Column total rows: {column.ColumnName} = {rowsWithDataInColumn}, {Environment.NewLine}" +
                                      $"Count right rows: {column.ColumnName} = {rowsWithCorrectDataInColumn}, " +
                                      $"Rows with correct form plates = {(Math.Round((decimal)rowsWithDataInColumn / (decimal)rowsWithCorrectDataInColumn, 1)) * 100}%");

                                    columnWithPlate = column.ColumnName;
                                }
                            }
                        }
                        else if (table.Value.TableName.Equals("ColumnNameAndAlias"))   //SpecialColumn for districtPalte      //ColumnPlateDistrict
                        {
                            queryCheck = $"SELECT ColumnName FROM '{table.Value.TableName}'";
                            using (DataTable dt = GetTable(queryCheck))
                            {
                                foreach (DataRow r in dt.Rows)
                                {
                                    if (r[0].ToString() == CommonConst.ColumnPlateDistrict)
                                    {
                                        isExistedColumnDistrictPlate = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
                    AddLineAtTextBoxResultShow(columns.ToStringNewLine());
                    AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);

                    //update table  'ColumnNameAndAlias' 
                    if (!isExistedColumnDistrictPlate)
                    {
                        queryCheck = $"INSERT INTO 'ColumnNameAndAlias' ('ColumnName', 'ColumnAlias') " +
                        $"VALUES ('{CommonConst.ColumnPlateDistrict}', 'Принадлежность номера региону');";
                        try { dBOperations.DoQuery(queryCheck); } catch (Exception err) { AddLineAtTextBoxResultShow(err.ToString()); }
                    }

                    //ADD COLUMN in the table 'MainData' 
                    if (!isColumnDistrictPlate)
                    {
                        queryCheck = $"ALTER TABLE 'MainData' ADD COLUMN '{CommonConst.ColumnPlateDistrict}' TEXT;";
                        try { dBOperations.DoQuery(queryCheck); } catch (Exception err) { AddLineAtTextBoxResultShow(err.ToString()); }
                    }

                    //update table 'MainData' 
                    if (columns.Count == 1)
                    {
                        //Start transaction
                        dBOperations.DoQuery("BEGIN TRANSACTION;", false);
                        foreach (var plate in CommonConst.plateDistrict)
                        {
                            queryCheck =
                                $"UPDATE 'MainData' " +
                                $"SET '{CommonConst.ColumnPlateDistrict}' = '" + plate.Value.ToString() + "' " +
                                $" WHERE  {columnWithPlate} LIKE '{plate.Key}%';";
                            try { dBOperations.DoQuery(queryCheck, false); } catch (Exception err) { AddLineAtTextBoxResultShow(err.ToString()); }
                        }

                        dBOperations.DoQuery("COMMIT;", false);
                        //Finished commit transaction

                    }
                    else
                    {
                        AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
                        AddLineAtTextBoxResultShow("Данная база подготовлена в другой программе и не подлежит оптимизации");
                        AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
                    }
                }
            }
            AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);
            AddLineAtTextBoxResultShow("Все задачи по обновлению принадлежности номеров а/м регионам завершены");
            AddLineAtTextBoxResultShow(Properties.Resources.SymbolsDashed);

            ShowMessage("Готово!");

            HideTextBoxResultShow();
        }

        private int CountRowsInTable(string queryRowCount)
        {
            int count = 0;
            using (var dt = GetTable(queryRowCount))
            {
                int.TryParse(dt.Rows[0][0].ToString(), out count);
            }

            return count;
        }
        #endregion

        #endregion 

        private void ComboBoxTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();
            newSettings.Table = comboBoxTable?.SelectedItem?.ToString()?.Split('(')[0]?.Trim();

            lstColumn.Clear();
            foreach (var col in SQLSelector.GetColumns(newSettings).ToModelList())
            {
                string name = col.Name.Equals(col.Alias) ? col.Name : $"{col.Name} ({col.Alias})";
                lstColumn.Add(name);
            }

            columnDeleteStore.Reset();

            currentSQLConnectionStore.Set(newSettings);
        }

        private void comboBoxColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            string columnName = comboBoxColumn?.SelectedItem?.ToString()?.Split(' ')[0]?.Trim();

            if (!(string.IsNullOrWhiteSpace(columnName)))
            {
                labelSelectedColumn.Text = $"Примерный набор данных в столбце: '{columnName}'";

                listBoxColumn_AddRemoveSelectedLine(columnName);
                _ = GetSample(columnName);
            }
        }


        private async Task GetSample(string columnName)
        {
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();
            ModelCommonStringStore models = null;

            await Task.Run(() => models = SQLSelector.GetDataSample(newSettings, columnName));

            if (models?.ToList()?.Count > 0)
            {
                sampleStore.Set(models);
            }
            else
            {
                sampleStore.Reset();
                sampleStore.Add(new ModelCommon() { Name = $"В таблице данные отсутствует в столбце '{columnName}'" });
            }
        }

        private void listBoxColumn_MouseDown(object sender, MouseEventArgs e)
        {
            listBoxColumn.SelectedIndex = listBoxColumn.IndexFromPoint(e.X, e.Y);

            string text = listBoxColumn?.SelectedItem?.ToString();
            if (columnDeleteStore?.GetItem(text) != null)
            { columnDeleteStore?.Remove(text); }
        }

        private void listBoxColumn_AddRemoveSelectedLine(string text)
        {
            if (!(string.IsNullOrWhiteSpace(text)))
            {
                if (columnDeleteStore?.GetItem(text) != null)
                { columnDeleteStore?.Remove(text); }
                else
                { columnDeleteStore.Add(new ModelCommon() { Name = text, Alias = text }); }
            }
        }

        private DataTable GetTable(string query)
        {
            return dBOperations.GetTable(query);
        }


        private void DBOperations_EvntInfoMessage(object sender, TextEventArgs e)
        {
            AddLineAtTextBoxResultShow(e.Message);
        }


        private void MakeTextBoxResultShow()
        {
            if (txtbResultShow != null)
            {
                panel1.Controls.Remove(txtbResultShow);
                txtbResultShow?.Dispose();
            }
            txtbResultShow = new TextBox
            {
                Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right))),
                Location = new Point(0, 1),
                Multiline = true,
                Name = "txtbResultShow",
                ScrollBars = ScrollBars.Both,
                Size = new Size(722, 570)
            };

            panel1.Controls.Add(txtbResultShow);
            txtbResultShow.Dock = DockStyle.Fill;
            txtbResultShow.BringToFront();
        }


        private void HideTextBoxResultShow()
        {
            listBoxSample.BringToFront();
        }

        private void AddLineAtTextBoxResultShow(string text = null)
        {
            if (txtbResultShow != null && text != null)
                txtbResultShow.AppendLine(text);
        }

        private void SetAdminStatusLabelText(string text)
        {
            AdminStatusLabel.Text = text ?? "";
            AddLineAtTextBoxResultShow(text ?? "");
        }

        private void AddLineAtTextBoxResultShow(object sender, TextEventArgs e)
        {
            AddLineAtTextBoxResultShow(e.Message);
        }

        private void ShowMessage(string text = null)
        {
            if (!(string.IsNullOrWhiteSpace(text)))
            { MessageBox.Show(text); }
        }
    }
}