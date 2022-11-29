using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RPPSampleMVC.Models
{
    public class RequestDetails
    {
        [Display(Name = "UserName", Description = "Enter User Name", Prompt = "USER NAME"), Required(ErrorMessage = "Please enter user name")]
        public string USERNAME { get; set; }
        [Display(Name = "Email", Description = "Enter Email", Prompt = "EMAIL"), Required(ErrorMessage = "Please enter email address")]
        public string USEREMAIL { get; set; }
        [Display(Name = "Mobile", Description = "Enter Mobile", Prompt = "MOBILE"), Required(ErrorMessage = "Please enter mobile")]
        public string USERMOBILE { get; set; }
        [Display(Name = "Purpose", Description = "Enter Purpose", Prompt = "PURPOSE"), Required(ErrorMessage = "Please enter purpose")]
        public string PURPOSE { get; set; }
        [Display(Name = "Amount", Description = "Enter Amount", Prompt = "AMOUNT"), Required(ErrorMessage = "Please enter amount")]
        public string AMOUNT { get; set; }
    }
}