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
        bool prihlasenySpravce;
        string prihlasenyLogin;
        // - proměnné pro kontrolu změny
        private string puvodniJmeno, puvodniPrijmeni, puvodniLogin, puvodniBlokace, puvodniRole;
        
       

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

            // - zjistím jetli je přihlášen správce a stav uložím
            //prihlasenySpravce = false;
            int role = Convert.ToInt32(sprUcet.dtPrihlasenyUzivatel.Rows[0]["Role"]);
            if (role == 1)
                prihlasenySpravce = true;
            else
                prihlasenySpravce = false;
            // - podle toho nastavým tlačítkům (enable)
            if (prihlasenySpravce)
            {
                zapVypTlacitka(true, true, true, true, false, false, false, false);
                zalozkaOpravneni.IsEnabled = true;
            }                
            else
                zapVypTlacitka(false, false, false, false, false, false, false,false);

            // - uložím si login přihlášeného
            prihlasenyLogin = sprUcet.dtPrihlasenyUzivatel.Rows[0]["Login"].ToString();        


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

                // zpřístupním tlačítko uprav Uživatele
                if (prihlasenySpravce || prihlasenyLogin == (string)drv.Row["Login"])
                    btnUzivatelUpravit.IsEnabled = true;
                else
                    btnUzivatelUpravit.IsEnabled = false;

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
                        cbxBlokace.SelectedIndex = 0;
                    else
                        cbxBlokace.SelectedIndex = 1;
                    switch ((int)radek["Role"])
                    {
                        case 1:
                            cbxRole.SelectedIndex = 0;
                            break;
                        case 2:
                            cbxRole.SelectedIndex = 1;
                            break;
                        case 3:
                            cbxRole.SelectedIndex = 2;
                            break;
                        case 4:
                            cbxRole.SelectedIndex = 3;
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
            nastavZobrazeniNovy();           
            DGevidenceUzivatel.SelectedIndex = -1; // VYNULUJE texboxi

            tbxJmeno.Focus();
            zalozkaOpravneni.IsEnabled = true;
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
                            nastavZobrazeniStart();
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
                //ulžím původní hodnoty
                puvodniJmeno = tbxJmeno.Text;
                puvodniPrijmeni = tbxPrijmeni.Text;
                puvodniLogin = tbxLogin.Text;                
                ComboBoxItem cbiBlokace = (ComboBoxItem)(cbxBlokace.Items[cbxBlokace.SelectedIndex]);
                puvodniBlokace = cbiBlokace.Content.ToString();
                ComboBoxItem cbiRole = (ComboBoxItem)(cbxRole.Items[cbxRole.SelectedIndex]);
                puvodniRole = cbiRole.Content.ToString();
                // nastavím zobrazení pro upravy
                nastavZobrazeniUpravit();
                
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
                            nastavZobrazeniStart();
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
            nastavZobrazeniStart();

        }



        // pokud se změní text v password_1 nebo password_2
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
                btnUzivatelUloz.IsEnabled = false;
                btnUzivatelUloz2.IsEnabled = false;

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

        private void btnZmenitHeslo_Click(object sender, RoutedEventArgs e)
        {
            //lblHeslo.IsEnabled = true;
            lblHeslo.Content = "Nové heslo";
            pbxHeslo.IsEnabled = true;
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
        private void zapVypTlacitka(bool BTnovy, bool BTuprav, bool BTblok, bool BTzrus, bool BTuloz, bool BTuloz2, bool BTstorno, bool BTstorno2)
        {
            btnUzivatelNovy.IsEnabled = BTnovy;
            btnUzivatelUpravit.IsEnabled = BTuprav;
            btnUzivatelBlok.IsEnabled = BTblok;
            btnUzivatelZrusit.IsEnabled = BTzrus;
            btnUzivatelUloz.IsEnabled = BTuloz;
            btnUzivatelUloz2.IsEnabled = BTuloz2;
            btnUzivatelStorno.IsEnabled = BTstorno;
            btnUzivatelStorno2.IsEnabled = BTstorno2;
        }

        // pomocná metoda pro nastavení ---START ---
        private void nastavZobrazeniStart()
        {
            if (prihlasenySpravce)
            {
                zapVypTlacitka(true, true, true, true, false, false, false, false);
            }                
            else
                zapVypTlacitka(false, false, false, false, false, false, false, false);

            
            btnUzivatelUloz.Visibility = Visibility.Hidden;
            btnUzivatelUloz2.Visibility = Visibility.Hidden;
            btnUzivatelStorno.Visibility = Visibility.Hidden;
            btnUzivatelStorno2.Visibility = Visibility.Hidden;

            DGevidenceUzivatel.IsEnabled = true;

            lblVystrahaJmeno.Content = "";
            //tbxJmeno.Text = "";
            tbxJmeno.IsEnabled = false;
            lblHvezda1.Visibility = Visibility.Hidden;

            lblVystrahaPrijmeni.Content = "";
            //tbxPrijmeni.Text = "";
            tbxPrijmeni.IsEnabled = false;
            lblHvezda2.Visibility = Visibility.Hidden;

            lblVystrahaLogin.Content = "";
            //tbxLogin.Text = "";
            tbxLogin.IsEnabled = false;
            lblHvezda3.Visibility = Visibility.Hidden;

            //heslo_1
            pbxHeslo.Password = "";
            pbxHeslo.Visibility = Visibility.Hidden;
            pbxHeslo.IsEnabled = false;
            lblHeslo.Content = "Heslo";
            lblHeslo.Visibility = Visibility.Hidden;
            lblHeslo.IsEnabled = false;
            lblHvezda4.Visibility = Visibility.Hidden;
            //heslo_2
            pbxHesloDva.Password = "";
            pbxHesloDva.Visibility = Visibility.Hidden;
            lblHesloDva.Visibility = Visibility.Hidden;
            lblHvezda5.Visibility = Visibility.Hidden;

            btnZmenitHeslo.Visibility = Visibility.Hidden;

            cbxBlokace.IsEnabled = false;
            cbxRole.IsEnabled = false;


            // označím první řádek
            DGevidenceUzivatel.SelectedIndex = 0;
        }

        // pomocná metoda pro nastavení ---NOVÝ ---
        private void nastavZobrazeniNovy()
        {
            zapVypTlacitka(false, false, false, false, false, false, true, true);
            DGevidenceUzivatel.IsEnabled = false;

            btnUzivatelUloz.Visibility = Visibility;
            btnUzivatelUloz.Content = "Uložit nový";

            btnUzivatelUloz2.Visibility = Visibility;
            btnUzivatelUloz2.Content = "Uložit nový";

            btnUzivatelStorno.Visibility = Visibility;
            btnUzivatelStorno2.Visibility = Visibility;

            tbxJmeno.IsEnabled = true;
            tbxJmeno.Background = lehceOranzova;
            lblHvezda1.Visibility = Visibility;

            tbxPrijmeni.IsEnabled = true;
            lblHvezda2.Visibility = Visibility;

            tbxLogin.IsEnabled = true;
            lblHvezda3.Visibility = Visibility;

            lblHeslo.Visibility = Visibility;
            pbxHeslo.Visibility = Visibility;
            pbxHeslo.IsEnabled = true;
            lblHvezda4.Visibility = Visibility;
            

            cbxBlokace.IsEnabled = true;
            cbxRole.IsEnabled = true;
        }

        // pomocná metoda pro nastavení ---UPRAVIT ---
        private void nastavZobrazeniUpravit()
        {
            zapVypTlacitka(false, false, false, false, false, false, true, true);
            DGevidenceUzivatel.IsEnabled = false;

            btnUzivatelUloz.Visibility = Visibility;
            btnUzivatelUloz.Content = "Uložit změny";

            btnUzivatelUloz2.Visibility = Visibility;
            btnUzivatelUloz2.Content = "Uložit změny";

            btnUzivatelStorno.Visibility = Visibility;
            btnUzivatelStorno2.Visibility = Visibility;

            if (prihlasenySpravce)
            {
                tbxJmeno.IsEnabled = true;
                tbxPrijmeni.IsEnabled = true;
                tbxLogin.IsEnabled = true;
                cbxRole.IsEnabled = true;
                cbxBlokace.IsEnabled = true;
            }
            if (prihlasenyLogin == tbxLogin.Text)
            {
                lblHeslo.Visibility = Visibility;
                
                pbxHeslo.Visibility = Visibility;
                btnZmenitHeslo.Visibility = Visibility;
            }
            

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
