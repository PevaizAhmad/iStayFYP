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
        static string SIGNATURE_IMAGE = "profile";
        static string EXTRA_SLIDER_IMAGES = "slider";

        // GET: Hostel
        public ActionResult Index()
        {
            return View();
        } 
      
        public ActionResult Register()
        {
            List<City> CityList = db.Cities.ToList();
            ViewBag.CityList = new SelectList(CityList, "ID", "City_Name");
            return View();
        }
        [HttpPost]
        public ActionResult Register(iStay.Models.HostelInfo model, string[] institute)
        {
            try
            {
                int userid = Convert.ToInt32(Session["userID"]);
                model.status = status;
                if (ModelState.IsValid && institute != null)
                {
                    iStay.DB.HostelInfo info = new iStay.DB.HostelInfo()
                    {
                        hostel_name = model.hostel_name,
                        hostel_email = model.hostel_email,
                        hostel_address = model.hostel_address,
                        type = model.type,
                        cityID=model.CityID,
                        status = model.status,
                        User_ID = userid,
                        Description=model.Desc
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
                                img = newFileName,
                                Image_type = EXTRA_SLIDER_IMAGES
                            };
                            db.Images.Add(img);
                            db.SaveChanges();
                        }

                    }

                    //Add signature image
                    //Checking file is available to save.
                    if (model.profile != null && model.profile.ContentLength > 0)
                    {
                        string newFileName = "";
                        string path = HttpContext.Server.MapPath(@"~/uploadfiles");

                        bool exists = System.IO.Directory.Exists(path);

                        if (!exists)
                            System.IO.Directory.CreateDirectory(path);

                        string extension = Path.GetExtension(model.profile.FileName);
                        newFileName = Guid.NewGuid() + extension;
                        string filePath = Path.Combine(path, newFileName);
                        model.profile.SaveAs(filePath);

                        Image img = new Image()
                        {
                            hostel_id = hostelID,
                            img = newFileName,
                            Image_type = SIGNATURE_IMAGE
                        };
                        db.Images.Add(img);
                        db.SaveChanges();
                    }
                    //add list of institutes related to hostel
                    foreach (var i in institute)
                    {
                        Hostel_Institues _Institues = new Hostel_Institues()
                        {
                            Hostel_ID = hostelID,
                            Institutes_ID = Convert.ToInt32(i)
                        };
                        db.Hostel_Institues.Add(_Institues);
                        db.SaveChanges();
                    }
                    TempData["hostel_register"] = "Hostel has been registered successfully";
                    TempData["sts"] = "success";
                    return RedirectToAction("MyHostel","HostelAdmin");
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.erroe = errors;
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.ToString();
                ViewBag.status = "Alert";
                return View();
            }
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
                    data.type = model.HostelInfo.type;
                    data.cityID = model.HostelInfo.CityID;
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
                                Institutes_ID = Convert.ToInt32(i)
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
                status = hostelInfo.status,
                CityID= (int)hostelInfo.cityID,
                type = hostelInfo.type,
            };
            return info;
        }

        public ActionResult AddService()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AddService(iStay.Models.HostelService model,int id)
        {
            if (ModelState.IsValid)
            {
                iStay.DB.HostelService service = new iStay.DB.HostelService()
                {
                    Service = model.Service,
                    Price = model.Price,
                    HostelID=id
                };
                db.HostelServices.Add(service);
                db.SaveChanges();
            }
            return View();
        }
        public ActionResult Services(int id) 
        {
            var data = db.HostelServices.Where(x => x.HostelID == id).ToList();
            return View(data);
        }
        public ActionResult AddRoomType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRoomType(iStay.Models.RoomType model, int id)
        {
            if (ModelState.IsValid)
            {
                iStay.DB.HostelRoomType type = new HostelRoomType()
                {
                    Price = model.Price,
                    RoomType = model.Type,
                    HostelID=id
                };
                db.HostelRoomTypes.Add(type);
                db.SaveChanges();
            }
            return View();
        }
        public ActionResult RoomTypes()
        {
            return View();
        }
        public JsonResult GetInstitutes(int CityID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Institute> institutesList = db.Institutes.Where(x => x.City_ID == CityID).ToList();
            return Json(institutesList, JsonRequestBehavior.AllowGet);
        }
    }
}