using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;

namespace Runalyzer
{
    /// <summary>
    /// Interaction logic for ChartsWindow.xaml
    /// </summary>
    public partial class ChartsWindow : Window
    {
        private ObservableCollection<Point> speed_points;
        private ObservableCollection<Point> pulse_points;
        private int rajtszam;
        ViewModel VM;
        public ChartsWindow(ObservableCollection<Point> speed_points, ObservableCollection<Point> pulse_points, int rajtszam)
        {
            this.rajtszam = rajtszam;
            this.Title = rajtszam.ToString() + ". versenyző";
            this.speed_points = speed_points;
            this.pulse_points = pulse_points;
            VM = ViewModel.Get();
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ObservableDataSource<Point> speeds = new ObservableDataSource<Point>(speed_points);
            ObservableDataSource<Point> pulses = new ObservableDataSource<Point>(pulse_points);

            plotter1.AddLineChart(speed_points);
            //plotter1.Viewport.FitToView();

            plotter2.AddLineChart(pulse_points);
            //plotter2.Viewport.FitToView();
        }
    }
}
