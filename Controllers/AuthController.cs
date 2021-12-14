using iStay.Db.DBOperation;
using iStay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using iStay.DB;
using System.Data.Entity;

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
                    int id = vm.AddUserData(model);
                    Session["userID"] = id;
                    ModelState.Clear();
                    istay_dbEntities db = new istay_dbEntities();
                    var data = db.Users.Where(x => x.ID == id).FirstOrDefault();
                    if(data.User_type==1)
                    {
                        //for simple user do code here
                        TempData["registered"] = "You have registered successfull";
                        return RedirectToAction("Login");
                    }
                    else if(data.User_type==2)
                    {
                        //for hostel owner do code here
                        TempData["registered"] = "You have registered successfull Please register your hostel";
                        return RedirectToAction("Register","Hostel");
                    }
                }
                return View();
                
            }
            catch(Exception ex)
            {
                ViewBag.exception = ex;
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
                    Session["userName"] = obj.User_Name.ToString();

                    return RedirectToAction("MyHostel", "HostelAdmin");
                }
                else
                    ModelState.AddModelError("", "Invalid Username Or Password");
            }
            return View();
        }

        public ActionResult MyProfile()
        {
            int id = Convert.ToInt32(Session["userID"]);
            istay_dbEntities db = new istay_dbEntities();
            var data = db.Users.Where(x => x.ID == id).FirstOrDefault();
            Users user = new Users()
            {
                ID=data.ID,
                User_Name=data.User_Name,
                User_Email=data.User_Email,
                User_Contact=data.User_Contact,
                User_Pass=data.User_Pass,
                User_Gender=data.User_Gender
            };
            return View(user);
        }

        // GET: Auth/Edit/5
        public ActionResult EditProfile(int id)
        {
            using(var db =new istay_dbEntities())
            {
                var data = db.Users.Where(x => x.ID == id).FirstOrDefault();
                Users user = new Users()
                {
                    ID = data.ID,
                    User_Name = data.User_Name,
                    User_Email = data.User_Email,
                    User_Contact = data.User_Contact,
                    User_Pass = data.User_Pass,
                    User_Gender = data.User_Gender
                };
                return View("MyProfile");
            }
        }

        // POST: Auth/Edit/5
        [HttpPost]
        public ActionResult EditProfile(int id, FormCollection collection, Users user)
        {
            try
            {
                using (var db = new istay_dbEntities())
                {
                    var data = db.Users.Where(x => x.ID == id).FirstOrDefault();
                    data.User_Name = data.User_Name;
                    data.User_Email = data.User_Email;
                    data.User_Contact = data.User_Contact;
                    data.User_Gender = data.User_Gender;

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();

                    return View(user);
                }

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

    }
}
