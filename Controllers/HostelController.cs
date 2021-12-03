using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iStay.Db.DBOperation;
using iStay.DB;
using iStay.Models;

namespace iStayHostelFinder.Controllers
{
    public class HostelController : Controller
    {
        private istay_dbEntities db = new istay_dbEntities();
        static string status = "pending";

        // GET: Hostel
        public ActionResult Index()
        {
            return View();
        } 
      
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(iStay.Models.HostelInfo model, string[] institute)
        {
            try
            {
                int user_id = Convert.ToInt32(Session["userID"]);
                if (ModelState.IsValid && institute != null)
                {
                    iStay.DB.HostelInfo info = new iStay.DB.HostelInfo()
                    {
                        hostel_name = model.hostel_name,
                        hostel_email = model.hostel_email,
                        hostel_address = model.hostel_address,
                        city_name = model.city_name,
                        type = model.type,
                        refregrator = model.refregrator,
                        AC = model.AC,
                        kitchen = model.kitchen,
                        laundry = model.laundry,
                        Mess = model.Mess,
                        wifi = model.wifi,
                        rooms = Convert.ToInt32(model.rooms),
                        rent = model.rent,
                        status = status,
                        User_ID = user_id
                    };
                    db.HostelInfoes.Add(info);
                    db.SaveChanges();
                    var hostelID = info.ID;

                    //add images related to the hostel

                    foreach (HttpPostedFileBase file in model.files)
                        {
                        //Checking file is available to save.
                            if (file != null && file.ContentLength > 0)
                            {
                            string newFileName = "";
                            string path = HttpContext.Server.MapPath(@"~/uploadfiles");

                            bool exists = System.IO.Directory.Exists(path);

                            if (!exists)
                                System.IO.Directory.CreateDirectory(path);

                            string extension = Path.GetExtension(file.FileName);
                            newFileName = Guid.NewGuid() + extension;
                            string filePath = Path.Combine(path, newFileName);
                            file.SaveAs(filePath);

                            Image img = new Image()
                            {
                                hostel_id = hostelID,
                                img = newFileName
                            };
                            db.Images.Add(img);
                            db.SaveChanges();
                        }

                    }

                    //add list of institutes related to hostel
                    foreach (var i in institute)
                    {
                        Hostel_Institues _Institues = new Hostel_Institues()
                        {
                            Hostel_ID = hostelID,
                            Institutes_Name = i
                        };
                        db.Hostel_Institues.Add(_Institues);
                        db.SaveChanges();
                    }
                    ViewBag.msg = "Hostel has been registered successfully";
                    ViewBag.status = "success";

                }

                return View();
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.ToString();
                ViewBag.status = "Alert";
                return View();
            }
        }

        public ActionResult MyHostel(int? id)
        {

            var data = db.HostelInfoes.Where(x => x.ID == id).Include(x => x.Hostel_Institues).Include(x => x.Images).FirstOrDefault();
            return View(data);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var data = db.HostelInfoes.Where(x => x.ID == id).Include(x => x.Hostel_Institues).Include(x => x.Images).FirstOrDefault();
            Session["imgPath"] = data.Images;
            if (data == null)
            {
                return HttpNotFound();
            }
            else
            {
                HostelViewModel hostel = new HostelViewModel();
                hostel.HostelInfo = getHostelInfo(data);
                //hostel.InstituteList = data.Hostel_Institues.ToList();
                return View(hostel);

            }
            

        }
        
        [HttpPost]
        public ActionResult Edit(int id, HostelViewModel model, HttpPostedFileBase[] files, string[] institute)
        {
            try
            {
                iStayRepository vm = new iStayRepository();
                var data = db.HostelInfoes.Where(x => x.ID == id).FirstOrDefault();
                if (data != null)
                {
                    int user_id = Convert.ToInt32(Session["userID"]);

                    data.hostel_name = model.HostelInfo.hostel_name;
                    data.hostel_email = model.HostelInfo.hostel_email;
                    data.hostel_address = model.HostelInfo.hostel_address;
                    data.city_name = model.HostelInfo.city_name;
                    data.type = model.HostelInfo.type;
                    data.refregrator = model.HostelInfo.refregrator;
                    data.AC = model.HostelInfo.AC;
                    data.kitchen = model.HostelInfo.kitchen;
                    data.laundry = model.HostelInfo.laundry;
                    data.Mess = model.HostelInfo.Mess;
                    data.wifi = model.HostelInfo.wifi;
                    data.rooms = Convert.ToInt32(model.HostelInfo.rooms);
                    data.rent = model.HostelInfo.rent;
                    data.status = status;
                    data.User_ID = user_id;

                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    //add images related to the hostel

                    if(files!=null)
                    {
                        foreach (HttpPostedFileBase file in files)
                        {
                            //Checking file is available to save.
                            if (file != null && file.ContentLength > 0)
                            {
                                string newFileName = "";
                                string path = HttpContext.Server.MapPath(@"~/uploadfiles");

                                bool exists = System.IO.Directory.Exists(path);

                                if (!exists)
                                    System.IO.Directory.CreateDirectory(path);

                                string extension = Path.GetExtension(file.FileName);
                                newFileName = Guid.NewGuid() + extension;
                                string filePath = Path.Combine(path, newFileName);
                                file.SaveAs(filePath);

                                Image img = new Image()
                                {
                                    hostel_id = id,
                                    img = newFileName
                                };
                                db.Images.Add(img);
                                db.SaveChanges();
                            }

                        }
                    }
                    
                    if(institute !=null)
                    {
                        //add list of institutes related to hostel
                        foreach (var i in institute)
                        {
                            Hostel_Institues _Institues = new Hostel_Institues()
                            {
                                Hostel_ID = id,
                                Institutes_Name = i
                            };
                            db.Hostel_Institues.Add(_Institues);
                            db.SaveChanges();
                        }
                    }
                    
                    TempData["msg"] = "Hostel has been registered successfully";
                    TempData["status"] = "Success";
                    return RedirectToAction("MyHostel","HostelAdmin");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to Update Data. Try again, and if the problem persists, see your system administrator.");
                    return View();
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("",ex);
                return View();
            }
        }
        public iStay.Models.HostelInfo getHostelInfo(iStay.DB.HostelInfo hostelInfo)
        {
            iStay.Models.HostelInfo info = new iStay.Models.HostelInfo()
            {
                hostel_name = hostelInfo.hostel_name,
                hostel_email = hostelInfo.hostel_email,
                hostel_address = hostelInfo.hostel_address,
                city_name = hostelInfo.city_name,
                status = hostelInfo.status,
                AC = hostelInfo.AC,
                wifi = hostelInfo.wifi,
                kitchen = hostelInfo.kitchen,
                laundry = hostelInfo.laundry,
                Mess = hostelInfo.Mess,
                refregrator = hostelInfo.refregrator,
                rent = hostelInfo.rent,
                rooms = Convert.ToString(hostelInfo.rooms),
                type = hostelInfo.type,
            };
            return info;
        }
    }
}