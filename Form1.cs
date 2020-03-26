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
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ImportData();
        }

        public void ImportData()
        {
            FileReader<Car> reader = new FileReader<Car>();
            IDictionary<string, Car> dic = reader.GetContent("1Model.txt");
            textBox1.AppendText(dic.Count.ToString());
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

        public int EDRPOU { get; set; }
        public int DRFO { get; set; }
        public string F { get; set; }
        public string I { get; set; }
        public string O { get; set; }
        public string Birthday { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string BuildingBody { get; set; }
        public string Apartment { get; set; }
        public string CodeOperation { get; set; }
        public string CodeDate { get; set; }
        
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
                    MessageBox.Show($"GetContent {typeof(T)}");

            IDictionary<string, T> dic = new Dictionary<string, T>();
            T model;
            bool exist = false;

            var lines = File.ReadLines(filePath, encoding);

            foreach (var line in lines)
            {
                if (line?.Length > 10)
                {
                    model = GetModel<T>.ToModel(line);

                    if (model != null && model.Name.Length > 0)
                    {
                        if (dic.Count > 0)
                        { exist = dic.ContainsKey(model.Name); }

                        if (!exist)
                        {
                            dic.Add(model.Name, model);
                        }
                    }
                }
            }
            MessageBox.Show("FileReader\n" + lines.Count()+"\n" + dic.Count.ToString());
            return dic;
        }

        public IDictionary<string, T> GetContent(string filePath)
        {
            return GetContent(filePath, _encoding);
        }
    }

    public static class GetModel<T> where T : IRow
    {

        public static T ToModel(string source)
        {
            string className = typeof(T).ToString().Replace(typeof(T).Namespace.ToString()+".",""); //Get name class without namespace
            T row;
            switch (className)
            {
                case "Car":
                    ParserRowToCar model = new ParserRowToCar(source);
                    model.SetOrderColumns("|Системный номер|Державний номер|Марка|Модель|Рік випуска|№ кузова|№ шасі|Об'єм двигуна|Код ДРФО власника|Прізвище власника|Ім'я власника|По-батькові власника|Дата народження власника|Код ЄДРПОУ власника|Назва власника|Область/місто|Н.П./район|Назва вулиці|Номер будинку|Корпус|Номер приміщення|Код операції|Дата операції|"); //first row
                    row = (T)model.ConvertRowToModel();
                    break;
                case "Owner":
                default:
                    ParserRowToOwner model1 = new ParserRowToOwner(source);
                    model1.SetOrderColumns("|Системный номер|Державний номер|Марка|Модель|Рік випуска|№ кузова|№ шасі|Об'єм двигуна|Код ДРФО власника|Прізвище власника|Ім'я власника|По-батькові власника|Дата народження власника|Код ЄДРПОУ власника|Назва власника|Область/місто|Н.П./район|Назва вулиці|Номер будинку|Корпус|Номер приміщення|Код операції|Дата операції|"); //first row
                    row = (T)model1.ConvertRowToModel();
                    break;
            }

            return (T)row;
        }
    }

    public class ParserRowToOwner
    {
        private IRow model;
        private string rowSource;
       
        public int[] numberColumns = new int[1];

        public ParserRowToOwner(string rowSource)
        { this.rowSource = rowSource; }

        public IRow ConvertRowToModel()
        {
            string[] parsedColumns = rowSource.Split('|');
            model = new Owner
            {
                Name = parsedColumns[numberColumns[0]],
                Type = parsedColumns[numberColumns[3]]?.Length > 0 ? TypeOwner.Enterprise : TypeOwner.Person,

                Address = $"{parsedColumns[numberColumns[9]]}/t{parsedColumns[numberColumns[10]]}/t" +
                $"{parsedColumns[numberColumns[11]]}/t{parsedColumns[numberColumns[12]]}/t" +
                $"{parsedColumns[numberColumns[13]]}/t{parsedColumns[numberColumns[14]]}",

                EDRPOU = int.TryParse(parsedColumns[numberColumns[3]], out int edrpou) ? edrpou : 0,
                DRFO = int.TryParse(parsedColumns[numberColumns[4]], out int drfo) ? drfo : 0,
                F = parsedColumns[numberColumns[5]],
                I = parsedColumns[numberColumns[6]],
                O = parsedColumns[numberColumns[7]],
                Birthday = parsedColumns[numberColumns[8]],
                City = parsedColumns[numberColumns[9]],
                District = parsedColumns[numberColumns[10]],
                Street = parsedColumns[numberColumns[11]],
                Building = parsedColumns[numberColumns[12]],
                BuildingBody = parsedColumns[numberColumns[13]],
                Apartment = parsedColumns[numberColumns[14]],
                CodeOperation = parsedColumns[numberColumns[15]],
                CodeDate = parsedColumns[numberColumns[16]]
            };
            if (((Owner)model).DRFO > 0)
            { ((Owner)model).Name = $"{((Owner)model).F}\t{((Owner)model).I}\t{((Owner)model).O}"; }

            return model;
        }

        public void SetOrderColumns(string nameColumnsInSource)
        {
            string[] parsedColumns = nameColumnsInSource.Split('|');
            numberColumns = new int[17];

            for (int i = 0; i < parsedColumns.Length; i++)
            {
                string s = parsedColumns[i]?.Trim();
                switch (s)
                {
                    case "Назва власника":
                        numberColumns[0] = i;
                        break;
                    case "Код ЄДРПОУ власника":
                        numberColumns[3] = i;
                        break;
                    case "Код ДРФО власника":
                        numberColumns[4] = i;
                        break;
                    case "Прізвище власника":
                        numberColumns[5] = i;
                        break;
                    case "Ім'я власника":
                        numberColumns[6] = i;
                        break;
                    case "По-батькові власника":
                        numberColumns[7] = i;
                        break;
                    case "Дата народження власника":
                        numberColumns[8] = i;
                        break;
                    case "Область/місто":
                        numberColumns[9] = i;
                        break;
                    case "Н.П./район":
                        numberColumns[10] = i;
                        break;
                    case "Назва вулиці":
                        numberColumns[11] = i;
                        break;
                    case "Номер будинку":
                        numberColumns[12] = i;
                        break;
                    case "Корпус":
                        numberColumns[13] = i;
                        break;
                    case "Номер приміщення":
                        numberColumns[14] = i;
                        break;
                    case "Код операції":
                        numberColumns[15] = i;
                        break;
                    case "Дата операції":
                        numberColumns[16] = i;
                        break;
                }
            }
        }

    }

    public class ParserRowToCar
    {
        private IRow model;
        private string rowSource;
        public int[] numberColumns = new int[1];

        public ParserRowToCar(string rowSource)
        { this.rowSource = rowSource; }

        public IRow ConvertRowToModel()
        {
            string[] parsedColumns = rowSource.Split('|');
            model = new Car
            {
                Plate = parsedColumns[numberColumns[0]],
                Factory = parsedColumns[numberColumns[1]],
                Model = parsedColumns[numberColumns[2]],
                ManufactureYear = parsedColumns[numberColumns[3]],
                BodyNumber = parsedColumns[numberColumns[4]],
                ChassisNumber = parsedColumns[numberColumns[5]],
                EngineVolume = parsedColumns[numberColumns[6]]
            };
            model.Name = model.ToString();

            return model;
        }

        public void SetOrderColumns(string nameColumnsInSource)
        {
            string[] parsedColumns = nameColumnsInSource.Split('|');
            numberColumns = new int[7];

            for (int i = 0; i < parsedColumns.Length; i++)
            {
                string s = parsedColumns[i]?.Trim();
                switch (s)
                {
                    case "Державний номер":
                        numberColumns[0] = i;
                        break;
                    case "Марка":
                        numberColumns[1] = i;
                        break;
                    case "Модель":
                        numberColumns[2] = i;
                        break;
                    case "Рік випуска":
                        numberColumns[3] = i;
                        break;
                    case "№ кузова":
                        numberColumns[4] = i;
                        break;
                    case "№ шасі":
                        numberColumns[5] = i;
                        break;
                    case "Об'єм двигуна":
                        numberColumns[6] = i;
                        break;
                }
            }
        }

    }
}
