using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunXMLGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            FootRaceData fd = new FootRaceData("földrengető_futás", 12, 50, 100);
            fd.InitCompetitors();

            Console.WriteLine("XML-állományok készítése....");

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
