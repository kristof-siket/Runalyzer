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
        private XmlTextReader xreader;
        string fname;

        public RunDataProcessor(string fname)
        {
            xreader = new XmlTextReader(fname);
            this.fname = fname;
        }

        public void GetXMLHeader(string fname)
        {
            // TODO: xml-linq-val megcsinalni, hogy rendes hasznalhato formatumba jojjenek az adatok valami structba vagy ilyesmi
            xreader = new XmlTextReader(fname);

            while (xreader.Read())
            {
                if ((xreader.Name == "Comps"))
                    break;
                Console.WriteLine(xreader.Name + " " + xreader.Value); // egyelore konzol
            }
        }

        public void GetCompetitorRecords(string fname)
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

        //public string GetCurrentSpeedData()
        //{
        //    xreader.MoveToContent();
        //    while (xreader.Read())
        //}

        public IEnumerable<XElement> EnumerateAxis(string axis)
        {
            xreader.MoveToContent();
            while (xreader.Read())
            {
                switch (xreader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xreader.Name == axis)
                        {
                            XElement el = XElement.ReadFrom(xreader) as XElement;
                            if (el != null)
                                yield return el;
                        }
                        break;
                }
            }
        }
    }
}