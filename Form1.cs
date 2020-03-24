using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoAnalyse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void ImportData()
        {

        }
    }

    public class ImportCarData
    {
        public ImportCarData(IDataImportable data)
        {
            IList<IRow> list = data.Import();
        }
    }

    public interface IDataImportable
    {
        IList<IRow> Import();
    }

    public interface IStoredData
    {
        IList<IRow> Get { get; set; }
    }

    public interface IRow
    {
        int ID { get; set; }
        string Name { get; set; }
    }

    public sealed class Car : IRow
    {
        private string _name;
        public int ID { get; set; }

        public string Plate { get; set; }
        public string Factory { get; set; }
        public string Model { get; set; }
        public string ManufactureYear { get; set; }
        public string BodyNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineVolume { get; set; }
        public string Name { get { return ToString(); } set { _name = ToString(); } }

        public override string ToString()
        { return $"{Plate}\t{Factory}\t{Model}\t{ManufactureYear}\t{BodyNumber}\t{ChassisNumber}\t{EngineVolume}"; }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Car p = (Car)obj;
                return (ToString().Equals(p.ToString()));
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public class Owner : IRow
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public TypeOwner Type { get; set; }
        public string Address { get; set; }
        //  { get { return $"{District}/t{City}/t{Street}/t{Building}/t{BuildingBody}/t{Apartment}"; } }

        public int EDRPOU { get; set; }
        public int DRFO { get; set; }
        public string F { get; set; }
        public string I { get; set; }
        public string O { get; set; }
        public DateTime Birthday { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string BuildingBody { get; set; }
        public string Apartment { get; set; }
        public string CodeOperation { get; set; }
        public DateTime CodeDate { get; set; }


        public override string ToString()
        {
            string result;
            switch (Type)
            {
                case TypeOwner.Enterprise:
                    result = $"{EDRPOU}\t{Name}\t{Address}";
                    break;
                case TypeOwner.Person:
                default:
                    result = $"{DRFO}\t{Name}\t{Address}";
                    break;
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Owner p = (Owner)obj;
                return (ToString().Equals(p.ToString()));
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public enum TypeOwner
    {
        Person = 0,
        Enterprise = 2
    }

    public class ImportTextFile : IDataImportable
    {
        public ImportTextFile(string path)
        {

        }

        public IList<IRow> Import()
        {
            throw new NotImplementedException();
        }
    }

    public class FileReader<T> where T : IRow
    {
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);

        public IDictionary<string, T> GetContent(string filePath, Encoding encoding)
        {
            IDictionary<string, T> dic = new Dictionary<string, T>();
            bool exist = false;

            var lines = File.ReadLines(filePath, encoding);
            foreach (var line in lines)
            {
                T model = GetModel<T>.ToModel(line);
                if (model != null && model.Name.Length > 0)
                {
                    if (dic.Count > 0)
                    { exist = dic.ContainsKey(model.Name); }

                    if (!exist)
                    { dic.Add(model.Name, model); }
                }
            }

            return dic;
        }

        public IDictionary<string, T> GetContent(string filePath)
        {
            return GetContent(filePath, _encoding);
        }
    }

    public static class GetModel<T> where T : IRow
    {

        public static T ToModel(string row)
        {
            string s = typeof(T).ToString();
            IRow car;
            switch (s)
            {
                case "Car":
                    car = new Car();
                    RowToCar model = new RowToCar(row);
                    car = model.Get();
                    return (T)car;

                default:
                    car = null;
                    return (T)car;
            }
        }
    }

    public class RowToCar
    {
        private Car car;
        private string _row;
        IList<string> _orderNameColumns;
        IList<int> _orderNumberColumns = new List<int>();
        public int[] numberColumns = new int[1];
        public string[] parsedColumns = new string[1];
        public string[] nameColumns = new string[1];


        public RowToCar(string rowData)
        { _row = rowData; }

        public void ConvertRowToCar()
        {
            parsedColumns = _row.Split('|');
            car = new Car();
            car.Plate = parsedColumns[numberColumns[0]];
            car.Factory = parsedColumns[numberColumns[1]];
            car.Model = parsedColumns[numberColumns[2]];
            car.ManufactureYear = parsedColumns[numberColumns[3]];
            car.BodyNumber = parsedColumns[numberColumns[4]];
            car.ChassisNumber = parsedColumns[numberColumns[5]];
            car.EngineVolume = parsedColumns[numberColumns[6]];
            car.Name = car.ToString();
        }

        public void SetOrderColumns(string nameColumnsInSource)
        {
            _orderNameColumns = new List<string>();
            _orderNameColumns.Add(nameof(car.Plate)); //Державний номер
            _orderNameColumns.Add(nameof(car.Factory));//Марка
            _orderNameColumns.Add(nameof(car.Model));//Модель
            _orderNameColumns.Add(nameof(car.ManufactureYear));//Рік випуска
            _orderNameColumns.Add(nameof(car.BodyNumber));//№ кузова
            _orderNameColumns.Add(nameof(car.ChassisNumber));//№ шасі 
            _orderNameColumns.Add(nameof(car.EngineVolume));//Об'єм двигуна

            parsedColumns = nameColumnsInSource.Split('|');
            nameColumns = new string[_orderNameColumns.ToArray().Length];
            numberColumns = new int[nameColumns.Length];

            for (int i = 0; i < parsedColumns.Length; i++)
            {
                string s = parsedColumns[i]?.Trim();
                switch (s)
                {
                    case "Державний номер":
                        nameColumns[0] = _orderNameColumns[0];
                        numberColumns[0] = i;
                        break;
                    case "Марка":
                        nameColumns[1] = _orderNameColumns[1];
                        numberColumns[1] = i;
                        break;
                    case "Модель":
                        nameColumns[2] = _orderNameColumns[2];
                        numberColumns[2] = i;
                        break;
                    case "Рік випуска":
                        nameColumns[3] = _orderNameColumns[3];
                        numberColumns[3] = i;
                        break;
                    case "№ кузова":
                        nameColumns[4] = _orderNameColumns[4];
                        numberColumns[4] = i;
                        break;
                    case "№ шасі":
                        nameColumns[5] = _orderNameColumns[5];
                        numberColumns[5] = i;
                        break;
                    case "Об'єм двигуна":
                        nameColumns[6] = _orderNameColumns[6];
                        numberColumns[6] = i;
                        break;
                }
            }
        }

        public Car Get()
        {
            return car;
        }
    }

}
