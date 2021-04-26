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

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUs xyz)
        {
            if (ModelState.IsValid)
            {
                var fromEmail = new MailAddress("coolhamoj@gmail.com");
                var toEmail = new MailAddress("kishanchandegra5070@gmail.com");
                var fromEmailPassword = "Kishan@1312"; // Replace with actual password
                string subject = xyz.FullName + " - Query";

                string body = "Hello," +
                    "<br/><br/>" + xyz.Comments +
                    "<br/><br/>Regards," + xyz.FullName;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                smtp.Send(message);

            }
            return RedirectToAction("ContactUs");

        }
    }
}