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
                return RedirectToAction("Dashboard", "Dashboard");
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


        [Authorize]
        [HttpGet]
        [Route("AddNotes/EditNotes/{id}")]
        public ActionResult EditNotes(int id)
        {
            // get logged in user
            var user = Context.UserTable.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // get note
            SellerNotes note = Context.SellerNotes.Where(x => x.ID == id && x.IsActive == true && x.SellerID == user.UserID).FirstOrDefault();
            // get note attachement
            SellerNotesAttachements attachement = Context.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            if (note != null)
            {
                // create object of edit note viewmodel
                EditNoteView xyz = new EditNoteView
                {
                    ID = note.ID,
                    NoteID = note.ID,
                    Title = note.Title,
                    Category = note.Category,
                    Picture = note.DisplayPicture,
                    Note = attachement.FilePath,
                    NumberofPages = note.NumberOfPages,
                    Description = note.Description,
                    NoteType = note.NoteType,
                    UniversityName = note.UniversityName,
                    Course = note.Course,
                    CourseCode = note.CourseCode,
                    Country = note.Country,
                    Professor = note.Professor,
                    IsPaid = note.IsPaid,
                    SellingPrice = note.SellingPrice,
                    Preview = note.NotesPreview,
                    NoteCategoryList = Context.NoteCatrgories.ToList(),
                    NoteTypeList = Context.NoteTypes.ToList(),
                    CountryList = Context.Countries.ToList()
                };

                // return viewmodel to edit notes page
                return View(xyz);
            }
            else
            {
                // if note not found
                return HttpNotFound();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AddNotes/EditNotes/{id}")]
        public ActionResult EditNotes(int id, EditNoteView notes)
        {
            // check if model state is valid or not
            if (ModelState.IsValid)
            {
                // get logged in user
                var user = Context.UserTable.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                // get note 
                var sellernotes = Context.SellerNotes.Where(x => x.ID == id && x.IsActive == true && x.SellerID == user.UserID).FirstOrDefault();
                // if sellernote null
                if (sellernotes == null)
                {
                    return HttpNotFound();
                }
                // check if note is paid or preview is not null
                if (notes.IsPaid == true && notes.Preview == null && sellernotes.NotesPreview == null)
                {
                    //ModelState.AddModelError("NotesPreview", "This field is required if selling type is paid");
                    @ViewBag.Notespreview = "This field is required if selling type is paid";
                    return View(notes);
                }
                // get note attachement 
                var notesattachement = Context.SellerNotesAttachements.Where(x => x.NoteID == notes.NoteID && x.IsActive == true).ToList();

                // attache note object and update
                Context.SellerNotes.Attach(sellernotes);
                sellernotes.Title = notes.Title.Trim();
                sellernotes.Category = notes.Category;
                sellernotes.NoteType = notes.NoteType;
                sellernotes.NumberOfPages = notes.NumberofPages;
                sellernotes.Description = notes.Description.Trim();
                sellernotes.Country = notes.Country;
                sellernotes.UniversityName = notes.UniversityName.Trim();
                sellernotes.Course = notes.Course.Trim();
                sellernotes.CourseCode = notes.CourseCode.Trim();
                sellernotes.Professor = notes.Professor.Trim();
                if (notes.IsPaid == true)
                {
                    sellernotes.IsPaid = true;
                    sellernotes.SellingPrice = notes.SellingPrice;
                }
                else
                {
                    sellernotes.IsPaid = false;
                    sellernotes.SellingPrice = 0;
                }

                Context.SaveChanges();

                // if display picture is not null
                if (notes.DisplayPicture != null)
                {
                    // if note object has already previously uploaded picture then delete it
                    if (sellernotes.DisplayPicture != null)
                    {
                        string path = Server.MapPath(sellernotes.DisplayPicture);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // save updated profile picture in directory and save directory path in database
                    string displaypicturefilename = System.IO.Path.GetFileName(notes.DisplayPicture.FileName);
                    string displaypicturepath = "~/Members/" + user.UserID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(displaypicturepath);
                    string displaypicturefilepath = Path.Combine(Server.MapPath(displaypicturepath), displaypicturefilename);
                    sellernotes.DisplayPicture = displaypicturepath + displaypicturefilename;
                    notes.DisplayPicture.SaveAs(displaypicturefilepath);
                }

                // if note preview is not null
                if (notes.NotesPreview != null)
                {
                    // if note object has already previously uploaded note preview then delete it
                    if (sellernotes.NotesPreview != null)
                    {
                        string path = Server.MapPath(sellernotes.NotesPreview);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    // save updated note preview in directory and save directory path in database
                    string notespreviewfilename = System.IO.Path.GetFileName(notes.NotesPreview.FileName);
                    string notespreviewpath = "~/Members/" + user.UserID + "/" + sellernotes.ID + "/";
                    CreateDirectoryIfMissing(notespreviewpath);
                    string notespreviewfilepath = Path.Combine(Server.MapPath(notespreviewpath), notespreviewfilename);
                    sellernotes.NotesPreview = notespreviewpath + notespreviewfilename;
                    notes.NotesPreview.SaveAs(notespreviewfilepath);
                }

                // check if user upload notes or not
                if (notes.UploadNotes[0] != null)
                {
                    // if user upload notes then delete directory that have previously uploaded notes
                    string path = Server.MapPath(notesattachement[0].FilePath);
                    DirectoryInfo dir = new DirectoryInfo(path);
                    EmptyFolder(dir);

                    // remove previously uploaded attachement from database
                    foreach (var item in notesattachement)
                    {
                        SellerNotesAttachements attachement = Context.SellerNotesAttachements.Where(x => x.ID == item.ID).FirstOrDefault();
                        Context.SellerNotesAttachements.Remove(attachement);
                    }

                    // add newly uploaded attachement in database and save it in database
                    foreach (HttpPostedFileBase file in notes.UploadNotes)
                    {
                        // check if file is null or not
                        if (file != null)
                        {
                            // save file in directory
                            string notesattachementfilename = System.IO.Path.GetFileName(file.FileName);
                            string notesattachementpath = "~/Members/" + user.UserID + "/" + sellernotes.ID + "/Attachements/";
                            CreateDirectoryIfMissing(notesattachementpath);
                            string notesattachementfilepath = Path.Combine(Server.MapPath(notesattachementpath), notesattachementfilename);
                            file.SaveAs(notesattachementfilepath);

                            // create object of sellernotesattachement 
                            SellerNotesAttachements notesattachements = new SellerNotesAttachements
                            {
                                NoteID = sellernotes.ID,
                                FileName = notesattachementfilename,
                                FilePath = notesattachementpath,

                                IsActive = true
                            };

                            // save seller notes attachement
                            Context.SellerNotesAttachements.Add(notesattachements);
                            Context.SaveChanges();
                        }
                    }
                }

                return RedirectToAction("Home", "Home");
            }
            else
            {
                return RedirectToAction("EditNotes", new { id = notes.ID });
            }

        }


        [Route("AddNotes/Publish")]
        public ActionResult PublishNote(int id)
        {
            // get note
            
            var note = Context.SellerNotes.Find(id);

            // if note is not found
            if (note == null)
            {
                return HttpNotFound();
            }
            // get logged in user
            var user = Context.UserTable.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();

            // seller full name
            string sellername = user.FirstName + " " + user.LastName;

            if (user.UserID == note.SellerID)
            {
                // update note status from draft to submitted for review
                Context.SellerNotes.Attach(note);
                note.Status = 21;
                //Context.ReferenceData.Where(x => x.Value == "Submitted For Review").Select(x => x.ID).FirstOrDefault();



                Context.SaveChanges();

                // send mail to admin for publish note request
                PublishNotemail(note.Title, sellername);
            }

            return RedirectToAction("Home", "Home");
        }

        // send mail to admin for publish note request
        public void PublishNotemail(string note, string seller)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "PublishNote" + ".cshtml");
            body = body.Replace("ViewBag.SellerName", seller);
            body = body.Replace("ViewBag.NoteTitle", note);
            body = body.ToString();

            // get support email
            var fromemail = Context.SystemConfigurations.Where(x => x.Key == "supportemail").FirstOrDefault();
            var tomail = Context.SystemConfigurations.Where(x => x.Key == "notifyemail").FirstOrDefault();

            // set from, to, subject, body
            string from, to, subject;
            from = fromemail.Value.Trim();
            to = tomail.Value.Trim();
            subject = seller + " sent his note for review";
            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            body = sb.ToString();

            using (MailMessage mm = new MailMessage(from, to))
            {
                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential Network = new NetworkCredential(from, "Kishan@1312");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = Network;
                smtp.Port = 587;
                smtp.Send(mm);
            }


        }



        private void EmptyFolder(DirectoryInfo directory)
        {
            // check if directory have files
            if (directory.GetFiles() != null)
            {
                // delete all files
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
            }

            // check if directory have subdirectory
            if (directory.GetDirectories() != null)
            {
                // call emptyfolder and delete subdirectory
                foreach (DirectoryInfo subdirectory in directory.GetDirectories())
                {
                    EmptyFolder(subdirectory);
                    subdirectory.Delete();
                }
            }
        }
    }
}
