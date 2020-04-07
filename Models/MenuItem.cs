namespace AutoAnalysis
{
    public class MenuItem
    {
        static int number;

        public MenuItem(string nameQuery, string bodyQuery)
        {
            number += 1;
            Name = $"Menu{number}";
            Text = nameQuery;
            Tag = bodyQuery;
        }

        public string Name { get; }
        public string Text { get; }
        public string Tag { get; }
    }
}
