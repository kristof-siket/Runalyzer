﻿using System;
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
            FootRaceData fd = new FootRaceData("Földrengető futás", 12, 50, 100);

            fd.GenerateFootRaceXML();

            Console.ReadLine();
        }
    }
}
