namespace AutoAnalysis
{

    public class Car : IModel
    {
        public int ID { get; set; }
        public string Plate { get; set; }
        public string Factory { get; set; }
        public string Model { get; set; }
        public string ManufactureYear { get; set; }
        public string BodyNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineVolume { get; set; }
        public string Name { get; set; }

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
}
