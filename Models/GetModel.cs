namespace AutoAnalysis
{
    public static class GetModel<T> where T : IModel
    {
        public static T ToModel(string source, string columns)
        {
            string className = typeof(T).ToString().Replace(typeof(T).Namespace.ToString() + ".", ""); //Get name class without namespace
            IParserRowTo model;
            string input = source.Replace("\t", " ").Replace("  ", " ").Trim();
            switch (className)
            {
                case "Car":
                    model = new ParserRowToCar(input, columns);
                    break;
                case "Owner":
                    model = new ParserRowToOwner(input, columns);
                    break;
                case "CarAndOwner":
                default:
                    model = new ParserRowToCarAndOwner(input, columns);
                    break;
            }

            return (T)model.ConvertRowToModel();
        }
    }
}
