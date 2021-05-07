using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class warning
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "During Pregnancy")]
        public string During_pregnancy { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Breast Feeding")]
        public string Breast_feeding { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Children And Infants")]
        public string Children_and_infants { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Elderly")]
        public string Elderly { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Driving")]
        public string Driving { get; set; }



        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Surgery And Anesthesia")]
        public string Surgery_and_anesthesia { get; set; }

        [Required]
        public int drug_active_nameId { get; set; }
        public drug_active_name drug_active_name { get; set; }
    }
    public class warning_get
    {
        public string During_pregnancy { get; set; }

        public string Breast_feeding { get; set; }

        public string Children_and_infants { get; set; }

        public string Elderly { get; set; }

        public string Driving { get; set; }

        public string Surgery_and_anesthesia { get; set; }

        public int drug_active_nameId { get; set; }
    }




}