﻿using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for oknoUzivatele.xaml
    /// </summary>
    public partial class oknoUzivatele : Window
    {
        DB db;
        SpravceUcet sprUcet;
        DataView dv;

        SolidColorBrush zelena, oranzova, cervena;

        public oknoUzivatele(DB db, SpravceUcet sprUcet)
        {
            this.db = db;
            this.sprUcet = sprUcet;
            // připravím si štetce pro pozadí
            zelena = new SolidColorBrush(Colors.LightGreen);
            oranzova = new SolidColorBrush(Colors.Orange);
            cervena = new SolidColorBrush(Colors.Red);

            InitializeComponent();
            sprUcet.nactiVsechnyUzivatele(db);
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
                tbxJmeno.Text = (string)drv.Row["Jmeno"];
                tbxPrijmeni.Text = (string)drv.Row["Prijmeni"];
                tbxLogin.Text = (string)drv.Row["Login"];
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
        }

        

        private void tbxProVsechny_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbxZadane = (TextBox)sender;
            if (tbxZadane.Text != "")
            {
                if(tbxZadane.Text.Length > 2)
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
                else
                {
                    switch (tbxZadane.Name)
                    {
                        case "tbxJmeno":
                            lblVystrahaJmeno.Content = "zadej jméno s minimální délkou tři znaky !";
                            break;
                        case "tbxPrijmeni":
                            lblVystrahaPrijmeni.Content = "zadej příjmení s minimální délkou tři znaky !";
                            break;
                        case "tbxLogin":
                            lblVystrahaLogin.Content = "zadej login s minimální délkou tři znaky !";
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

        private void btnUzivatelUloz_Click(object sender, RoutedEventArgs e)
        {
            if(tbxJmeno.Text.Length > 2 && tbxPrijmeni.Text.Length > 2 && tbxLogin.Text.Length > 2)
            {
                string jmeno = tbxJmeno.Text;
                string prijmeni = tbxPrijmeni.Text;
                string login = tbxLogin.Text;
                // 1. načtu znovu všechny uživatele
                sprUcet.nactiVsechnyUzivatele(db);
                // 2. vytvořím si dv
                dv = new DataView(sprUcet.dtVsichniUzivatele);
                // vynuluji Row filtry
                dv.RowFilter = string.Empty;
                // 3. filtruju dv dle jmena a příjmení
                dv.RowFilter = string.Format("Jmeno LIKE '{0}' AND Prijmeni LIKE '{1}'", tbxJmeno.Text, tbxPrijmeni.Text);
                if (dv.Count > 0)
                    MessageBox.Show("Uživatel s tímto jménem a příjmením již existuje ");
                else
                {
                    // vynuluji Row filtry
                    dv.RowFilter = string.Empty;
                    // filtruju dv dle loginu
                    dv.RowFilter = string.Format("Login LIKE '{0}'", tbxLogin.Text);
                    if(dv.Count > 0)
                        MessageBox.Show("Uživatel s loginem již existuje ");
                    else
                    {
                        string sul = sprUcet.vytvorSul(10);
                        string hash = sprUcet.vytvorHash(pbxHeslo.Password, sul);
                        // tet uz začnu ukládat do DB .....>
                        MessageBox.Show(sul );
                    }
                }
                    
            }
            else
            {
                if (tbxJmeno.Text == "")
                    lblVystrahaJmeno.Content = "Zadej své jméno !";
                if (tbxPrijmeni.Text == "")
                    lblVystrahaPrijmeni.Content = "Zadej své příjmení !";
                if (tbxLogin.Text == "")
                    lblVystrahaLogin.Content = "Zadej svůj login !";

            }
        }
        
        private void btnUzivatelStorno_Click(object sender, RoutedEventArgs e)
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

        }



        // pokud se změní text v password_1
        private void pbxHesla_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pbxHeslo.Password.Length > 2)
            {
                lblVystrahaTriZnaky.Visibility = Visibility.Hidden;
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
                lblVystrahaTriZnaky.Visibility = Visibility;
                pbxHeslo.Foreground = cervena;
                lblHesloDva.Visibility = Visibility.Hidden;
                pbxHesloDva.Visibility = Visibility.Hidden;
                pbxHesloDva.Password = "";
                pbxHesloDva.Foreground = cervena;
                lblHvezda5.Visibility = Visibility.Hidden;
                lblVystrahaHeslaSeNeshoduji.Visibility = Visibility.Hidden;

            }
            if(pbxHeslo.Password.Length < 1)
            {
                lblVystrahaTriZnaky.Visibility = Visibility.Hidden;
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
            if(pbxHesloDva.Password != "")
            {
                lblVystrahaHeslaSeNeshoduji.Visibility = Visibility;
                string h1 = pbxHeslo.Password;
                string h2 = pbxHesloDva.Password;
                if(h1 == h2)
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

    }
}
