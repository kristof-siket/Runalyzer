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

            Console.WriteLine("XML fájl elkészítése...");

            fd.GenerateFootRaceXML();

            Console.WriteLine("Kész az XML állomány, kilépéshez nyomjon meg egy billentyűt!");

            Console.ReadLine();
        }
    }
}
