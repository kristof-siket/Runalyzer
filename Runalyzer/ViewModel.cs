using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Runalyzer
{
    public abstract class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OPC([CallerMemberName]string n = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(n));
        }
    }

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
        public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
    }

    class ViewModel : Bindable
    {
        private RunDataProcessor processor;
        private List<ObservableCollection<BindingData>> sourceDataCollections;

        private static ViewModel _peldany;

        public RunDataProcessor Processor { get => processor; set => processor = value; }
        public List<ObservableCollection<BindingData>> SourceDataCollections { get { return sourceDataCollections; } set  { sourceDataCollections = value; OPC(); } }

        private ViewModel()
        {
            this.Processor = new RunDataProcessor();
            this.SourceDataCollections = new List<ObservableCollection<BindingData>>();
        }

        public static ViewModel Get()
        {
            if (_peldany == null)
                _peldany = new ViewModel();
            return _peldany;
        }
    }
}
