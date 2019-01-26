﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace ManagerPHM
{
    public class DB
    {
        //--privátní proměnná 
        private string pripojovaciRetez;

        //--konstruktor
        public DB(string pripojRetez)
        {
            pripojovaciRetez = pripojRetez;
        }

        public bool smazUzivatele(string sqlDotaz, DataTable dt, string login)
        {
            using (SqlConnection spojeni = new SqlConnection(pripojovaciRetez))
            {
                SqlCommand prikaz = new SqlCommand(sqlDotaz, spojeni);
                prikaz.CommandType = CommandType.Text;
                prikaz.Parameters.AddWithValue("@Login", login);
                SqlDataAdapter da = new SqlDataAdapter();
                da.DeleteCommand = prikaz;
                //-- 3. mažu z DB
                if (da.Update(dt) > 0)
                {
                    //smazání se podařilo
                    return true;
                }
                else
                {
                    //smazání se nezdařilo
                    return false;
                }
            }
        }

        public bool ulozTabulku(string sqlDotaz, DataTable dt, string jmeno, string prijmeni, string login, string heslo, string sul)
        {
            using (SqlConnection spojeni = new SqlConnection(pripojovaciRetez))
            {
                SqlCommand prikaz = new SqlCommand(sqlDotaz, spojeni);
                prikaz.CommandType = CommandType.Text;
                prikaz.Parameters.AddWithValue("@Jmeno", jmeno);
                prikaz.Parameters.AddWithValue("@Prijmeni", prijmeni);
                prikaz.Parameters.AddWithValue("@Login", login);
                prikaz.Parameters.AddWithValue("@Heslo", heslo);
                prikaz.Parameters.AddWithValue("@Sul", sul);
                SqlDataAdapter da = new SqlDataAdapter();
                da.InsertCommand = prikaz;

                //-- 3. uložím do DB
                if (da.Update(dt) > 0)
                {
                    //uložení se podařilo
                    return true;
                }
                else
                {
                    //uložení se nezdařilo
                    return false;
                }

            }
        }
        //--metoda pro nactení dat z DB do DT
        public DataTable nactiJednuTabulku(string sqlDotaz, DataTable dt)
        {
            using (SqlConnection spojeni = new SqlConnection(pripojovaciRetez))
            {
                //-- 1. připravíme si příkaz pro získání požadovaných dat z databáze ...
                SqlCommand prikaz = new SqlCommand();
                prikaz.Connection = spojeni;
                prikaz.CommandText = sqlDotaz;

                //-- 2. datový adaptér (pro  tabulku ) ...
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = prikaz;

                //-- 3. zbývá jen naplnit daty místní "DataTable dt" ...
                dt.Clear();
                dataAdapter.Fill(dt);
                //-- 4. vrátit dt jako výsledek
                return dt;
            }
        }
        //-- Metoda pro ověření UŽIVATELE
        public DataTable najdiUzivatele(string sqlDotaz, DataTable dt, string login)
        {
            using (SqlConnection spojeni = new SqlConnection(pripojovaciRetez))
            {
                SqlCommand prikaz = new SqlCommand(sqlDotaz, spojeni);
                prikaz.CommandType = CommandType.Text;
                prikaz.Parameters.AddWithValue("@Login", login);
                //prikaz.Parameters.AddWithValue("@Heslo", heslo);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = prikaz;
                dt.Clear();
                da.Fill(dt);
                return dt;

            }
                   
        }

        //-- DOPLNÍM METODY --> NA MAZÁNÍ, VKLÁDÁNÍ ...
    }
}
