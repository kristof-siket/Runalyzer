using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using RunXMLGenerator;
using System.IO;
using System.Collections.Concurrent;

namespace Calculation
{
    public class RunDataProcessor
    {
        static string SOURCE_DIRECTORY = "../../../Runalyzer/FootRaceXMLs/";
        static string RESULT_DIRECTORY = "../../../Runalyzer/Results/";

        private XmlTextReader xreader;
        static SemaphoreSlim sem = new SemaphoreSlim(4);
        object fileNameLock = new object();
        private ConcurrentQueue<IQueryable> documentBuffer = new ConcurrentQueue<IQueryable>();

        public string GetTimeStep(string fname)
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

        public void ProduceProcessingTasks()
        {
            foreach (string file in Directory.GetFiles(SOURCE_DIRECTORY))
            {
                Console.WriteLine(Path.GetFileName(file) + " várósorba állítása...");
                documentBuffer.Enqueue(this.LoadXMLRecordsToQueryable(file));
                Console.WriteLine(Path.GetFileName(file) + "készen áll a feldolgozásra!");
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("A termelés véget ért.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private IQueryable LoadXMLRecordsToQueryable(string sourcepath)
        {
            XDocument xd = XDocument.Load(sourcepath);

            IQueryable sourceRecords = (from x in xd.Descendants("Rekord")
                                        select new
                                        {
                                            Tavolsag = float.Parse(x.Element("tavolsag").Value, CultureInfo.InvariantCulture),
                                            Pulse = int.Parse(x.Element("pulse").Value)
                                        }).AsQueryable();
            return sourceRecords;
        }

        public List<string> CreateResultsList(string sourcefilename)
        {
            List<string> vissza = new List<string>();
            string sourcepath = SOURCE_DIRECTORY + sourcefilename;

            XDocument sourcedoc = XDocument.Load(sourcepath);   // ez egy hosszú folyamat a doksi nagysága miatt

            var sourceRecords = from x in sourcedoc.Descendants("Rekord")
                                select new
                                {
                                    Tavolsag = float.Parse(x.Element("tavolsag").Value, CultureInfo.InvariantCulture),
                                    Pulse = int.Parse(x.Element("pulse").Value)
                                };

            float elozo = 0;

            foreach (var rec in sourceRecords)
            {
                string ki = String.Format("Pulzus: {0}\r\nTávolság: {1}\r\nSebesség: {2}\r\n", rec.Pulse, rec.Tavolsag, GetCurrentSpeed(100, rec.Tavolsag, elozo));
                elozo = rec.Tavolsag;
                vissza.Add(ki);
            }

            return vissza;
        }



        public void SendResultsToTextFileLinq(string sourcefilename, string destfilename)
        {
            string sourcepath = SOURCE_DIRECTORY + sourcefilename;
            string destpath = RESULT_DIRECTORY + destfilename;

            XDocument sourcedoc = XDocument.Load(sourcepath);   // ez egy hosszú folyamat a doksi nagysága miatt

            var sourceRecords = from x in sourcedoc.Descendants("Rekord")
                                select new
                                {
                                    Tavolsag = float.Parse(x.Element("tavolsag").Value, CultureInfo.InvariantCulture),
                                    Pulse = int.Parse(x.Element("pulse").Value)
                                };

            StreamWriter sw = new StreamWriter(destpath);

            float elozo = 0;

            foreach (var rec in sourceRecords)
            {
                string ki = String.Format("Pulzus: {0}\r\nTávolság: {1}\r\nSebesség: {2}\r\n", rec.Pulse, rec.Tavolsag, GetCurrentSpeed(100, rec.Tavolsag, elozo));
                elozo = rec.Tavolsag;
                sw.WriteLine(ki);
            }
            sw.Close();
        }

        public float GetCurrentSpeed(int timestep, float currentPosition, float prevPosition)
        {
            return ((currentPosition - prevPosition) / timestep) * 60 * 60 * 1000;
        }


        private string GetRajtszamFromFileName(string filename)
        {
            string s = "";
            int i = 0;
            while (i < filename.Length)
            {
                if (Char.IsDigit(filename[i]))
                    s += filename[i];
                i++;
            }
            return s;
        }

        public string GetDestinationFileName(string filename)
        {
            return "result" + GetRajtszamFromFileName(filename) + ".txt";
        }

        public void ProcessAllSourceFilesSeq()
        {
            string[] filePaths = Directory.GetFiles(SOURCE_DIRECTORY);

            for (int i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = Path.GetFileName(filePaths[i]);
            }

            foreach (string file in filePaths)
            {
                SendResultsToTextFileLinq(file, this.GetDestinationFileName(file));
            }
        }

        public void ProcessAllSourceFiles()
        {
            string[] filePaths = Directory.GetFiles(SOURCE_DIRECTORY);

            for (int i = 0; i < filePaths.Length; i++)
            {
                filePaths[i] = Path.GetFileName(filePaths[i]);
            }

            foreach (string file in filePaths)
            {
                new Task(() => new FileToProcess(file, this).Process(), TaskCreationOptions.LongRunning).Start();
            }
        }
    }

    class FileToProcess
    {
        private RunDataProcessor processor;
        static SemaphoreSlim sem = new SemaphoreSlim(8);
        object lockObj = new object();

        private string nev;

        public FileToProcess(string nev, RunDataProcessor processor)
        {
            this.processor = processor;
            Nev = nev;
        }

        public string Nev { get => nev; set => nev = value; }


        public void Process()
        {
            Console.WriteLine("Feldolgozásra kész: " + Nev);
            sem.Wait();
            Console.WriteLine("Feldolgozás megkezdve: " + Nev);
            processor.CreateResultsList(Nev);
            Console.WriteLine("Feldolgozva: " + Nev);
            sem.Release();
        }
    }
}