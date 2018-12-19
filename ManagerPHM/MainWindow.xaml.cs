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

namespace ManagerPHM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void menuSkladKomodity_Click(object sender, RoutedEventArgs e)
        {
            oknoKomodit oKomodit = new oknoKomodit();
            druheOkno.Children.Add(oKomodit);
        }

        //----------pokus-------//
        private void menuEvidence_Click(object sender, RoutedEventArgs e)
        {
           
            //SolidColorBrush brushPozadi = new SolidColorBrush(Color.FromArgb(240, 240, 240, 240));
            //SolidColorBrush brushPismo = new SolidColorBrush(Colors.Black);
            //MenuItem j = (MenuItem)sender;
            //j.Background = brushPozadi;
           // j.Foreground = brushPismo;
        }

        private void menuEvidenceFirmy_Click(object sender, RoutedEventArgs e)
        {
            druheOkno.Children.Clear();
        }
    }
}
