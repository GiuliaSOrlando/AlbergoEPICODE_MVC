using AlbergoEPICODE_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoEPICODE_MVC.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        public ActionResult List()
        {
            Camera camera = new Camera();
            List<Camera> listaCamere = camera.ListaCamere();
            return View(listaCamere);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Camera camera)
        {
                if (camera.InserisciNuovaCamera())
                {
                    return RedirectToAction("List");
                }

            return View(camera);
        }

        public ActionResult Edit(int id)
        {
            Camera camera = new Camera();
            camera = camera.OttieniID(id);

            if (camera == null)
            {
                return HttpNotFound();
            }

            return View(camera);
        }

        [HttpPost]
        public ActionResult Edit(Camera camera)
        {
            if (ModelState.IsValid)
            {
                if (camera.AggiornaCamera(camera.IdCamera, camera.Descrizione, camera.IsDoppia))
                {
                    return RedirectToAction("List");
                }
            }

            return View(camera);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Camera camera = new Camera();
            if (camera.EliminaCamera(id))
            {
                return RedirectToAction("List");
            }
            return HttpNotFound();
        }
    }
}