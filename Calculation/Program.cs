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
            RunDataProcessor processor = new RunDataProcessor();

            Console.WriteLine("Nyomj entert a feldolgozás megkezdéséhez!");
            Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Dokumentumok előkészítése..");
            Console.ForegroundColor = ConsoleColor.White;

            new Task(() => processor.ProduceProcessingTasks(), TaskCreationOptions.LongRunning).Start();

            new Task(() =>
            {
                processor.ConsumeSpeed(1);
            }, TaskCreationOptions.LongRunning).Start();

            new Task(() =>
            {
                processor.ConsumeSpeed(2);
            }, TaskCreationOptions.LongRunning).Start();

            new Task(() =>
            {
                processor.ConsumeSpeed(3);
            }, TaskCreationOptions.LongRunning).Start();

            new Task(() =>
            {
                processor.ConsumeSpeed(4);
            }, TaskCreationOptions.LongRunning).Start();

            Console.ReadLine();
        }
    }
}
