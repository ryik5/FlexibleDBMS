using System.Collections.Generic;
using System.Text;

namespace AutoAnalysis
{

    public enum TypeOwner
    {
        Person = 0,
        Enterprise = 2
    }
   
    public class CarAndOwner : IModel
    {
        public int ID { get; set; }
        public string Plate { get; set; }
        public string Factory { get; set; }
        public string Model { get; set; }
        public string ManufactureYear { get; set; }
        public string BodyNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string EngineVolume { get; set; }
        public TypeOwner Type { get; set; }

        public int DRFO { get; set; }
        public string F { get; set; }
        public string I { get; set; }
        public string O { get; set; }
        public string Birthday { get; set; }
        public int EDRPOU { get; set; }
        public string Name { get; set; }  //Enterprise
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string BuildingBody { get; set; }
        public string Apartment { get; set; }
        public string CodeOperation { get; set; }
        public string CodeDate { get; set; }

        public virtual List<CarAndOwner> CarAndOwners { get; set; }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CarAndOwner p = (CarAndOwner)obj;
                return (ToString().Equals(p.ToString()));
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

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
