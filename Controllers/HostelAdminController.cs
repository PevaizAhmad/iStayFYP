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
            if (data != null)
            {
                ViewBag.id = data.ID;
                return View(data);

            }
            else
            {
                TempData["msg"] = "You have not registered your hostel yet please register it here!";
                return RedirectToAction("Register", "Hostel");
            }
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

        public ActionResult AddService(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddService(int id,iStay.Models.HostelService model)
        {
            if (ModelState.IsValid)
            {
                iStay.DB.HostelService service = new iStay.DB.HostelService()
                {
                    Service = model.Service,
                    Price = model.Price,
                    HostelID = id
                };
                db.HostelServices.Add(service);
                db.SaveChanges();
                ViewBag.id = service.HostelID;
            }
            return View();
        }
        public ActionResult Services(int id)
        {
            var data = db.HostelServices.Where(x => x.HostelID == id).ToList();
            ViewBag.id = id;
            return View(data);
        }
        public ActionResult AddRoomType(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddRoomType(int id,iStay.Models.RoomType model )
        {
            if (ModelState.IsValid)
            {
                iStay.DB.HostelRoomType type = new HostelRoomType()
                {
                    Price = model.Price,
                    RoomType = model.Type,
                    HostelID = id
                };
                db.HostelRoomTypes.Add(type);
                db.SaveChanges();
                ViewBag.id = type.HostelID;
            }
            return View();
        }
        public ActionResult RoomTypes(int id)
        {
            var data = db.HostelRoomTypes.Where(x => x.HostelID == id).ToList();
            ViewBag.id = id;
            return View(data);
        }
    }
}