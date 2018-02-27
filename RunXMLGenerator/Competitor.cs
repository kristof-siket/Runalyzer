using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public override string ToString()
        {
            return String.Format("Pulzus: {0} bpm\nTávolság: {1} km", pulse, tavolsag);
        }
    }
    public class Competitor
    {
        int rajtszam;
        List<Rekord> bejegyzesek;

        public int Rajtszam { get { return rajtszam; } set { rajtszam = value; } }
        public List<Rekord> Bejegyzesek { get { return bejegyzesek; } set { bejegyzesek = value; } }
    }


}
