using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnalyse
{
    public static class GetModel<T> where T : IRow
    {

        public static T ToModel(string source, string columns)
        {
            string className = typeof(T).ToString().Replace(typeof(T).Namespace.ToString() + ".", ""); //Get name class without namespace
            T row;
            switch (className)
            {
                case "Car":
                    ParserRowToCar model = new ParserRowToCar(source, columns);
                    model.SetOrderColumns(); //first row
                    row = (T)model.ConvertRowToModel();
                    break;
                case "Owner":
                    ParserRowToOwner model1 = new ParserRowToOwner(source, columns);
                    model1.SetOrderColumns(); //first row
                    row = (T)model1.ConvertRowToModel();
                    break;
                case "CarAndOwner":
                default:
                    ParserRowToCarAndOwner model2 = new ParserRowToCarAndOwner(source, columns);
                    model2.SetOrderColumns(); //first row
                    row = (T)model2.ConvertRowToModel();
                    break;
            }

            return (T)row;
        }
    }

}
