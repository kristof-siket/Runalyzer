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


    class ViewModel : Bindable
    {
        private RunDataProcessor processor;
        private List<ObservableCollection<BindingData>> sourceDataCollections;
        private ObservableCollection<BindingData> sumData;

        private static ViewModel _peldany;

        public RunDataProcessor Processor { get => processor; set => processor = value; }
        public List<ObservableCollection<BindingData>> SourceDataCollections { get { return sourceDataCollections; } set  { sourceDataCollections = value; OPC(); } }

        public ObservableCollection<BindingData> SumData { get => sumData; set { sumData = value; OPC(); } }

        private ViewModel()
        {
            this.Processor = new RunDataProcessor();
            this.SourceDataCollections = new List<ObservableCollection<BindingData>>();
            this.SumData = new ObservableCollection<BindingData>();
        }

        public static ViewModel Get()
        {
            if (_peldany == null)
                _peldany = new ViewModel();
            return _peldany;
        }

        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration) 
        {
            return new ObservableCollection<T>(enumeration);
        }
    }
}
