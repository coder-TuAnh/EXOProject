using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ProjectLibrary.Config;
using ProjectLibrary.Database;
using ProjectLibrary.Utility;
using TeamplateHotel.Areas.Administrator.EntityModel;
using TeamplateHotel.Areas.Administrator.ModelShow;

namespace TeamplateHotel.Areas.Administrator.Controllers
{
    public class ArticleController : BaseController
    {
        // GET: /Administrator/Article/
        public ActionResult Index()
        {
            ViewBag.Messages = CommentController.Messages(TempData["Messages"]);
            ViewBag.Title = "Article";
            LoadData();
            return View();
        }

        [HttpPost]
        public ActionResult UpdateIndex()
        {
            using (var db = new MyDbDataContext())
            {
                var records =
                    db.Articles.Join(db.Menus.Where(a => a.LanguageID == Request.Cookies["lang_client"].Value),
                        a => a.MenuID,
                        b => b.ID, (a, b) => new { a }).ToList();

                foreach (var record in records)
                {
                    string itemAdv = Request.Params[string.Format("Sort[{0}].Index", record.a.ID)];
                    if (itemAdv != null) {
                        int index;
                        int.TryParse(itemAdv, out index);
                        record.a.Index = index;
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
                    List<Article> articles = new List<Article>();
                    if (menuId == 0)
                    {
                        articles = db.Articles.ToList();
                    }
                    else
                    {
                        articles = db.Articles.Where(a => a.MenuID == menuId).ToList();
                    }
                    var listArticle =
                        articles.Join(db.Menus.Where(a => a.LanguageID == Request.Cookies["lang_client"].Value),
                            a => a.MenuID, b => b.ID, (a, b) => new {a, b});
                    List<ShowArticle> records = listArticle.Select(a => new ShowArticle
                    {
                        ID = a.a.ID,
                        Title = a.a.Title,
                        TitleMenu = a.b.Title,
                        Index = a.a.Index,
                        Status = a.a.Status,
                        Home = a.a.Home,
                        Hot = a.a.Hot,
                        New = a.a.New,
                        Value = a.a.Value,
                        About = a.a.About,
                    }).OrderBy(a => a.Index).Skip(jtStartIndex).Take(jtPageSize).ToList();
                    //Return result to jTable
                    return Json(new {Result = "OK", Records = records, TotalRecordCount = listArticle.Count()});
                }
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", ex.Message});
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            LoadData();
            ViewBag.Title = "Add article";
            LoadData2();
            var tab = new EArticle();
            return View(tab);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EArticle model)
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
                        var article = new Article
                        {
                            MenuID = model.MenuID,
                            Title = model.Title,
                            Alias = model.Alias,
                            Image = model.Image,
                            Description = model.Description,
                            Content = model.Content,
                            Index = 0,
                            MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? model.Title : model.MetaTitle,
                            MetaDescription =
                                string.IsNullOrEmpty(model.MetaDescription) ? model.Title : model.Description,
                            Status = model.Status,
                            Home = model.Home,
                            Hot = model.Hot,
                            New = model.New,
                            Value = model.Value,
                            About = model.About,
                            CreateDate = DateTime.Now,
                    };

                        db.Articles.InsertOnSubmit(article);
                        db.SubmitChanges();
                        if (model.Theme != null)
                        {
                            for (int i = 0; i < model.Theme.Length; i++)
                            {
                                var tabTheme = new MenuTap
                                {
                                    ArticleID = article.ID,
                                    TapID = int.Parse(model.Theme[i]),
                                };
                                db.MenuTaps.InsertOnSubmit(tabTheme);
                                db.SubmitChanges();
                            }
                        }

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
            ViewBag.Title = "Update Article";
            using (var db = new MyDbDataContext())
            {
                Article detailArticle = db.Articles.FirstOrDefault(a => a.ID == id);

                if (detailArticle == null)
                {
                    TempData["Messages"] = "does not exist";
                    return RedirectToAction("Index");
                }

                List<SelectListItem> listmenu = new List<SelectListItem>();
                foreach (var b in db.TabArticles.ToList())
                {
                    listmenu.Add(new SelectListItem() { Value = b.Title, Text = b.ID.ToString() });
                }
                ViewBag.ListMenu3 = new SelectList(listmenu, "Text", "Value");
                var artile = new EArticle
                {
                    ID = detailArticle.ID,
                    MenuID = detailArticle.MenuID,
                    Title = detailArticle.Title,
                    Alias = detailArticle.Alias,
                    Image = detailArticle.Image,
                    Description = detailArticle.Description,
                    Content = detailArticle.Content,
                    MetaTitle = detailArticle.MetaTitle,
                    MetaDescription = detailArticle.MetaDescription,
                    Status = detailArticle.Status,
                    Home = detailArticle.Home,
                    Hot = detailArticle.Hot,
                    New = detailArticle.New,
                    Value = detailArticle.Value,
                    About = detailArticle.About,
                };
                List<MenuTap> tabTheme = db.MenuTaps.Where(x => x.ArticleID == detailArticle.ID).ToList();
                if (!string.IsNullOrEmpty(tabTheme.ToString()))
                {
                    List<string> termList = new List<string>();
                    foreach (var item in tabTheme)
                    {
                        termList.Add(item.TapID.ToString());
                    }
                    string[] select = termList.ToArray();
                    artile.Theme = select;
                }
                LoadData();
                return View(artile);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(EArticle model)
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
                        Article article = db.Articles.FirstOrDefault(b => b.ID == model.ID);
                        if (article != null)
                        {
                            article.MenuID = model.MenuID;
                            article.Title = model.Title;
                            article.Alias = model.Alias;
                            article.Image = model.Image;
                            article.Description = model.Description;
                            article.Content = model.Content;
                            article.MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? model.Title : model.MetaTitle;
                            article.MetaDescription = string.IsNullOrEmpty(model.MetaDescription)
                                ? model.Title
                                : model.MetaDescription;
                            article.Status = model.Status;
                            article.Home = model.Home;
                            article.Hot = model.Hot;
                            article.New = model.New;
                            article.Value = model.Value;
                            article.About = model.About;
                           
                            db.SubmitChanges();
                            db.MenuTaps.DeleteAllOnSubmit(db.MenuTaps.Where(a => a.ArticleID == article.ID).ToList());
                            if (model.Theme != null)
                            {
                                for (int i = 0; i < model.Theme.Length; i++)
                                {
                                    var tabTheme = new MenuTap
                                    {
                                        ArticleID = article.ID,
                                        TapID = int.Parse(model.Theme[i]),
                                    };
                                    db.MenuTaps.InsertOnSubmit(tabTheme);
                                    db.SubmitChanges();
                                }
                            }
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
                    Article del = db.Articles.FirstOrDefault(c => c.ID == id);
                    if (del != null)
                    {
                        db.MenuTaps.DeleteAllOnSubmit(db.MenuTaps.Where(a => a.ArticleID == del.ID).ToList());
                        db.Articles.DeleteOnSubmit(del);
                        db.SubmitChanges();
                        return Json(new {Result = "OK", Message = "Successful" });
                    }
                    return Json(new {Result = "ERROR", Message = "does not exist"});
                }
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", ex.Message});
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
                        a.Type == SystemMenuType.Article ).ToList();

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
        public void LoadData2()
        {
            using(var db = new MyDbDataContext())
            {
            var listMenu = new List<SelectListItem>();
            listMenu.Add(new SelectListItem { Value = "0", Text = "---Select a menu---" });
                List<TabArticle> getListMenu = db.TabArticles.ToList();
            listMenu.AddRange(getListMenu.Select(a => new SelectListItem
            {
                Value =
                            a.ID.ToString(
                                CultureInfo.InvariantCulture),
                Text = a.Title
            }).ToList());
            ViewBag.ListMenu3 = listMenu;
            }
            
        }
    }
}