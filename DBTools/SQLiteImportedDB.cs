using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FlexibleDBMS
{
    internal static class SQLiteImportedDB
    {
        public static bool Check(string filePath)
        {
            bool isImported = false;
            DbSchema schemaDB = null;

            try { schemaDB = DbSchema.LoadDB(filePath); }
            catch { return false; }

            bool isHasImportedTables =
                schemaDB.Tables.Values.Any(x => x.TableName.Equals("MainData")) &&
                schemaDB.Tables.Values.Any(x => x.TableName.Equals("ColumnNameAndAlias"));

            if (!isHasImportedTables)
                return false;

            IList<string> nameColumns = new List<string>();
            string query = "SELECT ColumnName FROM 'ColumnNameAndAlias';";
            string connString = $"Data Source = {filePath}; Version=3;";
            try
            {
                using SqLiteDbWrapper readData = new SqLiteDbWrapper(connString);
                using (DataTable dt = readData?.GetQueryResultAsTable(query))
                {
                    if (dt?.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt?.Rows)
                        {
                            nameColumns.Add(r[0]?.ToString());
                        }
                    }
                }
            }
            catch { return false; }

            if (nameColumns?.Count > 0)
            {
                var colunmsInMainData = schemaDB.Tables.Values.FirstOrDefault(x => x.TableName.Equals("MainData")).Columns;

                foreach (var column in colunmsInMainData)
                {
                    nameColumns.Remove(column?.ColumnName);
                }
            }

            if (nameColumns?.Count == 0 && isHasImportedTables)
            {
                isImported = true;
            }

            return isImported;
        }
    }
}
