using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProjectLibrary.Config;
using ProjectLibrary.Database;
using TeamplateHotel.Models;

namespace TeamplateHotel.Controllers
{
    public class CommentController : BasicController
    {

        //danh sach ngon ngu
        public static List<Language> GetLanguages()
        {
            using (var db = new MyDbDataContext())
            {
                List<Language> languages = db.Languages.ToList();
                return languages;

            }
        }

        //Chi tiết khách sạn
        public static Hotel DetailHotel(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                var hotel =
                    db.Hotels.FirstOrDefault(a => a.LanguageID == languageKey) ??
                    new Hotel();
                return hotel;
            }
        }
        public static Menu GetMenu()
        {
            using (var db = new MyDbDataContext())
            {
                var menu = db.Menus.FirstOrDefault(x => x.Type == SystemMenuType.Tour && x.Level == 0);
                return menu;
            }
        }
        //public static List<AboutUs> GetAboutUs(string languageKey) 
        //{
        //    using (var db = new MyDbDataContext()) 
        //    {
        //        var result = db.AboutUs.Where(a => a.LanguageID == languageKey).ToList();
        //        return result;
        //    }
        //}

        //public static List<Employee> GetEmployee(string languageKey)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        var result = db.Employees.Where(a => a.LanguageID == languageKey).ToList();
        //        return result;
        //    }
        //}

        //Danh sách Main menu
        public static List<Menu> GetMainMenus(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                //var menus = from m in db.Menus
                //            where m.Status == true && m.Location == SystemMenuLocation.MainMenu
                //            orderby m.Index
                //            select m;

                List<Menu> menus = db.Menus.Where(a => a.Status == true && a.Location == SystemMenuLocation.MainMenu).OrderBy(a => a.Index).ToList();
                return menus.ToList();
            }
        }

        public static List<Menu> GetMenusActivities(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<Menu> menus = db.Menus.Where(a => a.Status && a.Location == SystemMenuLocation.MainMenu
                                                        && a.LanguageID == languageKey
                                                        && a.Type == SystemMenuType.Activities).ToList();
                return menus;
            }
        }

        public static List<Menu> GetMenuDes(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<Menu> menus = db.Menus.Where(a => a.Status && a.Location == SystemMenuLocation.MainMenu && a.LanguageID == languageKey && a.Type == SystemMenuType.Tour).ToList();
                return menus;
            }
        }

        //Danh sách Second menu
        public static List<Menu> GetSecondMenus(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<Menu> menufooter = db.Menus.Where(a => a.Status && a.Location == SystemMenuLocation.SecondMenu &&
                                                       a.LanguageID == languageKey).ToList();
                return menufooter;
            }
        }

        //Danh sách Second menu
        //public static List<Gallery> Gallery()
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        List<Gallery> galleries = db.Galleries.OrderBy(a => a.Index).ToList();
        //        return galleries;
        //    }
        //}

        //public static List<Menu> GetMenuInGallery(int menuId)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        var menus = db.Menus.Where(a => a.ParentID == menuId).OrderBy(a => a.Index).ToList();

        //        return menus;
        //    }
        //}

        //get plugin
        public static Plugin GetPluigPlugin()
        {
            using (var db = new MyDbDataContext())
            {
                return db.Plugins.FirstOrDefault() ?? new Plugin();
            }
        }

        //Danh sach slider
        public static List<Slider> GetListSlider(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                return db.Sliders.Where(a => a.LanguageID == languageKey && a.Status).ToList();
                //lấy menu hiển thị tất cả
            }
        }

        //Danh sach quang cao
        //public static List<Advertising> GetAdvertisings()
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        List<Advertising> advertisings = db.Advertisings.Where(a => a.Status).ToList();
        //        return advertisings;
        //    }
        //}

        //Danh sách bài viết theo chuyên mục
        public static List<Article> GetArticles(int menuId)
        {
            using (var db = new MyDbDataContext())
            {
                List<Article> articles = db.Articles.Where(a => a.MenuID == menuId && a.Status && a.Hot == false).OrderBy(a => a.Index).ToList();
                foreach (var article in articles)
                {
                    article.MenuAlias = article.Menu.Alias;
                }
                return articles;
            }
        }
        //Danh sách tap article
        public static List<TabArticle> GetTapArticles()
        {
            using (var db = new MyDbDataContext())
            {
                return db.TabArticles.ToList();
            }
        }
        //Chi tiết bài viết
        public static DetailArticle Detail_Article(int id)
        {
            using (var db = new MyDbDataContext())
            {
                Article article = db.Articles.FirstOrDefault(a => a.ID == id && a.Status) ?? new Article();
                List<Article> articles = db.Articles.Where(a => a.MenuID == article.MenuID && a.Status && a.ID != article.ID).OrderBy(a => a.Index).ToList();
                foreach (var item in articles)
                {
                    item.MenuAlias = article.Menu.Alias;
                }
                DetailArticle detailArticle = new DetailArticle()
                {
                    Article = article,
                    Articles = articles
                };
                return detailArticle;
            }
        }

        //Danh sách phòng
        //public static List<Room> GetRooms(int menuId)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        var menu = db.Menus.FirstOrDefault(a => a.ID == menuId);
        //        List<Room> rooms = db.Rooms.Where(a => a.Status && a.LanguageID == menu.LanguageID).OrderBy(a => a.Index).ToList();
        //        foreach (var room in rooms)
        //        {
        //            room.MenuAlias = menu.Alias;
        //        }
        //        return rooms;
        //    }
        //}

        //Chi tiết phòng
        //public static DetailRoom Detail_Room(int id, int menuId)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        var menu = db.Menus.FirstOrDefault(a => a.ID == menuId);
        //        Room room = db.Rooms.FirstOrDefault(a => a.ID == id && a.Status) ?? new Room();
        //        List<RoomGallery> roomGalleries = db.RoomGalleries.Where(a => a.RoomId == room.ID).ToList();
        //        List<Room> rooms = db.Rooms.Where(a => a.Status && a.ID != room.ID && a.LanguageID == menu.LanguageID).OrderBy(a => a.Index).ToList();

        //        foreach (var item in rooms)
        //        {
        //            item.MenuAlias = menu.Alias;
        //        }
        //        DetailRoom detailRoom = new DetailRoom()
        //        {
        //            Room = room,
        //            RoomGalleries = roomGalleries,
        //            Rooms = rooms
        //        };
        //        return detailRoom;
        //    }
        //}

        //Danh sách dich vu
        //public static List<Service> GetServices(int menuId)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        List<Service> restaurants = db.Services.Where(a => a.Status && a.MenuID == menuId).OrderBy(a => a.Index).ToList();
        //        foreach (var restaurant in restaurants)
        //        {
        //            restaurant.MenuAlias = restaurant.Menu.Alias;
        //        }
        //        return restaurants;
        //    }
        //}

        //Chi tiết dich vu
        //public static DetailService Detail_Service(int id)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        Service service = db.Services.FirstOrDefault(a => a.ID == id && a.Status) ?? new Service();
        //        List<ServiceGallery> restaurantGalleries = db.ServiceGalleries.Where(a => a.ServiceID == service.ID).ToList();
        //        List<Service> restaurants = db.Services.Where(a => a.Status && a.ID != service.ID).OrderBy(a => a.Index).ToList();
        //        foreach (var item in restaurants)
        //        {
        //            item.MenuAlias = service.Menu.Alias;
        //        }
        //        DetailService detailRestaurant = new DetailService()
        //        {
        //            Service = service,
        //            ServiceGalleries = restaurantGalleries,
        //            Services = restaurants
        //        };
        //        return detailRestaurant;
        //    }
        //}

        //Danh sách tours
        public static List<ShowObject> GetTours(int menuId)
        {
            using (var db = new MyDbDataContext())
            {
                Menu _menuDes = db.Menus.FirstOrDefault(x => x.ID == menuId);
                var query = db.Tours.Where(x => x.Status)
                  .Join(db.ThemesMenus.Where(x => x.MenuID == menuId), a => a.ID, b => b.TourID, (a, b) => new { a, b });
                var value = query.Join(db.Menus, c => c.a.ActivitiesID, d => d.ID, (c, d) => new ShowObject
                {
                    ID = c.a.ID,
                    Alias = c.a.Alias,
                    MenuAlias = _menuDes.Alias,
                    Themes = d.Title,
                    Title = c.a.Title,
                    Index = c.a.Index,
                    Image = c.a.Image,
                    Location = c.a.Location,
                    Description = c.a.Description,
                    Price = (decimal)c.a.Price,
                    PriceSale = (decimal)c.a.PriceSale
                }).Distinct().ToList();

                return value;
            }
        }

        //Danh sách Activities
        public static List<ShowObject> GetToursActivities(int menuId)
        {
            using (var db = new MyDbDataContext())
            {
                Menu _menuDes = db.Menus.FirstOrDefault(x => x.ID == menuId);
                var tour = db.Tours.Where(x => x.ActivitiesID == menuId).Include("Menu")
                    .Join(db.Menus, a => a.ActivitiesID, b => b.ID, (a, b) => new ShowObject
                    {
                        ID = a.ID,
                        Alias = a.Alias,
                        MenuAlias = a.Menu.Alias,
                        Themes = b.Title,
                        Title = a.Title,
                        Index = a.Index,
                        Image = a.Image,
                        Location = a.Location,
                        Description = a.Description,
                        Price = (decimal)a.Price,
                        PriceSale = (decimal)a.PriceSale
                    }).Distinct().ToList();
                return tour;
            }
        }

        //danh sach tour hiển thị side bar
        public static List<Tour> ShowTourSidebar(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<Tour> tours = db.Tours.Where(a => a.Status && a.TourOther == true).OrderBy(a => a.Index).ToList();
                foreach (var tour in tours)
                {
                    tour.MenuAlias = tour.Menu.Alias;
                }
                return tours;
            }
        }

        #region Tau

        //lay danh sach tau theo menu ID
        public static List<Cruise> GetCruise(int menuId)
        {
            using (var db = new MyDbDataContext())
            {
                List<Cruise> cruises = db.Cruises.OrderBy(a => a.ID).ToList();
                foreach (var item in cruises)
                {
                    item.tabs = db.Cruisetabs.Where(a => a.IDCruise == item.ID).ToList();
                }
                List<Cruise> sp = new List<Cruise>();
                cruises.ForEach(a =>
                {
                    if (string.IsNullOrEmpty(a.MenuID) == false)
                    {
                        List<string> lstSubMenu = a.MenuID.Split(',').ToList();

                        if (lstSubMenu.Any(b => b.Contains(menuId.ToString())))
                            sp.Add(a);
                    }
                });
                return sp;
            }
        }

        //Chi tiết Cruise
        public static DetailCruise Detail_Cruise(int id, int menuId)
        {
            using (var db = new MyDbDataContext())
            {
                Cruise cruise = db.Cruises.FirstOrDefault(a => a.ID == id);

                List<Cruisetab> cruiseTabs = db.Cruisetabs.Where(a => a.IDCruise == cruise.ID).ToList();
                List<Cabin> cruiceCabin = db.Cabins.Where(a => a.IDCruise == cruise.ID).ToList();
                List<Cruise> cruises = db.Cruises.Where(a => a.MenuID == menuId.ToString()).OrderBy(a => a.ID).ToList();
                //foreach (var item in tours)
                //{
                //    item.MenuAlias = item.Menu.Alias;
                //}
                DetailCruise detailcCruise = new DetailCruise()
                {
                    CruiseCabin = cruiceCabin,
                    Cruise = cruise,
                    Cruises = cruises,
                    Cruisetabs = cruiseTabs
                };
                return detailcCruise;
            }
        }

        // chiet tiết cruise

        #endregion Tau

        //Danh sách tours khác
        public static List<Tour> OtherTours()
        {
            using (var db = new MyDbDataContext())
            {
                List<Tour> tours = db.Tours.Where(a => a.Status && a.TourOther == true).OrderBy(a => a.Index).ToList();
                foreach (var tour in tours)
                {
                    tour.MenuAlias = tour.Menu.Alias;
                }
                return tours;
            }
        }

        public static decimal TotalSale()
        {
            using (var db = new MyDbDataContext())
            {
                decimal totalSale = db.Tours.Sum(x => 100 - (x.PriceSale / x.Price) * 100).Value;
                return totalSale;
            }
        }

        //Chi tiết tour
        public static DetailTour Detail_Tour(int id)
        {
            using (var db = new MyDbDataContext())
            {
                Tour tour = db.Tours.FirstOrDefault(a => a.ID == id && a.Status) ?? new Tour();
                List<TourGallery> tourGalleries = db.TourGalleries.Where(a => a.TourID == tour.ID).ToList();
                List<TabTour> tabTours = db.TabTours.Where(a => a.TourID == tour.ID).ToList();
                List<Tour> tours = db.Tours.Where(a => a.Status && a.ID != tour.ID && a.MenuID == tour.MenuID).OrderBy(a => a.Index).ToList();
                foreach (var item in tours)
                {
                    item.MenuAlias = item.Menu.Alias;
                }
                DetailTour detailTour = new DetailTour()
                {
                    Tour = tour,
                    TourGalleries = tourGalleries,
                    Tours = tours,
                    TabTours = tabTours,
                    Title = tour.Menu.Title,
                    AliasMenu = tour.Menu.Alias,
                };
                return detailTour;
            }
        }

        //tour hot
        public static List<ShowObject> Tourhots(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                var tour = db.Tours.Where(x => x.Like == true && x.Status).Include("Menu")
                    .Join(db.Menus, a => a.ActivitiesID, b => b.ID, (a, b) => new ShowObject
                    {
                        ID = a.ID,
                        Alias = a.Alias,
                        MenuAlias = a.Menu.Alias,
                        Themes = b.Title,
                        Title = a.Title,
                        Index = a.Index,
                        Image = a.Image,
                        Location = a.Location,
                        Description = a.Description,
                        Price = (decimal)a.Price,
                        PriceSale = (decimal)a.PriceSale
                    }).Distinct().ToList();
                return tour;
            }
        }

        //Tour like
        public static List<ShowObject> TourLike(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {              
                var tour = db.Tours.Where(x => x.Like == true && x.Status).Include("Menu")
                    .Join(db.Menus, a => a.ActivitiesID, b => b.ID, (a, b) => new ShowObject
                    {
                        ID = a.ID,
                        Alias = a.Alias,
                        MenuAlias = a.Menu.Alias,
                        Themes = b.Title,
                        Title = a.Title,
                        Index = a.Index,
                        Image = a.Image,
                        Location = a.Location,
                        Description = a.Description,
                        Price = (decimal)a.Price,
                        PriceSale = (decimal)a.PriceSale
                    }).Distinct().ToList();
                return tour;
            }
        }

        //ttour sale
        public static List<ShowObject> TourDeal(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<ShowObject> tourssales = db.Tours.Where(a => (bool)a.Deal && a.Status)
                        .Join(db.Menus.Where(a => a.LanguageID == languageKey), a => a.MenuID, b => b.ID,
                            (a, b) => new ShowObject
                            {
                                ID = a.ID,
                                Alias = a.Alias,
                                MenuAlias = b.Alias,
                                Title = a.Title,
                                Index = a.Index,
                                Image = a.Image,
                                Description = a.Description,
                                Price = (a.Price == null) ? 0 : (decimal)a.Price,
                                PriceSale = (a.PriceSale == null) ? 0 : (decimal)a.PriceSale,

                            }).ToList();
                return tourssales;
            }
        }

        ///////////// Trang home ////////////////////////
        //Bai viet welcome
        public static ShowObject WellcomeArticle(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                return
                    db.Articles.Where(a => a.Home && a.Status)
                        .Join(db.Menus.Where(a => a.LanguageID == languageKey), a => a.MenuID, b => b.ID,
                            (a, b) => new ShowObject()
                            {
                                ID = a.ID,
                                Alias = a.Alias,
                                MenuAlias = b.Alias,
                                Title = a.Title,
                                Index = a.Index,
                                Image = a.Image,
                                Description = a.Description
                            }).FirstOrDefault();
            }
        }

        //Bai viet hot
        public static List<ShowObject> HotArticles(string languageKey, int menuId)
        {
            using (var db = new MyDbDataContext())
            {
                List<ShowObject> articleHots = db.Articles.Where(a => a.MenuID == menuId && a.Hot && a.Status)
                        .Join(db.Menus.Where(a => a.LanguageID == languageKey), a => a.MenuID, b => b.ID,
                            (a, b) => new ShowObject
                            {
                                ID = a.ID,
                                Alias = a.Alias,
                                MenuAlias = b.Alias,
                                Title = a.Title,
                                Index = a.Index,
                                Image = a.Image,
                                Description = a.Description,
                            }).Take(3).ToList();
                return articleHots;
            }
        }

        //Bai viet New
        public static List<ShowObject> NewArticles(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<ShowObject> NewArticle = db.Articles.Where(a => a.New && a.Status)
                        .Join(db.Menus.Where(a => a.LanguageID == languageKey), a => a.MenuID, b => b.ID,
                            (a, b) => new ShowObject
                            {
                                ID = a.ID,
                                Alias = a.Alias,
                                MenuAlias = b.Alias,
                                Title = a.Title,
                                Index = a.Index,
                                Image = a.Image,
                                Description = a.Description,
                                MenuID = a.MenuID
                            }).ToList();
                return NewArticle;
            }
        }

        //Bai viet giá trị
        public static List<ShowObject> PartnerArticles(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<ShowObject> partnerarticle = db.Articles.Where(a => a.Value && a.Status)
                        .Join(db.Menus.Where(a => a.LanguageID == languageKey), a => a.MenuID, b => b.ID,
                            (a, b) => new ShowObject
                            {
                                ID = a.ID,
                                Alias = a.Alias,
                                MenuAlias = b.Alias,
                                Title = a.Title,
                                Index = a.Index,
                                Image = a.Image,
                                Description = a.Description,
                            }).OrderBy(a => a.Index).ToList();
                return partnerarticle;
            }
        }

        //Bai viet nhân viên
        public static List<ShowObject> AboutArticles(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                List<ShowObject> partnerarticle = db.Articles.Where(a => a.About && a.Status)
                        .Join(db.Menus.Where(a => a.LanguageID == languageKey), a => a.MenuID, b => b.ID,
                            (a, b) => new ShowObject
                            {
                                ID = a.ID,
                                Alias = a.Alias,
                                MenuAlias = b.Alias,
                                Title = a.Title,
                                Index = a.Index,
                                Image = a.Image,
                                Description = a.Description,
                            }).OrderBy(a => a.Index).ToList();
                return partnerarticle;
            }
        }
        // all tour
        public static List<ShowObject> TourAll(string languageKey)
        {
            using (var db = new MyDbDataContext())
            {
                var toursAll = from tour in db.Tours
                               join menu in db.Menus on tour.MenuID equals menu.ID
                               where tour.Status == true
                               select new ShowObject
                               {
                                   ID = tour.ID,
                                   Alias = tour.Alias,
                                   MenuID = tour.MenuID,
                                   MenuAlias = menu.Alias,
                                   Title = tour.Title,
                                   Index = tour.Index,
                                   Image = tour.Image,
                                   Description = tour.Description,
                                   Price = (tour.Price == null) ? 0 : (decimal)tour.Price,
                                   PriceSale = (tour.PriceSale == null) ? 0 : (decimal)tour.PriceSale,
                               };
                return toursAll.ToList();
            }



        }

        //phòng show home
        //public static List<ShowObject> RoomShowHome(string languageKey)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        var memu =
        //            db.Menus.FirstOrDefault(a => a.LanguageID == languageKey) ??
        //            new Menu();
        //        List<ShowObject> roomShowHome = db.Rooms.Where(a => a.Home && a.Status && a.LanguageID == languageKey).Select(a => new ShowObject
        //        {
        //            ID = a.ID,
        //            Alias = a.Alias,
        //            MenuAlias = memu.Alias,
        //            Title = a.Title,
        //            Index = a.Index,
        //            Image = a.Image,
        //            Description = a.Description,
        //            Price = a.Price,
        //        }).ToList();
        //        return roomShowHome;
        //    }
        //}

        //lay danh sach tau theo menu ID
        //public static List<Cruise> GetCruise(int menuId)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        List<Cruise> lstCruise = db.Cruises.Where(x => x.MenuID.Contains(menuId.ToString())).ToList();
        //        foreach (var item in lstCruise)
        //        {
        //            //item.
        //        }
        //        return lstCruise;
        //    }
        //}

        // GetListReview

        //public static List<Review> GetListReview(int menuID, int ObjID)
        //{
        //    using (var db = new MyDbDataContext())
        //    {
        //        List<Review> lstReview = db.Reviews.Where(
        //            x => x.DisplayStatus == true && x.ObjID == ObjID && x.MenuID == menuID
        //            ).ToList();
        //        return lstReview;
        //    }
        //}

        public static decimal GetTimeSale(DateTime? _StartDeal, int? TimeDeal)
        {
            DateTime StartDeal;
            if (!DateTime.TryParse(_StartDeal.ToString(), out StartDeal))
            {
                StartDeal = DateTime.Now;
            }
            TimeDeal = TimeDeal == null ? 0 : TimeDeal;
            int timemer = 0;
            TimeSpan d = StartDeal.AddDays((double)TimeDeal / 24) - DateTime.Now;
            if (d.Days > 0)
            {
                timemer = d.Days * 24 + ((int)TimeDeal % 24) - DateTime.Now.Hour;
                if (timemer <= 0)
                {
                    timemer = 0;
                }

            }
            if (d.Days == 0)
            {
                timemer = d.Days * 24 + ((int)TimeDeal % 24) - DateTime.Now.Hour;
                if (timemer <= 0)
                {
                    return timemer * 0;
                }
                else
                {
                    return timemer * 3600;
                }
            }
            return timemer * 3600 + 86400;
        }
    }
}