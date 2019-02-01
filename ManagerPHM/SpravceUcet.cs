using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerPHM
{
    public class SpravceUcet
    {
        public DataTable dtPrihlasenyUzivatel{ get; set; }
        public DataTable dtVsichniUzivatele { get; set; }
        
        // public int odpoved { get; set; }
         
        public SpravceUcet()
        {
            dtPrihlasenyUzivatel = new DataTable();
            dtVsichniUzivatele = new DataTable();
        }

        // --- NAČTI VŠE ---
        public void nactiVsechnyUzivatele(DB nazevDB)
        {
            dtVsichniUzivatele = nazevDB.nactiJednuTabulku("SELECT * FROM Ucet ", dtVsichniUzivatele);
        }

        // --- SMAŽ vybraného uživatele
        public bool smazUzivatele(DB nazevDB, string login)
        {
            bool odpoved = nazevDB.smazUzivatele("DELETE FROM Ucet WHERE Login = @login", dtVsichniUzivatele, login);
            return odpoved;
        }


        // --- UPRAV vybraného uživatele
        public bool upravUzivatele(DB nazevDB, string jmeno, string prijmeni, string login, int role, bool blokace, string heslo, string sul)
        {
            bool odpoved = true;//nazevDB.upravUzivatele("UPDATE Ucet SET
            return odpoved;
        }

        // --- ULOŽ uživatele
        public bool ulozUzivatele(DB nazevDB, string jmeno, string prijmeni, string login, int role, bool blokace, string heslo, string sul)
        {
            bool odpoved = nazevDB.ulozUzivatele("INSERT INTO Ucet (Jmeno, Prijmeni, Login, Role, Blokace, Heslo, Sul)VALUES(@Jmeno, @Prijmeni, @Login, @Role, @Blokace, @Heslo, @Sul)", dtVsichniUzivatele, jmeno, prijmeni, login, role, blokace, heslo, sul);
            return odpoved;
        }
        public bool overUzivatele(DB nazevDB, string login, string zadaneHeslo)
        {
            dtPrihlasenyUzivatel = nazevDB.najdiUzivatele("SELECT * FROM Ucet WHERE Login=@Login", dtPrihlasenyUzivatel, login);
            //dtUzivatel = nazevDB.overUzivatele("SELECT * FROM Ucet WHERE Jmeno=@Jmeno AND Heslo=@Heslo", dtUzivatel, jmeno, heslo);
            if (dtPrihlasenyUzivatel.Rows.Count > 0)
            {
                string ulozeneHeslo = dtPrihlasenyUzivatel.Rows[0]["Heslo"].ToString();
                string ulozenaSul = dtPrihlasenyUzivatel.Rows[0]["Sul"].ToString();
                string hashZadanehoHesla = vytvorHash(zadaneHeslo, ulozenaSul);
                if (hashZadanehoHesla == ulozeneHeslo)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;                       
        }
        

        // ---------------------------------------------
        // -------     HASH     ------------------------
        // ---------------------------------------------

        // ------------
        // 2. pokus
        public string vytvorHash(string zadaneHeslo, string sul)
        {
            //string sul = vytvorSul(10); // při kontrole nevytvářím tuto sůl jen si ji načtu z DB
            string heshHesla = vytvorSHA256Hash(zadaneHeslo, sul);
            return heshHesla;
        }
        // při kontrole nevytvářím tuto sůl jen si ji načtu z DB
        public string vytvorSul(int velikost)
        {
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[velikost];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
        public string vytvorSHA256Hash(string heslo, string sul)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(heslo + sul);
            System.Security.Cryptography.SHA256Managed sha256hashstring =
                new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha256hashstring.ComputeHash(bytes);
            return byteArrayToHexString(hash);
        }

        public string byteArrayToHexString(Byte[] hash)
        {
            // tento nepoužívám -->  string hashJakoText = BitConverter.ToString(hash); 
            string hashJakoTextBez = BitConverter.ToString(hash).Replace("-", "");
            return hashJakoTextBez;
        }


    }
}
