﻿using NotesMarketPlace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.ViewModels
{
    public class SearchNotesViewModel
    {
        public SellerNote Note { get; set; }
        public int AverageRating { get; set; }
        public int TotalRating { get; set; }
        public int TotalSpam { get; set; }
    }
}