using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Notes_Marketplace.Models;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web.Security;

namespace Notes_Marketplace.Controllers
{

    public class ContactUsController : Controller
    {
        notes_marketplaceEntities2 dbobj = new notes_marketplaceEntities2();
        // GET: Home
        [HttpGet]
        [Route("ContactUs")]
        public ActionResult ContactUs()
        {
            var user = dbobj.UserTable.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            ContactUs userprofilemodel = new ContactUs();

            userprofilemodel.Email = user.EmailID;
            userprofilemodel.FullName = user.FirstName + " " + user.LastName;


            return View(userprofilemodel);
        }

        [HttpPost]
        [Route("ContactUs")]
        public async Task<ActionResult> ContactUs(ContactUs xyz)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {1}</p><p>Hello,</p><p>{2}</p><br><p>Regards,</p><p>{0}</p>";
                var Message = new MailMessage();
                Message.To.Add(new MailAddress("coolhamoj@gmail.com"));

                Message.From = new MailAddress(xyz.Email);
                Message.Subject = xyz.Subject;
                Message.Body = string.Format(body, xyz.FullName, xyz.Email, xyz.Comments);
                Message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "coolhamoj@gmail.com",
                        Password = "Kishan@1312"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(Message);
                }

            }
            return View();

        }
    }
}