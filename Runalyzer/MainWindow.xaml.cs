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
                while (!VM.Processor.IsConsumptionReady)
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
    }
}
