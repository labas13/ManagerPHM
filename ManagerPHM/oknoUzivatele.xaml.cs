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
        private string puvodniJmeno, puvodniPrijmeni, puvodniLogin;
        int indexPuvodniBlokace;
        int indexPuvodniRole;
        
       

        SolidColorBrush zelena, oranzova, cervena, cerna, bila, svetleZluta;

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
            zelena = new SolidColorBrush(Colors.Green);
            oranzova = new SolidColorBrush(Colors.Orange);
            cervena = new SolidColorBrush(Colors.Red);
            cerna = new SolidColorBrush(Colors.Black);
            bila = new SolidColorBrush(Colors.White);
            svetleZluta = new SolidColorBrush(Colors.LightGoldenrodYellow);
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
                        // -- pokud upravuji uživatele
                        if(btnUzivatelUloz.Content.ToString() == "Uložit změny")
                        {
                            switch (tbxZadane.Name)
                            {
                                case "tbxJmeno":
                                    {
                                       if (tbxJmeno.Text == puvodniJmeno)
                                        {
                                            tbxJmeno.Foreground = cerna;
                                            if (tbxPrijmeni.Foreground != zelena)
                                            {
                                                btnUzivatelUloz.IsEnabled = false;
                                            }                                           
                                        }
                                        else
                                        {
                                            btnUzivatelUloz.IsEnabled = true;
                                            tbxJmeno.Foreground = zelena;
                                        }
                                    }
                                    break;
                                case "tbxPrijmeni":
                                    {
                                        if (tbxPrijmeni.Text == puvodniPrijmeni)
                                        {
                                            tbxPrijmeni.Foreground = cerna;
                                            if(tbxJmeno.Foreground != zelena)
                                                btnUzivatelUloz.IsEnabled = false;                                            
                                        }
                                        else
                                        {
                                            btnUzivatelUloz.IsEnabled = true;
                                            tbxPrijmeni.Foreground = zelena;
                                        }
                                    }
                                    break;
                            }
                        }
                        //-----------------------------
                            
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
                indexPuvodniBlokace = cbxBlokace.SelectedIndex;
                indexPuvodniRole = cbxRole.SelectedIndex;
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
                if ((string)btnUzivatelUloz.Content == "Uložit nový")
                {
                    //přepnu se na první záložku
                    tclZalozky.SelectedIndex = 0;

                    string jmeno = tbxJmeno.Text;
                    string prijmeni = tbxPrijmeni.Text;
                    string login = tbxLogin.Text;
                    int role = cbxRole.SelectedIndex + 1;
                    bool blokace;
                    if (cbxBlokace.SelectedIndex == 0)
                        blokace = false;
                    else
                        blokace = true;                    
                    
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
                        // tet vytvořím nový řádek do tabulky DG .....>
                        DataRow novyUzivatel = sprUcet.dtVsichniUzivatele.NewRow();
                        novyUzivatel["Jmeno"] = jmeno;
                        novyUzivatel["Prijmeni"] = prijmeni;
                        novyUzivatel["Login"] = login;
                        novyUzivatel["Role"] = role;
                        novyUzivatel["Blokace"] = blokace;
                        novyUzivatel["Heslo"] = hash;
                        novyUzivatel["Sul"] = sul;
                        sprUcet.dtVsichniUzivatele.Rows.Add(novyUzivatel);
                        // tet se pokusím uložit do DB .....>
                        bool ulozeniSeZdarilo = sprUcet.ulozUzivatele(db, jmeno, prijmeni, login, role, blokace, hash, sul);
                        if (ulozeniSeZdarilo)
                        {
                            // vše proběhlo OK
                            sprUcet.nactiVsechnyUzivatele(db);
                            nastavZobrazeniStart();
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
                    string noveJmeno = tbxJmeno.Text;
                    string novePrijmeni = tbxPrijmeni.Text;
                    string novaSul = sprUcet.vytvorSul(10);
                    string noveHeslo = sprUcet.vytvorHash(pbxHeslo.Password, novaSul);
                    int novaRole = cbxRole.SelectedIndex + 1;
                    bool novaBlokace;
                    if (cbxBlokace.SelectedIndex == 0)
                        novaBlokace = false;
                    else
                        novaBlokace = true;


                    sprUcet.upravUzivatele(db, noveJmeno, novePrijmeni, puvodniLogin, novaRole, novaBlokace, noveHeslo, novaSul);
                }


            }
            else
            {
                tclZalozky.SelectedIndex = 0; // přepnu se na první záložku
                //zablokuji okno a zablikám(upozorním)
                btnUzivatelStorno.IsEnabled = false;
                btnUzivatelUloz.IsEnabled = false;
                System.Media.SystemSounds.Beep.Play();
                zablikejVystrahy(lblVystrahaJmeno);

                if (tbxJmeno.Text == "")
                {
                    lblVystrahaJmeno.Content = "Zadej své jméno !";                    
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
           // tbxJmeno.Focus();
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
                    btnUzivatelUloz2.IsEnabled = true;
                    if(prihlasenySpravce)
                        zalozkaOpravneni.IsEnabled = true;
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

        private void btnZmenitHeslo_Click(object sender, RoutedEventArgs e)
        {
            //lblHeslo.IsEnabled = true;
            lblHeslo.Content = "Nové heslo";
            pbxHeslo.IsEnabled = true;
            pbxHeslo.Background = svetleZluta;
            btnZmenitHeslo.IsEnabled = false;
        }
        // --- ZABLIKEJ ---
        private void zablikejVystrahy(Label lblZadany)
        {
            //Label lbl = lblZadany;
            DT1 = new DispatcherTimer();
            DT1.Interval = TimeSpan.FromSeconds(0.1);
            DT1.Tick += DT1_Tick1;
            DT1.Start();

        }


        

        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbiZmenen = (ComboBox)sender;
            // pokud provádím změny
            if (btnUzivatelUloz.Content.ToString() == "Uložit změny")
            {
                
                if (indexPuvodniRole == cbxRole.SelectedIndex && indexPuvodniBlokace == cbxBlokace.SelectedIndex && puvodniJmeno == tbxJmeno.Text && puvodniPrijmeni == tbxPrijmeni.Text)
                {
                    btnUzivatelUloz.IsEnabled = false;
                    btnUzivatelUloz2.IsEnabled = false;
                    cbxBlokace.Foreground = cerna;
                    cbxRole.Foreground = cerna;

                    obarviVnitrniNabitkuCBX();

                }
                else
                {
                    switch (cmbiZmenen.Name)
                    {
                        case "cbxBlokace":
                            {
                                if (cbxBlokace.SelectedIndex != indexPuvodniBlokace)
                                {
                                    cbxBlokace.Foreground = zelena;
                                    obarviVnitrniNabitkuCBX();
                                }
                                else
                                {
                                    cbxBlokace.Foreground = cerna;
                                    obarviVnitrniNabitkuCBX();
                                }
}
                            break;
                        case "cbxRole":
                            {
                                if (cbxRole.SelectedIndex != indexPuvodniRole)
                                {
                                    cbxRole.Foreground = zelena;
                                    obarviVnitrniNabitkuCBX();
                                }
                                else
                                {
                                    cbxRole.Foreground = cerna;
                                    obarviVnitrniNabitkuCBX();
                                }
                            }
                            break;
                    }
                    btnUzivatelUloz.IsEnabled = true;
                    btnUzivatelUloz2.IsEnabled = true;
                }
            }
        }

        private void obarviVnitrniNabitkuCBX()
        {
            // vybarvým vnitřní nabýdku ROLE
            for (int i = 0; i < cbxRole.Items.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)cbxRole.Items[i];
                cbi.Foreground = zelena;
            }
            ComboBoxItem cbxiRolePuvodni = (ComboBoxItem)cbxRole.Items[indexPuvodniRole];
            cbxiRolePuvodni.Foreground = cerna;

            // vybarvým vnitřní nabýdku BLOKACE
            for (int i = 0; i < cbxBlokace.Items.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)cbxBlokace.Items[i];
                cbi.Foreground = zelena;
            }
            ComboBoxItem cbxiBlokace = (ComboBoxItem)cbxBlokace.Items[indexPuvodniBlokace];
            cbxiBlokace.Foreground = cerna;
        }

        private bool schovana = false;
        private int pocet = 6;
        private void DT1_Tick1(object sender, EventArgs e)
        {

            if (pocet != 0)
            {
                if (schovana)
                {                    
                    schovana = false;
                    if(lblVystrahaJmeno.Content.ToString() != "")
                    {
                        tbxJmeno.Background = svetleZluta;
                        lblVystrahaJmeno.Visibility = Visibility;
                    }
                    if (lblVystrahaPrijmeni.Content.ToString() != "")
                    {
                        tbxPrijmeni.Background = svetleZluta;
                        lblVystrahaPrijmeni.Visibility = Visibility;
                    }
                    if (lblVystrahaLogin.Content.ToString() != "")
                    {
                        tbxLogin.Background = svetleZluta;
                        lblVystrahaLogin.Visibility = Visibility;
                    }

                }
                else
                {
                    
                    schovana = true;
                    if (lblVystrahaJmeno.Content.ToString() != "")
                    {
                        tbxJmeno.Background = bila;
                        lblVystrahaJmeno.Visibility = Visibility.Hidden;
                    }
                    if (lblVystrahaPrijmeni.Content.ToString() != "")
                    {
                        tbxPrijmeni.Background = bila;
                        lblVystrahaPrijmeni.Visibility = Visibility.Hidden;
                    }
                    if (lblVystrahaLogin.Content.ToString() != "")
                    {
                        tbxLogin.Background = bila;
                        lblVystrahaLogin.Visibility = Visibility.Hidden;
                    }

                }
                pocet--;
            }
            else
            {
                DT1.Stop();
                // odblokuji tlačítka ULOZ/STORNO
                btnUzivatelUloz.IsEnabled = true;
                btnUzivatelStorno.IsEnabled = true;
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
                zalozkaOpravneni.IsEnabled = true;
            }                
            else
                zapVypTlacitka(false, false, false, false, false, false, false, false);


            btnUzivatelUloz.Content = "";
            btnUzivatelUloz.Visibility = Visibility.Hidden;
            btnUzivatelUloz2.Visibility = Visibility.Hidden;
            btnUzivatelStorno.Visibility = Visibility.Hidden;
            btnUzivatelStorno2.Visibility = Visibility.Hidden;

            DGevidenceUzivatel.IsEnabled = true;

            lblVystrahaJmeno.Content = "";
            tbxJmeno.Background = bila;
            tbxJmeno.Foreground = cerna;
            tbxJmeno.IsEnabled = false;
            lblHvezda1.Visibility = Visibility.Hidden;

            lblVystrahaPrijmeni.Content = "";
            tbxPrijmeni.Background = bila;
            tbxPrijmeni.Foreground = cerna;
            tbxPrijmeni.IsEnabled = false;
            lblHvezda2.Visibility = Visibility.Hidden;

            lblVystrahaLogin.Content = "";
            tbxLogin.Background = bila;
            tbxLogin.IsEnabled = false;
            lblHvezda3.Visibility = Visibility.Hidden;

            //heslo_1
            pbxHeslo.Password = "";
            pbxHeslo.Background = bila;
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
            btnZmenitHeslo.IsEnabled = true;

            cbxBlokace.IsEnabled = false;
            cbxBlokace.Foreground = cerna;
            cbxRole.IsEnabled = false;
            cbxRole.Foreground = cerna;
            // vybarvým vnitřní nabýdku ROLE
            for (int i = 0; i < cbxRole.Items.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)cbxRole.Items[i];
                cbi.Foreground = cerna;
            }            
            // vybarvým vnitřní nabýdku BLOKACE
            for (int i = 0; i < cbxBlokace.Items.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)cbxBlokace.Items[i];
                cbi.Foreground = cerna;
            }
            

            // označím první řádek
            DGevidenceUzivatel.SelectedIndex = -1;
            DGevidenceUzivatel.SelectedIndex = 0;
            //Keyboard.Focus(DGevidenceUzivatel);

            //pokus o obarvení řádků s blokovaným Uživatelem
           //DataGridRow dg = (DataGridRow)DGevidenceUzivatel.Items[2] ;
            //dgr.Background = zelena;
            
        }

        // pomocná metoda pro nastavení ---NOVÝ ---
        private void nastavZobrazeniNovy()
        {
            
            tclZalozky.SelectedIndex = 0; // přepnu se na první záložku
            zalozkaOpravneni.IsEnabled = false; // zablokuji druhou záložku
            DGevidenceUzivatel.SelectedIndex = -1; // VYNULUJE texboxi
            DGevidenceUzivatel.IsEnabled = false; // zablokuji DG


           // Keyboard.ClearFocus();
           // Keyboard.Focus(tclZalozky);

            zapVypTlacitka(false, false, false, false, false, false, true, true);
            

            btnUzivatelUloz.Visibility = Visibility;
            btnUzivatelUloz.Content = "Uložit nový";

            btnUzivatelUloz2.Visibility = Visibility;
            btnUzivatelUloz2.Content = "Uložit nový";

            btnUzivatelStorno.Visibility = Visibility;
            btnUzivatelStorno2.Visibility = Visibility;

            tbxJmeno.IsEnabled = true;
            tbxJmeno.Background = svetleZluta;
            lblHvezda1.Visibility = Visibility;
            tbxPrijmeni.Focus();
            
            //tbxJmeno.Select(0,0);
            //tbxJmeno.Focus();

            //Keyboard.Focus(tbxJmeno);


            tbxPrijmeni.IsEnabled = true;
            tbxPrijmeni.Background = svetleZluta;
            lblHvezda2.Visibility = Visibility;

            tbxLogin.IsEnabled = true;
            tbxLogin.Background = svetleZluta;
            lblHvezda3.Visibility = Visibility;

            lblHeslo.Visibility = Visibility;
            pbxHeslo.Visibility = Visibility;
            pbxHeslo.IsEnabled = true;
            pbxHeslo.Background = svetleZluta;
            lblHvezda4.Visibility = Visibility;
            

            cbxBlokace.IsEnabled = true;
            cbxBlokace.SelectedIndex = 0;
            cbxRole.IsEnabled = true;
            cbxRole.SelectedIndex = 3;
            //zalozkaInfoUzivatel.Focus();
            tbxJmeno.Focus();
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
                tbxJmeno.Background = svetleZluta;
                tbxPrijmeni.IsEnabled = true;
                tbxPrijmeni.Background = svetleZluta;
                

                cbxRole.IsEnabled = true;                
                cbxBlokace.IsEnabled = true;
                obarviVnitrniNabitkuCBX();

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
