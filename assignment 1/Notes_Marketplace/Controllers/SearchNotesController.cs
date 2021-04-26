using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notes_Marketplace.Controllers
{
    public class SearchNotesController : Controller
    {
        [Route("SearchNotes")]
        public ActionResult SearchNotes()
        {
            return View();
        }
    }
}