using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AlbergoEPICODE_MVC.Models
{
    public class Prenotazione
    {
        public int IdPrenotazione { get; set; }
        [Required]
        public int NumeroCliente { get; set; }
        [Required]
        public int NumeroCamera { get; set; }
        public DateTime DataPrenotazione { get; set; } = DateTime.Now;
        public int Anno { get; set; } = DateTime.Now.Year;
        public string NumeroProgressivo { get; set; }
        [Required]
        public DateTime DataCheckIn { get; set; }
        [Required]
        public DateTime DataCheckOut { get; set; }
        public decimal Caparra { get; set; }
        public string TipoSoggiorno { get; set; }
        [Required]
        public decimal Tariffa { get; set; }
        [Required]
        public string TipoCamera { get; set; }
        [Required]
        public bool IsSingola { get; set; }

        List<Prenotazione> listaPrenotazioni = new List<Prenotazione>();

        List<Prenotazione> listaPrenotazioniDisattive = new List<Prenotazione>();

        public static List<string> ListaTipoSoggiorno = new List<string>
        {"Mezza pensione","Pensione completa","Pernottamento con prima colazione"};

        public string SelTipoSoggiorno { get; set; }

        // Metodi

        private string DbString;
        private SqlConnection conn;

        public Prenotazione()
        {
            DbString = ConfigurationManager.ConnectionStrings["AlbergoDB"].ConnectionString;
            conn = new SqlConnection(DbString);
        }

        public (decimal tariffaTotale, decimal caparra) CalcolaTariffa(string tipoCamera, DateTime dataCheckIn, DateTime dataCheckOut, string tipoSoggiorno = null, bool isSingola = false)
        {
            decimal tariffaBase = 20;

            switch (TipoCamera)
            {
                case "Standard":
                    tariffaBase = 100;
                    break;
                case "Deluxe":
                    tariffaBase = 150;
                    break;
                case "Superior":
                    tariffaBase = 200;
                    break;
                case "Suite":
                    tariffaBase = 250; 
                    break;
                case "Presidenziale":
                    tariffaBase = 500; 
                    break;
                default:
                    throw new InvalidOperationException("Tipo di stanza non valido.");
            }

            TimeSpan durataSoggiorno = DataCheckOut - DataCheckIn;
            int giorniSoggiorno = (int)durataSoggiorno.TotalDays;

            decimal tariffaTotale = tariffaBase * giorniSoggiorno;

            if (giorniSoggiorno > 7)
            {
                tariffaTotale *= 0.9M; 
            }

            decimal caparra = tariffaTotale * 0.2M;

            if (!string.IsNullOrEmpty(TipoSoggiorno))
            {
                switch (TipoSoggiorno)
                {
                    case "Mezza pensione":
                        tariffaTotale += 50; 
                        break;
                    case "Pensione completa":
                        tariffaTotale += 100; 
                        break;
                    case "Pernottamento con prima colazione":
                        tariffaTotale += 20;
                        break;
                }
            }
            return (tariffaTotale, caparra);
        }

        public bool VerificaDisponibilitaCamera(int idCamera, DateTime dataCheckIn, DateTime dataCheckOut)
{
            try
                {
                conn.Open();

                SqlCommand verificaPrenotazioniEsistenti = new SqlCommand(
                    "SELECT COUNT(*) FROM Prenotazioni " +
                    "WHERE NumeroCamera = @IdCamera " +
                    "AND NOT ((@DataCheckOut < DataCheckIn) OR (@DataCheckIn > DataCheckOut))", conn);

                verificaPrenotazioniEsistenti.Parameters.AddWithValue("@IdCamera", idCamera);
                verificaPrenotazioniEsistenti.Parameters.AddWithValue("@DataCheckIn", dataCheckIn);
                verificaPrenotazioniEsistenti.Parameters.AddWithValue("@DataCheckOut", dataCheckOut);

                int prenotazioniSovrapposte = (int)verificaPrenotazioniEsistenti.ExecuteScalar();

                return prenotazioniSovrapposte == 0;
                }
            catch (Exception ex)
                {
                return false;
                }
            finally { conn.Close(); }
                }

        private string OttieniTipoCameraDaNumero(int numeroCamera)
        {
            string tipoCamera = null;

            try
            {
                conn.Open();

                SqlCommand queryTipoCamera = new SqlCommand(
                    "SELECT Descrizione FROM Camere WHERE NumeroCamera = @NumeroCamera", conn);

                queryTipoCamera.Parameters.AddWithValue("@NumeroCamera", numeroCamera);

                object risultato = queryTipoCamera.ExecuteScalar();

                if (risultato != null && risultato != DBNull.Value)
                {
                    tipoCamera = risultato.ToString();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }

            return tipoCamera;
        }


        public List<Prenotazione> ListaPrenotazioni()
        {
            try
            {
                conn.Open();
                SqlCommand visualizzaListaPrenotazioni = new SqlCommand("SELECT * FROM Prenotazioni", conn);
                SqlDataReader readerLista = visualizzaListaPrenotazioni.ExecuteReader();

                while (readerLista.Read())
                {
                    Prenotazione prenotazione = new Prenotazione
                    {
                        IdPrenotazione = (int)readerLista["IdPrenotazione"],
                        NumeroCliente = (int)readerLista["NumeroCliente"],
                        NumeroCamera = (int)readerLista["NumeroCamera"],
                        DataPrenotazione = (DateTime)readerLista["DataPrenotazione"],
                        Anno = (int)readerLista["Anno"],
                        NumeroProgressivo = readerLista["NumeroProgressivo"].ToString(),
                        DataCheckIn = (DateTime)readerLista["DataCheckIn"],
                        DataCheckOut = (DateTime)readerLista["DataCheckOut"],
                        Caparra = (decimal)readerLista["Caparra"],
                        TipoSoggiorno = readerLista["TipoSoggiorno"].ToString(),
                        Tariffa = (decimal)readerLista["Tariffa"]
                    };

                    listaPrenotazioni.Add(prenotazione);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }

            return listaPrenotazioni;
        }

        public bool InserisciNuovaPrenotazione()
        {
            try
            {
                conn.Open();

                int annoCorrente = Anno;
                int nuovoNP = 1;

                SqlCommand ottieniMaxNP = new SqlCommand("SELECT MAX(NumeroProgressivo) FROM Prenotazioni WHERE Anno = @Anno", conn);
                ottieniMaxNP.Parameters.AddWithValue("@Anno", annoCorrente);

                string MaxNP = ottieniMaxNP.ExecuteScalar() as string;
                if (!string.IsNullOrEmpty(MaxNP))
                {
                    nuovoNP = Convert.ToInt32(MaxNP.Substring(4)) + 1;
                }

                string nuovoNPSt = annoCorrente.ToString() + nuovoNP.ToString("D5");

                SqlCommand inserisciPrenotazione = new SqlCommand(
                    "INSERT INTO Prenotazioni (NumeroCliente, NumeroCamera, DataPrenotazione," +
                    "Anno, NumeroProgressivo, DataCheckIn, DataCheckOut, Caparra, TipoSoggiorno, Tariffa)" +
                    "VALUES (@NumeroCliente, @NumeroCamera, @DataPrenotazione, @Anno, @NumeroProgressivo, " +
                    "@DataCheckIn, @DataCheckOut, @Caparra, @TipoSoggiorno, @Tariffa)", conn);

                inserisciPrenotazione.Parameters.AddWithValue("@NumeroCliente", NumeroCliente);
                inserisciPrenotazione.Parameters.AddWithValue("@NumeroCamera", NumeroCamera);
                inserisciPrenotazione.Parameters.AddWithValue("@DataPrenotazione", DataPrenotazione);
                inserisciPrenotazione.Parameters.AddWithValue("@Anno", Anno);
                inserisciPrenotazione.Parameters.AddWithValue("@NumeroProgressivo", nuovoNPSt);
                inserisciPrenotazione.Parameters.AddWithValue("@DataCheckIn", DataCheckIn);
                inserisciPrenotazione.Parameters.AddWithValue("@DataCheckOut", DataCheckOut);
                inserisciPrenotazione.Parameters.AddWithValue("@Caparra", Caparra);
                inserisciPrenotazione.Parameters.AddWithValue("@TipoSoggiorno", string.IsNullOrEmpty(TipoSoggiorno) ? (object)DBNull.Value : TipoSoggiorno);
                inserisciPrenotazione.Parameters.AddWithValue("@Tariffa", Tariffa);

                int prenotazioneInserita = inserisciPrenotazione.ExecuteNonQuery();
                return prenotazioneInserita > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public Prenotazione RecuperaPrenotazione(int id)
        {
            Prenotazione prenotazione = null;

            try
            {
                conn.Open();

                SqlCommand recuperaPrenotazione = new SqlCommand(
                    "SELECT P.*, C.Descrizione, C.IsDoppia " +
                    "FROM Prenotazioni P " +
                    "INNER JOIN Camere C ON P.NumeroCamera = C.IdCamera " +
                    "WHERE P.IdPrenotazione = @Id", conn);
                recuperaPrenotazione.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = recuperaPrenotazione.ExecuteReader();
                if (reader.Read())
                {
                    prenotazione = new Prenotazione
                    {
                        IdPrenotazione = (int)reader["IdPrenotazione"],
                        NumeroCliente = (int)reader["NumeroCliente"],
                        NumeroCamera = (int)reader["NumeroCamera"],
                        DataPrenotazione = (DateTime)reader["DataPrenotazione"],
                        Anno = (int)reader["Anno"],
                        NumeroProgressivo = reader["NumeroProgressivo"].ToString(),
                        DataCheckIn = (DateTime)reader["DataCheckIn"],
                        DataCheckOut = (DateTime)reader["DataCheckOut"],
                        Caparra = (decimal)reader["Caparra"],
                        Tariffa = (decimal)reader["Tariffa"],
                        TipoCamera = reader["Descrizione"].ToString(),
                        IsSingola = !(bool)reader["IsDoppia"]
                    };
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }

            return prenotazione;
        }


        public bool ModificaPrenotazione(int id, int nuovoNumeroCliente, int nuovoNumeroCamera, DateTime nuovoDataCheckIn, DateTime nuovoDataCheckOut, string nuovoTipoSoggiorno)
        {
            Prenotazione prenotazione = RecuperaPrenotazione(id);

            if (prenotazione != null)

            {
                string nuovoTipoCamera = OttieniTipoCameraDaNumero(nuovoNumeroCamera);
                (decimal nuovaTariffa, decimal nuovaCaparra) = CalcolaTariffa(nuovoTipoCamera, prenotazione.DataCheckIn, prenotazione.DataCheckOut, nuovoTipoSoggiorno);

                    try
                    {
                        conn.Open();

                    SqlCommand modificaPrenotazione = new SqlCommand(
                        "UPDATE Prenotazioni " +
                        "SET NumeroCliente = @NumeroCliente, " +
                        "NumeroCamera = @NumeroCamera, " +
                        "DataCheckIn = @DataCheckIn, " +
                        "DataCheckOut = @DataCheckOut, " +
                        "Caparra = @Caparra, " +
                        "TipoSoggiorno = @TipoSoggiorno, " +
                        "Tariffa = @Tariffa " +
                        "WHERE IdPrenotazione = @Id", conn);

                    modificaPrenotazione.Parameters.AddWithValue("@NumeroCliente", nuovoNumeroCliente);
                    modificaPrenotazione.Parameters.AddWithValue("@NumeroCamera", nuovoNumeroCamera);
                    modificaPrenotazione.Parameters.AddWithValue("@DataCheckIn", nuovoDataCheckIn);
                    modificaPrenotazione.Parameters.AddWithValue("@DataCheckOut", nuovoDataCheckOut);
                    modificaPrenotazione.Parameters.AddWithValue("@Caparra", nuovaCaparra);
                    modificaPrenotazione.Parameters.AddWithValue("@TipoSoggiorno", string.IsNullOrEmpty(nuovoTipoSoggiorno) ? (object)DBNull.Value : nuovoTipoSoggiorno);
                    modificaPrenotazione.Parameters.AddWithValue("@Tariffa", nuovaTariffa);
                    modificaPrenotazione.Parameters.AddWithValue("@Id", id);

                    int prenotazioneModificata = modificaPrenotazione.ExecuteNonQuery();
                    return prenotazioneModificata > 0;
                }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                
            }

            return false;
        }



        public bool EliminaPrenotazione(int id)
        {
            try
            {
                conn.Open();
                SqlCommand eliminaPrenotazione = new SqlCommand("DELETE FROM Prenotazioni WHERE IdPrenotazione = @Id", conn);
                eliminaPrenotazione.Parameters.AddWithValue("@Id", id);

                int prenotazioneEliminata = eliminaPrenotazione.ExecuteNonQuery();
                return prenotazioneEliminata > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public List<Servizio> ListaExtraPrenotazione(int idPrenotazione)
        {
            List<Servizio> listaExtra = new List<Servizio>();

            try
            {
                conn.Open();

                SqlCommand recuperaExtra = new SqlCommand(
                    "SELECT Servizi.* " +
                    "FROM Servizi " + 
                    "WHERE Servizi.NumeroPrenotazione = @IdPrenotazione", conn);

                recuperaExtra.Parameters.AddWithValue("@IdPrenotazione", idPrenotazione);

                SqlDataReader reader = recuperaExtra.ExecuteReader();

                while (reader.Read())
                {
                    Servizio servizio = new Servizio
                    {
                        IdServizio = (int)reader["IdServizioAggiuntivo"],
                        Descrizione = reader["Nome"].ToString(),
                        Quantita = (int)reader["Quantita"],
                        Prezzo = (decimal)reader["Costo"]
                    };

                    listaExtra.Add(servizio);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }

            return listaExtra;
        }

        public decimal CalcolaImportoDaSaldare()
        {
            decimal importoDaSaldare = Tariffa - Caparra;

            importoDaSaldare += ListaExtraPrenotazione(IdPrenotazione).Sum(servizio => servizio.Prezzo * servizio.Quantita);

            return importoDaSaldare;
        }

        public string DettaglioCompleto()
        {
            string dettaglio = $"Numero di stanza: {NumeroCamera}\n";
            dettaglio += $"Periodo: dal {DataCheckIn.ToShortDateString()} al {DataCheckOut.ToShortDateString()}\n";
            dettaglio += $"Tariffa applicata: {Tariffa:C}\n";
            dettaglio += "Servizi aggiuntivi:\n";

            foreach (var servizio in ListaExtraPrenotazione(IdPrenotazione))
            {
                dettaglio += $"{servizio.Descrizione}: {servizio.Prezzo:C}\n";
            }

            dettaglio += $"Importo da saldare: {CalcolaImportoDaSaldare():C}\n";

            return dettaglio;
        }

        public bool EffettuaCheckout(int idPrenotazione)
        { 
            Prenotazione prenotazione = listaPrenotazioni.FirstOrDefault(p => p.IdPrenotazione == idPrenotazione);

            if (prenotazione != null)
            {
                listaPrenotazioni.Remove(prenotazione);

                listaPrenotazioniDisattive.Add(prenotazione);

                return true; 
            }

            return false; 
        }

    }
}