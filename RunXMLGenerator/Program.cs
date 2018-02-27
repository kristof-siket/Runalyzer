using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunXMLGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            FootRaceData fd = new FootRaceData("földrengető_futás", 12, 100, 1000);
            fd.InitCompetitors();


            Console.WriteLine("XML-állományok készítése....");
            if (Directory.EnumerateFiles("../../../Runalyzer/FootRaceXMLs/").Count() != 0)
            {
                foreach (string f in Directory.EnumerateFiles("../../../Runalyzer/FootRaceXMLs/"))
                {
                    File.Delete(f);
                }
            }


            for (int i = 0; i < fd.NumberOfCompetitors; i++)
            {
                fd.NextComp();
                fd.GenerateFootRaceXML(); 
            }

            Console.WriteLine("Kész az XML állományok, kilépéshez nyomjon meg egy billentyűt!");

            Console.ReadLine();
        }
    }
}
