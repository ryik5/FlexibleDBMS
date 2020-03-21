using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }


    public interface ImportData
    {
        void Import();
    }
   
    public interface IStoredData
    {
        List<IRowImportable> Get { get; set; }
    }

    public interface IRowImportable
    {

    }

    public abstract class ICar
    {
        public int ID { get; set; }

        public string Plate { get; set; }
        public string Factory { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }
        public string BodyNumber { get; set; }
        public string ChassisNumber { get; set; }
        public int EngineVolume { get; set; }
    }

    public interface IOwnerCar
    {
        int ID { get; set; }
        string Name { get; set; }
        string Address { get; set; }

        int IDCar { get; set; }
    }

}
