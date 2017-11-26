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
        static Random rnd = new Random();
        object tavolsagLock = new object();

        private static Queue<Competitor> compQueue = new Queue<Competitor>();

        string eventName;
        int length;
        int numberOfCompetitors;
        int timestep;   // ez millisecundumban értendő
        Competitor comp;

        public string EventName { get => eventName; set => eventName = value; }
        public int Length { get => length; set => length = value; }
        public int NumberOfCompetitors { get => numberOfCompetitors; set => numberOfCompetitors = value; }
        public int Timestep { get => timestep; set => timestep = value; }
        public Competitor Comp { get => comp;  set => comp = value;  }
        

        public FootRaceData(string eventName, int length, int numberOfCompetitors, int timestep)
        {
            this.EventName = eventName;
            this.Length = length;
            this.NumberOfCompetitors = numberOfCompetitors;
            this.Timestep = timestep;
        }

        public FootRaceData()   // hogy szerializálható legyen
        {

        }

        public void NextComp()
        {
            Comp = compQueue.Dequeue();
        }

        public void InitCompetitors()
        {
            for (int i = 0; i < numberOfCompetitors; i++)
            {
                Competitor c = GenerateRandomCompetitor();
                compQueue.Enqueue(c);
            }
        }

        public void GenerateFootRaceXML()
        {
            //  A versenyzőket elég rajtszámmal azonosítani, amivel elkerülhetjük a nevek beírását.
            XmlSerializer writer = new XmlSerializer(typeof(FootRaceData));
            var path = "../../../Runalyzer/FootRaceXMLs/" + EventName + Comp.Rajtszam +  ".runal.xml";
            System.IO.FileStream file = System.IO.File.Create(path);
            writer.Serialize(file, this);
            file.Close();
        }

        private Competitor GenerateRandomCompetitor()
        {
            Competitor comp = new Competitor();
            comp.Rajtszam = 100 + COMP_COUNTER++;

            long hatralevoIdo = this.Length * 60 * 60 * 1000;   // hatralevo ido millisecundumban (kezdetben ez a verseny ideje)

            comp.Bejegyzesek = new List<Rekord>();

            comp.Bejegyzesek.Add(new Rekord(0, rnd.Next(96, 130)));

            float alaptempo = (float)rnd.Next(250, 340) / 1000000;
            float freshness = (float)rnd.Next(1100, 1200) / 1000;
            long deadlock = rnd.Next(500000, 1500000);
            float tiring = (hatralevoIdo < deadlock ? (float)rnd.Next(5, 10) : 0) / 100000;
            float raceCondition = (float)((hatralevoIdo < deadlock - 300000) ? (float)rnd.Next(1100, 1200) / 1000 : (float)rnd.Next(900, 950) / 1000);

            int i = 1;
            while (hatralevoIdo > 0)
            {
                float ujTavolsag = 0;
                lock (tavolsagLock)
                {
                    ujTavolsag = (comp.Bejegyzesek[i - 1].tavolsag + (float)(alaptempo * freshness * raceCondition) * ((float)rnd.Next(980, 1020) / (100000 / timestep))); 
                }
                int ujPulzus = (comp.Bejegyzesek[i - 1].pulse < 175 ? rnd.Next(comp.Bejegyzesek[i - 1].pulse - 1, comp.Bejegyzesek[i - 1].pulse + 3) : rnd.Next(comp.Bejegyzesek[i - 1].pulse - 2, comp.Bejegyzesek[i - 1].pulse + 2));
                comp.Bejegyzesek.Add(new Rekord(ujTavolsag, ujPulzus));
                freshness -= tiring;
                hatralevoIdo -= this.Timestep; 
                i++;
            }

            return comp;
        }
    }
}
