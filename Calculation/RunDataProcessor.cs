using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Calculation
{
    public class RunDataProcessor
    {
        public static void GetXMLHeader(string fname)
        {
            // TODO: xml-linq-val megcsinalni, hogy rendes hasznalhato formatumba jojjenek az adatok valami structba vagy ilyesmi
            XmlTextReader xreader = new XmlTextReader(fname);

            while (xreader.Read())
            {
                if ((xreader.Name == "Comps"))
                    break;
                Console.WriteLine("{0}: {1}", xreader.Name, xreader.Value); // egyelore konzol
            }
        }

        public static void GetCompetitorRecords(string fname)
        {
            XDocument xd = XDocument.Load(fname);
            var competitors = from x in xd.Descendants("Competitor")
                              select new
                              {
                                  Rajtszam = x.Element("Rajtszam").Value,
                                  Bejegyzesek = x.Descendants("Bejegyzesek").AsQueryable()
                              };

            Console.WriteLine("Versenyzők: ");
            foreach (var comp in competitors)
            {
                Console.WriteLine(comp.Rajtszam);
            }
        }
    }
}