namespace AutoAnalysis
{
    public interface IParserRowTo
    {
        IModel ConvertRowToModel();
    }

    public class ParserRowToOwner: IParserRowTo
    {
        private string rowSource;
        private string nameColumnsInSource;

        public ParserRowToOwner(string rowSource, string nameColumnsInSource)
        {
            this.rowSource = rowSource;
            this.nameColumnsInSource = nameColumnsInSource;
        }

        public IModel ConvertRowToModel()
        {
            int[] numberColumns = SetOrderColumns();

            string[] parsedColumns = rowSource.Split('|');

            IModel model = new Owner
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

        private int[] SetOrderColumns()
        {
            string[] parsedColumns = nameColumnsInSource.Split('|');
            int[] numberColumns = new int[17];

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
            return numberColumns;
        }
    }

    public class ParserRowToCar: IParserRowTo
    {
        private string rowSource;
        private string nameColumnsInSource;

        public ParserRowToCar(string rowSource, string nameColumnsInSource)
        {
            this.rowSource = rowSource;
            this.nameColumnsInSource = nameColumnsInSource;
        }

        public IModel ConvertRowToModel()
        {
            int[] numberColumns = SetOrderColumns();
            string[] parsedColumns = rowSource.Split('|');
            IModel model = new Car
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

        private int[] SetOrderColumns()
        {
            string[] parsedColumns = nameColumnsInSource.Split('|');
            int[] numberColumns = new int[7];

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
        
            return numberColumns;
        }
    }

    public class ParserRowToCarAndOwner: IParserRowTo
    {
        private string rowSource;
        private string nameColumnsInSource;

        public ParserRowToCarAndOwner(string rowSource, string nameColumnsInSource)
        {
            this.rowSource = rowSource;
            this.nameColumnsInSource = nameColumnsInSource;
        }

        public IModel ConvertRowToModel()
        {
            int[] numberColumns = SetOrderColumns();
            string[] parsedColumns = rowSource.Split('|');
            IModel model = new CarAndOwner
            {
                Name = parsedColumns[numberColumns[0]],

                Type = parsedColumns[numberColumns[3]]?.Length > 0 ? TypeOwner.Enterprise : TypeOwner.Person,

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
                CodeDate = parsedColumns[numberColumns[16]],


                Plate = parsedColumns[numberColumns[17]],
                Factory = parsedColumns[numberColumns[18]],
                Model = parsedColumns[numberColumns[19]],
                ManufactureYear = parsedColumns[numberColumns[20]],
                BodyNumber = parsedColumns[numberColumns[21]],
                ChassisNumber = parsedColumns[numberColumns[22]],
                EngineVolume = parsedColumns[numberColumns[23]]
            };

            return model;
        }

        private int[] SetOrderColumns()
        {
            string[] parsedColumns = nameColumnsInSource.Split('|');
            int[] numberColumns = new int[24];

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

                    case "Державний номер":
                        numberColumns[17] = i;
                        break;
                    case "Марка":
                        numberColumns[18] = i;
                        break;
                    case "Модель":
                        numberColumns[19] = i;
                        break;
                    case "Рік випуска":
                        numberColumns[20] = i;
                        break;
                    case "№ кузова":
                        numberColumns[21] = i;
                        break;
                    case "№ шасі":
                        numberColumns[22] = i;
                        break;
                    case "Об'єм двигуна":
                        numberColumns[23] = i;
                        break;
                }
            }

            return numberColumns;
        }
    }

}
