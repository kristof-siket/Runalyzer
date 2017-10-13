using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunXMLGenerator
{
    public struct Rekord   // csak azé, hogy lehessen egybe kezelni talán
    {
        public float tavolsag;
        public int pulse;

        public Rekord(float tavolsag, int pulse)
        {
            this.tavolsag = tavolsag;
            this.pulse = pulse;
        }
    }
    public class Competitor
    {
        int rajtszam;
        List<Rekord> bejegyzesek;

        public int Rajtszam { get => rajtszam; set => rajtszam = value; }
        public List<Rekord> Bejegyzesek { get => bejegyzesek; set => bejegyzesek = value; }
    }


}
