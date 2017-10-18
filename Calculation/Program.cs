using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../Runalyzer/FootRaceXMLs/Földrengető futás.runal.xml";

            Console.WriteLine("Nyomj entert az xml feldolgozásához");
            Console.ReadLine();

            Console.WriteLine("HEADER:");
            RunDataProcessor.GetXMLHeader(path);

            Console.ReadLine();
        }
    }
}
