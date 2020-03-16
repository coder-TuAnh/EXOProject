using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using ProjectLibrary.Config;
using ProjectLibrary.Database;
using ProjectLibrary.Utility;
using TeamplateHotel.Areas.Administrator.EntityModel;

namespace TeamplateHotel.Areas.Administrator.Controllers
{
    public class TourController : BaseController
    {
        // GET: /Administrator/Tour/
        public ActionResult Index()
        {
            LoadData();
            ViewBag.Messages = CommentController.Messages(TempData["Messages"]);
            ViewBag.Title = "Page Tours";
            return View();
        }

        [HttpPost]
        public ActionResult UpdateIndex()
        {
            using (var db = new MyDbDataContext())
            {
                List<Tour> records = db.Tours.ToList();
                foreach (Tour record in records)
                {
                    string itemTour = Request.Params[string.Format("Sort[{0}].Index", record.ID)];
                    if (itemTour != null)
                    {
                        int index;
                        int.TryParse(itemTour, out index);
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
                var db = new MyDbDataContext();
                var listmenu = db.Menus.Where(m => m.LanguageID == Request.Cookies["lang_client"].Value).ToList();
                var listTour = new List<Tour>();
                if (menuId == 0)
                {
                    listTour = db.Tours.ToList();
                }
                else
                {
                    listTour = db.Tours.Where(a => a.MenuID == menuId).ToList();
                }

                var records = listTour.Join(db.Menus.Where(a => a.LanguageID == Request.Cookies["lang_client"].Value), a => a.MenuID, b => b.ID,
                            (a, b) => new
                            {
                                a.ID,
                                a.Title,
                                a.Index,
                                a.Status,
                                a.Deal,
                                a.Hot
                            }).Skip(jtStartIndex).Take(jtPageSize).OrderBy(a => a.Index).ToList();
                //Return result to jTable
                return Json(new { Result = "OK", Records = records, TotalRecordCount = listTour.Count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "add tour";
            LoadData();
            LoadDataActivities();
            LoadData2();
            var tour = new ETour();
            return View(tour);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ETour model)
        {
            using (var db = new MyDbDataContext())
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.Alias))
                    {
                        model.Alias = StringHelper.ConvertToAlias(model.Title);
                    }
                    try
                    {
                        var tour = new Tour
                        {
                            MenuID = model.MenuID,
                            ActivitiesID = model.ActivitisID,
                            Title = model.Title,
                            Alias = model.Alias,
                            Image = model.Image,
                            Index = 0,
                            Description = model.Description,
                            MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? model.Title : model.MetaTitle,
                            MetaDescription =
                                string.IsNullOrEmpty(model.MetaDescription) ? model.Title : model.MetaDescription,
                            Status = model.Status,
                            Price = model.Price,
                            Location = model.Location,
                            PriceSale = model.PriceSale,
                            Hot = model.Hot,
                            Deal = model.Deal,
                            Like = model.Like,
                            TourOther = model.TourOther,
                            TourIncluded = model.TourIncluded,
                            TourExcluded = model.TourExcluded,
                        };

                        db.Tours.InsertOnSubmit(tour);
                        db.SubmitChanges();

                        //Thêm tabHotel
                        if (model.TabHotels != null)
                        {
                            foreach (TabHotel item in model.TabHotels)
                            {
                                var tabTour = new TabHotel
                                {
                                    TourID = tour.ID,
                                    TitleTabHotel = item.TitleTabHotel,
                                    ContentHotel = item.ContentHotel,
                                };

                                db.TabHotels.InsertOnSubmit(tabTour);
                            }
                            db.SubmitChanges();
                        }

                        if (model.Theme != null)
                        {
                            for (int i = 0; i < model.Theme.Length; i++)
                            {
                                var tabTheme = new ThemesMenu
                                {
                                    TourID = tour.ID,
                                    MenuID = int.Parse(model.Theme[i]),
                                    Index = 0
                                };
                                db.ThemesMenus.InsertOnSubmit(tabTheme);
                                db.SubmitChanges();
                            }
                        }

                        //Thêm hình ảnh cho tour
                        if (model.EGalleryITems != null)
                        {
                            foreach (EGalleryITem itemGallery in model.EGalleryITems)
                            {
                                var gallery = new TourGallery
                                {
                                    LargeImage = itemGallery.Image,
                                    SmallImage = ReturnSmallImage.GetImageSmall(itemGallery.Image),
                                    TourID = tour.ID,
                                };
                                db.TourGalleries.InsertOnSubmit(gallery);
                            }
                            db.SubmitChanges();
                        }
                        //Thêm tabtour
                        if (model.TabTours != null)
                        {
                            foreach (TabTour itemTabTour in model.TabTours)
                            {
                                var tabTour = new TabTour
                                {
                                    TourID = tour.ID,
                                    TitleTab = itemTabTour.TitleTab,
                                    Content = itemTabTour.Content,
                                    Price = itemTabTour.Price,
                                };

                                db.TabTours.InsertOnSubmit(tabTour);
                            }
                            db.SubmitChanges();
                        }

                        TempData["Messages"] = "Successful";
                        return RedirectToAction("Index");
                    }
                    catch (Exception exception)
                    {
                        LoadData();
                        LoadDataActivities();
                        ViewBag.Messages = "Error: " + exception.Message;
                        return View(model);
                    }
                }
                LoadData();
                LoadDataActivities();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            ViewBag.Title = "udpate tour";
            var db = new MyDbDataContext();
            Tour detailTour = db.Tours.FirstOrDefault(a => a.ID == id);
            if (detailTour == null)
            {
                TempData["Messages"] = "Tour not exist";
                return RedirectToAction("Index");
            }
            LoadData();
            LoadDataActivities();
            List<SelectListItem> listmenu = new List<SelectListItem>();
            foreach (var b in db.Menus.Where(x => x.Type == SystemMenuType.Tour && x.Status).ToList())
            {
                listmenu.Add(new SelectListItem() { Value = b.Title, Text = b.ID.ToString() });
            }
            ViewBag.ListMenu3 = new SelectList(listmenu, "Text", "Value");
            var tour = new ETour
            {
                ID = detailTour.ID,
                MenuID = detailTour.MenuID,
                ActivitisID = (int)detailTour.ActivitiesID,
                Location = detailTour.Location,
                Like = (bool)detailTour.Like,
                Hot = (bool)detailTour.Hot,
                Deal = detailTour.Deal,
                Title = detailTour.Title,
                Alias = detailTour.Alias,
                Image = detailTour.Image,
                Description = detailTour.Description,
                MetaTitle = detailTour.MetaTitle,
                MetaDescription = detailTour.MetaDescription,
                Status = detailTour.Status,
                Price = (decimal)detailTour.Price,
                PriceSale = (decimal)detailTour.PriceSale,
                TourOther = (bool)detailTour.TourOther,
                TourIncluded = detailTour.TourIncluded,
                TourExcluded = detailTour.TourExcluded,
            };
            //lấy danh sách hình ảnh
            tour.EGalleryITems = db.TourGalleries.Where(a => a.TourID == detailTour.ID).Select(a => new EGalleryITem
            {
                Image = a.LargeImage
            }).ToList();
            //lấy danh sách tabtour
            List<TabTour> tabTours = db.TabTours.Where(a => a.TourID == detailTour.ID).ToList();
            List<TabHotel> tabhotel = db.TabHotels.Where(a => a.TourID == detailTour.ID).ToList();
            List<ThemesMenu> tabTheme = db.ThemesMenus.Where(x => x.TourID == detailTour.ID).ToList();
            tour.TabTours = tabTours;
            tour.TabHotels = tabhotel;
            if (!string.IsNullOrEmpty(tabTheme.ToString()))
            {
                List<string> termList = new List<string>();
                foreach (var item in tabTheme)
                {
                    termList.Add(item.MenuID.ToString());
                }
                string[] select = termList.ToArray();
                tour.Theme = select;
            }
            return View(tour);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(ETour model)
        {
            //Kiểm tra xem alias thuộc tour này đã tồn tại chưa
            var db = new MyDbDataContext();

            //Kiểm tra xem đã chọn đến chuyên mục con cuối cùng chưa
            //var check = db.Menus.Where(a => a.ParentID == model.ID).ToList();
            //if (db.Menus.Any(a => a.ParentID == model.ID))
            //{
            //    ModelState.AddModelError("MenuId", "Vui lòng chọn đến chuyên mục tour con cuối cùng");
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    Tour tour = db.Tours.FirstOrDefault(b => b.ID == model.ID);
                    if (tour != null)
                    {
                        tour.MenuID = model.MenuID;
                        tour.ActivitiesID = model.ActivitisID;
                        tour.Location = model.Location;
                        tour.Like = model.Like;
                        tour.Deal = model.Deal;
                        tour.Title = model.Title;
                        tour.Alias = model.Alias;
                        tour.Image = model.Image;
                        tour.Description = model.Description;
                        tour.MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? model.Title : model.MetaTitle;
                        tour.MetaDescription = string.IsNullOrEmpty(model.MetaDescription)
                            ? model.Title
                            : model.MetaDescription;
                        tour.Status = model.Status;
                        tour.Hot = model.Hot;
                        tour.Price = model.Price;
                        tour.PriceSale = model.PriceSale;
                        tour.TourOther = model.TourOther;
                        tour.TourIncluded = model.TourIncluded;
                        tour.TourExcluded = model.TourExcluded;
                        db.SubmitChanges();

                        //xóa gallery cho tour
                        db.TourGalleries.DeleteAllOnSubmit(db.TourGalleries.Where(a => a.TourID == tour.ID).ToList());
                        //Thêm hình ảnh cho tour
                        if (model.EGalleryITems != null)
                        {
                            foreach (EGalleryITem itemGallery in model.EGalleryITems)
                            {
                                var gallery = new TourGallery
                                {
                                    LargeImage = itemGallery.Image,
                                    SmallImage = ReturnSmallImage.GetImageSmall(itemGallery.Image),
                                    TourID = tour.ID,
                                };
                                db.TourGalleries.InsertOnSubmit(gallery);
                            }
                            db.SubmitChanges();
                        }
                        //xóa danh sách tabtour
                        db.TabTours.DeleteAllOnSubmit(db.TabTours.Where(a => a.TourID == tour.ID).ToList());
                        db.ThemesMenus.DeleteAllOnSubmit(db.ThemesMenus.Where(a => a.TourID == tour.ID).ToList());
                        if (model.Theme != null)
                        {
                            for (int i = 0; i < model.Theme.Length; i++)
                            {
                                var tabTheme = new ThemesMenu
                                {
                                    TourID = tour.ID,
                                    MenuID = int.Parse(model.Theme[i]),
                                    Index = 0
                                };
                                db.ThemesMenus.InsertOnSubmit(tabTheme);
                                db.SubmitChanges();
                            }
                        }

                        //Thêm tabtour
                        if (model.TabTours != null)
                        {
                            foreach (TabTour itemTabTour in model.TabTours)
                            {
                                var tabTour = new TabTour
                                {
                                    TourID = tour.ID,
                                    TitleTab = itemTabTour.TitleTab,
                                    Content = itemTabTour.Content,
                                    Price = itemTabTour.Price
                                };

                                db.TabTours.InsertOnSubmit(tabTour);
                            }
                            db.SubmitChanges();
                        }
                        db.TabHotels.DeleteAllOnSubmit(db.TabHotels.Where(a => a.TourID == tour.ID).ToList());
                        if (model.TabHotels != null)
                        {
                            foreach (TabHotel item in model.TabHotels)
                            {
                                var tabTour = new TabHotel
                                {
                                    TourID = tour.ID,
                                    TitleTabHotel = item.TitleTabHotel,
                                    ContentHotel = item.ContentHotel,
                                };

                                db.TabHotels.InsertOnSubmit(tabTour);
                            }
                            db.SubmitChanges();
                        }

                        TempData["Messages"] = "Successful";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception exception)
                {
                    LoadData();
                    LoadDataActivities();
                    ViewBag.Messages = "Error: " + exception.Message;
                    return View();
                }
            }
            LoadData();
            LoadDataActivities();
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                using (var db = new MyDbDataContext())
                {
                    Tour del = db.Tours.FirstOrDefault(c => c.ID == id);
                    if (del != null)
                    {
                        //xóa hết hình ảnh của tour này
                        db.TourGalleries.DeleteAllOnSubmit(db.TourGalleries.Where(a => a.TourID == del.ID).ToList());
                        //xóa hết tabtour của tour này
                        db.TabTours.DeleteAllOnSubmit(db.TabTours.Where(a => a.TourID == del.ID).ToList());
                        db.TabHotels.DeleteAllOnSubmit(db.TabHotels.Where(a => a.TourID == del.ID).ToList());
                        db.ThemesMenus.DeleteAllOnSubmit(db.ThemesMenus.Where(a => a.TourID == del.ID).ToList());
                        db.Tours.DeleteOnSubmit(del);
                        db.SubmitChanges();
                        return Json(new { Result = "OK", Message = "Successful" });
                    }
                    return Json(new { Result = "ERROR", Message = "Tour not exist" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public void LoadData()
        {
            var listMenu = new List<SelectListItem>();
            listMenu.Add(new SelectListItem { Value = "0", Text = "---Select a menu---" });
            List<Menu> getListMenu =
                MenuController.GetListMenu(SystemMenuLocation.ListLocationMenu().ToList()[0].LocationId,
                    Request.Cookies["lang_client"].Value)
                    .Where(a => a.Type == SystemMenuType.Tour && a.Level == 1)
                    .ToList();

            foreach (Menu menu in getListMenu)
            {
                string subTitle = "";
                for (int i = 1; i <= menu.Level; i++)
                {
                    subTitle += "|--";
                }
                menu.Title = subTitle + menu.Title;
            }
            listMenu.AddRange(getListMenu.Select(a => new SelectListItem
            {
                Value =
                    a.ID.ToString(
                        CultureInfo.InvariantCulture),
                Text = a.Title
            }).ToList());
            ViewBag.ListMenu = listMenu;
        }

        public void LoadData2()
        {
            var listMenu = new List<SelectListItem>();
            listMenu.Add(new SelectListItem { Value = "0", Text = "---Select a menu---" });
            List<Menu> getListMenu =
                MenuController.GetListMenu(SystemMenuLocation.ListLocationMenu().ToList()[0].LocationId,
                    Request.Cookies["lang_client"].Value)
                    .Where(a => a.Type == SystemMenuType.Tour && a.Status)
                    .ToList();
            listMenu.AddRange(getListMenu.Select(a => new SelectListItem
            {
                Value =
                    a.ID.ToString(
                        CultureInfo.InvariantCulture),
                Text = a.Title
            }).ToList());
            ViewBag.ListMenu3 = listMenu;
        }

        public void LoadDataActivities()
        {
            var listMenu = new List<SelectListItem>();
            listMenu.Add(new SelectListItem { Value = "0", Text = "---Select a menu---" });
            var db = new MyDbDataContext();
            List<Menu> themes = db.Menus.Where(x => (bool)x.Status && x.Type == SystemMenuType.Activities).ToList();
            listMenu.AddRange(themes.Select(a => new SelectListItem
            {
                Value =
                    a.ID.ToString(
                        CultureInfo.InvariantCulture),
                Text = a.Title
            }).ToList());
            ViewBag.ListMenu2 = listMenu;
        }
    }
}