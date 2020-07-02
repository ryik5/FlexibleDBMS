using System;
using System.Collections.Generic;

namespace FlexibleDBMS
{
    public class MenuItem : IComparable<MenuItem>
    {
        static int number;

        public MenuItem(string text, string tag)
        {
            number += 1;
            Name = $"Menu{number}";
            Text = text;
            Tag = tag;
        }

        public MenuItem(string text)
        {
            number += 1;
            Name = $"Menu{number}";
            Text = text;
            Tag = text;
        }

        public string Name { get; }
        public string Text { get; }
        public string Tag { get; }

        public override string ToString()
        { return $"{Text} - {Tag}"; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MenuItem df))
                return false;

            return ToString().Equals(df.ToString());
        }

        public override int GetHashCode()
        { return ToString().GetHashCode(); }
        //реализация для выполнения сортировки
      
        int IComparable<MenuItem>.CompareTo(MenuItem next)
        { return new MenuItemComparer().Compare(this, next); }

        public string CompareTo(MenuItem next)
        { return next.CompareTo(this); }
    }

    public class MenuItemComparer : IComparer<MenuItem>
    {
        public int Compare(MenuItem x, MenuItem y)
        { return CompareTwoConfig(x, y); }

        public int CompareTwoConfig(MenuItem x, MenuItem y)
        {
            string a = x.Name;
            string b = y.Name;

            return CompareTwoStrings.Compare(a, b);
        }
    }
}
