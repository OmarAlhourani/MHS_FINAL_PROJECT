using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class side_effect
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Name")]
        public string name { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Prevalence Effect")]
        public string prevalence_effect { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Inform Doctor")]
        public string inform_doctor { get; set; }

        [Required]
        public int drug_active_nameId { get; set; }
        public drug_active_name drug_active_name { get; set; }
    }

    public class Add_side_effedct
    {
        public int id { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Name")]
        public List<string> name { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Prevalence Effect")]
        public List<string> prevalence_effect { get; set; }


        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Inform Doctor")]
        public List<string> inform_doctor { get; set; }
    }


    public class side_effedct_get
    {
        public string name { get; set; }

        public string prevalence_effect { get; set; }

        public string inform_doctor { get; set; }

    }

}