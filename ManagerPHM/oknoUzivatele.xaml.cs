using System;
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

        public oknoUzivatele(DB db, SpravceUcet sprUcet)
        {
            this.db = db;
            this.sprUcet = sprUcet;
            InitializeComponent();
            sprUcet.nactiVsechnyUzivatele(db);
            dv = new DataView(sprUcet.dtVsichniUzivatele);
            dv.Sort = "Prijmeni,Jmeno";
            DGevidenceUzivatel.ItemsSource = null;
            DGevidenceUzivatel.ItemsSource = dv;


        }
    }
}
