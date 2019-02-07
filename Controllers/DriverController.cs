using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;

namespace Project.Controllers
{
    public class DriverController : Controller
    {
        //
        // GET: /Driver/

        public ActionResult Driver()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                DailyOrder DO = new DailyOrder();
                var Orders = Entity.DailyOrderData().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).OrderBy(q => q.Status).ToList();
                return View(Orders);

            }

            catch (Exception e)
            {
                throw e;
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
            //return View();
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
                user.Password = MD5Crypt.Encrypt(pass, "Dabba");
                en.Entry(user).State = System.Data.Entity.EntityState.Modified;

                en.SaveChanges();
                return RedirectToAction("Customer", "Customer");

            }
            catch (Exception ex)
            {
                //throw ex;

                return RedirectToAction("Customer");
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
        public ActionResult ViewSampleRequest()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                RequestMaster rm = new RequestMaster();
                var details = Entity.RequestMasters.OrderBy(q => q.Date).ToList();
                return View(details);
            }
            catch (Exception ex)
            {
                throw ex;
            }
      
        }
        public ActionResult ViewOrders()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                OrderMaster OM = new OrderMaster();
                var orderdetails = Entity.OrderMasters.OrderBy(q => q.CustomerMaster.FirstName).ToList();
                var cname = orderdetails.OrderBy(q => q.MenuId).ToList();
                return View(cname);
            }
            catch (Exception e)
            {
                throw e;
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

                var currentdate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();


                var flag = Entity.DailyOrders.Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).Count();

                if (flag == 0)
                {
                    return RedirectToAction("DOEntry");
                }

                var a = Entity.DailyOrders.Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();
                return View(a);
            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult DOEntry()
        {
            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            OrderMaster OM = new OrderMaster();
            DailyOrder chk = new DailyOrder();
            var currentdate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            var orderdetails = Entity.OrderMasters.Where(q => q.StartDate <= currentdate && q.EndDate >= currentdate && (q.Status.Equals("Accept") || q.Status.Equals("Delivered"))).OrderBy(q => q.CustomerMaster.FirstName).ToList();

            var cname = orderdetails.OrderBy(q => q.MenuId).ToList();


            if (cname != null)
            {
                foreach (var order in cname.ToList())
                {
                    chk.CustId = Convert.ToInt32(order.CustomerId);
                    chk.OrderId = Convert.ToInt32(order.OrderId);
                    chk.Date = DateTime.Now;
                    chk.Status = Convert.ToString(order.Status);

                    Entity.DailyOrders.Add(chk);
                    Entity.SaveChanges();
                }
            }

            return RedirectToAction("ViewDailyOrder");
        }

        public ActionResult CodeRetrival()
        {
            return View();
        }

       // [HttpPost]
        // public ActionResult CodeRetrival(FormCollection frm)
        //{
        //    var code = Convert.ToString(frm["code"]);
        //    var mailid = Convert.ToString(frm["mailid"]);
        //    var pw1 = Convert.ToString(frm["pw"]);
        //    var password = MD5Crypt.Encrypt(pw1, "Dabba");

        //    Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();

        //    var chk = Entity.CustomerMasters.Where(q => q.EmailID == mailid && q.Password == password).FirstOrDefault();

        //    if (chk != null)
        //    {

        //        var dbcode = Entity.DailyOrders.Where(q => q.CustId == chk.CustId && q.Code == code).Count();

        //        if (dbcode != 0)
        //        {
        //            var dbcode1 = Entity.DailyOrders.Where(q => q.CustId == chk.CustId && q.Code == code).ToList();

        //            dbcode1[0].Status = "Delivered";

        //            Entity.Entry(dbcode1[0]).State = System.Data.Entity.EntityState.Modified;
        //            Entity.SaveChanges();


        //            return RedirectToAction("ViewDailyOrder");
        //        }
        //        else
        //        {
        //            return RedirectToAction("CodeRetrival");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("CodeRetrival");
        //    }
        //}

        
      public ActionResult DailyOrders()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            var Orders = Entity.DailyOrderData().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).OrderBy(q => q.Status).ToList();
            return View(Orders);
        }

      public ActionResult ViewMenu()
      {
          try
          {
              if (Session["UserID"] == null)
              {
                  return RedirectToAction("Index", "Home");
              }
              var id = Convert.ToInt32(Session["UserID"]);
              Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
              var MenuDetails = EntityFile.MenuMasters.ToList();
              return View(MenuDetails);

          }
          catch (Exception e)
          {
              return RedirectToAction("Login", "Home");
          }
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
    }
}
