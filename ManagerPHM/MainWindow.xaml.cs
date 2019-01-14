using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        //připravím si
        DB db;
        //SpravceKomoditySklad sprKomSklad;

        //to to možná NE!!!------------------------------------------------------------------------------------
        // vytvařím dataSet a adaptéry všech tabulek v DB
        //LokalniSada DS = new LokalniSada();
        //LokalniSadaTableAdapters.KomoditySkladTableAdapter DAKomoditySklad = new LokalniSadaTableAdapters.KomoditySkladTableAdapter();
       // LokalniSadaTableAdapters.AdresaTableAdapter DAadresa = new LokalniSadaTableAdapters.AdresaTableAdapter();
       // LokalniSadaTableAdapters.celkemTableAdapter DAcelkem = new LokalniSadaTableAdapters.celkemTableAdapter();
       // LokalniSadaTableAdapters.FirmaTableAdapter DAfirma = new LokalniSadaTableAdapters.FirmaTableAdapter();
       // LokalniSadaTableAdapters.HodnostTableAdapter DAhodnost = new LokalniSadaTableAdapters.HodnostTableAdapter();
       // LokalniSadaTableAdapters.JakostakTableAdapter DAjakost = new LokalniSadaTableAdapters.JakostakTableAdapter();
       // LokalniSadaTableAdapters.KategorieTechnikaTableAdapter DAkategorieTechnika = new LokalniSadaTableAdapters.KategorieTechnikaTableAdapter();
       // LokalniSadaTableAdapters.KategorieZamestnanecTableAdapter DAkategorieZamestnanec = new LokalniSadaTableAdapters.KategorieZamestnanecTableAdapter();
       // LokalniSadaTableAdapters.KcmTableAdapter DAkcm = new LokalniSadaTableAdapters.KcmTableAdapter();
       // LokalniSadaTableAdapters.KomoditaTableAdapter DAkomodita = new LokalniSadaTableAdapters.KomoditaTableAdapter();
       // LokalniSadaTableAdapters.KvalifikacniListTableAdapter DAkvalifikacniList = new LokalniSadaTableAdapters.KvalifikacniListTableAdapter();
        //.....---------------------------------------------------------------------------------------------------------
        // ještě dodělat adaptéry ...

        public MainWindow(string jmenoUzivatele)
        {
          
            InitializeComponent();

            //vytvořím připojovací řetězec
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = @"LABAS-PC-LENOVO\SQLLABAS";
            csb.InitialCatalog = "DatabazeSklad";
            csb.IntegratedSecurity = true;
            string pripojovaciRetezec = csb.ConnectionString;

            //vytvořím instanci třídy "DB" a předám ji připojovací Řetěz
            db = new DB(pripojovaciRetezec);


           // Login oknoLogin = new Login(db);
           // oknoLogin.ShowDialog();
        }
        

        private void menuSkladKomodity_Click(object sender, RoutedEventArgs e)
        {
            //sprKomSklad = new SpravceKomoditySklad();
            //sprKomSklad.nactiVse(db);


            oknoKomodit oKomodit = new oknoKomodit(db);
            druheOkno.Children.Add(oKomodit);
            //oKomodit.DGkomodity.ItemsSource = sprKomSklad.dtKomoditySklad.DefaultView;
            //oKomodit.DGkomodity.ItemsSource = DS.KomoditySklad.DefaultView;
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

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            //DAkomodita.Fill(DS.Komodita);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DAkomodita.Fill(DS.Komodita);
            //DAKomoditySklad.Fill(DS.KomoditySklad);
        }

        
    }
}
