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
        public DataTable dtUcet { get; set; }
        public int odpoved { get; set; }
         
        public SpravceUcet()
        {
            dtUcet = new DataTable();
        }

        public int overUzivatele(DB nazevDB, string jmeno, string heslo)
        {
            odpoved = nazevDB.overUzivatele("SELECT COUNT(1) FROM Ucet WHERE Jmeno=@Jmeno AND Heslo=@Heslo", dtUcet, jmeno, heslo);
            return odpoved;
        }
    }
}
