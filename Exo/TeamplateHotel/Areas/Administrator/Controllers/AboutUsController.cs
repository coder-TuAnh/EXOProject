//using ProjectLibrary.Database;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using TeamplateHotel.Areas.Administrator.EntityModel;

//namespace TeamplateHotel.Areas.Administrator.Controllers
//{
//    public class AboutUsController : BaseController
//    {
//        public ActionResult Index()
//        {
//            ViewBag.Messages = CommentController.Messages(TempData["Messages"]);
//            ViewBag.Title = "AboutUs";
//            return View();
//        }

//        [HttpPost]
//        public ActionResult UpdateIndex()
//        {
//            using (var db = new MyDbDataContext())
//            {
//                List<AboutUs> records =
//                    db.AboutUs.Where(r => r.LanguageID == Request.Cookies["lang_client"].Value).ToList();
//                foreach (AboutUs record in records)
//                {
//                    string itemAdv = Request.Params[string.Format("Sort[{0}].Index", record.ID)];
//                    int index;
//                    int.TryParse(itemAdv, out index);
//                    record.Index = index;
//                    db.SubmitChanges();
//                }
//                TempData["Messages"] = "Successful";
//                return RedirectToAction("Index");
//            }
//        }

//        [HttpPost]
//        public JsonResult List(int menuId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
//        {
//            try
//            {
//                using (var db = new MyDbDataContext())
//                {
//                    var records = db.AboutUs.Where(a => a.LanguageID == Request.Cookies["lang_client"].Value).Select(a => new {
//                        a.ID,
//                        a.Title,
//                        a.Image,
//                        a.Description,                       
//                        a.Index
//                    }).ToList();
//                    return Json(new { Result = "OK", Records = records, TotalRecordCount = records.Count });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { Result = "ERROR", ex.Message });
//            }
//        }

//        [HttpGet]
//        public ActionResult Create()
//        {
//            ViewBag.Title = "add AboutUs";
//            var model = new EAboutUs();
//            return View(model);
//        }

//        [HttpPost]
//        [ValidateInput(false)]
//        public ActionResult Create(EAboutUs model)
//        {
//            using (var db = new MyDbDataContext())
//            {
//                if (ModelState.IsValid)
//                {
//                    try
//                    {
//                        var about = new AboutUs
//                        {
//                            LanguageID = Request.Cookies["lang_client"].Value,
//                            Title = model.Title,
//                            Image = model.Image,
//                            Index = 0,
//                            Description = model.Description,
//                        };

//                        db.AboutUs.InsertOnSubmit(about);
//                        db.SubmitChanges();

//                        TempData["Messages"] = "Successful";
//                        return RedirectToAction("Index");
//                    }
//                    catch (Exception exception)
//                    {
//                        ViewBag.Messages = "Error: " + exception.Message;
//                        return View(model);
//                    }
//                }

//                return View(model);
//            }
//        }

//        [HttpGet]
//        public ActionResult Update(int id)
//        {
//            using (var db = new MyDbDataContext())
//            {
//                AboutUs aboutUs = db.AboutUs.FirstOrDefault(a => a.ID == id);
//                if (aboutUs == null)
//                {
//                    TempData["Messages"] = "About not exist";
//                    return RedirectToAction("Index");
//                }
//                var slider = new EAboutUs
//                {
//                    ID = aboutUs.ID,
//                    Title = aboutUs.Title,
//                    Image = aboutUs.Image,
//                    Index = aboutUs.Index,
//                    Description = aboutUs.Description,
//                };
//                ViewBag.Title = "udpate slide";
//                return View(slider);
//            }
//        }

//        [HttpPost]
//        [ValidateInput(false)]
//        public ActionResult Update(EAboutUs model)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    using (var db = new MyDbDataContext())
//                    {
//                        AboutUs aboutUs = db.AboutUs.FirstOrDefault(b => b.ID == model.ID);
//                        if (aboutUs != null)
//                        {
//                            aboutUs.Title = model.Title;
//                            aboutUs.Image = model.Image;
//                            aboutUs.Description = model.Description;
//                            aboutUs.LanguageID = Request.Cookies["lang_client"].Value;
//                            db.SubmitChanges();
//                            TempData["Messages"] = "Successful";
//                            return RedirectToAction("Index");
//                        }
//                    }
//                }
//                catch (Exception exception)
//                {
//                    ViewBag.Messages = "Error: " + exception.Message;
//                    return View(model);
//                }
//            }
//            return View(model);
//        }

//        [HttpPost]
//        public JsonResult Delete(int id)
//        {
//            try
//            {
//                using (var db = new MyDbDataContext())
//                {
//                    AboutUs del = db.AboutUs.FirstOrDefault(c => c.ID == id);
//                    if (del != null)
//                    {
//                        db.AboutUs.DeleteOnSubmit(del);
//                        db.SubmitChanges();
//                        return Json(new { Result = "OK", Message = "Successful" });
//                    }
//                    return Json(new { Result = "ERROR", Message = "Slide not exist" });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { Result = "ERROR", ex.Message });
//            }
//        }
       
//    }
//}
