using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class health_drugs
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Name")]
        public string name { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Dosage")]
        public string dosage { get; set; }

        [Required]
        public int user_healthId { get; set; }
        public user_health user_health { get; set; }
    }

    public class Add_health_drugs
    {
        public List<int> drug_id { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Name")]
        public List<string> name { get; set; }

        
        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Dosage")]
        public List<string> dosage { get; set; }
    }



}