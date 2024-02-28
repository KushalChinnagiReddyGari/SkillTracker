using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillTracker.Models
{
    public class UserModel
    {
        [Key]
        public int ID { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "EmailId cannot be empty")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Invalid Email address")]
        public string EmailID { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,16}$", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public decimal ContactNo { get; set; }
        public string Gender { get; set; }
        public bool Is_Admin { get; set; }
    }
}