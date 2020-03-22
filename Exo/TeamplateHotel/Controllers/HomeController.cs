using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProjectLibrary.Config;
using ProjectLibrary.Database;
using TeamplateHotel.Models;

namespace TeamplateHotel.Controllers
{
    public class HomeController : BasicController
    {
        [HttpGet]
        public ActionResult Index(object aliasMenuSub, object idSub, object aliasSub, int? page, int? pageSize)
        {
            string lan = Request.Cookies["LanguageID"].Value;
            var db = new MyDbDataContext();
            Hotel hotel = CommentController.DetailHotel(lan);
            ViewBag.MetaTitle = hotel.MetaTitle;
            ViewBag.MetaDesctiption = hotel.MetaDescription;

            if (aliasMenuSub.ToString() == "System.Object")
            {
                return View("Index");
            }

            if (aliasMenuSub.ToString() == "Search")
            {
                string key = Request.Params["key"];
                if (string.IsNullOrEmpty(key))
                {
                    return View("Tour/Search", new List<Tour>());
                }
                List<ShowObject> listSearch = new List<ShowObject>();
                listSearch.AddRange(db.Tours.Where(a => a.Status && a.Title.Contains(key)).OrderBy(a => a.Index).Select(a => new ShowObject()
                {
                    ID = a.ID,
                    Alias = a.Alias,
                    Description = a.Description,
                    Image = a.Image,
                    Index = a.Index,
                    MenuAlias = a.Menu.Alias,
                    Title = a.Title,
                }).ToList());
                listSearch.AddRange(db.Articles.Where(a => a.Status && a.Title.Contains(key)).OrderBy(a => a.Index).Select(a => new ShowObject()
                {
                    ID = a.ID,
                    Alias = a.Alias,
                    Description = a.Description,
                    Image = a.Image,
                    Index = a.Index,
                    MenuAlias = a.Menu.Alias,
                    Title = a.Title,
                }).ToList());

                return View("Tour/Search", listSearch);
            }
            if (aliasMenuSub.ToString() == "SelectLanguge")
            {
                Language language = db.Languages.FirstOrDefault(a => a.ID == idSub.ToString());
                if (language == null)
                {
                    language = db.Languages.FirstOrDefault();
                }
                HttpCookie langCookie = Request.Cookies["LanguageID"];
                langCookie.Value = language.ID;
                langCookie.Expires = DateTime.Now.AddDays(10);
                HttpContext.Response.Cookies.Add(langCookie);
                return Redirect("/");
            }
            // xác định menu => tìm ra Kiểu hiển thị của menu
            Menu menu = db.Menus.FirstOrDefault(a => a.Alias == aliasMenuSub.ToString());
            if (menu == null)
            {
                return View("404");
            }
            //Seo
            ViewBag.MetaTitle = menu.MetaTitle ?? menu.Title;
            ViewBag.MetaDesctiption = menu.MetaDescription ?? menu.Title;
            ViewBag.Menu = menu;

            switch (menu.Type)
            {
                case SystemMenuType.Article:
                    goto Trangbaiviet;
                case SystemMenuType.Tour:
                    goto TrangTour;
                case SystemMenuType.Activities:
                    return LisTour(menu.Alias, idSub, page);
                case SystemMenuType.Contact:
                    return View("Contact");
                case SystemMenuType.Aboutus:
                    return View("About");
                default:
                    return View("Index");
            }

        #region "Trang bài viết"

        Trangbaiviet:
            if (idSub.ToString() != "System.Object")
            {
                int idArticle;
                int.TryParse(idSub.ToString(), out idArticle);
                DetailArticle detailArticle = CommentController.Detail_Article(idArticle);
                ViewBag.MetaTitle = detailArticle.Article.MetaTitle;
                ViewBag.MetaDesctiption = detailArticle.Article.MetaDescription;
                return View("Article/DetailArticle", detailArticle);
            }
            //Danh sách bài viết
            List<Article> articles = CommentController.GetArticles(menu.ID);
            if (articles.Count == 1)
            {
                DetailArticle detailArticle = CommentController.Detail_Article(articles[0].ID);
                ViewBag.MetaTitle = detailArticle.Article.MetaTitle;
                ViewBag.MetaDesctiption = detailArticle.Article.MetaDescription;
                return View("Article/DetailArticle", detailArticle);
            }
            int pagenumber = page ?? 1;
            int pagesize = pageSize ?? 5;
            IPagedList<Article> list = articles.ToPagedList(pagenumber, pagesize);
            return View("Article/ListArticle", list);

        #endregion "Trang bài viết"


        //trường hợp: Tour

        #region "Kiếu tour"

        TrangTour:
            if (idSub.ToString() != "System.Object")
            {
                int idTour;
                int.TryParse(idSub.ToString(), out idTour);
                DetailTour detailTour = CommentController.Detail_Tour(idTour);
                ViewBag.MetaTitle = detailTour.Tour.MetaTitle ?? detailTour.Tour.Title;
                ViewBag.MetaDesctiption = detailTour.Tour.MetaDescription ?? detailTour.Tour.Title;

                return View("Tour/DetailTour", detailTour);
            }
            pagenumber = page ?? 1;
            pagesize = pageSize ?? 12;
            ViewBag.slug = aliasMenuSub;
            List<ShowObject> listTours = CommentController.GetTours(menu.ID);
            IPagedList<ShowObject> listAll = listTours.ToPagedList(pagenumber, pagesize);
            return View("Tour/ListTour", listAll);

            #endregion "Kiếu tour"
        }

        public ActionResult LisTour(string alias,object idSub, int? page)
        {
            if (idSub.ToString() != "System.Object")
            {
                int idTour;
                int.TryParse(idSub.ToString(), out idTour);
                DetailTour detailTour = CommentController.Detail_Tour(idTour);
                ViewBag.MetaTitle = detailTour.Tour.MetaTitle ?? detailTour.Tour.Title;
                ViewBag.MetaDesctiption = detailTour.Tour.MetaDescription ?? detailTour.Tour.Title;

                return View("Tour/DetailTour", detailTour);
            }
            ViewBag.page = page;
            ViewBag.slug = alias;
            return View("Tour/ListTourByMenuTour");
        }

        public ActionResult ListTours(string slug, string item, int? page, int? pageSize)
        {
            using (var db = new MyDbDataContext())
            {
                Menu menu = db.Menus.FirstOrDefault(x => x.Alias == slug);
                ViewBag.slug = slug;
                ViewBag.Menu = menu;
                int pagenumber = page ?? 1;
                int pagesize = pageSize ?? 9;
                List<ShowObject> _listActivities = CommentController.GetToursActivities(menu.ID);
               
                switch(item)
                {
                    case "0":
                        _listActivities = _listActivities.OrderBy(x => x.ID).ToList();
                        break;
                    case "1":
                        _listActivities = _listActivities.OrderBy(x => x.Price).ToList();
                        break;
                    case "2":
                        _listActivities = _listActivities.OrderByDescending(x => x.Price).ToList();
                        break;
                    default:
                        break;
                }
                IPagedList<ShowObject> _list = _listActivities.ToPagedList(pagenumber, pagesize);

                return View("Tour/SpecialTour", _list);
            }
        }
        //public ActionResult ListTourDes(string slug, string item, int? page, int? pageSize)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        Menu menu = db.Menus.FirstOrDefault(x => x.Alias == slug);
        //        ViewBag.slug = slug;
        //        ViewBag.Menu = menu;
        //        int pagenumber = page ?? 1;
        //        int pagesize = pageSize ?? 9;
        //        List<ShowObject> _listActivities = CommentController.GetTours(menu.ID);

        //        switch (item)
        //        {
        //            case "0":
        //                _listActivities = _listActivities.OrderBy(x => x.ID).ToList();
        //                break;
        //            case "1":
        //                _listActivities = _listActivities.OrderBy(x => x.Title).ToList();
        //                break;
        //            case "2":
        //                _listActivities = _listActivities.OrderByDescending(x => x.Title).ToList();
        //                break;
        //            default:
        //                break;
        //        }
        //        IPagedList<ShowObject> _list = _listActivities.ToPagedList(pagenumber, pagesize);

        //        return View("Tour/SpecialTourDes", _list);
        //    }
        //}
      


    }
}