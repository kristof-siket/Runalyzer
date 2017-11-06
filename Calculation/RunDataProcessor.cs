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
using System.Diagnostics;

namespace Calculation
{
    public struct BindingData
    {
        private float tavolsag;
        private int pulse;
        private float currentSpeed;
        private float avgSpeed;
        private float maxSpeed;
        private float minSpeed;

        public BindingData(float tavolsag, int pulse, float currentSpeed, float avgSpeed, float maxSpeed, float minSpeed)
        {
            this.tavolsag = tavolsag;
            this.pulse = pulse;
            this.currentSpeed = currentSpeed;
            this.avgSpeed = avgSpeed;
            this.maxSpeed = maxSpeed;
            this.minSpeed = minSpeed;
        }

        // etc... egyéb dolgok, amiket kiszámolhatok

        public float Tavolsag { get => tavolsag; set => tavolsag = value; }
        public int Pulse { get => pulse; set => pulse = value; }
        public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
        public float AvgSpeed { get => avgSpeed; set => avgSpeed = value; }
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
        public float MinSpeed { get => minSpeed; set => minSpeed = value; }
    }

    public class RunDataProcessor
    {
        static string SOURCE_DIRECTORY = "../../../Runalyzer/FootRaceXMLs/";
        static string RESULT_DIRECTORY = "../../../Runalyzer/Results/";

        private XmlTextReader xreader;
        static SemaphoreSlim sem = new SemaphoreSlim(4);
        object fileNameLock = new object();
        private ConcurrentQueue<XDocument> documentBuffer = new ConcurrentQueue<XDocument>();
        private ConcurrentBag<BindingData> bindingBag = new ConcurrentBag<BindingData>();
        public bool IsProductionReady = false;
        public bool IsConsumptionReady = false;
        
        public bool IsQueueEmpty // ondöflaj
        {
            get { return documentBuffer.IsEmpty; }
        }

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
            Stopwatch sw1 = new Stopwatch();
            foreach (string file in Directory.GetFiles(SOURCE_DIRECTORY))
            {
                sw1.Start();
                Console.WriteLine(Path.GetFileName(file) + " várósorba állítása...");
                documentBuffer.Enqueue(this.LoadXMLRecords(file));
                sw1.Stop();
                Console.WriteLine(Path.GetFileName(file) + " készen áll a feldolgozásra! ({0} ms)", sw1.ElapsedMilliseconds);
                sw1.Reset();
            }
            IsProductionReady = true;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("A termelés véget ért.");
            Console.ForegroundColor = ConsoleColor.White;

        }

        private XDocument LoadXMLRecords(string sourcepath)
        {
            XDocument xd = XDocument.Load(sourcepath);
            return xd;
        }

        public void ConsumeSpeed(int taskno)
        {
            while (!(this.IsProductionReady && this.IsQueueEmpty))
            {
                XDocument ujDoksi;
                if (documentBuffer.TryDequeue(out ujDoksi))
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();

                    var rajtszam = (from x in ujDoksi.Descendants("Comp")
                                     select x.Element("Rajtszam").Value).Single();

                    var header = (from x in ujDoksi.Descendants("FootRaceData")
                                 select new
                                 {
                                     EventName = x.Element("EventName").Value,
                                     Length = x.Element("Length").Value,
                                     NumOfComps = x.Element("NumberOfCompetitors").Value,
                                     Timestep = x.Element("Timestep").Value
                                 }).Single();

                    IQueryable<Rekord> sourceRecords = (from x in ujDoksi.Descendants("Rekord")
                                                           select new Rekord
                                                           {
                                                               tavolsag = float.Parse(x.Element("tavolsag").Value, CultureInfo.InvariantCulture),
                                                               pulse = int.Parse(x.Element("pulse").Value.ToString())
                                                           }).AsQueryable();
                    sw1.Stop();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(taskno + ": {0} versenyző elkezdve! (konvertálás: {1} ms) ({2}, {3} óra, {4} induló, timestep: {5})",rajtszam, sw1.ElapsedMilliseconds, header.EventName, header.Length, header.NumOfComps, header.Timestep);
                    Console.ForegroundColor = ConsoleColor.White;
                    float elozo = 0;
                    float elozospd = 0;
                    int iterations = 0;
                    float spd = 0;
                    float acc = 0;
                    float avg = 0;
                    float max = 0;
                    float min = 0;
                    string pre = "";
                    lock (fileNameLock)
                    {
                        sw1.Restart(); 
                    }
                    foreach (Rekord elem in sourceRecords)
                    {
                        spd = GetCurrentSpeed(100, elem.tavolsag, elozo);
                        elozo = elem.tavolsag;
                        acc = spd - elozospd;
                        pre = (acc > 0 ? "Gyorsulás: " : "Lassulás");
                        acc = Math.Abs(acc);
                        avg = (avg == 0 ? spd : ((avg + spd) / 2));
                        min = (min == 0 ? spd : (min > spd ? spd : min));
                        max = (max == 0 ? spd : (max < spd ? spd : max));
                        elozospd = spd;
                        iterations++;
                    }
                    bindingBag.Add(new BindingData(sourceRecords.Last().tavolsag, sourceRecords.Last().pulse, 0, avg, max, min));
                    sw1.Stop();
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(taskno + ": {2} versenyző feldolgozva ({0} ms alatt), átlag: {1} km/h, max: {3} km/h, min: {4} km/h", sw1.ElapsedMilliseconds, avg, rajtszam, max, min);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            IsConsumptionReady = true;
        }

        public void CreateBindingBag()
        {
            new Task(() => ProduceProcessingTasks(), TaskCreationOptions.LongRunning).Start();

            Task[] tasks = new Task[4];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() => ConsumeSpeed(i), TaskCreationOptions.LongRunning);
                tasks[i].Start();
            }
        }

        public bool GetNextBindingData(out BindingData ki)
        {
            return bindingBag.TryTake(out ki);
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