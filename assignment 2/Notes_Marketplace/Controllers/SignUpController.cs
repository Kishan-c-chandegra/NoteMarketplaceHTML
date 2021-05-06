using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Web.Security;
using System.IO;
using System.Data.Entity.Validation;
using Notes_Marketplace.Models;



namespace Notes_Marketplace.Controllers
{
    public class SignUpController : Controller
    {
        
        notes_marketplaceEntities2 objCon = new notes_marketplaceEntities2();
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]

        public ActionResult SignUp(UserTable objUsr)
        {

            objUsr.EmailVerification = false;
            var IsExists = IsEmailExists(objUsr.EmailID);
            if (IsExists)
            {
                ModelState.AddModelError("EmailExists", "Email Already Exists");
                return View();
            }


            objUsr.ActivationCode = Guid.NewGuid();

            objUsr.Password = Notes_Marketplace.Models.encryptPassword.textToEncrypt(objUsr.Password);
            objCon.UserTable.Add(objUsr);
            objCon.SaveChanges();
            SendEmailToUser(objUsr.EmailID, objUsr.ActivationCode.ToString());
            var Message = "Registration Completed. Please Check Your Email :" + objUsr.EmailID;
            ViewBag.Message = Message;
            return View("SignUp");
        }

        public bool IsEmailExists(string eMail)
        {
            var IsCheck = objCon.UserTable.Where(email => email.EmailID == eMail).FirstOrDefault();
            return IsCheck != null;
        }

        public void SendEmailToUser(string emailId, string activationCode)
        {
            var GenarateUserVerificationLink = "/SignUp/EmailVerification/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("coolhamoj@gmail.com", "NotesMarketplace From KISHAN CHANDEGRA");
            var fromEmailpassword = "Kishan@1312";
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Registration Completed-Demo";
            Message.Body = "<br/> Your registration completed succesfully." +
                           "<br/> please click on the below link for account verification" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }

        public ActionResult EmailVerification(string id)
        {
            bool Status = false;

            objCon.Configuration.ValidateOnSaveEnabled = false; // Ignor to password confirmation     
            var IsVerify = objCon.UserTable.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();

            if (IsVerify != null)
            {
                IsVerify.EmailVerification = true;
                objCon.SaveChanges();
                ViewBag.Message = "Email Verification completed";
                Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid Request...Email not verify";
                ViewBag.Status = false;
            }

            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin LgnUsr)
        {
            var _passWord = Notes_Marketplace.Models.encryptPassword.textToEncrypt(LgnUsr.Password);
            bool Isvalid = objCon.UserTable.Any(x => x.EmailID == LgnUsr.EmailID &&
            x.Password == _passWord);
            if (Isvalid)
            {
                int timeout = LgnUsr.RememberMe ? 60 : 5; // Timeout in minutes, 60 = 1 hour.    
                var ticket = new FormsAuthenticationTicket(LgnUsr.EmailID, false, timeout);
                string encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                return RedirectToAction("Dashboard","Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Information... Please try again!");
            }
            return View();
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "SignUp");
        }




        [HttpGet]
        [Route("ForgotPassword")]

        public ActionResult ForgotPassWord()
        {
            ForgotPassword obj = new ForgotPassword();
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]


        public ActionResult ForgotPassWord(ForgotPassword xyz)
        {
            if (objCon.UserTable.Any(model => model.EmailID == xyz.EmailID))
            {
                ForgotPassSentEmail(xyz);
                return View();

            }
            else
            {
                ModelState.AddModelError("Error", "Email Id does not exists");
                return View();
            }


        }

        private void ForgotPassSentEmail(ForgotPassword xyz)
        {
            var check = objCon.UserTable.Where(x => x.EmailID == xyz.EmailID).FirstOrDefault();
            using (MailMessage mm = new MailMessage("coolhamoj@gmail.com", xyz.EmailID))
            {
                mm.Subject = "NoteMarketPlace - Temporary Password";

                var body = "<p>Hello,</p> <p>Your newly generated password is:<p> <p>{0}</p> <p>Thanks,</p><p>Team Notes MarketPlace</p>";
                string NewPassword = GeneratePassword().ToString();

                body = string.Format(body, NewPassword);
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;

                NetworkCredential Network = new NetworkCredential("coolhamoj@gmail.com", "Kishan@1312");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = Network;
                smtp.Port = 587;
                smtp.Send(mm);

                if (NewPassword != null)
                {
                    var Replace = objCon.UserTable.Where(x => x.EmailID == xyz.EmailID).FirstOrDefault();
                    if (Replace != null)
                    {
                        Replace.Password = NewPassword;
                        Replace.Password = Notes_Marketplace.Models.encryptPassword.textToEncrypt(Replace.Password);

                        objCon.SaveChanges();


                    }


                }

            }
        }

        public string GeneratePassword()
        {
            string PassLength = "6";
            string NewPass = "";

            String AllowChar = "";

            AllowChar = "1,2,3,4,5,6,7,8,9,0";

            char[] Seperated = { ',' };

            string[] arr = AllowChar.Split(Seperated);

            string IDString = "";
            string Temp = "";

            Random Rand = new Random();

            for (int i = 0; i < Convert.ToInt32(PassLength); i++)
            {
                Temp = arr[Rand.Next(0, arr.Length)];
                IDString += Temp;
                NewPass = IDString;
            }
            return NewPass;
        }
    }
}