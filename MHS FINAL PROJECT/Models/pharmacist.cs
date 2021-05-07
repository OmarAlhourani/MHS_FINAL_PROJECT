using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MHS_FINAL_PROJECT.Models
{
    public class pharmacist
    {
        [Required]
        public int id { get; set; }

        [Required(ErrorMessage = "Please Enter Your University Name")]
        [Display(Name = "Univarsty Name")]
        public string University_Name { get; set; }

        [Required(ErrorMessage = "Please Enter Your Degree")]
        [Display(Name = "Degree")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Please Upload A Copy Of Your Certification")]
        [Display(Name = "Certification Img")]
        public string certification_img { get; set; }
        public ApplicationUser User { get; set; }
    }

    public class pharmacist_info
    {

        [Required(ErrorMessage = "Please Enter Your University Name")]
        [Display(Name = "Univarsty Name")]
        public string University_Name { get; set; }

        [Required(ErrorMessage = "Please Enter Your Degree")]
        [Display(Name = "Degree")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Please Upload A Copy Of Your Certification")]
        [Display(Name = "Certification Img")]
        public string certification_img { get; set; }
        public HttpPostedFileBase Certification_url { get; set; }

    }

    public class UserPharmacistViewModel
    {
        public string Id { get; set; }
        public int pharmacistsID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string University_Name { get; set; }
        public string Degree { get; set; }
        public string ImgUrl { get; set; }
    }

    public class AddPharmacistViewModel
    {
        public string UserId { get; set; }
        public string NewUserId { get; set; }
        public string UserName { get; set; }
        public string University_Name { get; set; }
        public string Degree { get; set; }
        public string certification_img { get; set; }
        public HttpPostedFileBase Certification_url { get; set; }
    }
}