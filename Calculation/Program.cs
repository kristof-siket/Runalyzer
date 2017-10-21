using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using RunXMLGenerator;
using System.Globalization;

namespace Calculation
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../Runalyzer/FootRaceXMLs/földrengető_futás101.runal.xml";
            RunDataProcessor processor = new RunDataProcessor(path);

            Console.WriteLine("Nyomj entert az xml fejléc feldolgozásához");
            Console.ReadLine();

            Console.WriteLine("HEADER:");
            string[] header = processor.GetXMLHeader();

            foreach (string s in header)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("Nyomj entert az indulók listázásához");
            Console.ReadLine();

            float elozo = 0;

            foreach(var node in processor.EnumerateAxis("Rekord"))
            {
                float tavolsag = 0;
                int pulse = 0;
                foreach (var el in (node as XElement).Elements())
                {
                    if (el.Name == "tavolsag")
                        tavolsag = float.Parse(el.Value, CultureInfo.InvariantCulture);
                    else
                        pulse = int.Parse(el.Value);
                }
                Rekord r = new Rekord(tavolsag, pulse);
                Console.WriteLine(r);
                Console.WriteLine("Sebesség: " + processor.GetCurrentSpeed(100, tavolsag, elozo) + " km/h");
                Console.WriteLine();
                elozo = tavolsag;
            }

            Console.ReadLine();
        }
    }
}
