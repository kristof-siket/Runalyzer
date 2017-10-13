﻿using System;
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
        static Random rnd = new Random();

        string eventName;
        int length;
        int numberOfCompetitors;
        int timestep;   // ez millisecundumban értendő
        Competitor[] comps;

        public string EventName { get => eventName; set => eventName = value; }
        public int Length { get => length; set => length = value; }
        public int NumberOfCompetitors { get => numberOfCompetitors; set => numberOfCompetitors = value; }
        public int Timestep { get => timestep; set => timestep = value; }
        public Competitor[] Comps { get => comps; set => comps = value; }

        public FootRaceData(string eventName, int length, int numberOfCompetitors, int timestep)
        {
            this.EventName = eventName;
            this.Length = length;
            this.NumberOfCompetitors = numberOfCompetitors;
            this.Timestep = timestep;
            this.Comps = new Competitor[NumberOfCompetitors];

            for (int i = 0; i < numberOfCompetitors; i++)
            {
                Comps[i] = GenerateRandomCompetitor();
            }
        }

        public FootRaceData()
        {

        }

        public void GenerateFootRaceXML()
        {
            //  TODO: XML-t generálni a konstruktorban megadott fejléc-adatokkal, és irányított random értékekkel
            //  A versenyzőket elég rajtszámmal azonosítani, amivel elkerülhetjük a nevek beírását.
            XmlSerializer writer = new XmlSerializer(typeof(FootRaceData));
            var path = "../../../Runalyzer/FootRaceXMLs/" + EventName + ".runal.xml";
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

            int i = 1;
            while (hatralevoIdo > 0)
            {
                float ujTavolsag = (comp.Bejegyzesek[i - 1].tavolsag + (float)((float)rnd.Next(1, 4) / 100));
                int ujPulzus = (comp.Bejegyzesek[i - 1].pulse < 175 ? rnd.Next(comp.Bejegyzesek[i - 1].pulse - 1, comp.Bejegyzesek[i - 1].pulse + 3) : rnd.Next(comp.Bejegyzesek[i - 1].pulse - 2, comp.Bejegyzesek[i - 1].pulse + 2));
                comp.Bejegyzesek.Add(new Rekord(ujTavolsag, ujPulzus));
                hatralevoIdo -= this.Timestep;
                i++;
            }

            return comp;
        }
    }
}
