using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Project.common;
using System.IO;


namespace Project.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }                                                     
         
        public ActionResult Registration1()
        {
            Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
            var que = en.Questions.ToList();
            ViewBag.data = que;
            return View();
        }

        public ActionResult Login()
        {
            if(Session["UserId"]==null)
            {
                return View();
            }
            return RedirectToAction("Customer", "Customer");
          
        }

        public JsonResult CheckEmail(string Email, int UserID)
        {
            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();
                //var chkEmail = EntityFile.Tables.Where(q=>q.EmailId == Email).FirstOrDefault();
                //if (chkEmail != null)
                //{
                //    return Json(false, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(true, JsonRequestBehavior.AllowGet);
                //}


                if (UserID == 0)
                {
                    var chkEmail = EntityFile.CustomerMasters.Where(q => q.EmailID == Email).Count();
                    //var chkEmail = EntityFile.CustomerMasters.Where(q => q.EmailID == Email).Count();
                    if (chkEmail != 0)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var chkEmail = EntityFile.CustomerMasters.Where(q => q.EmailID == Email && q.CustId != UserID).Count();
                    //var chkEmail = EntityFile.CustomerMasters.Where(q => q.EmailID == Email && q.CustId != UserId).Count();
                    if (chkEmail != 0)
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }


            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckLogin(FormCollection frm)
        {
            try
            {


                var Emailid = Convert.ToString(frm["EmailID"]);
                var password = Convert.ToString(frm["Password"]);

                
                Chatpata_dabbaEntities1 Entity = new Chatpata_dabbaEntities1();
                CustomerMaster reg = new CustomerMaster();
                password = MD5Crypt.Encrypt(password, "Dabba");

                
                var chkEntry = Entity.CustomerMasters.Where(q => q.EmailID == Emailid && q.Password == password).FirstOrDefault();
                if (chkEntry != null)
                {

                    int?[] rid = (Entity.CustomerMasters.Where(p => p.EmailID == Emailid).Select(q => q.Roleid)).ToArray();

                    int r = Convert.ToInt16(rid[0]);

                    if (r == 1)
                    {
                        Session["UserID"] = chkEntry.CustId;
                        
                        Session["cname"] = chkEntry.FirstName;
                        return RedirectToAction("Admin", "Admin");
                    }
                    else if(r==2)
                    {
                        Session["UserID"] = chkEntry.CustId;
                        Session["cname"] = chkEntry.FirstName;
                        return RedirectToAction("Driver", "Driver");
                    }
                    else
                    {
                        Session["UserID"] = chkEntry.CustId;
                        Session["cname"] = chkEntry.FirstName;
                        return RedirectToAction("Customer", "Customer");
                    }
                }
                else
                {
                    TempData["ErrMsg"] = "Invalid user name or password";
                    return RedirectToAction("Index");
                }


            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
                                 
        }

         public ActionResult AddUser(FormCollection frm,HttpPostedFileBase file)
        {
            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();

                CustomerMaster reg = new CustomerMaster();

                reg.Roleid = 3;

                reg.FirstName = Convert.ToString(frm["FName"]);
                reg.LastName = Convert.ToString(frm["LName"]);
                reg.Address1 = Convert.ToString(frm["Address1"]);
                reg.Address2 = Convert.ToString(frm["Address2"]);
                reg.Address3 = Convert.ToString(frm["Address3"]);

                reg.Pincode = Convert.ToString(frm["Pincode"]);

                reg.PhoneNo = Convert.ToString(frm["PhoneNo"]);
                if (Convert.ToString(frm["gender"]) == "male")
                {
                    reg.Gender = "Male";
                }
                else
                {
                    reg.Gender = "Female";
                }

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

                reg.Image = Convert.ToString(frm["Image"]);
                reg.Image = newFileName;

                string[] dtarray = frm["Birthdate"].ToString().Split('/');
                reg.Birthdate = Convert.ToDateTime("" + dtarray[1] + "/" + dtarray[0] + "/" + dtarray[2]);

                reg.EmailID = Convert.ToString(frm["EmailID"]);
                reg.Password = MD5Crypt.Encrypt(frm["Password"],"Dabba");

                reg.QuestionID = Convert.ToInt32(frm["AID"]);
                reg.Answer = frm["Answer"];

                EntityFile.CustomerMasters.Add(reg);
                EntityFile.SaveChanges();
                int chkmail = Mail.SendMail("Registration","Thanks",reg.EmailID);
              //  TempData["msg"] = "Your are successfully registered , please login in to website for further details";

                return RedirectToAction("Index", "Home");

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Forgotpass()
        {
            Chatpata_dabbaEntities1 en = new Chatpata_dabbaEntities1();
         //   var que = en.Questions.ToList();
           // ViewBag.data = que;
            return View();
        }
        public ActionResult Forgotque(FormCollection frm)
        {

            try
            {
                Chatpata_dabbaEntities1 EntityFile = new Chatpata_dabbaEntities1();


                var email = Convert.ToString(frm["Email"]);

                var user = EntityFile.CustomerMasters.Where(q => q.EmailID == email).FirstOrDefault();
                //Session["UserID"]=details.CustId;

                string pass = GenerateRandomPassword(6);
                int chkmail = Mail.SendMail("Your New Password", pass, email);
                user.Password = MD5Crypt.Encrypt(pass, "Dabba");
                EntityFile.Entry(user).State = System.Data.Entity.EntityState.Modified;
                EntityFile.SaveChanges();
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                return RedirectToAction("Forgotpass_new", "Home");
            }


        }
        private string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }

        public ActionResult ForgetChangePass()
        {
            return View();
        }   

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }
    }
}
 