using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.ViewModels
{
    public class NotesUnderReviewViewModel
    {
        public int NoteID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Seller { get; set; }
        public int SellerID { get; set; }
        public DateTime DateAdded { get; set; }
        public string Status { get; set; }
    }
}