using AlbergoEPICODE_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoEPICODE_MVC.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        public ActionResult List()
        {
            Cliente cliente = new Cliente();
            List<Cliente> listaClienti = cliente.ListaClienti();
            return View(listaClienti);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
                if (!string.IsNullOrEmpty(cliente.Email) || !string.IsNullOrEmpty(cliente.Telefono) || !string.IsNullOrEmpty(cliente.Cellulare))
                    {
                        if (cliente.InserisciNuovoCliente())
                        {
                        return RedirectToAction("Create");
                        }
                    }
                    else
                    { ModelState.AddModelError(string.Empty, "Compila almeno una delle informazioni di contatto");
                    }
                
                return View(cliente);
        }

        public ActionResult Edit(int id)
        {
            Cliente cliente = new Cliente();
            cliente = cliente.OttieniID(id);

            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public ActionResult Edit(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                if (cliente.AggiornaCliente(cliente.CF, cliente.Cognome, cliente.Nome, cliente.Citta, cliente.Provincia, cliente.Email, cliente.Telefono, cliente.Cellulare))
                {
                    return RedirectToAction("List");
                }

            }
            return View(cliente);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Cliente cliente = new Cliente();
            if (cliente.EliminaCliente(id))
            {
                return RedirectToAction("List");
            }
            return HttpNotFound();
        }
    }
}