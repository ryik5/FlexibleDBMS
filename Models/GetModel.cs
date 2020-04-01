namespace AutoAnalyse
{
    public static class GetModel<T> where T : IModel
    {
        public static T ToModel(string source, string columns)
        {
            string className = typeof(T).ToString().Replace(typeof(T).Namespace.ToString() + ".", ""); //Get name class without namespace
            IParserRowTo model;

            switch (className)
            {
                case "Car":
                    model = new ParserRowToCar(source, columns);
                    break;
                case "Owner":
                    model = new ParserRowToOwner(source, columns);
                    break;
                case "CarAndOwner":
                default:
                    model = new ParserRowToCarAndOwner(source, columns);
                    break;
            }

            return (T)model.ConvertRowToModel();
        }
    }
}
