using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OlympWeb.Models;

namespace OlympWeb.Controllers
{
    public class AccountController : Controller
    {
  
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var db = new OlympEntities1();
            var user = db.Users.FirstOrDefault(u => u.login == username && u.password == password);

            if (user != null)
            {
                Session["UserID"] = user.id_user;
                Session["Username"] = user.login;
                Session["Role"] = user.role;
                return RedirectToAction("Dashboard", "Account");
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();
        }

        // ✅ Добавляем метод Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}