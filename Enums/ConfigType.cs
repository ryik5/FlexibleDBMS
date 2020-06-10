using System;

namespace FlexibleDBMS
{
    [Serializable]
    public enum ConfigType
    {
        None,
        Application,
        Registry,
        Connection,
        ToolStripMenuExtraQuery,
        ToolStripMenuStandartQuery,
        ToolStripMenuRecent
    }
}
