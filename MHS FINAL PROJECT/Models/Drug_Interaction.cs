using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class Drug_Interaction
    {
        [Required]
        public int id { get; set; }


        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Degree")]
        [StringLength(25, MinimumLength = 10)]
        public string Degree { get; set; }
        

        public string add_by { get; set; }


        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Drug")]
        public drug_active_name drug { get; set; }


        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Interact With")]
        public drug_active_name with { get; set; }
    }

    public class Get_Drug_Interaction
    {
        public int id { get; set; }
        public string Main_drug { get; set; }
        public string with_drug { get; set; }
        public string Degree { get; set; }
        public int drug_id { get; set; }
        public int with_id { get; set; }
        public string added_by { get; set; }
    }


    public class add_Drug_Interaction {
        public int id { get; set; }
        public string Degree { get; set; }
        public string drug_name { get; set; }
        public int drug_id { get; set; }
        public string with_name { get; set; }
        public int with_id { get; set; }
    }
}