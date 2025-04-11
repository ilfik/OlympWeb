using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OlympWeb.Models;

namespace OlympWeb.Controllers
{
    public class TestsController : Controller
    {
        private OlympEntities1 db = new OlympEntities1();

        // GET: Tests
        public ActionResult Index()
        {
            return View(db.Tests.ToList());
        }

        // GET: Tests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tests tests = db.Tests.Find(id);
            if (tests == null)
            {
                return HttpNotFound();
            }
            return View(tests);
        }

        // GET: Tests/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tests/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_test,title,description,created_by,created_at,time_limit")] Tests tests)
        {
            if (ModelState.IsValid)
            {
                db.Tests.Add(tests);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tests);
        }

        // GET: Tests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tests tests = db.Tests.Find(id);
            if (tests == null)
            {
                return HttpNotFound();
            }
            return View(tests);
        }

        // POST: Tests/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_test,title,description,created_by,created_at,time_limit")] Tests tests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tests);
        }

        // GET: Tests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tests tests = db.Tests.Find(id);
            if (tests == null)
            {
                return HttpNotFound();
            }
            return View(tests);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tests tests = db.Tests.Find(id);
            db.Tests.Remove(tests);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
