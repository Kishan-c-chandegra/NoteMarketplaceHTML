﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notes_Marketplace.Controllers
{
    public class NoteDetailsController : Controller
    {
        // GET: NoteDetails
        [Route("NoteDetails")]
        public ActionResult NoteDetails()
        {
            return View();
        }
    }
}