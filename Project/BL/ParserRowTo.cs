namespace FlexibleDBMS
{
    public class ParserRowModelCommon 
    {
        private readonly string rowSource;
        private readonly string nameColumnsInSource;
        public string ImportedColumnName;

        public ParserRowModelCommon(string rowSource, string nameColumnsInSource)
        {
            this.rowSource = rowSource;
            this.nameColumnsInSource = nameColumnsInSource;
        }
        public ParserRowModelCommon(string nameColumnsInSource)
        {
            this.nameColumnsInSource = nameColumnsInSource;
        }


        public IModels ConvertRowToModels()
        {
            IModels models = new ModelCommonStore();
            string[] parsedColumns = rowSource?.Split('|');
            string [] columns=nameColumnsInSource?.Split('|');
            if (parsedColumns?.Length > 0 && parsedColumns?.Length == columns?.Length)
            {
                if (parsedColumns?.Length > 2)
                {
                    for (int i = 0; i < (parsedColumns?.Length - 1); i++)
                    {
                        models.list.Add(i, new ModelCommon { ID = i, Name = columns[i], Alias = parsedColumns[i] });
                    }
                }
            }

            return models;
        }

        public IModels MatchColumnToAlias()
        {
            IModels models = new ModelCommonStore();
            string[] aliasColumns = nameColumnsInSource.Split('|');
            string strColumns = string.Empty;

            for (int i = 0; i < aliasColumns.Length; i++)
            {
                models.list.Add(i, new ModelCommon { Name = $"Column{i}", Alias = aliasColumns[i] });
                strColumns += $"Column{i}|";
            }
            ImportedColumnName = strColumns.TrimEnd('|');

            return models;
        }
    }
}