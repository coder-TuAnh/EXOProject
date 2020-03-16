using ProjectLibrary.Config;
using ProjectLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamplateHotel.Areas.Administrator.ModelShow;
using TeamplateHotel.Areas.Administrator.EntityModel;
using ProjectLibrary.Utility;

namespace TeamplateHotel.Areas.Administrator.Controllers
{
    public class TabArticleController : Controller
    {
        //
        // GET: /Administrator/TabArticle/

        public ActionResult Index()
        {
            ViewBag.Messages = CommentController.Messages(TempData["Messages"]);
            ViewBag.Title = "TabArticle";
            LoadData();
            return View();
        }
        [HttpPost]
        public ActionResult UpdateIndex()
        {
            using (var db = new MyDbDataContext())
            {
                var records = db.TabArticles.ToList();

                foreach (var record in records)
                {
                    string itemAdv = Request.Params[string.Format("Sort[{0}].Index", record.ID)];
                    if (itemAdv != null)
                    {
                        int index;
                        int.TryParse(itemAdv, out index);
                        record.Index = index;
                        db.SubmitChanges();
                    }

                }
                TempData["Messages"] = "Successful";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public JsonResult List(int menuId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                using (var db = new MyDbDataContext())
                {
                    List<TabArticle> tabArticles = new List<TabArticle>();
                    
                        tabArticles = db.TabArticles.ToList();
                    
                    List<ShowTab> records = tabArticles.Select(a => new ShowTab
                    {
                        ID = a.ID,
                        Title = a.Title,
                        Alias = a.Alias,
                        Index = a.Index,
                        MetaTitle = a.MetaTitle,
                    }).OrderBy(a => a.Index).Skip(jtStartIndex).Take(jtPageSize).ToList();
                    //Return result to jTable
                    return Json(new { Result = "OK", Records = records, TotalRecordCount = tabArticles.Count() });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            LoadData();
            ViewBag.Title = "Add tabarticle";
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ETapArticle model)
        {
            using (var db = new MyDbDataContext())
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.Alias))
                    {
                        model.Alias = StringHelper.ConvertToAlias(model.Alias);
                    }
                    try
                    {
                        var tab = new TabArticle
                        {
                            Title = model.Title,
                            Alias = model.Alias,
                            Index = 0,
                            MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? model.Title : model.MetaTitle,
                            MetaDescription =
                                string.IsNullOrEmpty(model.MetaDescription) ? model.Title : model.MetaDescription,
                        };

                        db.TabArticles.InsertOnSubmit(tab);
                        db.SubmitChanges();

                        TempData["Messages"] = "Successful";
                        return RedirectToAction("Index");
                    }
                    catch (Exception exception)
                    {
                        LoadData();
                        ViewBag.Messages = "Error: " + exception.Message;
                        return View();
                    }
                }
                LoadData();
                return View();
            }
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            ViewBag.Title = "Update TabArticle";
            using (var db = new MyDbDataContext())
            {
                TabArticle detailArticle = db.TabArticles.FirstOrDefault(a => a.ID == id);

                if (detailArticle == null)
                {
                    TempData["Messages"] = "does not exist";
                    return RedirectToAction("Index");
                }

                var tab = new ETapArticle
                {
                    ID = detailArticle.ID,
                    Title = detailArticle.Title,
                    Alias = detailArticle.Alias,
                    MetaTitle = detailArticle.MetaTitle,
                    MetaDescription = detailArticle.MetaDescription,
                };
                LoadData();
                return View(tab);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(ETapArticle model)
        {
            using (var db = new MyDbDataContext())
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.Alias))
                    {
                        model.Alias = StringHelper.ConvertToAlias(model.Alias);
                    }
                    try
                    {
                        TabArticle tabArticle = db.TabArticles.FirstOrDefault(b => b.ID == model.ID);
                        if (tabArticle != null)
                        {
                            tabArticle.Title = model.Title;
                            tabArticle.Alias = model.Alias;
                            tabArticle.MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? model.Title : model.MetaTitle;
                            tabArticle.MetaDescription = string.IsNullOrEmpty(model.MetaDescription)
                                ? model.Title
                                : model.MetaDescription;

                            db.SubmitChanges();
                            TempData["Messages"] = "Successful";
                            return RedirectToAction("Index");
                        }
                    }
                    catch (Exception exception)
                    {
                        LoadData();
                        ViewBag.Messages = "Error: " + exception.Message;
                        return View();
                    }
                }
                LoadData();
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                using (var db = new MyDbDataContext())
                {
                    TabArticle del = db.TabArticles.FirstOrDefault(c => c.ID == id);
                    if (del != null)
                    {
                        db.TabArticles.DeleteOnSubmit(del);
                        db.SubmitChanges();
                        return Json(new { Result = "OK", Message = "Successful" });
                    }
                    return Json(new { Result = "ERROR", Message = "does not exist" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public void LoadData()
        {
            var listMenu = new List<SelectListItem>
            {
                new SelectListItem {Value = "0", Text = "Select menu"}
            };
            var menus = new List<Menu>();

            menus =
                MenuController.GetListMenu(0, Request.Cookies["lang_client"].Value).Where(
                    a =>
                        a.Type == SystemMenuType.Article).ToList();

            foreach (Menu menu in menus)
            {
                string sub = "";
                for (int i = 0; i < menu.Level; i++)
                {
                    sub += "|--";
                }
                menu.Title = sub + menu.Title;
            }

            listMenu.AddRange(menus.OrderBy(a => a.Location).Select(a => new SelectListItem
            {
                Text = a.Title,
                Value = a.ID.ToString()
            }).ToList());
            ViewBag.ListMenu = listMenu;
        }
    }
}
