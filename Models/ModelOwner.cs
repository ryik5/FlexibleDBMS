namespace AutoAnalysis
{
    public class Owner : IModel
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

}
