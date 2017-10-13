using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RunXMLGenerator
{
    public class FootRaceData
    {
        static int COMP_COUNTER = 1; // a rajtszámokhoz kell
        static int XMLCOUNTER = 1;  // az xml-fájlnevekhez kell

        string eventName;
        int length;
        int numberOfCompetitors;
        int timestep;   // ez millisecundumban értendő

        public string EventName { get => eventName; set => eventName = value; }
        public int Length { get => length; set => length = value; }
        public int NumberOfCompetitors { get => numberOfCompetitors; set => numberOfCompetitors = value; }
        public int Timestep { get => timestep; set => timestep = value; }

        public FootRaceData(string eventName, int length, int numberOfCompetitors, int timestep)
        {
            this.EventName = eventName;
            this.Length = length;
            this.NumberOfCompetitors = numberOfCompetitors;
            this.Timestep = timestep;
        }

        public FootRaceData()
        {

        }

        public void GenerateFootRaceXML()
        {
            //  TODO: XML-t generálni a konstruktorban megadott fejléc-adatokkal, és irányított random értékekkel
            //  A versenyzőket elég rajtszámmal azonosítani, amivel elkerülhetjük a nevek beírását.
            XmlSerializer writer = new XmlSerializer(typeof(FootRaceData));
            var path = "../../../Runalyzer/FootRaceXMLs/" + EventName + ".xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, this);
            file.Close();
        }
    }
}
