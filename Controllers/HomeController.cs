using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iStay.Models;
using iStay.DB;
using System.Data.Entity;

namespace iStayFYP.Controllers
{
    public class HomeController : Controller
    {
        private istay_dbEntities db = new istay_dbEntities();
        public ActionResult Index()
        {
            var data = db.HostelInfoes.OrderByDescending(x=>x.User_ID).Include(x => x.Hostel_Institues).Include(x => x.Images).ToList();
            return View(data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CreateCity()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCity(CityModel model)
        {
            istay_dbEntities db = new istay_dbEntities();
            City city = new City()
            {
                City_Name=model.City_Name
            };
            db.Cities.Add(city);
            db.SaveChanges();
            ModelState.Clear();
            return View();
        }
        
        public ActionResult CreateInstitute()
        {
            istay_dbEntities db = new istay_dbEntities();
            ViewBag.city = new SelectList(db.Cities, "ID", "City_Name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateInstitute(InstituteModel model)
        {
            istay_dbEntities db = new istay_dbEntities();
           Institute institute = new Institute()
            {
                City_ID = model.City_ID,
                Institutes_Name=model.Institutes_Name
            };
            db.Institutes.Add(institute);
            db.SaveChanges();
            ModelState.Clear();
            ViewBag.city = new SelectList(db.Cities, "ID", "City_Name");
            return View();
        }

        public ActionResult Hostel_Detail(int id)
        {
            var data = db.HostelInfoes.Where(x => x.ID == id).Include(x => x.Hostel_Institues).Include(x => x.Images).FirstOrDefault();
            var usrData = db.Users.Where(x => x.ID == data.User_ID).FirstOrDefault();
            ViewBag.Contact = usrData.User_Contact;
            // similar hostels
            var data1 = db.HostelInfoes.Where(x => x.city_name == data.city_name && x.type==data.type && x.ID != id).Include(x => x.Hostel_Institues).Include(x => x.Images).ToList();
            ViewBag.SimilarHostels = data1;
            
            return View(data);
        }

        public ActionResult Hostel_Booking(int? hostelID)
        {
            return View();
        }
    }

}