namespace FlexibleDBMS
{
    public class ItemFlipper
    {
        string constText;
        string tempText;
        public string GetTemporaryText { get => tempText; }
        public string GetConstText { get => constText; }

        public delegate void AddedItemInCollection<TextEventArgs>(object sender, TextEventArgs e);
        public event AddedItemInCollection<TextEventArgs> EvntSetText;

        public ItemFlipper() { }
        public ItemFlipper(string text) { SetConstText(text); }

        public void SetConstText(string text)
        {
            constText = text;
        }

        public void SetTempText(string text)
        {
            tempText = text;
            EvntSetText?.Invoke(this, new TextEventArgs(text));
        }
    }
}