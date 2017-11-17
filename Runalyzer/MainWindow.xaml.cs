using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Calculation;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Diagnostics;

namespace Runalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel VM;
        public MainWindow()
        {
            VM = ViewModel.Get();
            this.DataContext = VM;
            InitializeComponent();
            BindingData next;
            object addLock = new object();

            VM.Processor.CreateBindingBag();

            Task a = new Task(() =>
            {
                while (!VM.Processor.IsConsumptionReady || !VM.Processor.IsQueueEmpty || !VM.Processor.IsProductionReady)
                {
                    if (VM.Processor.GetNextBindingData(out next))
                    {
                        Dispatcher.Invoke(
                            () =>
                            {
                                VM.SumData.Add(next);
                            }, DispatcherPriority.Normal);
                    }
                        
                }
            });

            a.Start();    
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (VM.SelectedItem != null)
            {
                ObservableCollection<Point> speed_points = new ObservableCollection<Point>();
                ObservableCollection<Point> pulse_points = new ObservableCollection<Point>();
                float avg_speed = 0;
                float avg_pulse = 0;
                for (int i = 0; i < VM.SelectedItem.Speeds.Count; i++)
                {
                    avg_speed = (avg_speed == 0 ? VM.SelectedItem.Speeds[i] : (avg_speed + VM.SelectedItem.Speeds[i]) / 2);
                    avg_pulse = (avg_pulse == 0 ? VM.SelectedItem.Pulses[i] : (avg_pulse + VM.SelectedItem.Pulses[i]) / 2);
                    if (i % 600 == 0) // kicsit redukálom az adatmennyiséget
                    {
                        speed_points.Add(new Point(i, avg_speed)); // adott percre vonatkozó átlagot nézem 
                        pulse_points.Add(new Point(i, avg_pulse));
                        avg_pulse = 0;
                        avg_speed = 0;

                    }
                }
                ChartsWindow cw = new ChartsWindow(speed_points, pulse_points, VM.SelectedItem.Rajtszam);
                cw.ShowDialog();
            }

            else
                MessageBox.Show("Nincs elem kijelölve!");
        }
        private ObservableDataSource<Point> LoadChartData()
        {
            ObservableDataSource<Point> points = new ObservableDataSource<Point>();

            for (int i = 0; i < VM.SelectedItem.Speeds.Count; i++)
            {
                points.Collection.Add(new Point(i, VM.SelectedItem.Speeds[i]));

            }
            return points;
        }
                
    }

}

        

