using System;

namespace FlexibleDBMS
{
    public class ConfigStore
    {
        public delegate void CollectionFull(object sender, BoolEventArgs e);
        public event CollectionFull EvntChanged;

        public ConfigFull<ConfigAbstract> Get { get; private set; }

        public void Set(ConfigFull<ConfigAbstract> config)
        {
            if (Get == null)
            {
                Get = new ConfigFull<ConfigAbstract>();
            }
            Get.Set( new ConfigFull<ConfigAbstract>(config));
            EvntChanged?.Invoke(this, new BoolEventArgs(true));
        }
    }

    public class ConfigUnitStore
    {
        public string Version { get; set; }
        public DateTime TimeStamp { get; set; }
        public ISQLConnectionSettings SQLConnection { get; set; }
        public MenuAbstractStore QueryStandartMenuStore { get; set; }
        public MenuAbstractStore QueryExtraMenuStore { get; set; }
        public MenuAbstractStore RecentMenuStore { get; set; }
    }
}