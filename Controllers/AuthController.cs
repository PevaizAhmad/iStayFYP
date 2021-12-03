using iStay.Db.DBOperation;
using iStay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using iStay.DB;

namespace iStayHostelFinder.Controllers
{
    public class AuthController : Controller
    {
        
        iStayRepository repository = null;
        public AuthController()
        {
            repository = new iStayRepository();
        }
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        // GET: Auth/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Auth/Create
        public ActionResult Create()
        {
            istay_dbEntities db = new istay_dbEntities();
            ViewBag.UserType = new SelectList(db.UserTypes, "ID", "Type");
            return View();
        }

        // POST: Auth/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, Users model)
        {
            try
            {
                AuthViewModel vm = new AuthViewModel();
                if (ModelState.IsValid)
                {
                    int type = vm.AddUserData(model);
                    ModelState.Clear();
                    TempData["registered"] = "You have registered successfull please login";
                    return RedirectToAction("Login");
                }

                return View();
                
            }
            catch
            {
                return View();
            }
        }

        // GET: Auth/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Auth/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Auth/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Auth/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Users user)
        {
            using (var db = new istay_dbEntities())
            {
                var obj = db.Users.Where(a => a.User_Name.Equals(user.User_Name) && a.User_Pass.Equals(user.User_Pass)).FirstOrDefault();
                if (obj != null)
                {
                    
                    FormsAuthentication.SetAuthCookie(user.User_Name, false);
                    Session["userID"] = obj.ID.ToString();
                    
                    return RedirectToAction("MyHostel", "HostelAdmin");
                }
                else
                ModelState.AddModelError("", "Invalid Username Or Password");
            }
            return View();
        }
    }
}
