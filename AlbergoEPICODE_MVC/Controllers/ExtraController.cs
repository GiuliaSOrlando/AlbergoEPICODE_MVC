using AlbergoEPICODE_MVC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoEPICODE_MVC.Controllers
{
    [Authorize]
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
        public ActionResult Edit(Servizio servizio, string formattedData)
        {
            if (ModelState.IsValid)
            {
                if (DateTime.TryParseExact(formattedData, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataServizio))
                {
                    if (servizio.ModificaServizio(servizio.IdServizio, dataServizio, servizio.Descrizione, servizio.Quantita, servizio.Prezzo))
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
                    ModelState.AddModelError("", "DataServizio non è nel formato corretto.");
                }
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