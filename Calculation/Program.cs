using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Calculation
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../Runalyzer/FootRaceXMLs/földrengető_futás.runal.xml";
            RunDataProcessor processor = new RunDataProcessor(path);

            Console.WriteLine("Nyomj entert az xml fejléc feldolgozásához");
            Console.ReadLine();

            Console.WriteLine("HEADER:");
            processor.GetXMLHeader(path);



            Console.WriteLine("Nyomj entert az indulók listázásához");
            Console.ReadLine();
            
            foreach (var node in processor.EnumerateAxis("Competitor"))
                foreach (var el in (node as XElement).Elements())
                    Console.WriteLine((el as XElement).Name + " " + (el as XElement).Value);

            Console.ReadLine();
        }
    }
}
