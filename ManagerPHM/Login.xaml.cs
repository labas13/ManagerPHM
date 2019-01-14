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
using System.Windows.Shapes;

namespace ManagerPHM
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private DB db;
        SpravceUcet sprUcet;

        public Login()
        {
            InitializeComponent();
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
            string jmeno = TBjmeno.Text;
            string heslo = TBheslo.Password;
            int odpoved = sprUcet.overUzivatele(db, jmeno, heslo);
            switch (odpoved)
            {
                case 0:
                    string jmenoUzivatel = "sem doplním jmeno s odpoved"; // potřeba dopsat nějak způsob jak to načíst z databáze předávání z metody "overUzivatele"
                    MainWindow mojeOkno = new MainWindow(jmenoUzivatel);
                    mojeOkno.Show();
                    this.Close();
                    break;
                case 1:
                    TBjmeno.Text = "";
                    TBheslo.Password = "";
                    TBjmeno.Focus();
                    MessageBox.Show("Nesprávně zadané Jméno nebo Heslo !");
                    break;
                case 2:
                    MessageBox.Show("Spojení s databází se nezdařilo, skontroluj prvky připojení k databází");
                    break;
            }
        }
    }
}
