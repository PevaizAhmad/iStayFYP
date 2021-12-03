using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iStay.DB;
using iStay.Models;

namespace iStayFYP.Controllers
{
    public class HostelAdminController : Controller
    {
        

        private istay_dbEntities db = new istay_dbEntities();
        // GET: HostelAdmin
        public ActionResult Index()
        {
            return View();
        }// GET: HostelAdmin

        public ActionResult MyHostel(int? id)
        {
            int user_id = Convert.ToInt32(Session["userID"]);
            var data = db.HostelInfoes.Where(x => x.User_ID == user_id).Include(x => x.Hostel_Institues).Include(x => x.Images).FirstOrDefault();
            return View(data);
        }
        public ActionResult HostelGuests()
        {
            return View();
        }// GET: HostelAdmin
        public ActionResult GuestsApproval()
        {
            return View();
        }// GET: HostelAdmin
        public ActionResult FeeDefaltuerGuests()
        {
            return View();
        }
    }
}