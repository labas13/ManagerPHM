using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ManagerPHM
{
    /// <summary>
    /// Interaction logic for oknoUzivatele.xaml
    /// </summary>
    public partial class oknoUzivatele : Window
    {
        DB db;
        SpravceUcet sprUcet;
        DataView dv;        

        SolidColorBrush zelena, oranzova, cervena, cerna, bila, lehceOranzova;

        DispatcherTimer DT1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // sprUcet.nactiVsechnyUzivatele(db);
        }
        public oknoUzivatele(DB db, SpravceUcet sprUcet)
        {
            this.db = db;
            this.sprUcet = sprUcet;
            // připravím si štetce pro pozadí
            zelena = new SolidColorBrush(Colors.LightGreen);
            oranzova = new SolidColorBrush(Colors.Orange);
            cervena = new SolidColorBrush(Colors.Red);
            cerna = new SolidColorBrush(Colors.Black);
            bila = new SolidColorBrush(Colors.White);
            lehceOranzova = new SolidColorBrush(Colors.LightGoldenrodYellow);
            sprUcet.nactiVsechnyUzivatele(db);
            InitializeComponent();

            dv = new DataView(sprUcet.dtVsichniUzivatele);
            dv.Sort = "Prijmeni,Jmeno";
            DGevidenceUzivatel.ItemsSource = null;
            DGevidenceUzivatel.ItemsSource = dv;

            


        }

        

        private void DGevidenceUzivatel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGevidenceUzivatel.SelectedIndex >= 0)
            {
                //najdu označený řádek a uložím si ho jako "DataRowView"
                DataRowView drv = DGevidenceUzivatel.SelectedItem as DataRowView;
                //najdu hodnotu ve sloupci se jménem "..." tohoto řádku a uložím do texbox
                //tbxJmeno.Text = (string)drv.Row["Jmeno"];
                //tbxPrijmeni.Text = (string)drv.Row["Prijmeni"];
                //tbxLogin.Text = (string)drv.Row["Login"];

                // -- pokus --  trochu jinak
                string login = (string)drv.Row["Login"];
                string filtr = string.Format("Login LIKE '{0}'", login);
                DataRow[] nalezeneRadky = sprUcet.dtVsichniUzivatele.Select(filtr);
                DataRow radek = nalezeneRadky[0];
                if (nalezeneRadky.Length != 1)
                {
                    MessageBox.Show("Záznam nenalezen");
                }
                else
                {
                    tbxLogin.Text = (string)radek["Login"];
                    tbxJmeno.Text = (string)radek["Jmeno"];
                    tbxPrijmeni.Text = (string)radek["Prijmeni"];
                   
                    
                    if ((bool)radek["Blokace"] == false)
                        tbxBlokace.Text = "NE";
                    else
                        tbxBlokace.Text = "ANO";
                    switch ((int)radek["Role"])
                    {
                        case 1:
                            tbxRole.Text = "Správce";
                            break;
                        case 2:
                            tbxRole.Text = "Skladník";
                            break;
                        case 3:
                            tbxRole.Text = "Technik";
                            break;
                        case 4:
                            tbxRole.Text = "Ostatní";
                            break;
                    }

                }
            }
            else
            {
                tbxJmeno.Text = "";
                tbxPrijmeni.Text = "";
                tbxLogin.Text = "";
            }
        }

        private void BTuzivatelNovy_Click(object sender, RoutedEventArgs e)
        {
            zapVypTlacitka(false, false, false, false, false, true);
            DGevidenceUzivatel.IsEnabled = false;
            DGevidenceUzivatel.SelectedIndex = -1;
            lblHvezda1.Visibility = Visibility;
            lblHvezda2.Visibility = Visibility;
            lblHvezda3.Visibility = Visibility;
            lblHvezda4.Visibility = Visibility;
            tbxJmeno.IsEnabled = true;
            tbxPrijmeni.IsEnabled = true;
            tbxLogin.IsEnabled = true;
            lblHeslo.Visibility = Visibility;
            lblHeslo.IsEnabled = true;
            pbxHeslo.Visibility = Visibility;
            pbxHeslo.IsEnabled = true;
            tbxJmeno.Focus();
        }



        private void tbxProVsechny_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DGevidenceUzivatel.IsEnabled != true)
            {
                TextBox tbxZadane = (TextBox)sender;
                if (tbxZadane.Text != "")
                {
                    if (tbxZadane.Text.Length > 2)
                    {
                        switch (tbxZadane.Name)
                        {
                            case "tbxJmeno":
                                lblVystrahaJmeno.Content = "";
                                break;
                            case "tbxPrijmeni":
                                lblVystrahaPrijmeni.Content = "";
                                break;
                            case "tbxLogin":
                                {
                                    if (overExistenciLoginu(tbxZadane.Text))
                                    {
                                        lblVystrahaLogin.Content = "Login již existuje !";
                                        tbxZadane.Foreground = cervena;
                                    }
                                    else
                                    {
                                        lblVystrahaLogin.Content = "";
                                        tbxZadane.Foreground = cerna;
                                    }
                                }
                                break;
                        }

                    }
                    else
                    {
                        switch (tbxZadane.Name)
                        {
                            case "tbxJmeno":
                                lblVystrahaJmeno.Content = "zadej minimálně tři znaky !";
                                break;
                            case "tbxPrijmeni":
                                lblVystrahaPrijmeni.Content = "zadej minimálně tři znaky !";
                                break;
                            case "tbxLogin":
                                lblVystrahaLogin.Content = "zadej minimálně tři znaky !";
                                break;
                        }

                    }
                }
                else
                {
                    switch (tbxZadane.Name)
                    {
                        case "tbxJmeno":
                            lblVystrahaJmeno.Content = "";
                            break;
                        case "tbxPrijmeni":
                            lblVystrahaPrijmeni.Content = "";
                            break;
                        case "tbxLogin":
                            lblVystrahaLogin.Content = "";
                            break;
                    }
                } 
            }
        }

        private void btnUzivatelZrusit_Click(object sender, RoutedEventArgs e)
        {
            if (DGevidenceUzivatel.SelectedIndex >= 0)
            {
                string uzivatJmeno = tbxLogin.Text;
                string zprava = string.Format("Opravdu si přejete smazat uživatele {0} ?", uzivatJmeno);
                string nadpis = "Smazání uživatele";
                MessageBoxButton tlacitka = MessageBoxButton.YesNo;
                MessageBoxImage ikona = MessageBoxImage.Question;
                MessageBoxResult result = MessageBox.Show(zprava, nadpis, tlacitka, ikona);

                if (result == MessageBoxResult.Yes)
                {
                    // pokud souhlasím se smazáním tak:
                    // najdu označený řádek a uložím si ho jako "DataRowView"
                    DataRowView drv = DGevidenceUzivatel.SelectedItem as DataRowView;
                    // přečtu si login a uložím
                    string login = (string)drv.Row["Login"];
                    // nastavím filtr
                    string filtr = string.Format("Login LIKE '{0}'", login);
                    // najdu odpovídající řádek v datatable
                    DataRow[] nalezeneRadky = sprUcet.dtVsichniUzivatele.Select(filtr);
                    if (nalezeneRadky.Length != 1)
                    {
                        MessageBox.Show("Chyba - nenalezen pozadovany zaznam.");
                    }
                    else
                    {
                        DataRow nalezeneRadek = nalezeneRadky[0];
                        nalezeneRadek.Delete();
                        bool smazano = sprUcet.smazUzivatele(db, login);
                        if (smazano)
                        {
                            // vše proběhlo OK
                            zakladniNastavInfUzivatel();
                            sprUcet.nactiVsechnyUzivatele(db);
                            DGevidenceUzivatel.SelectedIndex = 0;
                        }
                        else
                        {
                            MessageBox.Show("smazání se nezdařilo !");
                        }


                    }
                }
                else
                    if (result == MessageBoxResult.No)
                {
                    // pokud ne tak:


                }

            }
            else
            {
                MessageBox.Show("Nebyl vybrán žádný uživatel !");
            }
        }

        private void btnUzivatelUpravit_Click(object sender, RoutedEventArgs e)
        {
            if (DGevidenceUzivatel.SelectedIndex >= 0)
            {
                // změním popisek tlačítka (Uložit) na (Ulož změny) od teď neukládá nový ale upravují stávající 
                btnUzivatelUloz.Content = "Uložit změny";
                btnUzivatelUloz2.Content = "Uložit změny";
                // uložím si login vybraného uživatele
                string uzivatJmeno = tbxLogin.Text;
                zapVypTlacitka(false, false, false, false, true, true);
                string jmeno = tbxJmeno.Text;
                string prijmeni = tbxPrijmeni.Text;
                string login = tbxLogin.Text;
                string heslo = pbxHeslo.Password;
                string blok = tbxBlokace.Text;
                string role = tbxRole.Text;

                tbxJmeno.IsEnabled = true;
                tbxPrijmeni.IsEnabled = true;
                tbxLogin.IsEnabled = true;


            }
            else
            {
                MessageBox.Show("není vybrán žádný uživatel");
            }
        }

        private void btnUzivatelUloz_Click(object sender, RoutedEventArgs e)
        {
            

            if (tbxJmeno.Text.Length > 2 && tbxPrijmeni.Text.Length > 2 && tbxLogin.Text.Length > 2)
            {
                // -- pokud ukládám nového uživatele tak:
                if ((string)btnUzivatelUloz.Content == "Uložit")
                {
                    string jmeno = tbxJmeno.Text;
                    string prijmeni = tbxPrijmeni.Text;
                    string login = tbxLogin.Text;
                    
                    if (overExistenciLoginu(login))
                    {                        
                        string zprava = string.Format("Uživatel s loginem {0} již existuje !", login);
                        string nadpis = "Uložení nového uživatele neproběhlo.";
                        MessageBoxImage ikona = MessageBoxImage.Error;
                        MessageBox.Show(zprava, nadpis, MessageBoxButton.OK, ikona);
                    }

                    else
                    {
                        string sul = sprUcet.vytvorSul(10);
                        string hash = sprUcet.vytvorHash(pbxHeslo.Password, sul);
                        // tet uz začnu ukládat do DB .....>
                        DataRow novyUzivatel = sprUcet.dtVsichniUzivatele.NewRow();
                        novyUzivatel["Jmeno"] = jmeno;
                        novyUzivatel["Prijmeni"] = prijmeni;
                        novyUzivatel["Login"] = login;
                        novyUzivatel["Heslo"] = hash;
                        novyUzivatel["Sul"] = sul;
                        sprUcet.dtVsichniUzivatele.Rows.Add(novyUzivatel);
                        bool ulozeniSeZdarilo = sprUcet.ulozUzivatele(db, jmeno, prijmeni, login, hash, sul);
                        if (ulozeniSeZdarilo)
                        {
                            // vše proběhlo OK
                            zakladniNastavInfUzivatel();
                            sprUcet.nactiVsechnyUzivatele(db);
                        }
                        else
                        {
                            // uložení selhalo
                            MessageBox.Show("Uživatele se nezdařilo uložit !");
                        }


                    }
                }
                // -- pokud ukládám jen změny tak toto:
                else
                {

                }


            }
            else
            {
                if (tbxJmeno.Text == "")
                {
                    lblVystrahaJmeno.Content = "Zadej své jméno !";
                    zablikej(lblVystrahaJmeno);
                    
                    
                    
                }
                    
                if (tbxPrijmeni.Text == "")
                    lblVystrahaPrijmeni.Content = "Zadej své příjmení !";
                if (tbxLogin.Text == "")
                    lblVystrahaLogin.Content = "Zadej svůj login !";

            }
        }

        

        private void btnUzivatelStorno_Click(object sender, RoutedEventArgs e)
        {
            zakladniNastavInfUzivatel();

        }



        // pokud se změní text v password_1
        private void pbxHesla_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pbxHeslo.Password.Length > 2)
            {
                // ověřím přítomnost (čísla, vel.pís, mal.pís)
                int cisla = 0;
                int velPis = 0;
                int malPis = 0;
                string heslo = pbxHeslo.Password;
                foreach (char c in heslo)
                {
                    int i = (int)c;
                    if (i >= 48 && i <= 57)
                        cisla++;
                    if (i >= 65 && i <= 90)
                        velPis++;
                    if (i >= 97 && i <= 122)
                        malPis++;
                }
                if (cisla > 0 && velPis > 0 && malPis > 0)
                {
                    lblVystrahaHeslo.Visibility = Visibility.Hidden;
                    pbxHeslo.Foreground = zelena;
                    lblHesloDva.Visibility = Visibility;
                    lblHesloDva.IsEnabled = true;
                    pbxHesloDva.Visibility = Visibility;
                    pbxHesloDva.IsEnabled = true;
                    lblHvezda5.Visibility = Visibility;
                    porovnejHesla();
                }
                else
                {
                    lblVystrahaHeslo.Visibility = Visibility;
                    lblVystrahaHeslo.Content = "Použij číslici, malé a velké písmeno !";
                    pbxHeslo.Foreground = cervena;
                    lblHesloDva.Visibility = Visibility.Hidden;
                    pbxHesloDva.Visibility = Visibility.Hidden;
                    pbxHesloDva.Password = "";
                    pbxHesloDva.Foreground = cervena;
                    lblHvezda5.Visibility = Visibility.Hidden;
                    lblVystrahaHeslaSeNeshoduji.Visibility = Visibility.Hidden;
                }

            }
            else
            {
                lblVystrahaHeslo.Visibility = Visibility;
                lblVystrahaHeslo.Content = "Zadej nejméně tři znaky !";
                pbxHeslo.Foreground = cervena;
                lblHesloDva.Visibility = Visibility.Hidden;
                pbxHesloDva.Visibility = Visibility.Hidden;
                pbxHesloDva.Password = "";
                pbxHesloDva.Foreground = cervena;
                lblHvezda5.Visibility = Visibility.Hidden;
                lblVystrahaHeslaSeNeshoduji.Visibility = Visibility.Hidden;

            }
            if (pbxHeslo.Password.Length < 1)
            {
                lblVystrahaHeslo.Visibility = Visibility.Hidden;
            }
        }

        private void tbxJmeno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbxPrijmeni.Focus();
            }
        }

        private void tbxPrijmeni_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbxLogin.Focus();
            }
        }

        private void tbxLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pbxHeslo.Focus();
            }
        }

        private void pbxHeslo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pbxHesloDva.Focus();
            }
        }

        private void pbxHesloDva_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnUzivatelUloz.Focus();
            }
        }

        //pomocná metoda pro kontrolu zadávání hesla hesla
        private void porovnejHesla()
        {
            if (pbxHesloDva.Password != "")
            {
                lblVystrahaHeslaSeNeshoduji.Visibility = Visibility;
                string h1 = pbxHeslo.Password;
                string h2 = pbxHesloDva.Password;
                if (h1 == h2)
                {
                    lblVystrahaHeslaSeNeshoduji.Visibility = Visibility.Hidden;
                    pbxHesloDva.Foreground = zelena;
                    btnUzivatelUloz.IsEnabled = true;
                }
                else
                {
                    pbxHesloDva.Foreground = cervena;
                    btnUzivatelUloz.IsEnabled = false;
                }
            }
            else
            {
                lblVystrahaHeslaSeNeshoduji.Visibility = Visibility.Hidden;
            }
        }

        private void zablikej(Label lblZadany)
        {
            Label lbl = lblZadany;
            DT1 = new DispatcherTimer();
            DT1.Interval = TimeSpan.FromSeconds(0.2);
            DT1.Tick += DT1_Tick1;
            DT1.Start(); 
            
            
            
               
        }
        private bool schovana = false;
        private int pocet = 6;
        private void DT1_Tick1(object sender, EventArgs e)
        {

            if (pocet != 0)
            {
                if (schovana)
                {
                    lblVystrahaJmeno.Visibility = Visibility;
                    schovana = false;
                    tbxJmeno.Background = lehceOranzova;
                }
                else
                {
                    lblVystrahaJmeno.Visibility = Visibility.Hidden;
                    schovana = true;
                    tbxJmeno.Background = bila;
                }
                pocet--;
            }
            else
            {
                DT1.Stop();
                pocet = 6;
            }
            //throw new NotImplementedException();
        }

        
        private void DT1_Tick()
        {
            //Label tbxZadane = lblZadany;
            
            
            //throw new NotImplementedException();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //------------------------------------------------------------------------------        





        // pomocná metoda pro zapínaní tlačítek
        private void zapVypTlacitka(bool BTnovy, bool BTuprav, bool BTblok, bool BTzrus, bool BTuloz, bool BTstorno)
        {
            btnUzivatelNovy.IsEnabled = BTnovy;
            btnUzivatelUpravit.IsEnabled = BTuprav;
            btnUzivatelBlok.IsEnabled = BTblok;
            btnUzivatelZrusit.IsEnabled = BTzrus;
            btnUzivatelUloz.IsEnabled = BTuloz;
            btnUzivatelStorno.IsEnabled = BTstorno;
        }
        // pomocná metoda pro uvedení záložky (inf.Uživatel) do základního nastavení
        private void zakladniNastavInfUzivatel()
        {
            zapVypTlacitka(true, true, true, true, false, false);
            DGevidenceUzivatel.IsEnabled = true;

            lblVystrahaJmeno.Content = "";
            tbxJmeno.Text = "";
            tbxJmeno.IsEnabled = false;
            lblHvezda1.Visibility = Visibility.Hidden;

            lblVystrahaPrijmeni.Content = "";
            tbxPrijmeni.Text = "";
            tbxPrijmeni.IsEnabled = false;
            lblHvezda2.Visibility = Visibility.Hidden;

            lblVystrahaLogin.Content = "";
            tbxLogin.Text = "";
            tbxLogin.IsEnabled = false;
            lblHvezda3.Visibility = Visibility.Hidden;

            //heslo_1
            pbxHeslo.Password = "";
            pbxHeslo.Visibility = Visibility.Hidden;
            lblHeslo.Visibility = Visibility.Hidden;
            lblHvezda4.Visibility = Visibility.Hidden;
            //heslo_2
            pbxHesloDva.Password = "";
            pbxHesloDva.Visibility = Visibility.Hidden;
            lblHesloDva.Visibility = Visibility.Hidden;
            lblHvezda5.Visibility = Visibility.Hidden;

            // označím první řádek
            DGevidenceUzivatel.SelectedIndex = 0;
        }
        private bool overExistenciLoginu(string login)
        {
            // 1. načtu znovu všechny uživatele
            sprUcet.nactiVsechnyUzivatele(db);
            // nastavím filtr
            string filtr = string.Format("Login LIKE '{0}'", login);
            // najdu odpovídající řádek v datatable
            DataRow[] nalezeneRadky = sprUcet.dtVsichniUzivatele.Select(filtr);
            if (nalezeneRadky.Length == 1)
            {
                return true;
            }
            else
                return false;
        }

        
    }
}
