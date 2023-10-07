using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AlbergoEPICODE_MVC.Models
{
    public class Dipendente
    {
        // Proprietà
        public int IdDipendente { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Metodi

        private string DbString;
        private SqlConnection conn;
        public Dipendente()
        {
            DbString = ConfigurationManager.ConnectionStrings["AlbergoDB"].ConnectionString;
            conn = new SqlConnection(DbString);
        }

        public bool InserisciNuovoDipendente()
        {
            try
            {
                conn.Open();
                SqlCommand inserisciDipendente = new SqlCommand(
                    "INSERT INTO Dipendenti (Username, Password) VALUES (@Username, @Password)", conn);

                inserisciDipendente.Parameters.AddWithValue("@Username", Username);
                inserisciDipendente.Parameters.AddWithValue("@Password", Password);

                int dipendenteInserito = inserisciDipendente.ExecuteNonQuery();
                return dipendenteInserito > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public bool AggiornaDipendente()
        {
            try
            {
                conn.Open();
                SqlCommand updateDipendente = new SqlCommand(
                    "UPDATE Dipendenti SET Password = @Password WHERE IdDipendente = @IdDipendente", conn);

                updateDipendente.Parameters.AddWithValue("@IdDipendente", IdDipendente);
                updateDipendente.Parameters.AddWithValue("@Password", Password);

                int dipendenteAggiornato = updateDipendente.ExecuteNonQuery();
                return dipendenteAggiornato > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public bool RimuoviDipendenteDaDB()
        {
            try
            {
                conn.Open();
                SqlCommand rimuoviDipendenteDB = new SqlCommand(
                    "DELETE FROM Dipendenti WHERE IdDipendente = @IdDipendente", conn);

                rimuoviDipendenteDB.Parameters.AddWithValue("@IdDipendente", IdDipendente);

                int dipendenteEliminato = rimuoviDipendenteDB.ExecuteNonQuery();
                return dipendenteEliminato > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public bool Login()
        {
            try
            {
                conn.Open();
                SqlCommand loginCommand = new SqlCommand(
                    "SELECT COUNT(*) FROM Dipendenti WHERE Username = @Username AND Password = @Password", conn);

                loginCommand.Parameters.AddWithValue("@Username", Username);
                loginCommand.Parameters.AddWithValue("@Password", Password);

                int count = (int)loginCommand.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

    }
}