using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class drug_active_name
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter the name of the drug")]
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 4)]
        [Index(IsUnique = true)]
        public string name { get; set; }
        
        [Required(ErrorMessage = "Please enter the description of the drug")]
        [Display(Name = "Description")]
        public string description { get; set; }

        public ApplicationUser Add_By { get; set; }
        public ICollection<side_effect> side_effect { get; set; }
        public ICollection<drug_dosage> drug_dosage { get; set; }
        public ICollection<drug_trade_name> drug_trade_name { get; set; }
        public ICollection<warning> warnings { get; set; }
    }
    public class drug_active_name_serach
    {
        [Required(ErrorMessage = "Please enter the name of the drug")]
        [Display(Name = "Name")]
        public string name { get; set; }
    }
    public class drug_serach_result
    {
        public int id { get; set; }
        [Display(Name = "Name")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "How To Take")]
        public string how_to_take { get; set; }

        [Display(Name = "Number Of Dosage")]
        public string number_of_dosage { get; set; }

        [Display(Name = "Dosage")]
        public string dosage { get; set; }
        [Display(Name = "Beginning Of Effectiveness")]
        public string beginning_of_effectiveness { get; set; }
        [Display(Name = "Duration Of Effectiveness")]
        public string duration_of_effectiveness { get; set; }
        [Display(Name = "Feed")]
        public string feed { get; set; }
        [Display(Name = "Storage And Preservation")]
        public string Storage_and_preservation { get; set; }
        [Display(Name = "Forget A Dose")]
        public string Forget_a_dose { get; set; }
        [Display(Name = "OverDose")]
        public string overdose { get; set; }



        [Display(Name = "During Pregnancy")]
        public string During_pregnancy { get; set; }
        [Display(Name = "Breast Feeding")]
        public string Breast_feeding { get; set; }
        [Display(Name = "Children And Infants")]
        public string Children_and_infants { get; set; }
        [Display(Name = "Elderly")]
        public string Elderly { get; set; }
        [Display(Name = "Driving")]
        public string Driving { get; set; }
        [Display(Name = "Surgery And Anesthesia")]
        public string Surgery_and_anesthesia { get; set; }

    }
    public class drug_serach_result_finsh
    {
        public string name { get; set; }
        public IEnumerable<drug_trade_search_name> active_name_for_trade { get; set; }
        public IEnumerable<drug_serach_result> drug { get; set; }
        public IEnumerable<side_effect> side_effect { get; set; }
        public IEnumerable<drug_active_name> drug_active { get; set; }
    }
    public class drug_Advanced_Search_choices
    {
        public string name { get; set; }
        public bool description { get; set; }
        public bool drug_dosage { get; set; }
        public bool warnings { get; set; }
        public bool side_effects { get; set; }
        public bool trade_names { get; set; }
    }
    public class drug_interaction {
        public string name { get; set; }
        public string Degree { get; set; }
        public string with_name { get; set; }
        public string active_name_for_trade { get; set; }
        public string active_without_interaction { get; set; }
    }
    public class drug_active_advanced
    {
        public string name { get; set; }
        public bool description { get; set; }
        public bool drug_dosage { get; set; }
        public bool warnings { get; set; }
        public bool side_effects { get; set; }
        public bool trade_names { get; set; }
        public string search_name { get; set; }
        public List<drug_active_name> active_name { get; set; }
        public List<drug_dosage> dosage { get; set; }
        public List<warning> warning { get; set; }
        public List<side_effect> side_effect { get; set; }
        public IEnumerable<drug_trade_search_name> active_name_for_trade { get; set; }

    }
    public class drug_trade_search_name
    {
        public string active_name_for_trade { get; set; }
    }
    public class Drug_a_zModelView {
        public string name { get; set; }
    }
    public class get_drug_pharmacist {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string how_to_take { get; set; }
        public string number_of_dosage { get; set; }
        public string dosage { get; set; }
        public string beginning_of_effectiveness { get; set; }
        public string duration_of_effectiveness { get; set; }
        public string feed { get; set; }
        public string Storage_and_preservation { get; set; }
        public string Forget_a_dose { get; set; }
        public string overdose { get; set; }
        public string User_name { get; set; }

    }
    public class Add_ActiveViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter the name of the drug")]
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 4 , ErrorMessage ="The Name length should be between 10 to 50 character")]
        [Index(IsUnique = true)]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter the description of the drug")]
        [Display(Name = "Description")]
        public string description { get; set; }
        public drug_dosage drug_dosage { get; set; }
        public warning warnings { get; set; }
        public Add_side_effedct Add_side_effedcts { get; set; }
        public Add_trade_name Add_trade_names { get; set; }
        public add_Drug_Interaction Add_interaction { get; set; }

    }
    public class get_ActiveViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter the name of the drug")]
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "The Name length should be between 10 to 50 character")]
        [Index(IsUnique = true)]
        public string name { get; set; }

        [Required(ErrorMessage = "Please enter the description of the drug")]
        [Display(Name = "Description")]
        public string description { get; set; }
        public drug_dosage drug_dosage { get; set; }
        public warning warnings { get; set; }
        public List<side_effedct_get> Add_side_effedcts { get; set; }
        public List<Get_trade_name> Add_trade_names { get; set; }

    }
}