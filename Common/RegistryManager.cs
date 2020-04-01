using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace AutoAnalyse
{

    public interface IRegistryWriteable
    {
        void Write(string key, string value);
        void Write(string key, string value, string subkey);
        void Write(IDictionary<string, string> dic);
        void Write(IDictionary<string, string> dic, string subkey);

    }

    public interface IRegistryReadable
    {
        RegistryEntity Read(string subkey);
        IList<RegistryEntity> ReadRegistryKeys();
        IList<RegistryEntity> ReadRegistryKeys(string subkey);
    }

    public class RegistryEntity
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public RegistryValueKind Type { get; set; }
    }

    public class RegistryManager : IRegistryWriteable, IRegistryReadable
    {
        private readonly string appRegistryKey;
        public RegistryManager(string appRegistryKey)
        { this.appRegistryKey = appRegistryKey; }

        public delegate void StatusInfo(object sender, TextEventArgs e);
        public event StatusInfo EvntStatusInfo;

        public RegistryEntity Read(string key)
        {
            RegistryEntity entity = new RegistryEntity();
            string errors = string.Empty;
            using (RegistryKey EvUserKey = Registry.CurrentUser.OpenSubKey(appRegistryKey, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey))
            {
                try
                {
                    entity.Key = key?.Trim();
                    entity.Value = EvUserKey?.GetValue(key);
                    entity.Type = EvUserKey.GetValueKind(key);
                }
                catch (Exception err) { errors += ($"Can't get value of '{key}' from Registry:\r\n{err.ToString()}"); }
            }

            if (string.IsNullOrEmpty(errors)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"'{key}' was read in Registry")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error reading '{key}' in Registry:\r\n{errors}")); }

            return entity;
        }

        public IList<RegistryEntity> ReadRegistryKeys()
        {
            IList<RegistryEntity> list = new List<RegistryEntity>();

            string errors = string.Empty;

            using (RegistryKey EvUserKey = Registry.CurrentUser.OpenSubKey(appRegistryKey, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey))
            {
                string[] subNames = EvUserKey.GetSubKeyNames();
                foreach (string name in subNames)
                {
                    string key = name?.Trim();
                    if (key.Length > 0)
                        try
                        {
                            RegistryEntity entity = new RegistryEntity
                            {
                                Key = key,
                                Value = EvUserKey?.GetValue(name),
                                Type = EvUserKey.GetValueKind(name)
                            };
                            list.Add(entity);
                        }
                        catch (Exception err) { errors += ($"Can't get value of '{name}' from Registry:\r\n{err.ToString()}"); }
                }
            }

            if (string.IsNullOrEmpty(errors)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"'{appRegistryKey}' was found to read keys in Registry")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error reading '{appRegistryKey}' from Registry:\r\n{errors}")); }

            return list;
        }

        public IList<RegistryEntity> ReadRegistryKeys(string subkey)
        {
            IList<RegistryEntity> list = new List<RegistryEntity>();

            string errors = string.Empty;

            using (RegistryKey EvUserKey = Registry.CurrentUser.OpenSubKey(appRegistryKey, false))
            {
                string[] names = EvUserKey.GetSubKeyNames();
                foreach (var n in names)
                {
                    if (n.Equals(subkey))
                    {
                        using (RegistryKey EvUserSubKey = EvUserKey.OpenSubKey(subkey, false))
                        {
                            string[] subNames = EvUserSubKey.GetValueNames();

                            foreach (string name in subNames)
                            {
                                string key = name?.Trim();
                                if (key.Length > 0)
                                {
                                    try
                                    {
                                        RegistryEntity entity = new RegistryEntity
                                        {
                                            Key = key,
                                            Value = EvUserSubKey.GetValue(name),
                                            Type = EvUserSubKey.GetValueKind(name)
                                        };
                                        list.Add(entity);
                                    }

                                    catch (Exception err) { errors += $"Can't get value of '{name}' from Registry:\r\n{err.ToString()}"; }
                                }
                            }
                        }
                        break;
                    }
                    else { errors += $"'{subkey}' was not found in Registry"; }
                }
            }

            if (string.IsNullOrEmpty(errors))
            { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Under Registry subkey '{appRegistryKey}\\{subkey}' was read {list.Count} elements")); }
            else
            { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error reading '{appRegistryKey}' in Registry:\r\n{errors}")); }

            return list;
        }


        /// <summary>
        /// Save data in Registry
        /// </summary>
        /// <param name="key">name</param>
        /// <param name="value">value</param>
        public void Write(string key, string value)
        {
            if (string.IsNullOrEmpty(key?.Trim()))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("key can not be null or empty!"));
                return;
            }

            string errMessage = string.Empty;
            try
            {
                using (RegistryKey EvUserKey = Registry.CurrentUser.CreateSubKey(appRegistryKey))
                {
                    try { EvUserKey.SetValue(key, value, RegistryValueKind.String); }
                    catch (Exception err) { errMessage += $"Error writting of value{key}: {err.ToString()} + \r\n"; }
                }
            }
            catch (Exception err) { errMessage += $"Forbiden access to write in Registry:{appRegistryKey}: {err.ToString()} + \r\n"; }

            if (string.IsNullOrEmpty(errMessage)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Data was written in Registry succesful")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error to write in Registry:\r\n{errMessage}")); }
        }

        /// <summary>
        /// Save data in Registry
        /// </summary>
        /// <param name="key">name</param>
        /// <param name="value">value</param>
        /// <param name="subkey">keys' strore folder</param>
        public void Write(string key, string value, string subkey)
        {
            if (string.IsNullOrEmpty(key?.Trim()))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("key can not be null or empty!"));
                return;
            }

            if (string.IsNullOrEmpty(appRegistryKey?.Trim()) || string.IsNullOrEmpty(subkey?.Trim()))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("appRegistryKey or subkey can not be null or empty!"));
                return;
            }

            string errMessage = string.Empty;
            try
            {
                using (RegistryKey EvUserKey = Registry.CurrentUser.CreateSubKey(appRegistryKey))
                {
                    try
                    {
                        using (RegistryKey EvUserSubKey = EvUserKey.CreateSubKey(subkey))
                        {

                            try { EvUserSubKey.SetValue(key, $"{value}", RegistryValueKind.String); }
                            catch (Exception err) { errMessage += $"Error writting of value{key}: {err.ToString()} + \r\n"; }
                        }
                    }
                    catch (Exception err) { errMessage += $"Forbiden access to write in Registry:{subkey}: {err.ToString()} + \r\n"; }
                }
            }
            catch (Exception err) { errMessage += $"Forbiden access to write in Registry:{appRegistryKey}: {err.ToString()} + \r\n"; }

            if (string.IsNullOrEmpty(errMessage)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Data was written in Registry succesful")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error to write in Registry:\r\n{errMessage}")); }
        }

        /// <summary>
        /// Save data in Registry
        /// </summary>
        /// <param name="dic">name,value</param>
        public void Write(IDictionary<string, string> dic)
        {
            if (dic == null || !(dic?.Count > 0))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("IDictionary<string, string> can not be null or empty!"));
                return;
            }

            if (string.IsNullOrEmpty(appRegistryKey?.Trim()))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("appRegistryKey can not be null or empty!"));
                return;
            }

            string errMessage = string.Empty;
            try
            {
                using (RegistryKey EvUserKey = Registry.CurrentUser.CreateSubKey(appRegistryKey))
                {
                    foreach (var parameter in dic)
                    {
                        try { EvUserKey.SetValue(parameter.Key, $"{parameter.Value}", RegistryValueKind.String); }
                        catch (Exception err) { errMessage += $"Error writting of value{parameter.Key}: {err.ToString()} + \r\n"; }
                    }
                }
            }
            catch (Exception err) { errMessage += $"Forbiden access to write in Registry:{appRegistryKey}: {err.ToString()} + \r\n"; }

            if (string.IsNullOrEmpty(errMessage)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Data was written in Registry succesful")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error to write in Registry:\r\n{errMessage}")); }
        }

        /// <summary>
        /// Save data in Registry
        /// </summary>
        /// <param name="dic">name,value</param>
        /// <param name="subkey">keys' strore folder</param>
        public void Write(IDictionary<string, string> dic, string subkey)
        {
            if (dic == null || !(dic?.Count > 0))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("IDictionary<string, string> can not be null or empty!"));
                return;
            }

            if (string.IsNullOrEmpty(appRegistryKey?.Trim()) || string.IsNullOrEmpty(subkey?.Trim()))
            {
                EvntStatusInfo?.Invoke(this, new TextEventArgs("appRegistryKey or subkey can not be null or empty!"));
                return;
            }

            string errMessage = string.Empty;
            try
            {
                using (RegistryKey EvUserKey = Registry.CurrentUser.CreateSubKey(appRegistryKey))
                {
                    try
                    {
                        using (RegistryKey EvUserSubKey = EvUserKey.CreateSubKey(subkey))
                        {
                            foreach (var parameter in dic)
                            {
                                try { EvUserSubKey.SetValue(parameter.Key, $"{parameter.Value}", RegistryValueKind.String); }
                                catch (Exception err) { errMessage += $"Error writting of value{parameter.Key}: {err.ToString()} + \r\n"; }
                            }
                        }
                    }
                    catch (Exception err) { errMessage += $"Forbiden access to write in Registry:{subkey}: {err.ToString()} + \r\n"; }
                }
            }
            catch (Exception err) { errMessage += $"Forbiden access to write in Registry:{appRegistryKey}: {err.ToString()} + \r\n"; }

            if (string.IsNullOrEmpty(errMessage)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Data was written in Registry succesful")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error to write in Registry:\r\n{errMessage}")); }
        }

        public void DeleteSubKeyTreeQueryExtraItems(string subkey)
        {
            string errMessage = string.Empty;
            try
            {
                using (RegistryKey EvUserKey = Registry.CurrentUser.OpenSubKey(appRegistryKey, true))
                {
                    try
                    {
                        EvUserKey.DeleteSubKeyTree(subkey);
                    }
                    catch (Exception err) { errMessage += $"Forbiden to delete in Registry:{subkey}: {err.ToString()} + \r\n"; }
                }
            }
            catch (Exception err) { errMessage += $"Forbiden to write in Registry:{appRegistryKey}: {err.ToString()} + \r\n"; }

            if (string.IsNullOrEmpty(errMessage)) { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Data was written in Registry succesful")); }
            else { EvntStatusInfo?.Invoke(this, new TextEventArgs($"Error to write in Registry:\r\n{errMessage}")); }
        }
    }
}
