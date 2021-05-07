using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class drug_dosage
    {
        [Required]
        public int id { get; set; }


        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "How To Take")]
        public string how_to_take { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Number Of Dosage")]
        public string number_of_dosage { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Dosage")]
        public string dosage { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Beginning Of Effectiveness")]
        public string beginning_of_effectiveness { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Duration Of Effectiveness")]
        public string duration_of_effectiveness { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Feed")]
        public string feed { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Storage And Preservation")]
        public string Storage_and_preservation { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Forget A Dose")]
        public string Forget_a_dose { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Overdose")]
        public string overdose { get; set; }

        [Required]
        public drug_active_name drug_active_name { get; set; }

    }

    public class drug_dosage_get
    {
        public string how_to_take { get; set; }

        public string number_of_dosage { get; set; }

        public string dosage { get; set; }

        public string beginning_of_effectiveness { get; set; }

        public string duration_of_effectiveness { get; set; }

        public string feed { get; set; }

        public string Storage_and_preservation { get; set; }

        public string Forget_a_dose { get; set; }

        public string overdose { get; set; }
    }
}