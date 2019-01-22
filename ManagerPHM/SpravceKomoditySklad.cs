using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerPHM
{
     class SpravceKomoditySklad
    {
        //-- proměnné
        public DataTable dtKomoditySklad { get; set; }
        //public DataTable dtKonkretniVyber { get; set; }


        //-- konstruktor
        public SpravceKomoditySklad()
        {
            dtKomoditySklad = new DataTable();
        }


        //-- metoda nactení vseho s pohledu "KomoditySklad"
        public void nactiVse(DB nazevDB)
        {
            dtKomoditySklad = nazevDB.nactiJednuTabulku("SELECT * FROM KomoditySklad", dtKomoditySklad);
        }
       
        
        
        //   --------------------------------
        //   ---- TOTO ZATÍM NEPOUŽÍVÁM ----

        //-- nati po Šarzich
        public void nactiVsePoSarzich(DB nazevDB)
        {
            dtKomoditySklad = nazevDB.nactiJednuTabulku(
                "SELECT min(IdKomodita) as IdKomodita, KcmCislo, NazevISL, ObchodniNazev, Sarze, SUM(MnozstviDisponabilni) AS MnozstviDisponabilni, SUM(MnozstviBlokovane) AS MnozstviBlokovane, SUM(MnozstviSklad) AS MnozstviSklad, MIN(Expirace) AS Expirace FROM KomoditySklad GROUP BY KcmCislo, NazevISL, ObchodniNazev, Sarze", dtKomoditySklad);
        }
        //-- nacti po Obchodní Název
        public void nactiVsePoObchNaz(DB nazevDB)
        {
            dtKomoditySklad = nazevDB.nactiJednuTabulku(
                "SELECT min(IdKomodita), KcmCislo, NazevISL, ObchodniNazev, ' ...' AS Sarze, SUM(MnozstviDisponabilni) AS MnozstviDisponabilni, SUM(MnozstviBlokovane) AS MnozstviBlokovane, SUM(MnozstviSklad) AS MnozstviSklad, MIN(Expirace) AS Expirace FROM KomoditySklad GROUP BY KcmCislo, NazevISL, ObchodniNazev"
                , dtKomoditySklad);
        }
        //-- metoda nacti po KCM
        public void nactiVsePoKCM(DB nazevDB)
        {
            dtKomoditySklad = nazevDB.nactiJednuTabulku(
                "SELECT min(IdKomodita), KcmCislo, NazevISL, ' ...' AS ObchodniNazev, ' ...' AS Sarze, SUM(MnozstviDisponabilni) AS MnozstviDisponabilni, SUM(MnozstviBlokovane) AS MnozstviBlokovane, SUM(MnozstviSklad) AS MnozstviSklad, MIN(Expirace) AS Expirace FROM KomoditySklad GROUP BY KcmCislo, NazevISL "
                , dtKomoditySklad);
        }
        //-- metoda nacti KCM
        public void nactiKCM(DB nazevDB)
        {
            dtKomoditySklad = nazevDB.nactiJednuTabulku(
                "", dtKomoditySklad);
        }
        public void nactiKonkretniDleVyberu(DB nazevDB, string vybraneId)
        {

            //dtKonkretniVyber = nazevDB.nactiJednuTabulkuDB("SELECT * FROM Konkretni_tech WHERE Id = 1");
        }
    }
}
