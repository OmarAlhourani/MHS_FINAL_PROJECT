using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MHS_FINAL_PROJECT.Models
{
    public class ManageAccountViewModel
    {
        public pharmacist_info pharmacist_info { get; set; }

        public user_Add_health health_condation { get; set; }

        public Add_health_drugs health_drugs { get; set; }

        public IndexViewModel UserInformation { get; set; }

        public ChangePasswordViewModel ChangePassword { get; set; }

        public SetPasswordViewModel SetPassword { get; set; }

        public AddNewUserViewModel addnewUser { get; set; }

        public AddPharmacistViewModel Addcertification { get; set; }
    }
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }

        [Required(ErrorMessage = "Please Enter Your Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter Your User name")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Special character should not be entered")]
        [Display(Name = "User name")]
        public string Username{ get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Select Your Gender")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please Enter Your Age")]
        [Display(Name = "Age")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Age must be Number")]
        [Range(minimum: 15, maximum: 60, ErrorMessage = "Age Between 15 and 60")]
        public int age { get; set; }

        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class informationViewModel
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string gender { get; set; }

        public int Age { get; set; }
    }


    //public class ManageLoginsViewModel
    //{
    //    public IList<UserLoginInfo> CurrentLogins { get; set; }
    //    public IList<AuthenticationDescription> OtherLogins { get; set; }
    //}

    //public class FactorViewModel
    //{
    //    public string Purpose { get; set; }
    //}

    public class SetPasswordViewModel
    {
        public string id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Please Enter Old Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please Enter New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please Confirm The New Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //public class AddPhoneNumberViewModel
    //{
    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string Number { get; set; }
    //}

    //public class VerifyPhoneNumberViewModel
    //{
    //    [Required]
    //    [Display(Name = "Code")]
    //    public string Code { get; set; }

    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string PhoneNumber { get; set; }
    //}

    //public class ConfigureTwoFactorViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //}

    public class EditHealthViewModel {
        public int id { get; set; }
        public string health_condition_name { get; set; }
        public string description { get; set; }
        public string User_id { get; set; }
        public string name { get; set; }
        public string dosage { get; set; }
        public int? drug_id { get; set; }
    }

    public class GetUserWithRoelViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string UserName{ get; set; }
        public string Email { get; set; }
        public string role { get; set; }
        public string role_id { get; set; }
    }



}