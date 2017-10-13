using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunXMLGenerator
{
    class FootRaceData
    {
        static int COUNTER = 1; // a rajtszámokhoz kell

        string eventName;
        int length;
        int numberOfCompetitors;
        int timestep;   // ez millisecundumban értendő

        public FootRaceData(string eventName, int length, int numberOfCompetitors, int timestep)
        {
            this.eventName = eventName;
            this.length = length;
            this.numberOfCompetitors = numberOfCompetitors;
            this.timestep = timestep;
        }

        public void GenerateFootRaceXML()
        {
            //  TODO: XML-t generálni a konstruktorban megadott fejléc-adatokkal, és irányított random értékekkel
            //  A versenyzőket elég rajtszámmal azonosítani, amivel elkerülhetjük a nevek beírását.
        }
    }
}
