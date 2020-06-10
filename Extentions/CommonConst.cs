using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public static class CommonConst
    {
        public static string TimeStamp { get { return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"); } }
        public static string DayStamp { get { return DateTime.Now.ToString("yyyy-MM-dd"); } }
        public static DateTime DateTimeStamp { get { return DateTime.Now; } }

        public readonly static System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        public readonly static System.Diagnostics.FileVersionInfo appFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
        public readonly static string AppVersion = assembly.GetName().Version.ToString();
        public readonly static string LocalAppFolder= Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public readonly static string LocalBackupFolder = Path.Combine(LocalAppFolder, "bak");
        public readonly static string LocalLogFolder = Path.Combine(LocalAppFolder, "logs");
        public readonly static string LocalTempFolder = Path.Combine(LocalAppFolder, "Temp");
        public readonly static string LocalUpdateFolder = Path.Combine(LocalAppFolder, "Update");
        public readonly static string AppName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        public readonly static string AppCfgFilePath = Path.Combine(LocalAppFolder, $"{AppName}.cfg");
        public  static string AppLogFilePath = Path.Combine(LocalLogFolder, $"{DayStamp}.log");
        public readonly static string AppFileUpdateXml = $"{AppName}.xml";
        public readonly static string AppFileUpdateUrl = $"{AppName}.url";
        public static string AppFileUpdateZip = $"{AppName} {AppVersion}.zip";

        public static string PathToXml = Path.Combine(LocalUpdateFolder, AppFileUpdateXml);
        public static string PathToUpdateZip = Path.Combine(LocalUpdateFolder, AppFileUpdateZip);
        public static string PathToUrl = Path.Combine(LocalAppFolder, AppFileUpdateUrl);

        //   static readonly System.Diagnostics.FileVersionInfo appFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);


        public const string regSubKeyMenu = "Menu";
        public const string regSubKeyRecent = "Recent";

        //Configuration
        //const of default connection
        public const string MAIN = "Main";
        public const string RECENT = "Recent";
        public const string DEFAULT_CONNECTION = "Default";

        public const string QUERY_COMMON = "SELECT * FROM MainData";
        public const string TABLE_COMMON = "MainData";
        public const string QUERY_ALIAS = "SELECT * FROM ColumnNameAndAlias";
        public const string TABLE_ALIAS = "ColumnNameAndAlias";

        public readonly static IList<string> UNIT_DEFAULT_LIST = new List<string>() { MAIN, RECENT, DEFAULT_CONNECTION };


        /// <summary>
        /// Переводчик англ-русс. Key - английское слово, values - перевод
        /// </summary>
        public static readonly IDictionary<string, string> TRANSLATION = new Dictionary<string, string>
            {
                { "District", "Район" },
                { "City", "Область" },
                { "Factory", "Марка" },
                { "ManufactureYear", "Год" }
            };

        public const string ColumnPlateDistrict = "ColumnPlateDistrict";

        /// <summary>
        /// Расшифровка регионов первых букв из автомобильных номеров Украины
        /// </summary>
        public static IDictionary<string, string> plateDistrict =
            new Dictionary<string, string>
            {
                ["AK"] = "АР Крым",
                ["KK"] = "АР Крым",
                ["AB"] = "Винницкая область",
                ["KB"] = "Винницкая область",
                ["AC"] = "Волынская область",
                ["KC"] = "Волынская область",
                ["AE"] = "Днепропетровская область",
                ["KE"] = "Днепропетровская область",
                ["AH"] = "Донецкая область",
                ["KH"] = "Донецкая область",
                ["AM"] = "Житомирская область",
                ["KM"] = "Житомирская область",
                ["AO"] = "Закарпатская область",
                ["KO"] = "Закарпатская область",
                ["AP"] = "Запорожская область",
                ["KP"] = "Запорожская область",
                ["AT"] = "Ивано-Франковская область",
                ["KT"] = "Ивано-Франковская область",
                ["AI"] = "Киевская область",
                ["KI"] = "Киевская область",
                ["AA"] = "г. Киев",
                ["KA"] = "г. Киев",
                ["BA"] = "Кировоградская область",
                ["HA"] = "Кировоградская область",
                ["BB"] = "Луганская область",
                ["HB"] = "Луганская область",
                ["BC"] = "Львовская область",
                ["HC"] = "Львовская область",
                ["BE"] = "Николаевская область",
                ["HE"] = "Николаевская область",
                ["BH"] = "Одесская область",
                ["HH"] = "Одесская область",
                ["BI"] = "Полтавская область",
                ["HI"] = "Полтавская область",
                ["BK"] = "Ровенская область",
                ["HK"] = "Ровенская область",
                ["BM"] = "Сумская область",
                ["HM"] = "Сумская область",
                ["BO"] = "Тернопольская область",
                ["HO"] = "Тернопольская область",
                ["AX"] = "Харьковская область",
                ["KX"] = "Харьковская область",
                ["BT"] = "Херсонская область",
                ["HT"] = "Херсонская область",
                ["BX"] = "Хмельницкая область",
                ["HX"] = "Хмельницкая область",
                ["CA"] = "Черкасская область",
                ["IA"] = "Черкасская область",
                ["CB"] = "Черниговская область",
                ["IB"] = "Черниговская область",
                ["CE"] = "Черновицкая область",
                ["IE"] = "Черновицкая область",
                ["CH"] = "г. Севастополь",
                ["IH"] = "г. Севастополь",
                ["II"] = "Общегосударственный",

                ["АК"] = "АР Крым",
                ["КК"] = "АР Крым",
                ["АВ"] = "Винницкая область",
                ["КВ"] = "Винницкая область",
                ["АС"] = "Волынская область",
                ["КС"] = "Волынская область",
                ["АЕ"] = "Днепропетровская область",
                ["КЕ"] = "Днепропетровская область",
                ["АН"] = "Донецкая область",
                ["КН"] = "Донецкая область",
                ["АМ"] = "Житомирская область",
                ["КМ"] = "Житомирская область",
                ["АО"] = "Закарпатская область",
                ["КО"] = "Закарпатская область",
                ["АР"] = "Запорожская область",
                ["КР"] = "Запорожская область",
                ["АТ"] = "Ивано-Франковская область",
                ["КТ"] = "Ивано-Франковская область",
                ["АІ"] = "Киевская область",
                ["КІ"] = "Киевская область",
                ["АА"] = "г. Киев",
                ["КА"] = "г. Киев",
                ["ВА"] = "Кировоградская область",
                ["НА"] = "Кировоградская область",
                ["ВВ"] = "Луганская область",
                ["НВ"] = "Луганская область",
                ["ВС"] = "Львовская область",
                ["НС"] = "Львовская область",
                ["ВЕ"] = "Николаевская область",
                ["НЕ"] = "Николаевская область",
                ["ВН"] = "Одесская область",
                ["НН"] = "Одесская область",
                ["ВІ"] = "Полтавская область",
                ["НІ"] = "Полтавская область",
                ["ВК"] = "Ровенская область",
                ["НК"] = "Ровенская область",
                ["ВМ"] = "Сумская область",
                ["НМ"] = "Сумская область",
                ["ВО"] = "Тернопольская область",
                ["НО"] = "Тернопольская область",
                ["АХ"] = "Харьковская область",
                ["КХ"] = "Харьковская область",
                ["ВТ"] = "Херсонская область",
                ["НТ"] = "Херсонская область",
                ["ВХ"] = "Хмельницкая область",
                ["НХ"] = "Хмельницкая область",
                ["СА"] = "Черкасская область",
                ["ІА"] = "Черкасская область",
                ["СВ"] = "Черниговская область",
                ["ІВ"] = "Черниговская область",
                ["СЕ"] = "Черновицкая область",
                ["ІЕ"] = "Черновицкая область",
                ["СН"] = "г. Севастополь",
                ["ІН"] = "г. Севастополь",
                ["ІІ"] = "Общегосударственный"
            };

        /// <summary>
        /// Example only
        /// </summary>
        public static readonly IDictionary<string, string> newDictionary = new Dictionary<string, string>
        {
            ["Column"] = "Marker A",
            ["Column0"] = "Marker B",
            ["Column1"] = "Marker C",
            ["Column10"] = "Marker D",
            ["Column2"] = "Marker E",
            ["Column5"] = "Marker F",
            ["ColumnPlates"] = "Marker G"
        };
    }
}