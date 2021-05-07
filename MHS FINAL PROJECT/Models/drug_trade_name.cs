using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class drug_trade_name
    {
        [Required]
        public int id { get; set; }
        
        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Name")]
        public string trade_name { get; set; }
        
        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Manufacturer Company")]
        public string Manufacturer_company { get; set; }

        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "For Drug Active Name")]
        public int drug_active_nameId { get; set; }
        public drug_active_name drug_active_name { get; set; }
    }
    public class Get_trade_name
    {
        public int id { get; set; }
        public string trade_name { get; set; }
        public string Manufacturer_company { get; set; }
        public string active_name { get; set; }
        public int active_id { get; set; }

        public string added_by { get; set; }
    }
    public class Add_trade_name {

        public int id { get; set; }

        
        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Trade Name")]
        public List<string> trade_name { get; set; }

        
        [Required(ErrorMessage = "This field must be filled")]
        [Display(Name = "Manufacturer company")]
        public List<string> Manufacturer_company { get; set; }

        
        [Required(ErrorMessage = "You Have To Select Active Drug Name")]
        [Display(Name = "Active Drug Name")]
        public string active_name { get; set; }
        public string active_id { get; set; }
    } 


}