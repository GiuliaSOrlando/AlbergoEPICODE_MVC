using AlbergoEPICODE_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoEPICODE_MVC.Controllers
{
    public class BookingController : Controller
    {
        private void PopolaDropdownClienti()
        {
            Prenotazione prenotazione = new Prenotazione();
            List<SelectListItem> listaClienti = prenotazione.VisualizzaListaClienti();
            ViewBag.listaClienti = listaClienti;
        }
        public ActionResult List()
        {
            List<Prenotazione> listaPrenotazioni = Session["ListaPrenotazioni"] as List<Prenotazione>;

            if (listaPrenotazioni == null)
            {
                Prenotazione prenotazione = new Prenotazione();
                listaPrenotazioni = prenotazione.ListaPrenotazioni();

                Session["ListaPrenotazioni"] = listaPrenotazioni;
            }

            return View(listaPrenotazioni);
        }

        public ActionResult Create()
        {
            PopolaDropdownClienti();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Prenotazione prenotazione)
        {
            PopolaDropdownClienti();
            Camera cameraCorrispondente = new Camera().ListaCamere().FirstOrDefault(camera => camera.Descrizione == prenotazione.TipoCamera && camera.IsDoppia == !prenotazione.IsSingola);
            if (cameraCorrispondente == null)
            {
                ModelState.AddModelError("", "Nessuna camera disponibile corrisponde alla selezione effettuata.");
                return View(prenotazione);
            }

            if (!prenotazione.VerificaDisponibilitaCamera(cameraCorrispondente.IdCamera, prenotazione.DataCheckIn, prenotazione.DataCheckOut))
            {
                ModelState.AddModelError("", "La camera selezionata non è disponibile per le date indicate.");
                return View(prenotazione);
            }

            prenotazione.NumeroCamera = cameraCorrispondente.IdCamera;

            prenotazione.TipoSoggiorno = prenotazione.SelTipoSoggiorno;

            var (tariffaTotale, caparra) = prenotazione.CalcolaTariffa(cameraCorrispondente.Descrizione, prenotazione.DataCheckIn, prenotazione.DataCheckOut, prenotazione.SelTipoSoggiorno, prenotazione.IsSingola);
            prenotazione.Tariffa = tariffaTotale;
            prenotazione.Caparra = caparra;
            if (prenotazione.InserisciNuovaPrenotazione())
            {
                return RedirectToAction("List");
            }
            return View(prenotazione);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            PopolaDropdownClienti();
            Prenotazione prenotazione = new Prenotazione();
            Prenotazione prenotazioneDaModificare = prenotazione.RecuperaPrenotazione(id);

            if (prenotazioneDaModificare != null)
            {
                return View(prenotazioneDaModificare);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(Prenotazione prenotazione)
        {
            PopolaDropdownClienti();
            if (ModelState.IsValid)
            {
                if (prenotazione.VerificaDisponibilitaCamera(prenotazione.NumeroCamera, prenotazione.DataCheckIn, prenotazione.DataCheckOut))
                {
                    if (prenotazione.ModificaPrenotazione(prenotazione.IdPrenotazione, prenotazione.NumeroCliente, prenotazione.NumeroCamera, prenotazione.DataCheckIn, prenotazione.DataCheckOut, prenotazione.TipoSoggiorno))
                    {
                        return RedirectToAction("List");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore durante la modifica della prenotazione.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "La camera non è disponibile per le date specificate.");
                }
            }

            return View(prenotazione);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            Prenotazione prenotazione = new Prenotazione();
            if (prenotazione.EliminaPrenotazione(id))
            {
                return RedirectToAction("List");
            }
            return HttpNotFound();
        }

        public ActionResult Checkout(int id)
        {
            Prenotazione prenotazione = new Prenotazione();
            prenotazione = prenotazione.RecuperaPrenotazione(id);
            List<Servizio> serviziAggiuntivi = prenotazione.ListaExtraPrenotazione(id);
            decimal importoDaSaldare = prenotazione.CalcolaImportoDaSaldare(id);
            bool checkoutEffettuato = prenotazione.EffettuaCheckout(id);

            ViewBag.Prenotazione = prenotazione;
            ViewBag.CheckoutEffettuato = checkoutEffettuato;
            ViewBag.ServiziAggiuntivi = serviziAggiuntivi;
            ViewBag.ImportoDaSaldare = importoDaSaldare;

            if (checkoutEffettuato)
            {

                return View("Checkout");
            }
            else
            {
                return View("List");
            }
        }

    }
}
