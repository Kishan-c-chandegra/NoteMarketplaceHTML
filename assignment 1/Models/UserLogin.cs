﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Notes_Marketplace.Models
{
    public class UserLogin
    {
        [Required]
        [EmailAddress]
        public string EmailID { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}