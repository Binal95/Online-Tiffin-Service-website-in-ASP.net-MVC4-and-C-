using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Project.common;
using System.IO;

namespace Project.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult Customer()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            Chatpata_dabbaEntities1 model = new Chatpata_dabbaEntities1();
            var UserDetails = model.CustomerMasters.Where(q => q.CustId == id).FirstOrDefault();
            return View(UserDetails);
            //DateTime.Now;
        }

        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove("UserId");
                HttpContext.Session.RemoveAll();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                return RedirectToAction("Customer");
            }
        }

        public ActionResult Profile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Customer");
            }

            var id = Convert.ToInt32(Session["UserID"]);
            Chatpata_dabbaEntities1 model = new Chatpata_dabbaEntities1();
            var UserDetails = model.CustomerMasters.Where(q => q.CustId == id).FirstOrDefault();
            return View(UserDetails);
        }

        public ActionResult UpdateProfile(CustomerMaster ProfileInfo, FormCollection frm, HttpPostedFileBase file)
        {
            try
            {
                ProfileInfo.CustId = Convert.ToInt32(frm["CustId"]);
                Chatpata_dabbaEntities1 cd = new Chatpata_dabbaEntities1();
                var user = cd.CustomerMasters.Where(q => q.CustId == ProfileInfo.CustId).FirstOrDefault();
                var newFileName = "";
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    FileInfo fi = new FileInfo(pic);
                    string fileextension = fi.Extension.Substring(1).ToUpper();
                    newFileName = Convert.ToString(Guid.NewGuid()) + "." + fileextension;
                    var FolderPath = "~/Images/";
                    var Imgpath = Path.Combine(Server.MapPath(FolderPath), newFileName);
                    file.SaveAs(Imgpath);
                    newFileName = FolderPath + newFileName;
                }
                ProfileInfo.Image = newFileName;
                ProfileInfo.Birthdate = user.Birthdate;
                cd.Entry(user).State = System.Data.Entity.EntityState.Modified;
                cd.SaveChanges();
                return RedirectToAction("Customer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult FeedbackEntry(FormCollection frm)
        {

            try
            {
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                FeedbackMaster fbm = new FeedbackMaster();

                fbm.Feedback = Convert.ToString(frm["Feedback"]);
                fbm.Date = DateTime.Now;
                fbm.CustId = Convert.ToInt16(Session["UserID"]);

                Entity.FeedbackMasters.Add(fbm);
                Entity.SaveChanges();

                return RedirectToAction("Customer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Changepa()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
            var user = en.CustomerMasters.Where(q => q.CustId == id).FirstOrDefault();
            return View(user);
        }

        public ActionResult Verifychpass(FormCollection frm)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }


                Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
                CustomerMaster cm = new CustomerMaster();

                var pass = Convert.ToString(frm["new_pass"]);
                var id = Convert.ToInt32(Session["UserID"]);
                var user = en.CustomerMasters.Where(q => q.CustId == id).FirstOrDefault();
                //user.Password = pass;
                user.Password = MD5Crypt.Encrypt(pass, "Dabba");
                en.Entry(user).State = System.Data.Entity.EntityState.Modified;

                en.SaveChanges();
                var roll = user.Roleid;
                if (roll == 3)
                {
                    return RedirectToAction("Customer", "Customer");
                }
                else if (roll == 1)
                {
                    return RedirectToAction("Admin", "Admin");
                }
                else
                {
                    return RedirectToAction("Driver", "Driver");
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Customer");
            }


        }


        public ActionResult Order()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                var Id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var menudetails = Entity.MenuMasters.ToList();
                return View(menudetails);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult PlaceOrder(int Id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            Session["MenuID"] = Id;
            return View();
        }

        [HttpPost]
        public ActionResult PlaceOrder(FormCollection frm)
        {
            try
            {
                if(Session["MenuID"] == null)
                {
                    return RedirectToAction("Order");
                }
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                var MenuId = Convert.ToInt32(Session["MenuID"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                OrderMaster order = new OrderMaster();
                var email = Entity.CustomerMasters.Where(q => q.CustId == id).FirstOrDefault();
                order.CustomerId = id;
                
                order.MenuId = Convert.ToInt32(Session["MenuID"]);
                order.Date = DateTime.Now;
                order.Status = "Pending";
                order.Description = "abc";
                order.Quantity = Convert.ToInt32(frm["Quantity"]);
                order.StartDate = Convert.ToDateTime(frm["StartDate"]);
                order.EndDate = Convert.ToDateTime(frm["EndDate"]);
                TimeSpan ts = order.EndDate - order.StartDate;
                var NoOfDays =Convert.ToDecimal(ts.Days)+1;
                order.Tax = Convert.ToDecimal(10);
                order.Total = (Entity.MenuMasters.Where(q => q.Id == MenuId).Sum(q=> q.Price)) * order.Quantity * NoOfDays ;
                order.GrandTotal = order.Total + (order.Total * order.Tax / 100);
                order.DeliveryAddr = Convert.ToString(frm["Street"])+","+Convert.ToString(frm["Area"])+","+Convert.ToString(frm["Pincode"]);
              //  order.PaymentMode = "COD";
                Entity.OrderMasters.Add(order);
                Entity.SaveChanges();
                Session["OrderID"] = order.OrderId;
                int ck = Mail.SendMail("Order Details", "Your Order is Placed successfully for Price of Rs."+order.GrandTotal+"Thanks for Order", email.EmailID);
                return RedirectToAction("Payment");
                //   return Json("Success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                throw e;
                // return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewOrderStatus()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var Id = Convert.ToInt32(Session["UserId"]);
   
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var orderdetail = Entity.OrderList().Where(q => q.CustomerId == Id).OrderBy(q=> q.Status).ToList();
                return View(orderdetail);
            }
            catch (Exception e)
            {
                    throw e;
            }
        }
        public ActionResult SampleRequest()
        {

            try
            {
                if (Session["UserId"] == null)
                {
                    return RedirectToAction("Login", "Home");
                }
                var Id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var MenuDetails = Entity.MenuMasters.ToList();
                return View(MenuDetails);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult SampleOrder(int id)
        {

            try
            {
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                RequestMaster rm = new RequestMaster();
                var custid = Convert.ToInt32(Session["UserID"]);
                var chkOrder = Entity.RequestMasters.Where(q => q.CustId == custid).FirstOrDefault();
                var email = Entity.CustomerMasters.Where(q => q.CustId == custid).FirstOrDefault();
              
                if (chkOrder == null)
                {
                    rm.CustId = Convert.ToInt32(Session["UserID"]);
                    rm.MenuId = id;
                    rm.Status = "Pending";
                    rm.Date = DateTime.Now;
                    Entity.RequestMasters.Add(rm);
                    Entity.SaveChanges();
                    int ck = Mail.SendMail("Sample-tiffin", "Your Order for sample tiffin is registered", email.EmailID);
                    TempData["notice"] = "Your Order for sample tiffin is successfully registerd";
                    Session["ReqmenuId"] = rm.MenuId;
                    return RedirectToAction("ViewReqStasus");
                }
                else // (chkOrder.Status == "A")
                {

                  //  if (chkOrder.Status == "Accept" || chkOrder.Status == "Reject")
                    //{
                        TempData["notice"] = "You are alerdy oredred for sample tiffin";
                        rm.MenuId = id;
                        Session["ReqmenuId"] = rm.MenuId;
                        return RedirectToAction("ViewReqStasus", "Customer");
                  //  }
                   

                }

            }
            catch (Exception ex)
            {
                throw ex;

            }


        }

        public ActionResult ViewReqStasus()
        {
            
            try
            {
                if (Session["UserId"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["ReqmenuId"] == null)
                {
                    return RedirectToAction("SampleRequest");
                }
                var Id = Convert.ToInt32(Session["UserId"]);
                var MenuId = Convert.ToInt32(Session["ReqmenuId"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var ReqDetails = Entity.ViewSampleRequest().Where(q => q.CustId == Id && q.MenuId == MenuId).ToList();
                return View(ReqDetails);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Payment()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if(Session["OrderID"]==null)
                {
                    return RedirectToAction("PlaceOrder", "Customer");
                }
                var Id = Convert.ToInt32(Session["UserId"]);
                var MenuId = Convert.ToInt32(Session["MenuID"]);
                var oid = Convert.ToInt32(Session["OrderID"]);
                var currentdate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var orderdetails = Entity.OrderMasters.Where(q => q.OrderId == oid).FirstOrDefault();
                return View(orderdetails);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public ActionResult Payment(FormCollection frm,OrderMaster OM)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var Id = Convert.ToInt32(Session["UserId"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                OM.OrderId = Convert.ToInt32(frm["OrderID"]);
                var orderdetails = Entity.OrderMasters.Where(q => q.OrderId == OM.OrderId).FirstOrDefault();
                if(Convert.ToString(frm["Method"])=="online")
                {
                    orderdetails.PaymentMode = "Online";
         
                }
                if(Convert.ToString(frm["Method"])=="cash")
                {
                    orderdetails.PaymentMode = "COD";
                }
                Entity.Entry(orderdetails).State = System.Data.Entity.EntityState.Modified;
                Entity.SaveChanges();
                return RedirectToAction("OrderReview");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult OrderReview()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var Id = Convert.ToInt32(Session["UserId"]);
                var Oid = Convert.ToInt32(Session["OrderID"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var orderdetail = Entity.OrderList().Where(q => q.CustomerId == Id && q.OrderId == Oid).ToList();
                return View(orderdetail);
            }
            catch (Exception e)
            {
                throw e;
            }
       
        }

        public JsonResult CancelOrder(int oid)
        {
            try
            {
                Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
                var currentTime = Convert.ToDateTime(DateTime.Now.ToShortTimeString());
                var time = Convert.ToDateTime("10:00:00 AM");
                var id = Convert.ToInt32(Session["UserID"]);

                if (currentTime < time)
                {
                    OrderMaster om = new OrderMaster();
                    var ss = en.OrderMasters.Where(q => q.OrderId == oid && q.CustomerId == id).FirstOrDefault();
                    DailyOrder Do = new DailyOrder();
                    var details = en.DailyOrders.Where(q => q.OrderId == oid && q.CustId == id && q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();
                    details[0].Status = "Cancel";

                    if (ss.PaymentMode == "Online")
                    {
                        decimal[] rid = (en.OrderList().Where(q => q.OrderId == oid).Select(q => q.Price)).ToArray();
                        decimal r = Convert.ToDecimal((((rid[0]) * 10) / 100) + rid[0]);
                        ss.Ecoupon = r;
                        en.Entry(details).State = System.Data.Entity.EntityState.Modified;
                        en.SaveChanges();
                    }

                    en.Entry(details[0]).State = System.Data.Entity.EntityState.Modified;
                    en.SaveChanges();
                    return Json("Cancel", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ViewDailyOrder()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }


                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
                OrderMaster om = new OrderMaster();
                var currentdate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                var dailyOrder = en.DailyOrderData().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month && q.CustId==id).OrderBy(q => q.Status).ToList();
            
               // var orderdetails = en.DailyOrder().Where(q => q currentdate 
                 //   && q.CustomerId == id).ToList();  //&&q.Status.Equals("Accept")
              

                return View(dailyOrder);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult CodeRetrival()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var Id = Convert.ToInt32(Session["UserId"]);
            return View();
        }
        [HttpPost]
        public ActionResult CodeRetrival(FormCollection frm)
        {
            var code = Convert.ToString(frm["code"]);

            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            DailyOrder DO = new DailyOrder();
          
            var cid = Convert.ToInt32(Session["UserID"]);

         //   var dbcode = Entity.DailyOrders.Where(q => q.CustId == cid && q.Code == code).Count();
            var dbcode = Entity.DailyStatusChange().LastOrDefault();
           
            if (dbcode != null)
            {
              //  var dbcode1 = Entity.DailyOrders.Where(q => q.CustId == cid && q.Code == code).ToList();
             //   var orderstatus=Entity.
               // dbcode1[0].Status = "Delivered";
                var orderdetails = Entity.DailyOrders.Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();

                var data = orderdetails.Where(q => Convert.ToDecimal(q.Code) == Convert.ToDecimal(dbcode.rcode) && q.OrderId == dbcode.OrderId).ToList();
               
               // var orderdetails = Entity.DailyOrderData().Where(q => q.CustId == dbcode.CustId).FirstOrDefault();
                //var orderchange = Entity.DailyOrders.Where(q => q.CustId == orderdetails.CustId).ToList();
                data[0].Status = "Delivered";
                Entity.Entry(data[0]).State = System.Data.Entity.EntityState.Modified;
                Entity.SaveChanges();
                return RedirectToAction("ThankYouPage");
            }
            else
            {
                return RedirectToAction("CodeRetrival");
            }

        }
        public ActionResult ThankYouPage()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var Id = Convert.ToInt32(Session["UserId"]);
            return View();
        }

        public ActionResult ViewMenuItems(int id)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.MenuID = id;
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                var items = EntityFile.GetMenuItems(id).OrderBy(q => q.Name).ToList();
                return View(items);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Complain()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Complain(FormCollection frm)
        {
            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            Complain com = new Complain();


            var d = 0;
            if (frm["Dname"] != null)
            {
                var dname = frm["Dname"].ToString();
                var did = Entity.CustomerMasters.Where(q => q.FirstName.Equals(dname)).FirstOrDefault();
                if (did != null)
                {
                    d = did.CustId;
                }
            }
            com.CustId = Convert.ToInt32(Session["UserId"]);
            com.Date = DateTime.Now;
            com.Status = "complain";
            com.Description = Convert.ToString(frm["Complain"]);
            com.DriverId = d;

            Entity.Complains.Add(com);
            Entity.SaveChanges();

            return RedirectToAction("Customer");
        }

    }
}
