//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using ProjectLibrary.Config;
//using ProjectLibrary.Database;
//using System.Web;
//using System.IO;
//using ProjectLibrary.Utility;
//using TeamplateHotel.Models;
//using TeamplateHotel.Handler;
//using System.Xml;

//namespace TeamplateHotel.Controllers
//{
//    public class BookingTourController : Controller
//    {
//        GET: /BookingTour/

//        [HttpPost]
//        public ActionResult SendBooking(BookTour model)
//        {
//            string status = "success";
//            try
//            {
//                using (var db = new MyDbDataContext())
//                {
//                    Hotel hotel = CommentController.DetailHotel(Request.Cookies["LanguageID"].Value);
//                    SendEmail sendEmail =
//                           db.SendEmails.FirstOrDefault(
//                               a => a.Type == TypeSendEmail.BookTour && a.LanguageID == Request.Cookies["LanguageID"].Value);
//                    model.CreateDate = DateTime.Now;
//                    model.LanguageID = Request.Cookies["LanguageID"].Value;
//                    db.BookTours.InsertOnSubmit(model);
//                    db.SubmitChanges();

//                    sendEmail.Title = sendEmail.Title.Replace("{HotelName}", hotel.Name);
//                    string content = sendEmail.Content;
//                    content = content.Replace("{HotelName}", hotel.Name);
//                    content = content.Replace("{Code}", model.Code);
//                    content = content.Replace("{Departure}", model.Departure);
//                    content = content.Replace("{Adult}", model.Adult.ToString());
//                    content = content.Replace("{Child}", model.Child.ToString());
//                    content = content.Replace("{Price}", model.Total.ToString());
//                    content = content.Replace("{TravelType}", model.Activities);
//                    content = content.Replace("{FullName}", model.FullName);
//                    content = content.Replace("{Tel}", model.Tel);
//                    content = content.Replace("{Email}", model.Email);

//                    content = content.Replace("{Add}", hotel.Address);
//                    content = content.Replace("{Hotline}", hotel.Hotline);
//                    content = content.Replace("{EmailHotel}", hotel.Email);
//                    content = content.Replace("{Website}", hotel.Website);

//                    MailHelper.SendMail(model.Email, sendEmail.Title, content);
//                    MailHelper.SendMail(hotel.Email, hotel.Name + " Booking tour of " + model.FullName, content);
//                    return Redirect("/BookTour/Messages?status=" + status);

//                }
//            }
//            catch (Exception)
//            {
//                status = "error";
//            }

//            return Redirect("/BookTour/Messages?status=" + status);
//        }

//        [HttpGet]
//        public ActionResult Messages()
//        {
//            using (var db = new MyDbDataContext())
//            {
//                SendEmail sendEmail =
//                       db.SendEmails.FirstOrDefault(
//                           a => a.Type == TypeSendEmail.BookTour && a.LanguageID == Request.Cookies["LanguageID"].Value);

//                string status = Request.Params["status"];
//                ViewBag.Messages = "";
//                if (string.IsNullOrEmpty(status) == false)
//                {
//                    if (status.Equals("success"))
//                    {
//                        ViewBag.Messages = sendEmail.Success;
//                    }
//                    else
//                    {
//                        ViewBag.Messages = sendEmail.Error;
//                    }
//                }
//                else
//                {
//                    ViewBag.Messages = sendEmail.Error;
//                }
//                return View();
//            }
//        }

//        [HttpPost]
//        public JsonResult CheckCode(string code, int IDTour)
//        {
//            try
//            {
//                using (var db = new MyDbDataContext())
//                {
//                    DateTime today = DateTime.Now;
//                    PromotionCode Pcode = db.PromotionCodes.FirstOrDefault(c => c.Code == code
//                    && today <= c.EndDay && today >= c.StartDay && c.Status == true);
//                    if (Pcode != null)
//                    {
//                        /*
//                         1-het ma
//                         2-ok co ma va con luot dung
//                         3-loi ko co ma nao
//                         */
//                        if (Pcode.Used >= Pcode.Total)
//                        {
//                            return Json(new { Result = "OK", Message = "1" });
//                        }
//                        else
//                        {
//                            return Json(new { Result = "OK", Message = "2", rate = Pcode.Rate / 100 });
//                        }
//                    }
//                    return Json(new { Result = "ERROR", Message = "3" });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { Result = "ERROR", ex.Message });
//            }
//        }

//        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG" };

//        public ActionResult SubmitInvoidOnePay(string idOrder, double deposit)
//        {
//            PaymentConfigOnePay pay = new PaymentConfigOnePay();
//            using (var db = new MyDbDataContext())
//            {
//                pay = db.PaymentConfigOnePays.FirstOrDefault();
//            }
//            if (pay != null)
//            {
//                System.Web.HttpContext context = System.Web.HttpContext.Current;
//                string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
//                VPCRequest conn = new VPCRequest("https://onepay.vn/vpcpay/vpcpay.op");

//                conn.SetSecureSecret(pay.Hashcode);
//                conn.AddDigitalOrderField("AgainLink", "http://onepay.vn");
//                conn.AddDigitalOrderField("Title", "onepay paygate");
//                conn.AddDigitalOrderField("vpc_Locale", "en");
//                conn.AddDigitalOrderField("vpc_Version", "2");
//                conn.AddDigitalOrderField("vpc_Command", "pay");
//                conn.AddDigitalOrderField("vpc_Merchant", pay.MerchantId);
//                conn.AddDigitalOrderField("vpc_AccessCode", pay.AccessCode);
//                conn.AddDigitalOrderField("vpc_MerchTxnRef", idOrder);
//                conn.AddDigitalOrderField("vpc_OrderInfo", idOrder);
//                conn.AddDigitalOrderField("vpc_Amount", deposit.ToString());
//                conn.AddDigitalOrderField("vpc_ReturnURL", pay.WebSite + "/BookTour/MessageOnePay");
//                conn.AddDigitalOrderField("vpc_TicketNo", ip);
//                String url = conn.Create3PartyQueryString();
//                return Redirect(url);
//            }
//            else
//            {
//                return View();
//            }
//        }

//        [HttpGet]
//        public ActionResult MessageOnePay(string vpc_OrderInfo, string vpc_TxnResponseCode, string vpc_SecureHash)
//        {
//            PaymentConfigOnePay paymentConfig = new PaymentConfigOnePay();
//            using (var db = new MyDbDataContext())
//            {
//                paymentConfig = db.PaymentConfigOnePays.FirstOrDefault();
//                SendEmail sendEmail =
//                      db.SendEmails.FirstOrDefault(
//                          a => a.Type == TypeSendEmail.BookTour && a.LanguageID == Request.Cookies["LanguageID"].Value);

//                int state = 0;
//                string message = "";
//                var conn = new VPCRequest("https://onepay.vn/vpcpay/vpcpost.op");
//                conn.SetSecureSecret(paymentConfig.Hashcode);
//                Xu ly tham so tra ve va kiem tra chuoi du lieu ma hoa
//                string hashvalidateResult = conn.Process3PartyResponse(Request.QueryString);
//                Lay gia tri tham so tra ve tu cong thanh toan
//               var merchTxnRef = conn.GetResultField("vpc_MerchTxnRef", "Unknown");
//                if (hashvalidateResult == "CORRECTED" && vpc_TxnResponseCode.Trim() == "0")
//                {
//                    message = "Transaction was paid successful";
//                    state = 1;
//                    ViewBag.Messages = sendEmail.Success;
//                }
//                else if (hashvalidateResult == "INVALIDATED" && vpc_TxnResponseCode.Trim() == "0")
//                {
//                    message = "Transaction is pending";
//                    state = 2;
//                    ViewBag.Messages = sendEmail.Error;
//                }
//                else
//                {
//                    message = "Transaction was not paid successful";
//                    state = 3;
//                    ViewBag.Messages = sendEmail.Error;
//                }
//                ViewBag.Message = message;
//                ViewBag.State = state;
//                if (state == 1)
//                {
//                    do something
//                    return View();
//                }
//                return View();
//            }
//        }

//        Add Review Clident
//        [HttpPost]
//        public ActionResult AddReview(Review rv, string Country, HttpPostedFileBase file)
//        {
//            ViewBag.Status = 0;
//            // Xử lý file up lên
//            try
//            {
//                if (file != null && file.ContentLength <= 300000)
//                {
//                    if (ImageExtensions.Contains(Path.GetExtension(file.FileName).ToUpperInvariant()))
//                    {
//                        var fileName = StringHelper.ConvertToAlias(rv.FullName) + '-' + DateTime.Now.ToString("yyyymmddMMss") + System.IO.Path.GetExtension(file.FileName);
//                        string path = Path.Combine(Server.MapPath("~/Files/Images/AvataUser"), fileName);

//                        file.SaveAs(path);
//                        ///Xử lý dữ liệu
//                        ///
//                        rv.ProfileImages = "/Files/Images/AvataUser/" + fileName;
//                        rv.TimeReview = DateTime.Now;
//                        rv.DisplayStatus = false;
//                        rv.Address = rv.Address + " - " + Country;
//                        MyDbDataContext db = new MyDbDataContext();
//                        db.Reviews.InsertOnSubmit(rv);
//                        db.SubmitChanges();
//                        ViewBag.name = rv.FullName;
//                    }
//                    else
//                    {
//                        ViewBag.notifi = "Unsupported file type has been uploaded";
//                        ViewBag.Status = 2;
//                    }
//                }
//                else
//                {
//                    ViewBag.notifi = "file too large!";
//                    ViewBag.Status = 2;
//                }
//            }
//            catch (Exception e)
//            {
//                ViewBag.Status = 1;
//                ViewBag.description = e.Message;
//            }
//            return View();
//        }

//        public JsonResult LoadDataReview(int pageNumber, int menuID, int ID)
//        {
//            using (var db = new MyDbDataContext())
//            {
//                var lst = CommentController.GetListReview(menuID, ID);
//                var data = lst.Select(a => new
//                {
//                    a.FullName,
//                    a.ProfileImages,
//                    TimeReview = a.TimeReview.ToString("dd/MM/yyy"),
//                    a.KindOfTrip,
//                    a.UseService,
//                    rate = new int[a.Point],
//                    unrate = new int[5 - a.Point],
//                    a.Title,
//                    a.Content,
//                    a.ItineraryPoint,
//                    a.FoodDrinkPoint,
//                    a.AccomodationsPoint,
//                    a.GuidePoint,
//                    a.ActivityPoint
//                }).Take(pageNumber).ToList();
//                if (pageNumber > data.Count() + 5)
//                {
//                    return Json(new { status = false, data = data });
//                }
//                return Json(new { status = true, data = data });
//            }
//        }
//    }
//}