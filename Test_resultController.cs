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
    public class Test_resultController : Controller
    {
        private OlympEntities1 db = new OlympEntities1();

        // GET: Test_result
        public ActionResult Index()
        {
            return View(db.Test_result.ToList());
        }

        // GET: Test_result/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test_result test_result = db.Test_result.Find(id);
            if (test_result == null)
            {
                return HttpNotFound();
            }
            return View(test_result);
        }

        // GET: Test_result/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Test_result/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_result,student_id,test_id,completion_date,score")] Test_result test_result)
        {
            if (ModelState.IsValid)
            {
                db.Test_result.Add(test_result);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(test_result);
        }

        // GET: Test_result/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test_result test_result = db.Test_result.Find(id);
            if (test_result == null)
            {
                return HttpNotFound();
            }
            return View(test_result);
        }

        // POST: Test_result/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_result,student_id,test_id,completion_date,score")] Test_result test_result)
        {
            if (ModelState.IsValid)
            {
                db.Entry(test_result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(test_result);
        }

        // GET: Test_result/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test_result test_result = db.Test_result.Find(id);
            if (test_result == null)
            {
                return HttpNotFound();
            }
            return View(test_result);
        }

        // POST: Test_result/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Test_result test_result = db.Test_result.Find(id);
            db.Test_result.Remove(test_result);
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
