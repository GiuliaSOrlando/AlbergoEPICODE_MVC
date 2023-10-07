using AlbergoEPICODE_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AlbergoEPICODE_MVC.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(Dipendente dipendente)
        {
            if (ModelState.IsValid)
            {
                if (dipendente.InserisciNuovoDipendente())
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Errore durante la registrazione.");
                }
            }

            return View(dipendente);
        }
        public ActionResult Login(string username, string password)
        {
            Dipendente dipendente = new Dipendente
            {
                Username = username,
                Password = password
            };

            if (dipendente.Login())
            {
                FormsAuthentication.SetAuthCookie(username, false);

                return RedirectToAction("Index", "Home"); 
            }
            else
            {
                ModelState.AddModelError("", "Username o password non corrette");
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}