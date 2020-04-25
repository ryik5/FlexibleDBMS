using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
 public   class ConfigBuilder
    {
        AbstractConfigList currentConfig;
        AbstractUnitConfigParameterList parametersList;
        AbstractConfigParameter parameters;
        IDictionary<string, AbstractConfigParameter> unit;
        
        public ConfigBuilder()
        {
            currentConfig = new ConfigList();
            parametersList = new ConfigUnitParameterList();
            parameters = new ConfigParameter();
        }

        public AbstractUnitConfigParameterList NewUnit()
        {
            parametersList = new ConfigUnitParameterList();
            return parametersList;
        }
        public AbstractUnitConfigParameterList AddIntoCurrentUnit(AbstractConfigParameter parameter)
        {
            parametersList.Add(parameter);
            return parametersList;
        }
        public AbstractUnitConfigParameterList AddIntoCurrentUnit(IDictionary<string, AbstractConfigParameter> parameters)
        {
            foreach (var parameter in parameters)
            { parametersList.Add(parameter.Value); }

            return parametersList;
        }


        //public AbstractConfigList MakeCurrentFullConfig()
        //{
        //    //Unit1
        //    //Main Config
        //     parametersList = new ConfigParameterList();

        //     parameters = new ConfigParameter();
        //    parameters.Name = DB_LIST;
        //    foreach (var menuItem in changeBaseToolStripMenuItem.ToDictionary())
        //    { parameters.Add(menuItem.Key, menuItem.Value); }
        //    parametersList.Add(parameters);//add the whole parameter in unit

        //    parameters = new ConfigParameter();
        //    parameters.Name = nameof(ISQLConnectionSettings);
        //    foreach (var item in currentSQLConnection.GetPropertyValues())
        //    { parameters.Add(item.Key, item.Value); }

        //    parametersList.Name = MAIN;
        //    parametersList.Add(parameters);//add the whole parameter in unit

        //    currentConfig.Name = "CurrentFullConfig";
        //    currentConfig.Name = "Current Full Configuration";
        //    currentConfig.Add(parametersList); //add the whole unit in Config

        //    //Unit2
        //    //connections
        //    parametersList = new ConfigParameterList();

        //    //queriesExtraMenu
        //    parameters = new ConfigParameter();
        //    parameters.Name = $"{nameof(queriesExtraMenu)}";
        //    foreach (var menuItem in queriesExtraMenu.ToDictionary())
        //    {
        //        parameters.Add(menuItem.Key, menuItem.Value);
        //    }
        //    parametersList.Name = currentSQLConnection.Name;
        //    parametersList.Add(parameters);//add the whole parameter in unit


        //    //queriesStandartMenu
        //    parameters = new ConfigParameter();
        //    foreach (var menuItem in queriesStandartMenu.ToDictionary())
        //    {
        //        parameters.Add(menuItem.Key, menuItem.Value);
        //    }
        //    parameters.Name = $"{nameof(queriesStandartMenu)}";
        //    parametersList.Add(parameters);//add the whole parameter in unit

        //    //newConfig
        //    currentConfig.Add(parametersList); //add the whole unit in Config

        //    //Unit3 .....

        //    return currentConfig;
        //}
    }
}
