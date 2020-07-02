using System;

namespace FlexibleDBMS
{
    [Serializable]
    public class SQLConnectionStore
    {
        ISQLConnectionSettings currentConnection;
        ISQLConnectionSettings oldConnection;
        public delegate void ConfigChanged<BoolEventArgs>(object sender, BoolEventArgs args);
        public event ConfigChanged<BoolEventArgs> EvntConfigChanged;

        public SQLConnectionStore() { }

        public SQLConnectionStore(ISQLConnectionSettings settings) { Set(settings); }

        public ISQLConnectionSettings GetCurrent() { return currentConnection; }

        public ISQLConnectionSettings GetPrevious() { return oldConnection; }

        public void Set(ISQLConnectionSettings newConnection)
        {
            if (newConnection == null)
            { return; }

            if (currentConnection != null)
            { oldConnection = new SQLConnectionSettings(currentConnection); }

            currentConnection = new SQLConnectionSettings(newConnection);

            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }

        public void Refresh()
        {
            EvntConfigChanged?.Invoke(this, new BoolEventArgs(true));
        }
    }
}