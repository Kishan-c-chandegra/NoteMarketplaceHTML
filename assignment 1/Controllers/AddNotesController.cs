using Notes_Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using System.Text;
using System.Web.Hosting;
using System.Net;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;

namespace Notes_Marketplace.Controllers
{
    public class AddNotesController : Controller
    {
        notes_marketplaceEntities2 Context = new notes_marketplaceEntities2();

        [HttpGet]
        [Route("AddNotes")]
        [Authorize]

        public ActionResult AddNotes()
        {
            // create add note viewmodel and set values in dropdown list

            //AddNotes viewModel = new AddNotes
            //{
            //    NoteCategoryList = Context.NoteCatrgories.ToList(),
            //    NoteTypeList = Context.NoteTypes.ToList(),
            //    CountryList = Context.Countries.ToList()
            //};
            ViewBag.Category = new SelectList(Context.NoteCatrgories, "ID", "Name");
            ViewBag.Type = new SelectList(Context.NoteTypes, "ID", "Name");
            ViewBag.Country = new SelectList(Context.Countries, "ID", "Name");
            return View();
            

        }

        [HttpPost]
        [Route("AddNotes")]
        [Authorize]
        public ActionResult AddNotes(AddNotes xyz)
        {

            if (ModelState.IsValid)
            {
                // create seller note object
                SellerNotes abc = new SellerNotes();


                var user = Context.UserTable.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                abc.Status = Context.ReferenceData.Where(x => x.Value.ToLower() == "Draft").Select(x => x.ID).FirstOrDefault();
                abc.SellerID = user.UserID;
                abc.Title = xyz.Title.Trim();
                abc.Category = xyz.Category;
                abc.NoteType = xyz.NoteType;
                abc.NumberOfPages = xyz.NumberofPages;
                abc.Description = xyz.Description.Trim();
                abc.UniversityName = xyz.UniversityName.Trim();
                abc.Country = xyz.Country;
                abc.Course = xyz.Course.Trim();
                abc.CourseCode = xyz.CourseCode.Trim();
                abc.Professor = xyz.Professor.Trim();
                abc.IsPaid = xyz.IsPaid;
                if (abc.IsPaid)
                {
                    abc.SellingPrice = xyz.SellingPrice;
                }
                else
                {
                    abc.SellingPrice = 0;
                }

                abc.IsActive = true;
                Context.SellerNotes.Add(abc);
                Context.SaveChanges();
                
                //


                // add note in database and save




                // get seller note
                //nid = Context.SellerNotes.Find(abc.ID);

                // if display picture is not null then save picture into directory and directory path into database
                if (xyz.DisplayPicture != null)
                {
                    string displaypicturefilename = System.IO.Path.GetFileName(xyz.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.UserID + "/" + abc.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    abc.DisplayPicture = displaypicturepath + displaypicturefilename;
                    xyz.DisplayPicture.SaveAs(displaypicturefilepath);
                }

                // if note preview is not null then save picture into directory and directory path into database
                if (xyz.NotesPreview != null)
                {
                    string notespreviewfilename = System.IO.Path.GetFileName(xyz.NotesPreview.FileName);
                    string notespreviewpath = "~/Members/" + user.UserID + "/" + abc.ID + "/";
                    CreateDirectoryIfMissing(notespreviewpath);
                    string notespreviewfilepath = Path.Combine(Server.MapPath(notespreviewpath), notespreviewfilename);
                    abc.NotesPreview = notespreviewpath + notespreviewfilename;
                    xyz.NotesPreview.SaveAs(notespreviewfilepath);
                }

                // update note preview path and display picture path and save changes
                //Context.SellerNotes.Add(abc);
                Context.SaveChanges();

                // attachement files
                foreach (HttpPostedFileBase file in xyz.UploadNotes)
                {
                    // check if file is null or not
                    if (file != null)
                    {
                        // save file in directory
                        string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                        string notesattachementpath = "~/Members/" + user.UserID + "/" + abc.ID + "/Attachements/";
                        CreateDirectoryIfMissing(notesattachementpath);
                        string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath), notesattachementfilename);
                        file.SaveAs(notesattachementfilepath);

                        // create object of sellernotesattachement 
                        SellerNotesAttachements notesattachements = new SellerNotesAttachements
                        {
                            NoteID = abc.ID,
                            FileName = notesattachementfilename,
                            FilePath = notesattachementpath,

                            IsActive = true
                        };

                        // save seller notes attachement
                        Context.SellerNotesAttachements.Add(notesattachements);
                        Context.SaveChanges();
                    }
                }
                return RedirectToAction("Dashboard","Dashboard");
            }
            // if model state is not valid
            else
            {
                // create object of xyz
                AddNotes viewModel = new AddNotes
                {
                    NoteCategoryList = Context.NoteCatrgories.ToList(),
                    NoteTypeList = Context.NoteTypes.ToList(),
                    CountryList = Context.Countries.ToList()
                };

                return View(viewModel);
            }
        }

        private void CreateDirectoryIfMissing(string folderpath)
        {
            // check if directory exists
            bool folderalreadyexists = Directory.Exists(Server.MapPath(folderpath));
            // if directory does not exists then create
            if (!folderalreadyexists)
                Directory.CreateDirectory(Server.MapPath(folderpath));
        }
    }
}