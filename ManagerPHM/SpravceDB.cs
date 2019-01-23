using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerPHM
{
    class SpravceDB
    {
        // vytvořím si dataSet (DS) a dataAdaptéry (DA...)
        public LokalniSada DS;
        public LokalniSadaTableAdapters.UcetTableAdapter DAucet;

        // konstruktor
        public SpravceDB()
        {
            DS = new LokalniSada();
            DAucet = new LokalniSadaTableAdapters.UcetTableAdapter();
        }
    }
}
