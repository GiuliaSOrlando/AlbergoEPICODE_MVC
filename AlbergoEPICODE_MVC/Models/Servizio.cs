using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AlbergoEPICODE_MVC.Models
{
    public class Servizio
    {
        // Proprietà
        public int IdServizio { get; set; }
        [Required]
        public int NumeroPrenotazione { get; set; }
        public DateTime DataServizio { get; set; }
        public string Descrizione { get; set; }
        public int Quantita { get; set; }
        [Required]
        public decimal Prezzo { get; set; }

        public static List<string> ListaServizi = new List<string>
        {"Colazione in camera","Bevande e cibo nel mini bar","Internet","Letto aggiuntivo","Culla"};

        // Metodi

        private string DbString;
        private SqlConnection conn;

        public Servizio()
        {
            DbString = ConfigurationManager.ConnectionStrings["AlbergoDB"].ConnectionString;
            conn = new SqlConnection(DbString);
        }

        public List<Servizio> ListaExtra()
        {
            List<Servizio> listaServizi = new List<Servizio>();

            try
            {
                conn.Open();
                SqlCommand visualizzaListaServizi = new SqlCommand("SELECT * FROM Servizi", conn);
                SqlDataReader readerLista = visualizzaListaServizi.ExecuteReader();

                while (readerLista.Read())
                {
                    Servizio servizio = new Servizio
                    {
                        IdServizio = (int)readerLista["IdServizio"],
                        NumeroPrenotazione = (int)readerLista["NumeroPrenotazione"],
                        DataServizio = (DateTime)readerLista["DataServizio"],
                        Descrizione = readerLista["Descrizione"].ToString(),
                        Quantita = (int)readerLista["Quantita"],
                        Prezzo = (decimal)readerLista["Prezzo"]
                    };

                    listaServizi.Add(servizio);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }

            return listaServizi;
        }

        public bool CreaNuovoServizio()
        {
            try
            {
                conn.Open();

                SqlCommand inserisciServizio = new SqlCommand(
                    "INSERT INTO Servizi (NumeroPrenotazione, DataServizio, Descrizione, Quantita, Prezzo)" +
                    "VALUES (@NumeroPrenotazione, @DataServizio, @Descrizione, @Quantita, @Prezzo)", conn);

                inserisciServizio.Parameters.AddWithValue("@NumeroPrenotazione", NumeroPrenotazione);
                inserisciServizio.Parameters.AddWithValue("@DataServizio", DataServizio);
                inserisciServizio.Parameters.AddWithValue("@Descrizione", Descrizione);
                inserisciServizio.Parameters.AddWithValue("@Quantita", Quantita);
                inserisciServizio.Parameters.AddWithValue("@Prezzo", Prezzo);

                int servizioInserito = inserisciServizio.ExecuteNonQuery();
                return servizioInserito > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public Servizio RecuperaServizio(int id)
        {
            try
            {
                conn.Open();
                SqlCommand dettagliServizio = new SqlCommand("SELECT * FROM Servizi WHERE IdServizio = @Id", conn);
                dettagliServizio.Parameters.AddWithValue("@Id", id);
                SqlDataReader readerDettagliServizio = dettagliServizio.ExecuteReader();

                if (readerDettagliServizio.Read())
                {
                    Servizio servizio = new Servizio
                    {
                        IdServizio = (int)readerDettagliServizio["IdServizio"],
                        NumeroPrenotazione = (int)readerDettagliServizio["NumeroPrenotazione"],
                        DataServizio = (DateTime)readerDettagliServizio["DataServizio"],
                        Descrizione = readerDettagliServizio["Descrizione"].ToString(),
                        Quantita = (int)readerDettagliServizio["Quantita"],
                        Prezzo = (decimal)readerDettagliServizio["Prezzo"]
                    };
                    return servizio;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally { conn.Close(); }
        }

        public bool ModificaServizio(int id, DateTime nuovoDataServizio, string nuovaDescrizione, int nuovaQuantita, decimal nuovoPrezzo)
        {
            try
            {
                conn.Open();
                SqlCommand modificaServizio = new SqlCommand(
                    "UPDATE Servizi SET DataServizio = @NuovoDataServizio, Descrizione = @NuovaDescrizione, " +
                    "Quantita = @NuovaQuantita, Prezzo = @NuovoPrezzo " +
                    "WHERE IdServizio = @Id", conn);

                modificaServizio.Parameters.AddWithValue("@NuovoDataServizio", nuovoDataServizio);
                modificaServizio.Parameters.AddWithValue("@NuovaDescrizione", nuovaDescrizione);
                modificaServizio.Parameters.AddWithValue("@NuovaQuantita", nuovaQuantita);
                modificaServizio.Parameters.AddWithValue("@NuovoPrezzo", nuovoPrezzo);
                modificaServizio.Parameters.AddWithValue("@Id", id);

                int servizioModificato = modificaServizio.ExecuteNonQuery();
                return servizioModificato > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public bool EliminaServizio(int id)
        {
            try
            {
                conn.Open();
                SqlCommand eliminaServizio = new SqlCommand("DELETE FROM Servizi WHERE IdServizio = @Id", conn);
                eliminaServizio.Parameters.AddWithValue("@Id", id);

                int servizioEliminato = eliminaServizio.ExecuteNonQuery();
                return servizioEliminato > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

    }
}