namespace AutoAnalysis
{
    public class MenuItem
    {
        static int number;

        public MenuItem(string text, string tag)
        {
            number += 1;
            Name = $"Menu{number}";
            Text = text;
            Tag = tag;
        }

        public string Name { get; }
        public string Text { get; }
        public string Tag { get; }
    }
}
