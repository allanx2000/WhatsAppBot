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
using WhatsAppBot.Client;

namespace WhatsAppBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            var waClient = new WhatsAppClient();
            var azClient = new AZLyricsClient();
            this.vm = new MainWindowViewModel(this, waClient, azClient);
            DataContext = vm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            vm.StartUp();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.Close();
        }
    }
}
