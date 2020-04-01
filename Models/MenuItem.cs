namespace AutoAnalyse
{
    public class MenuItem
    {
        static int number;

        public MenuItem(string nameQuery, string bodyQuery)
        {
            number += 1;
            Name = $"menu{number}";
            NameQuery = nameQuery;
            BodyQuery = bodyQuery;
        }

        public string Name { get; }
        public string NameQuery { get; }
        public string BodyQuery { get; }
    }
}
