using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;

namespace Notes_Marketplace.Controllers
{
    public class SearchNotesController : Controller
    {
        notes_marketplaceEntities2 dbobj = new notes_marketplaceEntities2();
        [Route("SearchNotes")]
        public ActionResult SearchNotes(string search,int? page)
        {
            var emailid = User.Identity.Name.ToString();
            UserTable obj = dbobj.UserTable.Where(x => x.EmailID == emailid).FirstOrDefault();

            System.Linq.IQueryable<SellerNotes> Books;     //Empty Variable for Holding Notes

            Books = dbobj.SellerNotes.ToList().AsQueryable();
            

            ViewBag.Type = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.NoteTypes).Distinct().ToList(), "ID", "Name");
            ViewBag.Category = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.NoteCatrgories).Distinct().ToList(), "ID", "Name");
            ViewBag.Univercity = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.UniversityName).Distinct().ToList());
            ViewBag.Course = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.Course).Distinct().ToList());
            ViewBag.Country = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.Countries).Distinct().ToList(), "ID", "Name");
           
            return View(dbobj.SellerNotes.ToList().ToPagedList(page ?? 1, 5));
        }
    }
}