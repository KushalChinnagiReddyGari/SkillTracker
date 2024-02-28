using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillTracker.Models
{
    public class ForgotPasswordModel
    {
        public string EmailID { get; set; }
        public DateTime DOB { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }
}