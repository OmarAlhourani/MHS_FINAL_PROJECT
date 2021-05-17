using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class user_health
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "Please Enter the Health Condition Name")]
        [Display(Name = "Health Condition Name")]
        public string health_condition_name { get; set; }

        [Required(ErrorMessage = "Please Enter the Description of Health Condition")]
        [Display(Name = "Health Condition Name")]
        public string description { get; set; }

        public ICollection<health_drugs> health_drugs { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class user_health_return
    {
        public int id { get; set; }
        public string health_condition_name { get; set; }
        public string description { get; set; }
        public string User_id { get; set; }
        public int number_of_drugs { get; set; }

    }
    public class user_drug_num_return
    {
        public int number_of_drugs { get; set; }
        public int user_healthId { get; set; }


    }
    public class user_health_drug_return
    {
        public int id { get; set; }
        public string health_condition_name { get; set; }
        public string description { get; set; }
        public string User_id { get; set; }

        public int drugs_id { get; set; }
        public string name { get; set; }
        public string dosage { get; set; }
        public int user_healthId { get; set; }
    }
    public class user_Add_health
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Please Enter the Health Condition Name")]
        [Display(Name = "Health Condition Name")]
        public string health_condition_name { get; set; }
        [Required(ErrorMessage = "Please Enter the Description of Health Condition")]
        [Display(Name = "Health Condition Description")]
        public string description { get; set; }

    }

    public class Added_id
    {
        public Decimal id { get; set; }
    }



}