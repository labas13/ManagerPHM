using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerPHM
{
    class SpravceUcet
    {
        public DataTable dtUzivatel{ get; set; }
        public int odpoved { get; set; }
         
        public SpravceUcet()
        {
            dtUzivatel = new DataTable();
        }

        public void overUzivatele(DB nazevDB, string jmeno, string heslo)
        {
            dtUzivatel = nazevDB.overUzivatele("SELECT * FROM Ucet WHERE Jmeno=@Jmeno AND Heslo=@Heslo", dtUzivatel, jmeno, heslo);            
        }
    }
}
