using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace ManagerPHM
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        //private DB db;
        //SpravceUcet sprUcet;
        //DataTable dtPrihlasenyUzivatel;

        // vytvořím si instanci třídy (SpravceDB)
        SpravceDB spravceDB = new SpravceDB();

        public Login()
        {
            InitializeComponent();
            // naplním si tabulku (Ucet)
            spravceDB.DAucet.Fill(spravceDB.DS.Ucet);

            TBjmeno.Focus();
            //vytvořím připojovací řetězec
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = @"LABAS-PC-LENOVO\SQLLABAS";
            csb.InitialCatalog = "DatabazeSklad";
            csb.IntegratedSecurity = true;
            string pripojovaciRetezec = csb.ConnectionString;
            
            //vytvořím instanci třídy "DB" a předám ji připojovací Řetěz
            db = new DB(pripojovaciRetezec);
            //vytvořím správceUcet
            sprUcet = new SpravceUcet();
            // vytvořim odkaz na dtPrihlasenyUzivatel ve správciUzivatelů
            dtPrihlasenyUzivatel = sprUcet.dtPrihlasenyUzivatel;


        }

        private void konec_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            //Application.Current.MainWindow.Close();
        }


        // -- tady je ještě problém !!!!! ----------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

           // this.Close();
            //e.Cancel = false;
            //Application.Current.MainWindow.Close();
        }
        // -------------------------------------------------------------------------------


        private void prihlasit_Click(object sender, RoutedEventArgs e)
        {
            overUzivatele();                   
        }

        private void TBjmeno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TBheslo.Focus();
            }
        }

        private void TBheslo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                overUzivatele();
            }
        }

        // -- pomocná metoda OverUzivatel
        private void overUzivatele()
        {
            // načtu zadání
            string login = TBjmeno.Text;
            string heslo = TBheslo.Password;
            
            if (sprUcet.overUzivatele(db, login, heslo))
            {
                MainWindow mojeOkno = new MainWindow(dtPrihlasenyUzivatel);
                mojeOkno.Show();
                this.Close();
            }
            else
            {
                TBjmeno.Text = "";
                TBheslo.Password = "";
                TBjmeno.Focus();
                MessageBox.Show("Nesprávně zadané Jméno nebo Heslo !");
            }
        }

        private void jmeno_Loaded(object sender, RoutedEventArgs e)
        {
            if(DAucet.Fill(DS.Ucet) != 1)
            {
                MessageBox.Show("Databáze nenalezena ! ");
            }
        }

        /*
        // z netu --->
        byte[] vals = { 0x01, 0xAA, 0xB1, 0xDC, 0x10, 0xDD };

        string str = BitConverter.ToString(vals);
        Console.WriteLine(str);

str = BitConverter.ToString(vals).Replace("-", "");
        Console.WriteLine(str);
        */
        /*Output:
          01-AA-B1-DC-10-DD
          01AAB1DC10DD
         */
    }
}
