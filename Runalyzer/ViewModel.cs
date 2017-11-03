using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculation;
using System.Collections.ObjectModel;

namespace Runalyzer
{
    public struct BindingData
    {
        private float tavolsag;
        private int pulse;
        private float currentSpeed;
        // etc... egyéb dolgok, amiket kiszámolhatok

        public BindingData(float tavolsag, int pulse, float currentSpeed)
        {
            this.tavolsag = tavolsag;
            this.pulse = pulse;
            this.currentSpeed = currentSpeed;
        }

        public float Tavolsag { get => tavolsag; set => tavolsag = value; }
        public int Pulse { get => pulse; set => pulse = value; }
  
    }

    class ViewModel
    {
        private RunDataProcessor processor;
        private List<ObservableCollection<BindingData>> sourceDataCollections;

        private ViewModel()
        {
            this.processor = new RunDataProcessor();
            this.sourceDataCollections = new List<ObservableCollection<BindingData>>();
        }
    }
}
