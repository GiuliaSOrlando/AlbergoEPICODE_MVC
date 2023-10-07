using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AlbergoEPICODE_MVC.Models
{
    public class Camera
    {
        // Proprietà
        [Display(Name = "Id camera")]
        public int IdCamera { get; set; }
        public string Descrizione { get; set; }
        [Display(Name = "Camera doppia")]
        public bool IsDoppia { get; set; }

        List<Camera> listaCamere = new List<Camera>();

        public static List<string> ListaTipoCamera = new List<string>
        {"Standard", "Deluxe", "Superior", "Suite", "Presidenziale"};

        public string SelTipoCamera { get; set; }

        // Metodi

        private string DbString;
        private SqlConnection conn;
        public Camera()
        {
            DbString = ConfigurationManager.ConnectionStrings["AlbergoDB"].ConnectionString;
            conn = new SqlConnection(DbString);
        }

        public List<Camera> ListaCamere()
        {
            try
            {
                conn.Open();
                SqlCommand visualizzaListaCamere = new SqlCommand("SELECT * FROM Camere", conn);
                SqlDataReader readerLista = visualizzaListaCamere.ExecuteReader();

                while (readerLista.Read())
                {
                    Camera camera = new Camera
                    {
                        IdCamera = (int)readerLista["IdCamera"],
                        Descrizione = readerLista["Descrizione"].ToString(),
                        IsDoppia = (bool)readerLista["IsDoppia"]
                    };

                    listaCamere.Add(camera);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                conn.Close();
            }

            return listaCamere;
        }

        public bool InserisciNuovaCamera()
        {
            try
            {
                conn.Open();
                SqlCommand inserisciCamera = new SqlCommand(
                    "INSERT INTO Camere (Descrizione, IsDoppia)" +
                    "VALUES (@Descrizione, @IsDoppia)", conn);

                inserisciCamera.Parameters.AddWithValue("@Descrizione", Descrizione);
                inserisciCamera.Parameters.AddWithValue("@IsDoppia", IsDoppia);

                int cameraInserita = inserisciCamera.ExecuteNonQuery();
                return cameraInserita > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public Camera OttieniID(int id)
        {
            try
            {
                conn.Open();
                SqlCommand dettagliCamera = new SqlCommand("SELECT * FROM Camere WHERE IdCamera = @Id", conn);
                dettagliCamera.Parameters.AddWithValue("@Id", id);
                SqlDataReader readerDettagliCamera = dettagliCamera.ExecuteReader();

                if (readerDettagliCamera.Read())
                {
                    Camera camera = new Camera
                    {
                        IdCamera = (int)readerDettagliCamera["IdCamera"],
                        Descrizione = readerDettagliCamera["Descrizione"].ToString(),
                        IsDoppia = (bool)readerDettagliCamera["IsDoppia"]
                    };
                    return camera;
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

        public bool AggiornaCamera(int id, string nuovaDescrizione, bool nuovaIsDoppia)
        {
            try
            {
                conn.Open();
                SqlCommand aggiornaCamera = new SqlCommand(
                    "UPDATE Camere SET Descrizione = @NuovaDescrizione, IsDoppia = @NuovaIsDoppia " +
                    "WHERE IdCamera = @Id", conn);

                aggiornaCamera.Parameters.AddWithValue("@Id", id);
                aggiornaCamera.Parameters.AddWithValue("@NuovaDescrizione", nuovaDescrizione);
                aggiornaCamera.Parameters.AddWithValue("@NuovaIsDoppia", nuovaIsDoppia);

                int cameraAggiornata = aggiornaCamera.ExecuteNonQuery();
                return cameraAggiornata > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }

        public bool EliminaCamera(int id)
        {
            try
            {
                conn.Open();
                SqlCommand eliminaCamera = new SqlCommand("DELETE FROM Camere WHERE IdCamera = @Id", conn);
                eliminaCamera.Parameters.AddWithValue("@Id", id);

                int cameraEliminata = eliminaCamera.ExecuteNonQuery();
                return cameraEliminata > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { conn.Close(); }
        }
    }
}