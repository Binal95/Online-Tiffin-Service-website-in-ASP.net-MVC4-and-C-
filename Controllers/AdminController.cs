
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using System.IO;
using Project.common;


namespace Project.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Admin()
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
                if (Orders.Count != 0 )
                {
                    return View(Orders);
                  
                }
                else
                {
                    return RedirectToAction("ViewDailyOrder");
                    
                }
            
            }

            catch (Exception e)
            {
                throw e;
            }
            
        }

        public ActionResult DriverReg()
        {
            return View();
        }
        
        public ActionResult AddDriver(FormCollection frm)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();

                CustomerMaster reg = new CustomerMaster();

                reg.Roleid = 2;

                reg.FirstName = Convert.ToString(frm["FName"]);
                reg.LastName = Convert.ToString(frm["LName"]);
                reg.Address1 = Convert.ToString(frm["Address1"]);
                reg.Address2 = Convert.ToString(frm["Address2"]);
                reg.Address3 = Convert.ToString(frm["Address3"]);

                reg.Pincode = Convert.ToString(frm["Pincode"]);

                reg.PhoneNo = Convert.ToString(frm["PhoneNo"]);
                //reg.Gender = "";

                //reg.Birthdate = Convert.ToDateTime(frm["Birthdate"]);

                string[] dtarray = frm["Birthdate"].ToString().Split('/');
                reg.Birthdate = Convert.ToDateTime("" + dtarray[1] + "/" + dtarray[0] + "/" + dtarray[2]);

                reg.EmailID = Convert.ToString(frm["EmailID"]);

                reg.Password = MD5Crypt.Encrypt(frm["Password"], "Dabba");
                EntityFile.CustomerMasters.Add(reg);
                EntityFile.SaveChanges();

                return RedirectToAction("Admin", "Admin");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
         
        public ActionResult Area()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
            var AllUsers = EntityFile.AreaMasters.ToList();
            return View(AllUsers);

        }


        public ActionResult Addarea(FormCollection frm)
        {
            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                AreaMaster reg = new AreaMaster();

                reg.Name = Convert.ToString(frm["Name"]);
                EntityFile.AreaMasters.Add(reg);
                EntityFile.SaveChanges();

                return RedirectToAction("Admin", "Admin");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JsonResult CheckAreaName(string Name)
        {
            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            var checkName = Entity.AreaMasters.Where(q => q.Name == Name).FirstOrDefault();
            if (checkName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllArea()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
            var AllUsers = EntityFile.AreaMasters.ToList();

            return View(AllUsers);
        }

        public JsonResult DeleteArea(int id)
        {
            try
            {

                Chatpata_dabbaEntities1 model = new Chatpata_dabbaEntities1();
                var UserDetails = model.AreaMasters.Where(q => q.Id == id).FirstOrDefault();
                model.AreaMasters.Remove(UserDetails);
                model.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
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
                user.Password = MD5Crypt.Encrypt(pass, "Dabba");
                en.Entry(user).State = System.Data.Entity.EntityState.Modified;

                en.SaveChanges();
                return RedirectToAction("Admin", "Admin");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Admin");
            }


        }


        public ActionResult UpdateProfile(CustomerMaster ProfileInfo, FormCollection frm)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                ProfileInfo.CustId = Convert.ToInt32(frm["CustId"]);
                Chatpata_dabbaEntities1 cd = new Chatpata_dabbaEntities1();
                var user = cd.CustomerMasters.Where(q => q.CustId == ProfileInfo.CustId).FirstOrDefault();
                ProfileInfo.Birthdate = user.Birthdate;
                ProfileInfo.FirstName = user.FirstName;
                user.LastName = Convert.ToString(frm["LastName"]);
                user.Address1 = Convert.ToString(frm["Address1"]);
                user.Address2 = Convert.ToString(frm["Address2"]);
                user.Address3 = Convert.ToString(frm["Address3"]);
                user.Pincode = Convert.ToString(frm["Pincode"]);
                user.PhoneNo = Convert.ToString(frm["PhoneNo"]);

                cd.Entry(user).State = System.Data.Entity.EntityState.Modified;
                cd.SaveChanges();
                return RedirectToAction("Customer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AddItem()
        {
            return View("AddItem");
        }

        [HttpPost]
        public ActionResult AddItem(FormCollection frm, HttpPostedFileBase file)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                ItemMaster im = new ItemMaster();
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

                im.Name = Convert.ToString(frm["Name"]);
                im.Description = Convert.ToString(frm["Description"]);
                im.Image = Convert.ToString(frm["Image"]);
                im.Price = Convert.ToDecimal(frm["Price"]);
                im.Image = newFileName;
                EntityFile.ItemMasters.Add(im);
                EntityFile.SaveChanges();


                return RedirectToAction("Admin", "Admin");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public JsonResult CheckItemName(string Name)
        {
            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            var checkName = Entity.ItemMasters.Where(q => q.Name == Name).FirstOrDefault();
            if (checkName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ViewItem()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                var ItemDetails = EntityFile.ItemMasters.OrderBy(q=> q.Name).ToList();
                return View(ItemDetails);

            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public JsonResult DeleteItem(int id)
        {
            try
            {          

                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                var ItemDetails = EntityFile.ItemMasters.Where(q => q.Id == id).FirstOrDefault();
                var menuitem = EntityFile.MenuItems.Where(q => q.ItemId == id).FirstOrDefault();
                EntityFile.ItemMasters.Remove(ItemDetails);
                if (menuitem != null)
                {
                    EntityFile.MenuItems.Remove(menuitem);
                }
                EntityFile.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult EditItem(int Id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            Chatpata_dabbaEntities1 model = new Chatpata_dabbaEntities1();
            var UserDetails = model.ItemMasters.Where(q => q.Id == Id).FirstOrDefault();
            return View(UserDetails);
        }

        [HttpPost]

        public ActionResult EditItem(ItemMaster iteminfo, HttpPostedFileBase file)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var id = Convert.ToInt32(Session["UserID"]);
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
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
                iteminfo.Image = newFileName;
                EntityFile.Entry(iteminfo).State = System.Data.Entity.EntityState.Modified;
                EntityFile.SaveChanges();
                return RedirectToAction("ViewItem");

            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult AddMenu()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            return View("AddMenu");
        }

        [HttpPost]

        public ActionResult AddMenu(FormCollection frm, HttpPostedFileBase file)
        {
            try
            {
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                MenuMaster menu = new MenuMaster();
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

                menu.Name = Convert.ToString(frm["Name"]);
                menu.Description = Convert.ToString(frm["Description"]);
                menu.Image = Convert.ToString(frm["Image"]);
                menu.Price = Convert.ToDecimal(frm["Price"]);
                menu.Image = newFileName;
                Entity.MenuMasters.Add(menu);
                Entity.SaveChanges();


                return RedirectToAction("Admin", "Admin");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public JsonResult CheckMenuName(string Name)
        {
            Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            var checkName = Entity.MenuMasters.Where(q => q.Name == Name).FirstOrDefault();
            if (checkName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult EditMenu(int Id)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var id = Convert.ToInt32(Session["UserID"]);
            Chatpata_dabbaEntities1 model = new Chatpata_dabbaEntities1();
            var UserDetails = model.MenuMasters.Where(q => q.Id == Id).FirstOrDefault();
            return View(UserDetails);

        }

        [HttpPost]
        public ActionResult EditMenu(MenuMaster menuinfo, HttpPostedFileBase file)
        {
            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
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
                menuinfo.Image = newFileName;
                EntityFile.Entry(menuinfo).State = System.Data.Entity.EntityState.Modified;
                EntityFile.SaveChanges();
                return RedirectToAction("ViewMenu");

            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Home");
           }
        }
        public JsonResult DeleteMenu(int id)
        {
            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                var MenuDetails = EntityFile.MenuMasters.Where(q => q.Id == id).FirstOrDefault();
                EntityFile.MenuMasters.Remove(MenuDetails);
                EntityFile.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddMenuItems(int id)
        {

            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Chatpata_dabbaEntities1 Entity=new Chatpata_dabbaEntities1();
            ViewBag.MenuId = id;
            var Items = Entity.ItemMasters.ToList();
            return View(Items);

        }

        [HttpPost]
        public JsonResult AddMenuItem(int Id,int menuid)
        {
            try
            {
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                MenuItem menuitem = new MenuItem();
                
                var chk = Entity.MenuItems.Where(q => q.ItemId == Id && q.MenuId == menuid).FirstOrDefault();
                if (chk != null)
                {
                    return Json("item", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    menuitem.ItemId = Id;
                    menuitem.MenuId = menuid;
                    Entity.MenuItems.Add(menuitem);
                    Entity.SaveChanges();
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
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
                var items = EntityFile.GetMenuItems(id).OrderBy(q=> q.Name).ToList();
                return View(items);
            }
            catch(Exception e)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult DeleteMenuItem(int Id, int menuId)
        {
            try
            {

                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                var ItemDetails = EntityFile.MenuItems.Where(q => q.ItemId == Id && q.MenuId == menuId).FirstOrDefault();
                EntityFile.MenuItems.Remove(ItemDetails);
                EntityFile.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
       
        }

        public ActionResult ViewFeedback()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
            var Feedback = EntityFile.FeedbackMasters.ToList();

            return View(Feedback);
        }

        public ActionResult CustomerTable()
        {
            try
            {
                Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
                var records = en.CustomerMasters.Where(Q => Q.Roleid == 3).OrderBy(q=> q.FirstName).ToList();
                ViewBag.data = records;
                return View();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult Deletecust(int id)
        {
            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
               
                var CustDetails = EntityFile.CustomerMasters.Where(q => q.CustId == id).OrderBy(q => q.FirstName).FirstOrDefault();
                EntityFile.CustomerMasters.Remove(CustDetails);
                EntityFile.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DriverTable()
        {
            try
            {
                Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
                var records = en.CustomerMasters.Where(Q => Q.Roleid == 2).ToList();
                ViewBag.data = records;
                return View();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Searcharea(FormCollection frm)
        {
            try
            {

                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                var temp = frm["SearchString"];
                var roll = Convert.ToInt32(frm["h1"]);
                var area = from s in Entity.CustomerMasters
                           select s;


                if (!String.IsNullOrEmpty("temp"))
                {
                         area = area.Where(s => (s.Roleid == roll) && (s.Address3.ToUpper().Contains(temp.ToUpper()) || s.FirstName.ToUpper().Contains(temp.ToUpper())));
                }
               return View(area.ToList());
            }
            catch (Exception ex)
            {
                return RedirectToAction("customertable", "Admin");
            }
        }

       //public ActionResult SortFisrtname(string sortOrder, int rollid)
       // {
       //     try
       //     {
       //         Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
       //         ViewBag.firstname = String.IsNullOrEmpty(sortOrder) ? "" : "ase";
       //         //sortOrder = ViewBag.firstname;
       //         var fname = from s in Entity.CustomerMasters
       //                     select s;
       //         fname = fname.Where(s => s.Roleid == rollid);
       //         switch (sortOrder)
       //         {
       //             case "ase":
       //                 fname = fname.OrderByDescending(s => s.FirstName);
       //                 break;
       //             default:
       //                 fname = fname.OrderBy(s => s.FirstName);
       //                 break;
       //         }
       //         //if (sortOrder == "ase")
       //         //{
       //         //    fname = fname.OrderBy(s => s.FirstName);
       //         //}
       //         return View(fname.ToList());
       //     }
       //     catch (Exception ex)
       //     {
       //         return RedirectToAction("customertable", "Admin");
       //     }

       // }
       // public ActionResult Sortarea(string sortOrder, int rollid)
       // {
       //     try
       //     {
       //         Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
       //         ViewBag.firstname = String.IsNullOrEmpty(sortOrder) ? "" : "ase";
       //         //sortOrder = ViewBag.firstname;
       //         var fname = from s in Entity.CustomerMasters
       //                     select s;
       //         fname = fname.Where(q => q.Roleid == rollid);
       //         switch (sortOrder)
       //         {
       //             case "ase":
       //                 fname = fname.OrderByDescending(s => s.Address3);
       //                 break;
       //             default:
       //                 fname = fname.OrderBy(s => s.Address3);
       //                 break;
       //         }
       //         //if (sortOrder == "ase")
       //         //{
       //         //    fname = fname.OrderBy(s => s.FirstName);
       //         //}
       //         return View(fname.ToList());
       //     }
       //     catch (Exception ex)
       //     {
       //         return RedirectToAction("customertable", "Admin");
       //     }

       // }

        public ActionResult ViewSampleRequest()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                RequestMaster rm = new RequestMaster();
                var details = Entity.RequestMasters.Where(q=>q.Status=="Pending").ToList();
                return View(details);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult UpdateStatus(int id,string st)
        {
            try
            {
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                RequestMaster rm = new RequestMaster();
                var details=Entity.RequestMasters.Where(q=> q.Id==id).FirstOrDefault();
          
                if (st == "Accept")
                {
                    details.Status = "Accept";
                    Entity.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    Entity.SaveChanges();
                    return Json("Accept", JsonRequestBehavior.AllowGet);
                }
                else if(st == "Reject")
                {
                    details.Status = "Reject";
                    Entity.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    Entity.SaveChanges();
                    return Json("Reject", JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                } 
            }
            catch(Exception e)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewOrder()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                OrderMaster OM = new OrderMaster();
                var orderdetails = Entity.OrderMasters.OrderBy(q=>q.CustomerMaster.FirstName).ToList();
                var cname = orderdetails.OrderBy(q => q.MenuId).OrderBy(q=> q.Status).ToList();
                return View(cname);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public JsonResult UpdateOrderStatus(int id)
        {
            try
            {
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                OrderMaster OM = new OrderMaster();
                var details = Entity.OrderMasters.Where(q => q.OrderId == id).FirstOrDefault();
                    details.Status = "Accept";
                    Entity.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    Entity.SaveChanges();


                    if (details.StartDate.Day <= DateTime.Now.Day && details.EndDate.Day >= DateTime.Now.Day)
                    {
                        DailyOrder dor = new DailyOrder();
                        dor.CustId =details.CustomerId;
                        dor.OrderId =id;
                        dor.Date = DateTime.Now;
                        dor.Status = "Pending";
                        dor.Code = 0;

                        Entity.DailyOrders.Add(dor);
                        Entity.SaveChanges();
                    }

                    return Json("Accept", JsonRequestBehavior.AllowGet);
                
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

                var currentdate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();

                var dailyordercount = Entity.DailyOrders.Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).Count();

                if (dailyordercount == 0)
                {
                    return RedirectToAction("DOEntry");
                }

               // var dailyOrder  = Entity.DailyOrder().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();
                var dailyOrder = Entity.DailyOrderData().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();
              
                return View(dailyOrder);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult DOEntry()
        {
           Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
            OrderMaster OM = new OrderMaster();
            DailyOrder DO = new DailyOrder();
            var currentdate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            var orderdetails = Entity.OrderMasters.Where(q => q.StartDate <= currentdate && q.EndDate >= currentdate && (q.Status.Equals("Accept") || q.Status.Equals("Delivered"))).OrderBy(q => q.CustomerMaster.FirstName).ToList();

         //   var cname = orderdetails.OrderBy(q => q.MenuId).ToList();


            if (orderdetails != null)
            {
                foreach (var order in orderdetails.ToList())
                {
                    DO.CustId = Convert.ToInt32(order.CustomerId);
                    DO.OrderId = Convert.ToInt32(order.OrderId);
                    DO.Date = DateTime.Now;
                    DO.Status = "Pending";
                    DO.Code = 0;

                    Entity.DailyOrders.Add(DO);
                    Entity.SaveChanges();
                }
            }

            return RedirectToAction("ViewDailyOrder");
        }


        public ActionResult GenerateDispatch()
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                Chatpata_dabbaEntities1 Entity1 = new Chatpata_dabbaEntities1();

                var entry = Entity1.DailyOrders.Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();

                foreach (var order in entry.ToList())
                {
                    var co = Convert.ToDecimal(0);
                    if (order.Code == co)
                    {
                        var myrand = new Random();
                        var c = Convert.ToDecimal(myrand.Next(100000, 1000000));
                        order.Code = c;

                        Entity1.Entry(order).State = System.Data.Entity.EntityState.Modified;
                        Entity1.SaveChanges();

                        
                        var mailid = order.CustomerMaster.EmailID;
                        int ck = Mail.SendMail("Today's Code for you","Code:"+ c+"\n have a nice food..", mailid);
                    }
                }

                return RedirectToAction("ViewDailyOrder");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult CodeRetrival()
        {
                try
                {
                if (Session["UserID"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                DailyOrder DO = new DailyOrder();
                var dbcode = Entity.DailyStatusChange().LastOrDefault();
                var orderdetails = Entity.DailyOrders.Where(q =>q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();

                var code = orderdetails.Where(q=> Convert.ToDecimal(q.Code) == Convert.ToDecimal(dbcode.rcode) && q.OrderId==dbcode.OrderId).ToList();
                if (code.Count == 0)
                {
                    var Orders = Entity.DailyOrderData().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).OrderBy(q => q.Status).ToList();
                    return View(Orders);
                }
                else
                {
                    code[0].Status = "Delivered";
                    Entity.Entry(code[0]).State = System.Data.Entity.EntityState.Modified;
                    Entity.SaveChanges();
                    var Orders = Entity.DailyOrderData().Where(q => q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).OrderBy(q => q.Status).ToList();
                    return View(Orders);
                }
           }

             catch(Exception e)
             {
                 throw e;
             }
            
        }
         
        public ActionResult ViewComplain()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
            var Feedback = EntityFile.Complains.ToList();

            return View(Feedback);
        }
           
        }
}

/*
  // var orderdetail = Orders.Where(q => q.CustId == dbcode.CustId && q.Code == dbcode.rcode).FirstOrDefault();

                   // var dailyorder = Entity.DailyOrders.Where(q => q.CustId == orderdetail.CustId && q.Code==dbcode.rcode && q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();
   
         Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
         DailyOrder DO = new DailyOrder();
         var dbcode = Entity.GetCodeData().FirstOrDefault();
         var Orders = Entity.DailyOrderData().Where(q=>q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).OrderBy(q => q.Status).ToList();
         if (dbcode != null)
         {
             //  var dbcode1 = Entity.DailyOrders.Where(q => q.CustId == cid && q.Code == code).ToList();
             //   var orderstatus=Entity.
             // dbcode1[0].Status = "Delivered";

             var orderdetails = Entity.DailyOrderData().Where(q => q.CustId == dbcode.CustId && q.Code == dbcode.rcode&& q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).FirstOrDefault();
             var orderchange = Entity.DailyOrders.Where(q => q.CustId == orderdetails.CustId && q.Code == dbcode.rcode && q.Date.Day == DateTime.Now.Day && q.Date.Month == DateTime.Now.Month).ToList();

                 orderchange[0].Status = "Delivered";
                 Entity.Entry(orderchange[0]).State = System.Data.Entity.EntityState.Modified;
                 Entity.SaveChanges();
                
             return View(Orders);
         }
         else
         {
             return RedirectToAction("ViewDailyOrder");
         }*/