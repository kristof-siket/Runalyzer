using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void WriteAllData()
        {
            xreader = new XmlTextReader(fname);
            while (xreader.Read() && xreader.Name != "Comps")
                Console.WriteLine(xreader.Name + ": " + xreader.Value); Thread.Sleep(1000);
        }

        public string GetTimeStep()
        {
            xreader = new XmlTextReader(fname);
            string timestep = "";
            while (xreader.Read())
            {
                if (xreader.Name == "Timestep")
                    return xreader.Value;
            }
            return timestep;
        }

        public string[] GetXMLHeader()
        {
            // TODO: xml-linq-val megcsinalni, hogy rendes hasznalhato formatumba jojjenek az adatok valami structba vagy ilyesmi
            xreader = new XmlTextReader(fname);
            xreader.WhitespaceHandling = WhitespaceHandling.None;

            int i = 0;
            string[] s = new string[10];
            while (xreader.Read())
            {
                if (xreader.Name == "Comps" || i >= 6)
                    break;
                s[i] = xreader.Value.ToString();
                i++;
            }
            return s;
        }

        public float GetCurrentSpeed(int timestep, float currentPosition, float prevPosition)
        {
            return ((currentPosition - prevPosition) / timestep) * 60 * 60 * 1000;
        }

        public IEnumerable<XElement> EnumerateAxis(string axis)
        {
            xreader = new XmlTextReader(fname);
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