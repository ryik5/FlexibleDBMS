using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace AutoAnalysis
{
    public class SQLiteDBOperations
    {
        public delegate void Message(object sender, TextEventArgs e);
        public event Message Status;

        string sqLiteConnectionString;
        System.IO.FileInfo dbFileInfo;

        public SQLiteDBOperations(string sqLiteConnectionString, System.IO.FileInfo dbFileInfo)
        {
            this.sqLiteConnectionString = sqLiteConnectionString;
            this.dbFileInfo = dbFileInfo;
        }

        public DataTable GetTable(string query)
        {
            DataTable dt = new DataTable();

            using (SqLiteDbWrapper readData = new SqLiteDbWrapper(sqLiteConnectionString, dbFileInfo))
            {
                dt = readData.GetTable(query);
            }

            return dt;
        }

        /// <summary>
        /// get only simple query like 'SELECT DISTINCT name_column FROM name_table'
        /// </summary>
        /// <param name="queryWithOneColumnOnly"></param>
        /// <returns></returns>
        public IList<string> GetList(string queryWithOneColumnOnly)
        {
            IList<string> result = null;
            string table=string.Empty, column=string.Empty;
            string[] word = queryWithOneColumnOnly.Split(' ');
            
            if (word?.Length < 4|| word?.Length >5)
            {
                Status?.Invoke(this, new TextEventArgs($"Запрос построен с ошибками: {queryWithOneColumnOnly}. Правильный формат 'SELECT DISTINCT name_column FROM name_table'"));
                return result;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i].ToLower().Equals("from"))
                {
                    table = word[i + 1];
                    column = word[i - 1];
                }
            }

            using (SqLiteDbWrapper readData = new SqLiteDbWrapper(sqLiteConnectionString, dbFileInfo))
            {
                result = readData.GetList(table, column);
            }

            return result;
        }


        public void TryMakeLocalDB()
        {
            string strQueryCreateDb = "CREATE TABLE IF NOT EXISTS 'CarAndOwner' ('Id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Plate TEXT, " +
                "Factory TEXT, Model TEXT, ManufactureYear TEXT, BodyNumber TEXT, ChassisNumber TEXT, EngineVolume TEXT, " +
                "Type TEXT, DRFO INTEGER, F TEXT, I TEXT, O TEXT, Birthday TEXT, " +
                "EDRPOU INTEGER, Name TEXT, City TEXT, District TEXT, Street TEXT, Building TEXT, BuildingBody TEXT, Apartment TEXT, " +
                "CodeOperation TEXT, CodeDate TEXT);";
            if (!System.IO.File.Exists(dbFileInfo.Name))
            {
                SQLiteConnection.CreateFile(dbFileInfo.Name);
            }

            using (SqLiteDbWrapper dbWriter = new SqLiteDbWrapper(sqLiteConnectionString, dbFileInfo))
            {
                dbWriter.Execute("begin");

                dbWriter.Execute(strQueryCreateDb);

                dbWriter.Execute("end");

                Status?.Invoke(this, new TextEventArgs("Таблицы в БД созданы"));
            }
        }

        public void WriteListInDB(IList<CarAndOwner> list)
        {
            string query =
                "INSERT OR REPLACE INTO 'CarAndOwner' (Plate, Factory, Model, ManufactureYear, BodyNumber, ChassisNumber, EngineVolume, Type, DRFO, F, I, O, Birthday, EDRPOU, " +
                "Name, City, District, Street, Building, BuildingBody, Apartment, CodeOperation, CodeDate) " +
                "VALUES (@Plate, @Factory, @Model, @ManufactureYear, @BodyNumber, @ChassisNumber, @EngineVolume, @Type, @DRFO, @F, @I, @O, @Birthday, @EDRPOU, " +
                "@Name, @City, @District, @Street, @Building, @BuildingBody, @Apartment, @CodeOperation, @CodeDate)";

            using (SqLiteDbWrapper dbWriter = new SqLiteDbWrapper(sqLiteConnectionString, dbFileInfo))
            {
                Status?.Invoke(this, new TextEventArgs($"Запись список в {list.Count} записей в базу список"));

                dbWriter.Execute("begin");
                foreach (var row in list)
                {
                    using (SQLiteCommand SqlQuery = new SQLiteCommand(query, dbWriter.sqlConnection))
                    {
                        SqlQuery.Parameters.Add("@Plate", DbType.String).Value = row?.Plate;
                        SqlQuery.Parameters.Add("@Factory", DbType.String).Value = row?.Factory;
                        SqlQuery.Parameters.Add("@Model", DbType.String).Value = row?.Model;
                        SqlQuery.Parameters.Add("@ManufactureYear", DbType.String).Value = row?.ManufactureYear;
                        SqlQuery.Parameters.Add("@BodyNumber", DbType.String).Value = row?.BodyNumber;
                        SqlQuery.Parameters.Add("@ChassisNumber", DbType.String).Value = row?.ChassisNumber;
                        SqlQuery.Parameters.Add("@EngineVolume", DbType.String).Value = row?.EngineVolume;
                        SqlQuery.Parameters.Add("@Type", DbType.String).Value = row?.Type;
                        SqlQuery.Parameters.Add("@DRFO", DbType.Int32).Value = row?.DRFO;
                        SqlQuery.Parameters.Add("@F", DbType.String).Value = row?.F;
                        SqlQuery.Parameters.Add("@I", DbType.String).Value = row?.I;
                        SqlQuery.Parameters.Add("@O", DbType.String).Value = row?.O;
                        SqlQuery.Parameters.Add("@Birthday", DbType.String).Value = row?.Birthday;
                        SqlQuery.Parameters.Add("@EDRPOU", DbType.Int32).Value = row?.EDRPOU;
                        SqlQuery.Parameters.Add("@Name", DbType.String).Value = row?.Name;
                        SqlQuery.Parameters.Add("@City", DbType.String).Value = row?.City;
                        SqlQuery.Parameters.Add("@District", DbType.String).Value = row?.District;
                        SqlQuery.Parameters.Add("@Street", DbType.String).Value = row?.Street;
                        SqlQuery.Parameters.Add("@Building", DbType.String).Value = row?.Building;
                        SqlQuery.Parameters.Add("@BuildingBody", DbType.String).Value = row?.BuildingBody;
                        SqlQuery.Parameters.Add("@Apartment", DbType.String).Value = row?.Apartment;
                        SqlQuery.Parameters.Add("@CodeOperation", DbType.String).Value = row?.CodeOperation;
                        SqlQuery.Parameters.Add("@CodeDate", DbType.String).Value = row?.CodeDate;

                        dbWriter.ExecuteBulk(SqlQuery);
                    }
                }

                dbWriter.Execute("end");

                Status?.Invoke(this, new TextEventArgs("Запись списка завершена"));
            }
        }
    }
}
