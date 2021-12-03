using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iStay.Models;
using iStay.Db.DBOperation;

namespace iStayHostelFinder.Controllers
{
    public class UserController : Controller
    {
        iStayRepository repository = null;
        public UserController()
        {
            repository = new iStayRepository();
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HostelViewModel model)
        {
            if (ModelState.IsValid)
            {
                    //repository.AddUserData(model);
                    //ModelState.Clear();
                    //ViewBag.issucces = "Data Added";
            }
            return View();
        }

    }
}