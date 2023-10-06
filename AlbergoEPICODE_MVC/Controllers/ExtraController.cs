using AlbergoEPICODE_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoEPICODE_MVC.Controllers
{
    public class ExtraController : Controller
    {
        public ActionResult List()
        {
            Servizio servizio = new Servizio();
            List<Servizio> listaServizi = servizio.ListaExtra();
            return View(listaServizi);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Servizio servizio)
        {
                 servizio.DataServizio = DateTime.Now; 

                if (servizio.CreaNuovoServizio())
                {
                   
                    return RedirectToAction("List"); 
                }
                else
                {
                    ModelState.AddModelError("", "Si è verificato un errore durante la creazione del servizio.");
                }

            return View(servizio);
        }

        public ActionResult Edit(int id)
        {
            Servizio servizio = new Servizio();
            Servizio servizioDaModificare = servizio.RecuperaServizio(id);

            if (servizioDaModificare != null)
            {
                return View(servizioDaModificare);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(Servizio servizio)
        {
                Servizio servizioDamodificare = new Servizio().RecuperaServizio(servizio.IdServizio);

                if (servizioDamodificare != null)
                {
                servizioDamodificare.DataServizio = servizio.DataServizio;
                servizioDamodificare.Descrizione = servizio.Descrizione;
                servizioDamodificare.Quantita = servizio.Quantita;
                servizioDamodificare.Prezzo = servizio.Prezzo;

                    if (servizioDamodificare.ModificaServizio(servizio.IdServizio, servizio.DataServizio, servizio.Descrizione, servizio.Quantita, servizio.Prezzo))
                    {
                        return RedirectToAction("List");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Si è verificato un errore durante la modifica del servizio.");
                    }
                }
                else
                {
                    return HttpNotFound();
                }

            return View(servizio);
        }

        public ActionResult Delete(int id)
        {
            Servizio servizio = new Servizio();
            Servizio servizioDaEliminare = servizio.RecuperaServizio(id);

            if (servizioDaEliminare != null)
            {
                return View(servizioDaEliminare);
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}