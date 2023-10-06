using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AlbergoEPICODE_MVC.Models
{
    public class Cliente
    {
        // Proprietà
        public int IdCliente { get; set; }
        [Required]
        public string CF { get; set; }
        [Required]
        public string Cognome { get; set; }
        [Required]
        public string Nome { get; set; }
        public string Citta { get; set; }
        public string Provincia { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Cellulare { get; set; }

        List<Cliente> listaClienti = new List<Cliente>();

        // Metodi

        private string DbString;
        private SqlConnection conn;
        public Cliente()
        {
            DbString = ConfigurationManager.ConnectionStrings["AlbergoDB"].ConnectionString;
            conn = new SqlConnection(DbString);
        }

        public List<Cliente> ListaClienti()
        {
            try
            {
                conn.Open();
                SqlCommand visualizzaListaClienti = new SqlCommand("SELECT * FROM Clienti", conn);
                SqlDataReader readerLista = visualizzaListaClienti.ExecuteReader();

                while (readerLista.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        IdCliente = (int)readerLista["IdCliente"],
                        CF = readerLista["CF"].ToString(),
                        Cognome = readerLista["Cognome"].ToString(),
                        Nome = readerLista["Nome"].ToString(),
                        Citta = readerLista["Citta"].ToString(),
                        Provincia = readerLista["Provincia"].ToString(),
                        Email = readerLista["Email"].ToString(),
                        Telefono = readerLista["Telefono"].ToString(),
                        Cellulare = readerLista["Cellulare"].ToString()
                    };

                    listaClienti.Add(cliente);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }

            return listaClienti;
        }

        public bool InserisciNuovoCliente()
        {
            try
            {
                conn.Open();
                SqlCommand inserisciCliente = new SqlCommand(
                    "INSERT INTO Clienti (CF, Cognome, Nome," +
                    "Citta, Provincia, Email, Telefono, Cellulare)" +
                    "VALUES (@CF, @Cognome, @Nome, @Citta, @Provincia," +
                    "@Email, @Telefono, @Cellulare)", conn);

                inserisciCliente.Parameters.AddWithValue("@CF", CF);
                inserisciCliente.Parameters.AddWithValue("@Cognome", Cognome);
                inserisciCliente.Parameters.AddWithValue("@Nome", Nome);
                inserisciCliente.Parameters.AddWithValue("@Citta", Citta);
                inserisciCliente.Parameters.AddWithValue("@Provincia", Provincia);
                inserisciCliente.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email );
                inserisciCliente.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(Telefono) ? (object)DBNull.Value : Telefono);
                inserisciCliente.Parameters.AddWithValue("@Cellulare", string.IsNullOrEmpty(Cellulare) ? (object)DBNull.Value : Cellulare);

                int clienteInserito = inserisciCliente.ExecuteNonQuery();
                return clienteInserito > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public Cliente OttieniID(int id)
        {
            try
            {
                conn.Open();
                SqlCommand dettagliCliente = new SqlCommand("SELECT * FROM Clienti WHERE IdCliente = @Id", conn);
                dettagliCliente.Parameters.AddWithValue("@Id", id);
                SqlDataReader readerDettagliCliente = dettagliCliente.ExecuteReader();

                if (readerDettagliCliente.Read())
                {
                    Cliente cliente = new Cliente
                    {
                        IdCliente = (int)readerDettagliCliente["IdCliente"],
                        CF = readerDettagliCliente["CF"].ToString(),
                        Cognome = readerDettagliCliente["Cognome"].ToString(),
                        Nome = readerDettagliCliente["Nome"].ToString(),
                        Citta = readerDettagliCliente["Citta"].ToString(),
                        Provincia = readerDettagliCliente["Provincia"].ToString(),
                        Email = readerDettagliCliente["Email"].ToString(),
                        Telefono = readerDettagliCliente["Telefono"].ToString(),
                        Cellulare = readerDettagliCliente["Cellulare"].ToString()
                    };
                    return cliente;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) {
                return null;
            }
            finally { conn.Close(); }
        }

        public bool AggiornaCliente(string CF, string nuovoCognome, string nuovoNome, string nuovaCitta, string nuovaProvincia, string nuovaEmail, string nuovoTelefono, string nuovoCellulare)
        {
            try
            {
                conn.Open();
                SqlCommand aggiornaCliente = new SqlCommand(
                    "UPDATE Clienti SET Cognome = @NuovoCognome, Nome = @NuovoNome, " +
                    "Citta = @NuovaCitta, Provincia = @NuovaProvincia, Email = @NuovaEmail, " +
                    "Telefono = @NuovoTelefono, Cellulare = @NuovoCellulare " +
                    "WHERE CF = @CF", conn);

                aggiornaCliente.Parameters.AddWithValue("@CF", CF);
                aggiornaCliente.Parameters.AddWithValue("@NuovoCognome", nuovoCognome);
                aggiornaCliente.Parameters.AddWithValue("@NuovoNome", nuovoNome);
                aggiornaCliente.Parameters.AddWithValue("@NuovaCitta", nuovaCitta);
                aggiornaCliente.Parameters.AddWithValue("@NuovaProvincia", nuovaProvincia);
                aggiornaCliente.Parameters.AddWithValue("@NuovaEmail", (object)nuovaEmail ?? DBNull.Value );
                aggiornaCliente.Parameters.AddWithValue("@NuovoTelefono", (object)nuovoTelefono ?? DBNull.Value);
                aggiornaCliente.Parameters.AddWithValue("@NuovoCellulare", (object)nuovoCellulare ?? DBNull.Value);

                int clienteAggiornato = aggiornaCliente.ExecuteNonQuery();
                return clienteAggiornato > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public bool EliminaCliente(int id)
        {
            try
            {
                conn.Open();
                SqlCommand eliminaCliente = new SqlCommand("DELETE FROM Clienti WHERE IdCliente = @Id", conn);
                eliminaCliente.Parameters.AddWithValue("@Id", id);

                int clienteEliminato = eliminaCliente.ExecuteNonQuery();
                return clienteEliminato > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

    }
}