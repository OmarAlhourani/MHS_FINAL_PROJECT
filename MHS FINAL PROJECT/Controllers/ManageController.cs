using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MHS_FINAL_PROJECT.Models;
using System.Web.Security;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Dynamic;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Text.RegularExpressions;

namespace MHS_FINAL_PROJECT.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }




        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            if (Session["change_done"] != null)
            {
                ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed"
                : message == ManageMessageId.ChangePasswordError ? "The Old Password is not correct"
                : message == ManageMessageId.ChangePasswordErrorSpecial ? "Passwords must have at least one Special character."
                : message == ManageMessageId.ChangePasswordErrorUpperCase ? "Passwords must have at least one uppercase (A-Z)"
                : message == ManageMessageId.OldNewError ? "The New Password is Same as Old Password"

                : message == ManageMessageId.SetPasswordError ? "Something Went Wrong while setting the password"
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set"
                : message == ManageMessageId.SetPasswordErrorSpecial ? "Passwords must have at least one Special character"

                : message == ManageMessageId.EditInformationSuccess ? "Your Information Was Updated Successfully"
                : message == ManageMessageId.EmailExist ? "The Email Is Exist"
                : message == ManageMessageId.UserNameExist ? "The User Name Is Exist"
                : message == ManageMessageId.NothingChange ? "Nothing has Changed"
                : message == ManageMessageId.EditInformationError ? "Something Went Wrong while editing information"


                : message == ManageMessageId.AddHealthCase ? "Your Health Case has been Added Successfully"
                : message == ManageMessageId.EditHealthCase ? "Your Health Case has been Edited Successfully"
                : message == ManageMessageId.errorHealthCase ? "Something Went Wrong"

                : message == ManageMessageId.AddNewDrug ? "New Active Name Drug Has been Added"
                : message == ManageMessageId.EditDrugSuccess ? "Drug Information Was Updated Successfully"
                : message == ManageMessageId.AddTradeNameNew ? "Trade Name Has been Added Successfully"
                : message == ManageMessageId.EdieTradeName ? "Trade Name Has been Edited Successfully"

                : message == ManageMessageId.AddInterActionNew ? "New interaction between two drugs has been added"
                : message == ManageMessageId.EditInterActionNew ? "interaction between two drugs has been Edited Successfully"

                : message == ManageMessageId.AddNewUser ? "User has been Added Successfully"
                : message == ManageMessageId.EditUserInformationSuccess ? "User Information Was Updated Successfully"
                : message == ManageMessageId.EditUserRoleSuccess ? "User Role Was Updated Successfully"
                : message == ManageMessageId.SetPasswordUserSuccess ? "User Password Was Updated Successfully"
                : message == ManageMessageId.AddCertificationError ? "You Have To Add Certification For This User Before Adding It For Pharmacists Role"
                : message == ManageMessageId.SetPasswordErrorUpperCase ? "Passwords must have at least one uppercase (A-Z)."
                : message == ManageMessageId.AddCertificationSuccess ? "Certification was Added Successfully"
                : message == ManageMessageId.AddNewpharmacists ? "Pharmacist Added Successfully But The Account Will Be Locked Until The Certification Get Added"
                : message == ManageMessageId.EditCertificationSuccess ? "Pharmacist Edited Successfully"

                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
                Session["change_done"] = null;

            }
            else
            {
                ViewBag.StatusMessage = null;
            }

            var userId = User.Identity.GetUserId();
            var info = db.Users.Where(x => x.Id == userId).FirstOrDefault();
            var model = new ManageAccountViewModel();
            model.pharmacist_info = new pharmacist_info();
            model.UserInformation = new IndexViewModel
            {
                Name = info.Name,
                Username = info.UserName,
                Email = info.Email,
                Gender = info.Gender,
                age = info.age,
                HasPassword = HasPassword(),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };



            List<SelectListItem> Gender = new List<SelectListItem>();
            if (model.UserInformation.Gender == "Anonymous")
            {
                Gender = new List<SelectListItem>()
                    {
                        new SelectListItem{ Text = "Anonymous", Value = "Anonymous" },
                        new SelectListItem{ Text = "Female", Value = "Female" },
                        new SelectListItem{ Text = "Male", Value = "Male" },
                    };
            }
            else
            {
                Gender = new List<SelectListItem>()
                    {
                        new SelectListItem{ Text = "Female", Value = "Female" },
                        new SelectListItem{ Text = "Male", Value = "Male" },
                    };
            }
            ViewBag.Gender = Gender;

            if (User.IsInRole("Normal_User"))
            {

                int num = 1;
                List<SelectListItem> number = new List<SelectListItem>();
                while (num < 7)
                {
                    number.Add(new SelectListItem() { Text = num.ToString(), Value = num.ToString() });
                    num++;
                }
                ViewBag.Provinces = new SelectList(number, "Value", "Text");
            }
            else if (User.IsInRole("pharmacists"))
            {
                List<SelectListItem> degree = new List<SelectListItem>()
                {
                        new SelectListItem {  Text = "Associate degree", Value = "Associate degree"},
                        new SelectListItem { Text = "Bachelor's degree", Value = "Bachelor's degree"},
                        new SelectListItem { Text = "Master's degree", Value = "Master's degree"},
                        new SelectListItem { Text = "Doctoral degree", Value = "Doctoral degree"},
                        new SelectListItem { Text = "Professional degree", Value = "Professional degree"},
                };
                ViewBag.degree = degree.ToList();

            }
            else if (User.IsInRole("Admins"))
            {
                List<SelectListItem> degree = new List<SelectListItem>()
                {
                        new SelectListItem {  Text = "Associate degree", Value = "Associate degree"},
                        new SelectListItem { Text = "Bachelor's degree", Value = "Bachelor's degree"},
                        new SelectListItem { Text = "Master's degree", Value = "Master's degree"},
                        new SelectListItem { Text = "Doctoral degree", Value = "Doctoral degree"},
                        new SelectListItem { Text = "Professional degree", Value = "Professional degree"},
                };
                ViewBag.degree = degree.ToList();

                var Role = db.Roles.ToList();
                List<SelectListItem> Roles = new List<SelectListItem>();
                foreach (var name in Role)
                {
                    Roles.Add(new SelectListItem() { Text = name.Name.ToString(), Value = name.Id.ToString() });
                }
                ViewBag.Roles = new SelectList(Roles, "Value", "Text");

                var users = db.Users.ToList();
                List<SelectListItem> UserName = new List<SelectListItem>();
                foreach (var name in users)
                {
                    var IfRole = UserManager.GetRoles(name.Id);
                    if (IfRole[0] == "pharmacists")
                    {
                        var IfCert = db.pharmastics.Where(x => x.User.Id == name.Id).FirstOrDefault();
                        if (IfCert == null)
                        {
                            UserName.Add(new SelectListItem() { Text = name.UserName.ToString(), Value = name.Id.ToString() });
                        }

                    }
                    else if (IfRole[0] == "Normal_User")
                    {
                        UserName.Add(new SelectListItem() { Text = name.UserName.ToString(), Value = name.Id.ToString() });
                    }

                }
                ViewBag.UserName = new SelectList(UserName, "Value", "Text");

            }

            return View(model);
        }




        //******************************************* Personal Information *********************************************
        //it contain also the reset password by admin for user**********************************************************
        #region User Infromation information
        //load PartialView for user information page ************************************************
        [Authorize]
        public async Task<PartialViewResult> personal_information()
        {
            ViewBag.Health_information = null;
            ViewBag.personal_information = "true";
            ApplicationDbContext db = new ApplicationDbContext();
            var userId = User.Identity.GetUserId();
            var info = db.Users.Where(x => x.Id == userId).FirstOrDefault();
            var model = new ManageAccountViewModel();
            model.UserInformation = new IndexViewModel
            {
                Name = info.Name,
                Username = info.UserName,
                Email = info.Email,
                Gender = info.Gender,
                age = info.age,
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };

            return PartialView("_user_info", model);
        }




        //change password for user account********************************************************************
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Include = "ChangePassword")] ManageAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordError });
            }
            if (model.ChangePassword.NewPassword == model.ChangePassword.OldPassword)
            {
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.OldNewError });
            }
            else if (!(model.ChangePassword.NewPassword.Any(char.IsUpper)))
            {
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordErrorUpperCase });
            }
            else if ((Regex.IsMatch(model.ChangePassword.NewPassword, @"^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
            {
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordErrorSpecial });
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.ChangePassword.OldPassword, model.ChangePassword.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                ViewBag.ChangePasswordSuccess = ManageMessageId.ChangePasswordSuccess;
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordError });
        }




        //set new password for login using google or facebook or reset password by admin for user*************************
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword([Bind(Include = "SetPassword")] ManageAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!(model.SetPassword.NewPassword.Any(char.IsUpper)))
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordErrorUpperCase });
                }
                else if((Regex.IsMatch(model.SetPassword.NewPassword, @"^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordErrorSpecial });
                }
                if (model.SetPassword.id == null || model.SetPassword.id == "")
                {
                    var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.SetPassword.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        if (user != null)
                        {
                            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            Session["change_done"] = "true";
                            return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                        }
                       
                    }
                }
                //for reset password from admin************************************************************
                else
                {
                    var result_remove = await UserManager.RemovePasswordAsync(model.SetPassword.id);
                    if (result_remove.Succeeded)
                    {
                        var result = await UserManager.AddPasswordAsync(model.SetPassword.id, model.SetPassword.NewPassword);
                        if (result.Succeeded)
                        {
                            var user = await UserManager.FindByIdAsync(model.SetPassword.id);
                            if (user != null)
                            {
                                Session["change_done"] = "true";
                                return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordUserSuccess });
                            }
                            
                        }
                    }
                    
                }
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordError });
        }



        //Edit user information*************************************************************************************
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditInformation([Bind(Include = "UserInformation")] ManageAccountViewModel model)
        {

            if (ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var userId = User.Identity.GetUserId();
                    var user = db.Users.Where(x => x.Id == userId).AsQueryable().FirstOrDefault();
                    if (user.Name == model.UserInformation.Name && user.Email == model.UserInformation.Email && user.UserName == model.UserInformation.Username && user.Gender == model.UserInformation.Gender && user.age == model.UserInformation.age)
                    {
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.NothingChange });
                    }
                    var email = db.Users.Where(y => y.Email == model.UserInformation.Email).FirstOrDefault();
                    if (email != null && email.Id != userId)
                    {
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.EmailExist });
                    }
                    user.Name = model.UserInformation.Name;
                    user.Email = model.UserInformation.Email;
                    user.UserName = model.UserInformation.Username;
                    user.Gender = model.UserInformation.Gender;
                    user.age = model.UserInformation.age;
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        db.SaveChanges();
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.EditInformationSuccess });
                    }
                    else
                    {
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.UserNameExist });
                    }
                }
            }
            if (ModelState["Age"].Errors.Count > 0 && ModelState["Age"].Errors[0].ErrorMessage.Contains("is not valid for Age"))
            {
                ModelState["Age"].Errors.Clear();
                ModelState["Age"].Errors.Add("Age must be Number");
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.EditInformationError });
        }

        #endregion








        //******************************************* Health condation *********************************************
        #region Health condations
        //load partialView for health condation page**********************************************************************************
        [Authorize(Roles = "Normal_User")]
        public PartialViewResult Health_information([Bind(Include = "health_condation , health_drugs")] ManageAccountViewModel model)
        {
            ViewBag.Health_information = "true";
            ViewBag.personal_information = null;
            return PartialView("_user_health", model);
        }



        //Load Health condation datatable*********************************************************************************************
        [Authorize(Roles = "Normal_User")]
        public JsonResult LoadHeath()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                var userId = User.Identity.GetUserId();
                SqlParameter[] par_health = new SqlParameter[] {
                        new SqlParameter("@id" , userId)
                        };
                List<user_health_return> health = db.Database.SqlQuery<user_health_return>("get_user_health @id", par_health).ToList();

                List<user_drug_num_return> drug = null;
                if (health.Count() != 0 || health != null)
                {
                    foreach (var row in health)
                    {
                        int id = row.id;
                        SqlParameter[] par_drug_health = new SqlParameter[] {
                            new SqlParameter("@id" , id)
                            };
                        drug = db.Database.SqlQuery<user_drug_num_return>("get_drug_health @id", par_drug_health).ToList();
                        foreach (var drugs in drug)
                        {
                            row.number_of_drugs = drugs.number_of_drugs;
                        }
                    }
                }
                ////Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    health = health.Where(m => (m.health_condition_name.ToLower().Contains(searchValue)) || (m.health_condition_name.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = health.Count();
                //Paging     
                var data = health.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }




        //Add New Health condation********************************************************************************************
        [Authorize(Roles = "Normal_User")]
        public ActionResult AddHealth([Bind(Include = "health_condation , health_drugs")] ManageAccountViewModel model)
        {
            model = (ManageAccountViewModel)TempData["_person"];
            //edit
            if (model.health_condation.id > 0)
            {
                SqlParameter[] health = new SqlParameter[]
                        {
                            new SqlParameter("@name" , model.health_condation.health_condition_name.ToString()),
                            new SqlParameter("@desc" , model.health_condation.description.ToString()),
                            new SqlParameter("@id" , model.health_condation.id),
                        };
                var insert_id = db.Database.ExecuteSqlCommand("Update_health @name , @desc , @id", health);
                if (insert_id == -1)
                {
                    var count = 0;
                    while (count < model.health_drugs.name.Count())
                    {
                        var name = model.health_drugs.name[count].ToString();
                        var dosage = model.health_drugs.dosage[count].ToString();
                        if (model.health_drugs.drug_id.Count() == 0 || model.health_drugs.drug_id[count] <= 0)
                        {
                            SqlParameter[] drug_helth_add = new SqlParameter[]
                            {
                                    new SqlParameter("@name" , name.ToString()),
                                    new SqlParameter("@dosage" , dosage.ToString()),
                                    new SqlParameter("@user_healthId" , model.health_condation.id),
                            };
                            var insert_new_drug = db.Database.ExecuteSqlCommand("set_drug_for_health @name , @dosage , @user_healthId", drug_helth_add);
                        }
                        else
                        {
                            var id = model.health_drugs.drug_id[count];
                            SqlParameter[] drug_helth = new SqlParameter[]
                                {
                                    new SqlParameter("@name" , name.ToString()),
                                    new SqlParameter("@dosage" , dosage.ToString()),
                                    new SqlParameter("@id" , id),
                                };
                            var insert_drug = db.Database.ExecuteSqlCommand("Update_drug_health @name , @dosage , @id", drug_helth);
                        }
                        count++;
                    }
                }
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.EditHealthCase });
            }
            else
            {
                //add
                if (ModelState.IsValid)
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        var userId = User.Identity.GetUserId();
                        SqlParameter[] health = new SqlParameter[]
                        {
                            new SqlParameter("@health_condition_name" , model.health_condation.health_condition_name.ToString()),
                            new SqlParameter("@description" , model.health_condation.description.ToString()),
                            new SqlParameter("@User_Id" , userId.ToString()),
                        };
                        var insert_id = db.Database.SqlQuery<Added_id>("set_user_health @health_condition_name , @description , @User_Id", health).ToList();
                        var count = 0;
                        while (count < model.health_drugs.name.Count())
                        {
                            var name = model.health_drugs.name[count].ToString();
                            var dosage = model.health_drugs.dosage[count].ToString();
                            SqlParameter[] drug_helth = new SqlParameter[]
                                {
                                    new SqlParameter("@name" , name.ToString()),
                                    new SqlParameter("@dosage" , dosage.ToString()),
                                    new SqlParameter("@user_healthId" , insert_id[0].id),
                                };
                            var insert_drug = db.Database.ExecuteSqlCommand("set_drug_for_health @name , @dosage , @user_healthId", drug_helth);
                            count++;
                        }
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.AddHealthCase });
                    }
                }
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.errorHealthCase });

        }





        //check validation for adding or editing healthg case*************************************************
        [Authorize(Roles = "Normal_User")]
        public JsonResult chaeckAddHealthe([Bind(Include = "health_condation , health_drugs")] ManageAccountViewModel model)
        {
            if (model.health_condation.health_condition_name == null || model.health_condation.health_condition_name == "")
            {
                ViewBag.health_condition_name = "Please Enter Health Condition Name";
            }
            if (model.health_condation.description == null || model.health_condation.description == "")
            {
                ViewBag.description = "Please Enter Health Condition Description";
            }
            var count_valid = 0;
            while (count_valid < model.health_drugs.name.Count())
            {
                var name = model.health_drugs.name[count_valid].ToString();
                var dosage = model.health_drugs.dosage[count_valid].ToString();
                if (name == "" || dosage == "")
                {
                    ViewBag.name_dosage = "Please Enter All Health Condition Drugs information";
                }
                count_valid++;
            }
            if (ViewBag.health_condition_name == null && ViewBag.description == null && ViewBag.name_dosage == null)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("AddHealth");
                TempData["_person"] = model;
                return Json(new { Url = redirectUrl });
            }
            List<SelectListItem> json = new List<SelectListItem> {
                    new SelectListItem{ Value =  ViewBag.health_condition_name },
                    new SelectListItem{ Value =  ViewBag.description },
                    new SelectListItem{ Value =  ViewBag.name_dosage },
                };
            return Json(json, JsonRequestBehavior.AllowGet);

        }





        //load health case all information for editing***************************************************************
        [Authorize(Roles = "Normal_User")]
        public JsonResult loadHealth(string id_edut)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SqlParameter[] health = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id_edut))
                        };
                var insert_id = db.Database.SqlQuery<EditHealthViewModel>("get_health_with_drug @id ", health).ToList();
                return Json(insert_id, JsonRequestBehavior.AllowGet);
            }
        }





        //Delete drug for helth case*********************************************************************************
        [Authorize(Roles = "Normal_User")]
        public JsonResult DeleteHealthDrug(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SqlParameter[] remove_drug_id = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var delete_drug = db.Database.ExecuteSqlCommand("delete_helth_drug @id ", remove_drug_id);
                return Json(delete_drug, JsonRequestBehavior.AllowGet);
            }
        }




        //Delete the whole health condation************************************************************************
        [Authorize(Roles = "Normal_User")]
        public JsonResult DeleteHealthcondation(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SqlParameter[] remove_drug_id = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var delete_drug = db.Database.ExecuteSqlCommand("delete_helth @id ", remove_drug_id);
                return Json(delete_drug, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion









        //********************************************* certification for pharmacists *********************************************
        #region certification for pharmacists

        //load certification PartialView and the information for it***************************************
        [Authorize(Roles = "pharmacists")]
        public PartialViewResult certification_information()
        {

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();

                SqlParameter[] par = new SqlParameter[] {
                        new SqlParameter( "@id" , userId )
                    };
                ManageAccountViewModel model = new ManageAccountViewModel();
                var certification = db.Database.SqlQuery<pharmacist_info>("get_pharmastics @id", par);
                foreach (var certificat in certification)
                {
                    model.pharmacist_info = new pharmacist_info
                    {
                        University_Name = certificat.University_Name,
                        Degree = certificat.Degree,
                        certification_img = certificat.certification_img
                    };
                }
                List<SelectListItem> degree = new List<SelectListItem>()
                    {
                        new SelectListItem {  Text = "Associate degree", Value = "Associate degree"},
                        new SelectListItem { Text = "Bachelors degree", Value = "Bachelor's degree"},
                        new SelectListItem { Text = "Masters degree", Value = "Master's degree"},
                        new SelectListItem { Text = "Doctoral degree", Value = "Doctoral degree"},
                        new SelectListItem { Text = "Professional degree", Value = "Professional degree"},
                    };
                ViewBag.degree = degree.ToList();

                return PartialView("_certification_information", model);
            }
        }




        //Load popup for Edit The certification*********************************************************
        [Authorize(Roles = "pharmacists")]
        public JsonResult Edit_certification_data()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();

                SqlParameter[] par = new SqlParameter[] {
                        new SqlParameter( "@id" , userId )
                    };
                ManageAccountViewModel model = new ManageAccountViewModel();
                var certification = db.Database.SqlQuery<pharmacist_info>("get_pharmastics @id", par);
                foreach (var certificat in certification)
                {
                    model.pharmacist_info = new pharmacist_info
                    {
                        University_Name = certificat.University_Name,
                        Degree = certificat.Degree,
                        certification_img = certificat.certification_img,
                    };
                    TempData["IMG"] = certificat.certification_img;
                }
                List<SelectListItem> degree = new List<SelectListItem>()
                    {
                        new SelectListItem { Text = "Associate degree", Value = "Associate degree"},
                        new SelectListItem { Text = "Bachelors degree", Value = "Bachelor's degree"},
                        new SelectListItem { Text = "Masters degree", Value = "Master's degree"},
                        new SelectListItem { Text = "Doctoral degree", Value = "Doctoral degree"},
                        new SelectListItem { Text = "Professional degree", Value = "Professional degree"},
                    };
                ViewBag.degree = degree.ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }





        //check validation for the entered certification information on edit***********************************************
        [Authorize(Roles = "pharmacists")]
        public JsonResult Edit_certification_valudation([Bind(Include = "pharmacist_info")] ManageAccountViewModel model)
        {
            if (model.pharmacist_info.University_Name == null || model.pharmacist_info.University_Name == "")
            {
                ViewBag.University_Name = "Please Enter Your University Name";
            }
            if (model.pharmacist_info.Degree == null || model.pharmacist_info.Degree == "")
            {
                ViewBag.Degrees = "Please Select Your Degree";
            }
            if (model.pharmacist_info.certification_img == null || model.pharmacist_info.certification_img == "")
            {
                ViewBag.certification_img = "Please Upload A Copy Of Your Certification";
            }
            if (ViewBag.University_Name == null && ViewBag.Degrees == null && ViewBag.certification_img == null)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit_certification_save");
                TempData["_certification"] = model;
                return Json(new { Url = redirectUrl });
            }
            List<SelectListItem> json = new List<SelectListItem> {
                    new SelectListItem{ Value = ViewBag.University_Name },
                    new SelectListItem{ Value = ViewBag.Degrees },
                    new SelectListItem{ Value = ViewBag.certification_img },
                };
            return Json(json, JsonRequestBehavior.AllowGet);
        }






        //Save All New certification Information*********************************************************************
        [Authorize(Roles = "pharmacists")]
        public ActionResult Edit_certification_save([Bind(Include = "pharmacist_info")] ManageAccountViewModel model, HttpPostedFileBase file)
        {
            model = (ManageAccountViewModel)TempData["_certification"];

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userId = User.Identity.GetUserId();
                SqlParameter[] par = new SqlParameter[] {
                        new SqlParameter( "@id" , userId ),
                    };
                var certification = db.Database.SqlQuery<pharmacist_info>("get_pharmastics @id", par).ToList();
                if (certification[0].University_Name == model.pharmacist_info.University_Name &&
                    certification[0].Degree == model.pharmacist_info.Degree &&
                    certification[0].certification_img == model.pharmacist_info.certification_img)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Index", new { Message = ManageMessageId.NothingChange });
                }
                else
                {

                    if (model.pharmacist_info.Certification_url != null)
                    {
                        string realPath = certification[0].certification_img.Split('/')[2];
                        string fullPath = Request.MapPath("~/CERTIFICATION_IMG/" + realPath);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        file = model.pharmacist_info.Certification_url;
                        model.pharmacist_info.certification_img = file.ToString(); //getting complete url  
                        var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg) 
                        var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension
                        string myfile = name + "_" + userId + ext; //appending the name with id  
                        var path = Path.Combine(Server.MapPath("~/CERTIFICATION_IMG"), myfile); // store the file inside ~/CERTIFICATION_IMG folder(CERTIFICATION_IMG)  
                        file.SaveAs(path);
                        path = "/CERTIFICATION_IMG/" + myfile;
                        model.pharmacist_info.certification_img = path;
                    }
                    SqlParameter[] Update = new SqlParameter[] {
                            new SqlParameter( "@id" , userId ),
                            new SqlParameter( "@Univarsty_Name" , model.pharmacist_info.University_Name ),
                            new SqlParameter( "@Degree" , model.pharmacist_info.Degree ),
                            new SqlParameter( "@certification_img" , model.pharmacist_info.certification_img )
                        };
                    var update = db.Database.ExecuteSqlCommand("Update_certification @id , @Univarsty_Name , @Degree , @certification_img", Update);
                    if (update == -1)
                    {
                        UserManager.SetLockoutEnabled(userId, true);
                        TempData["done"] = "Your Account Will Be Locked Until The Admin Accept Your New Certification Information";
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.EditInformationError });
        }

        #endregion











        //********************************************** Active Drug ************************************************
        //it contain also the delete for trade name alone************************************************************
        #region Active drug


        //************************************** add new active drug *********************************************
        #region Add Active Drug
        //load partialview for drug active name page *******************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public PartialViewResult active_name_drugs()
        {
            return PartialView("_active_name_drugs");
        }






        //load datatable for all drug active for drug active page*******************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult LoadActiveName()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                List<get_drug_pharmacist> Name = new List<get_drug_pharmacist>();
                if (User.IsInRole("Admins"))
                {
                    SqlParameter[] par_health = new SqlParameter[] {
                        new SqlParameter("@user_id" , "0")
                        };
                    Name = db.Database.SqlQuery<get_drug_pharmacist>("get_drug_pharmacist_name @user_id", par_health).ToList();
                }
                else
                {
                    var userId = User.Identity.GetUserId();
                    SqlParameter[] par_health = new SqlParameter[] {
                        new SqlParameter("@user_id" , userId)
                        };
                    Name = db.Database.SqlQuery<get_drug_pharmacist>("get_drug_pharmacist_name @user_id", par_health).ToList();
                }


                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Name = Name.Where(m => (m.name.ToLower().Contains(searchValue)) || (m.name.Contains(searchValue)) ||
                    (m.User_name.ToLower().Contains(searchValue)) || (m.User_name.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Name.Count();
                //Paging     
                var data = Name.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }






        //load page add new drug active*************************************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public ActionResult Add_New_Drug()
        {
            return View();
        }





        //check validation for single information for add Active drug**********************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult ValidationDrugInfo([Bind(Include = "name , id , description , drug_dosage , warnings")] Add_ActiveViewModel model)
        {
            if (ModelState.IsValid)
            {
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (model.name == null || model.name == "")
                {
                    ViewBag.name = "Please enter the name of the drug";
                }
                else
                {
                    var name = db.drug_active_names.Where(n => n.name == model.name).FirstOrDefault();
                    if (name != null && model.id == 0)
                    {
                        ViewBag.name = "The Drug Name You Entered Is Exist";
                    }
                    else if (model.id != 0)
                    {
                        var check = db.drug_active_names.Where(n => n.name == model.name && n.id != model.id).FirstOrDefault();
                        if (check != null)
                        {
                            ViewBag.name = "The Drug Name You Entered Is Exist";
                        }
                    }

                }
                if (model.description == null || model.description == "")
                {
                    ViewBag.description = "Please enter the description of the drug";
                }
                if (model.drug_dosage == null)
                {
                    ViewBag.drug_dosage = "All Drug Dosage Fields must be Filled";
                }
                else if ((model.drug_dosage.how_to_take == null || model.drug_dosage.how_to_take == "") ||
                    (model.drug_dosage.number_of_dosage == null || model.drug_dosage.number_of_dosage == "") ||
                    (model.drug_dosage.dosage == null || model.drug_dosage.dosage == "") ||
                    (model.drug_dosage.beginning_of_effectiveness == null || model.drug_dosage.beginning_of_effectiveness == "") ||
                    (model.drug_dosage.duration_of_effectiveness == null || model.drug_dosage.duration_of_effectiveness == "") ||
                    (model.drug_dosage.feed == null || model.drug_dosage.feed == "") ||
                    (model.drug_dosage.Storage_and_preservation == null || model.drug_dosage.Storage_and_preservation == "") ||
                    (model.drug_dosage.Forget_a_dose == null || model.drug_dosage.Forget_a_dose == "") ||
                    (model.drug_dosage.overdose == null || model.drug_dosage.overdose == ""))
                {
                    ViewBag.drug_dosage = "All Drug Dosage Fields must be Filled";
                }
                if (model.warnings == null)
                {
                    ViewBag.warnings = "All Warning Fields must be Filled";
                }
                else if ((model.warnings.During_pregnancy == null || model.warnings.During_pregnancy == "") ||
                    (model.warnings.Breast_feeding == null || model.warnings.Breast_feeding == "") ||
                    (model.warnings.Children_and_infants == null || model.warnings.Children_and_infants == "") ||
                    (model.warnings.Elderly == null || model.warnings.Elderly == "") ||
                    (model.warnings.Driving == null || model.warnings.Driving == "") ||
                    (model.warnings.Surgery_and_anesthesia == null || model.warnings.Surgery_and_anesthesia == ""))
                {

                    ViewBag.warnings = "All Warning Fields must be Filled";
                }

                if (ViewBag.name == null && ViewBag.description == null && ViewBag.drug_dosage == null && ViewBag.warnings == null && model.id == 0)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else if (ViewBag.name == null && ViewBag.description == null && ViewBag.drug_dosage == null && ViewBag.warnings == null && model.id != 0)
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("EditDrugActive");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    ViewBag.All = "Please Make sure That You Filled All Fields And There Is No Validation";
                }
                List<SelectListItem> json = new List<SelectListItem> {
                    new SelectListItem{ Value = ViewBag.name },
                    new SelectListItem{ Value = ViewBag.description },
                    new SelectListItem{ Value = ViewBag.drug_dosage },
                    new SelectListItem{ Value = ViewBag.warnings },
                    new SelectListItem{ Value = ViewBag.All },
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }






        //check validation for add side effect for add Active drug**********************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult ValidationEffect([Bind(Include = "Add_side_effedcts")] Add_ActiveViewModel model)
        {

            if (model.Add_side_effedcts.name[0] == null || model.Add_side_effedcts.name[0] == "")
            {
                ViewBag.name = "Please enter the name of the Side Effect";
            }
            if (model.Add_side_effedcts.prevalence_effect[0] == null || model.Add_side_effedcts.prevalence_effect[0] == "")
            {
                ViewBag.prevalence_effect = "Please enter The degree of influence of the Side Effect";
            }
            if (model.Add_side_effedcts.inform_doctor[0] == null || model.Add_side_effedcts.inform_doctor[0] == "")
            {
                ViewBag.inform_doctor = "Please Enter if the user should inform the doctor";
            }
            if (ViewBag.name == null && ViewBag.prevalence_effect == null && ViewBag.inform_doctor == null)
            {
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            List<SelectListItem> json = new List<SelectListItem> {
                        new SelectListItem{ Value = ViewBag.name },
                        new SelectListItem{ Value = ViewBag.prevalence_effect },
                        new SelectListItem{ Value = ViewBag.inform_doctor },
                        };
            return Json(json, JsonRequestBehavior.AllowGet);

        }






        //check validation for add trade name for add Active drug**********************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult ValidationTrade([Bind(Include = "Add_trade_names")] Add_ActiveViewModel model)
        {

            if (model.Add_trade_names.trade_name[0] == null || model.Add_trade_names.trade_name[0] == "")
            {
                ViewBag.trade_name = "Please enter the name of the Trade Drug";
            }
            if (model.Add_trade_names.Manufacturer_company[0] == null || model.Add_trade_names.Manufacturer_company[0] == "")
            {
                ViewBag.Manufacturer_company = "Please enter The Manufacturer Company Name of The Drug";
            }
            if (ViewBag.trade_name == null && ViewBag.Manufacturer_company == null)
            {
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            List<SelectListItem> json = new List<SelectListItem> {
                        new SelectListItem{ Value = ViewBag.trade_name },
                        new SelectListItem{ Value = ViewBag.Manufacturer_company },
                        };
            return Json(json, JsonRequestBehavior.AllowGet);

        }





        //add(save) new druge active********************************************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult AddDrugEnd(Add_ActiveViewModel model)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {   //add main
                var userId = User.Identity.GetUserId();
                SqlParameter[] Active_main = new SqlParameter[]
                {
                            new SqlParameter("@name" , model.name.ToString()),
                            new SqlParameter("@description" , model.description.ToString()),
                            new SqlParameter("@added_by" , userId.ToString()),
                };
                var insert_id = db.Database.SqlQuery<Added_id>("Set_Active_For_New @name , @description , @added_by", Active_main).ToList();
                var count_effect = 0;
                //add effect
                while (count_effect < model.Add_side_effedcts.name.Count())
                {
                    var name = model.Add_side_effedcts.name[count_effect].ToString();
                    var prevalence = model.Add_side_effedcts.prevalence_effect[count_effect].ToString();
                    var inform = model.Add_side_effedcts.inform_doctor[count_effect].ToString();
                    SqlParameter[] drug_effect = new SqlParameter[]
                        {
                                    new SqlParameter("@name" , name.ToString()),
                                    new SqlParameter("@prevalence_effect" , prevalence.ToString()),
                                    new SqlParameter("@inform_doctor" , inform.ToString()),
                                    new SqlParameter("@drug_active_nameId" , (int)insert_id[0].id),
                        };
                    var insert_effect = db.Database.ExecuteSqlCommand("Set_Effect_For_New @name , @prevalence_effect , @inform_doctor , @drug_active_nameId", drug_effect);
                    count_effect++;
                }
                Int32 insert_trade = 0;
                Int32 insert_Dosage = 0;
                Int32 insert_Warnings = 0;
                //add trade
                var count_Trade = 0;
                while (count_Trade < model.Add_trade_names.trade_name.Count())
                {
                    if (model.Add_trade_names.trade_name[count_Trade].ToLower() == model.name.ToString().ToLower())
                    {
                        model.Add_trade_names.trade_name[count_Trade] = model.Add_trade_names.trade_name[count_Trade] + "/" + model.Add_trade_names.Manufacturer_company[count_Trade];
                    }
                    var name = model.Add_trade_names.trade_name[count_Trade].ToString();
                    var Manufacturer = model.Add_trade_names.Manufacturer_company[count_Trade].ToString();
                    SqlParameter[] drug_trade = new SqlParameter[]
                        {
                                    new SqlParameter("@trade_name" , name.ToString()),
                                    new SqlParameter("@Manufacturer_company" , Manufacturer.ToString()),
                                    new SqlParameter("@drug_active_nameId" , (int)insert_id[0].id),
                        };
                    insert_trade = db.Database.ExecuteSqlCommand("Set_Trade_For_New @trade_name , @Manufacturer_company , @drug_active_nameId", drug_trade);
                    count_Trade++;
                }
                if (insert_trade == 1 || insert_trade == -1)
                {
                    //add Dosage
                    SqlParameter[] drug_Dosage = new SqlParameter[]
                            {
                                    new SqlParameter("@how_to_take" , model.drug_dosage.how_to_take.ToString()),
                                    new SqlParameter("@number_of_dosage" , model.drug_dosage.number_of_dosage.ToString()),
                                    new SqlParameter("@dosage" , model.drug_dosage.dosage.ToString()),
                                    new SqlParameter("@beginning_of_effectiveness" , model.drug_dosage.beginning_of_effectiveness.ToString()),
                                    new SqlParameter("@duration_of_effectiveness" , model.drug_dosage.duration_of_effectiveness.ToString()),
                                    new SqlParameter("@feed" , model.drug_dosage.feed),
                                    new SqlParameter("@Storage_and_preservation" , model.drug_dosage.Storage_and_preservation.ToString()),
                                    new SqlParameter("@Forget_a_dose" , model.drug_dosage.Forget_a_dose.ToString()),
                                    new SqlParameter("@overdose" , model.drug_dosage.overdose.ToString()),
                                    new SqlParameter("@drug_active_name_id" , (int)insert_id[0].id),
                            };
                    insert_Dosage = db.Database.ExecuteSqlCommand("Set_Dosage_For_New @how_to_take , @number_of_dosage , @dosage , @beginning_of_effectiveness , @duration_of_effectiveness , @feed , @Storage_and_preservation , @Forget_a_dose , @overdose , @drug_active_name_id", drug_Dosage);
                    if (insert_Dosage == 1 || insert_Dosage == -1)
                    {
                        //add Warnings
                        SqlParameter[] drug_Warnings = new SqlParameter[]
                                {
                                    new SqlParameter("@During_pregnancy" , model.warnings.During_pregnancy.ToString()),
                                    new SqlParameter("@Breast_feeding" , model.warnings.Breast_feeding.ToString()),
                                    new SqlParameter("@Children_and_infants" , model.warnings.Children_and_infants.ToString()),
                                    new SqlParameter("@Elderly" , model.warnings.Elderly.ToString()),
                                    new SqlParameter("@Driving" , model.warnings.Driving.ToString()),
                                    new SqlParameter("@Surgery_and_anesthesia" , model.warnings.Surgery_and_anesthesia.ToString()),
                                    new SqlParameter("@drug_active_nameId" , (int)insert_id[0].id),
                                };
                        insert_Warnings = db.Database.ExecuteSqlCommand("Set_Warnings_For_New @During_pregnancy ,@Breast_feeding , @Children_and_infants ," +
                            " @Elderly , @Driving , @Surgery_and_anesthesia , @drug_active_nameId", drug_Warnings);
                    }
                    else
                    {
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
                        Session["change_done"] = "true";
                        return Json(new { Url = redirectUrl });
                    }

                    if (insert_Warnings == 1 || insert_Warnings == -1)
                    {
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.AddNewDrug });
                        Session["change_done"] = "true";
                        return Json(new { Url = redirectUrl });
                    }
                    else
                    {
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
                        Session["change_done"] = "true";
                        return Json(new { Url = redirectUrl });
                    }
                }
                else
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
                    Session["change_done"] = "true";
                    return Json(new { Url = redirectUrl });
                }
            }
        }






        //Delete active drug ***********************************************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult DeleteActiveDrug(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SqlParameter[] remove_drug_id = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var delete_drug = db.Database.ExecuteSqlCommand("Delete_active_drug @id ", remove_drug_id);
                return Json(delete_drug, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion




        //************************************** Edit active drug *************************************************
        #region Edit active drug
        //load data in page edit Active Drug ********************************************************************
        [Authorize(Roles = "pharmacists  , Admins")]
        public ActionResult Edit_Active_drug(string id, ManageMessageId? message)
        {
            if (Session["change_done"] != null)
            {
                ViewBag.StatusMessage =
                    message == ManageMessageId.AddSideEffedcts ? "New Side Effect Has been Added Successfully"
                : message == ManageMessageId.AddTradeName ? "New Trade Name Has been Added Successfully"
                : message == ManageMessageId.EdieSideEffedcts ? "Side Effect Has been Updated Successfully"
                : message == ManageMessageId.EdieTradeName ? "Trade Name Has been Updated Successfully"
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
                Session["change_done"] = null;
            }
            else
            {
                ViewBag.StatusMessage = null;
            }

            if (id != null)
            {
                ViewBag.id = id;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Add_ActiveViewModel model = new Add_ActiveViewModel();
                    SqlParameter[] drug_id_m = new SqlParameter[]
                            {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                            };
                    var drug_main = db.Database.SqlQuery<Add_ActiveViewModel>("get_active_main @id ", drug_id_m).ToList();

                    SqlParameter[] drug_id_w = new SqlParameter[]
                            {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                            };
                    var drug_warnings = db.Database.SqlQuery<warning_get>("get_active_warnings @id ", drug_id_w).ToList();

                    SqlParameter[] drug_id_d = new SqlParameter[]
                            {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                            };
                    var drug_dosage = db.Database.SqlQuery<drug_dosage_get>("get_active_dosage @id", drug_id_d).ToList();


                    if (drug_main.Count > 0)
                    {
                        model.name = drug_main[0].name;
                        model.description = drug_main[0].description;
                    }

                    if (drug_warnings.Count > 0)
                    {
                        model.warnings = new warning
                        {
                            During_pregnancy = drug_warnings[0].During_pregnancy,
                            Breast_feeding = drug_warnings[0].Breast_feeding,
                            Children_and_infants = drug_warnings[0].Children_and_infants,
                            Elderly = drug_warnings[0].Elderly,
                            Driving = drug_warnings[0].Driving,
                            Surgery_and_anesthesia = drug_warnings[0].Surgery_and_anesthesia
                        };
                    }
                    if (drug_dosage.Count > 0)
                    {
                        model.drug_dosage = new drug_dosage
                        {
                            how_to_take = drug_dosage[0].how_to_take,
                            number_of_dosage = drug_dosage[0].number_of_dosage,
                            feed = drug_dosage[0].feed,
                            dosage = drug_dosage[0].dosage,
                            beginning_of_effectiveness = drug_dosage[0].beginning_of_effectiveness,
                            duration_of_effectiveness = drug_dosage[0].duration_of_effectiveness,
                            Storage_and_preservation = drug_dosage[0].Storage_and_preservation,
                            Forget_a_dose = drug_dosage[0].Forget_a_dose,
                            overdose = drug_dosage[0].overdose,
                        };
                    }
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        //load Side Effect table for edit Active Drug page************************************************************
        [Authorize(Roles = "pharmacists  , Admins")]
        public JsonResult LoadSideEffect(string id)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                SqlParameter[] drug_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
                var Name = db.Database.SqlQuery<side_effect>("get_side_effect_id @id", drug_id).ToList();

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Name = Name.Where(m => (m.name.ToLower().Contains(searchValue)) || (m.name.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Name.Count();
                //Paging     
                var data = Name.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }




        //load trade name table for more info page and edit Active Drug page************************************************************
        [Authorize(Roles = "pharmacists  , Admins")]
        public JsonResult LoadTradeNames(string id)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                SqlParameter[] drug_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
                var Name = db.Database.SqlQuery<drug_trade_name>("get_trade_name_id @id", drug_id).ToList();

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Name = Name.Where(m => (m.trade_name.ToLower().Contains(searchValue)) || (m.trade_name.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Name.Count();
                //Paging     
                var data = Name.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }



        //save edit single information for active drug in Edit Active Drug page***********************************************************
        [Authorize(Roles = "pharmacists  , Admins")]
        public JsonResult EditDrugActive([Bind(Include = "name , id , description , drug_dosage , warnings")] Add_ActiveViewModel model)
        {
            try
            {
                var find = db.drug_active_names.Where(m => m.id == model.id).FirstOrDefault();
                var find_dosages = db.drug_dosages.Where(m => m.drug_active_name.id == model.id).FirstOrDefault();
                var find_warnings = db.warnings.Where(m => m.drug_active_nameId == model.id).FirstOrDefault();
                if (find.name == model.name && find.description == model.description &&
                    find_dosages.how_to_take == model.drug_dosage.how_to_take &&
                    find_dosages.number_of_dosage == model.drug_dosage.number_of_dosage &&
                    find_dosages.dosage == model.drug_dosage.dosage &&
                    find_dosages.beginning_of_effectiveness == model.drug_dosage.beginning_of_effectiveness &&
                    find_dosages.duration_of_effectiveness == model.drug_dosage.duration_of_effectiveness &&
                    find_dosages.feed == model.drug_dosage.feed &&
                    find_dosages.Storage_and_preservation == model.drug_dosage.Storage_and_preservation &&
                    find_dosages.Forget_a_dose == model.drug_dosage.Forget_a_dose &&
                    find_dosages.overdose == model.drug_dosage.overdose &&
                    find_warnings.During_pregnancy == model.warnings.During_pregnancy &&
                    find_warnings.Breast_feeding == model.warnings.Breast_feeding &&
                    find_warnings.Children_and_infants == model.warnings.Children_and_infants &&
                    find_warnings.Elderly == model.warnings.Elderly &&
                    find_warnings.Driving == model.warnings.Driving &&
                    find_warnings.Surgery_and_anesthesia == model.warnings.Surgery_and_anesthesia)
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.NothingChange });
                    Session["change_done"] = "true";
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    if (find.name != model.name || find.description != model.description)
                    {
                        var Main_info = db.drug_active_names.Where(filter => filter.id == model.id).First();
                        Main_info.name = model.name;
                        Main_info.description = model.description;
                        db.SaveChanges();
                    }
                    if (find_dosages.how_to_take != model.drug_dosage.how_to_take ||
                        find_dosages.number_of_dosage != model.drug_dosage.number_of_dosage ||
                        find_dosages.dosage != model.drug_dosage.dosage ||
                        find_dosages.beginning_of_effectiveness != model.drug_dosage.beginning_of_effectiveness ||
                        find_dosages.duration_of_effectiveness != model.drug_dosage.duration_of_effectiveness ||
                        find_dosages.feed != model.drug_dosage.feed ||
                        find_dosages.Storage_and_preservation != model.drug_dosage.Storage_and_preservation ||
                        find_dosages.Forget_a_dose != model.drug_dosage.Forget_a_dose ||
                        find_dosages.overdose != model.drug_dosage.overdose)
                    {
                        var drug_dosages_info = db.drug_dosages.Where(filter => filter.drug_active_name.id == model.id).First();
                        drug_dosages_info.how_to_take = model.drug_dosage.how_to_take;
                        drug_dosages_info.number_of_dosage = model.drug_dosage.number_of_dosage;
                        drug_dosages_info.dosage = model.drug_dosage.dosage;
                        drug_dosages_info.beginning_of_effectiveness = model.drug_dosage.beginning_of_effectiveness;
                        drug_dosages_info.duration_of_effectiveness = model.drug_dosage.duration_of_effectiveness;
                        drug_dosages_info.feed = model.drug_dosage.feed;
                        drug_dosages_info.Storage_and_preservation = model.drug_dosage.Storage_and_preservation;
                        drug_dosages_info.Forget_a_dose = model.drug_dosage.Forget_a_dose;
                        drug_dosages_info.overdose = model.drug_dosage.overdose;
                        db.SaveChanges();
                    }
                    if (find_warnings.During_pregnancy != model.warnings.During_pregnancy ||
                            find_warnings.Breast_feeding != model.warnings.Breast_feeding ||
                            find_warnings.Children_and_infants != model.warnings.Children_and_infants ||
                            find_warnings.Elderly != model.warnings.Elderly ||
                            find_warnings.Driving != model.warnings.Driving ||
                            find_warnings.Surgery_and_anesthesia != model.warnings.Surgery_and_anesthesia)
                    {
                        var warnings_info = db.warnings.Where(filter => filter.drug_active_nameId == model.id).First();
                        warnings_info.During_pregnancy = model.warnings.During_pregnancy;
                        warnings_info.Breast_feeding = model.warnings.Breast_feeding;
                        warnings_info.Children_and_infants = model.warnings.Children_and_infants;
                        warnings_info.Elderly = model.warnings.Elderly;
                        warnings_info.Driving = model.warnings.Driving;
                        warnings_info.Surgery_and_anesthesia = model.warnings.Surgery_and_anesthesia;
                        db.SaveChanges();

                    }
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.EditDrugSuccess });
                    Session["change_done"] = "true";
                    return Json(new { Url = redirectUrl });

                }

            }
            catch
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
                Session["change_done"] = "true";
                return Json(new { Url = redirectUrl });
            }

        }




        //save edit or add new Side Effect in Edit Active Drug page***********************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public ActionResult Edit_Add_Side_Effect([Bind(Include = "Add_side_effedcts")] Add_ActiveViewModel model, int drug_id)
        {
            if (model.Add_side_effedcts.id <= 0)
            {
                SqlParameter[] side_effect = new SqlParameter[]{
                        new SqlParameter("@name" , model.Add_side_effedcts.name[0].ToString()),
                        new SqlParameter("@prevalence_effect" , model.Add_side_effedcts.prevalence_effect[0].ToString()),
                        new SqlParameter("@inform_doctor" , model.Add_side_effedcts.inform_doctor[0].ToString()),
                        new SqlParameter("@drug_active_nameId" , drug_id)
                    };
                var insert = db.Database.ExecuteSqlCommand("Set_Effect_For_New @name, @prevalence_effect, @inform_doctor, @drug_active_nameId", side_effect);
                if (insert == -1 || insert == 1)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Edit_Active_drug", new { id = drug_id, Message = ManageMessageId.AddSideEffedcts });
                }
            }
            else
            {
                SqlParameter[] Up_side_effect = new SqlParameter[]{
                        new SqlParameter("@name" , model.Add_side_effedcts.name[0].ToString()),
                        new SqlParameter("@prevalence_effect" , model.Add_side_effedcts.prevalence_effect[0].ToString()),
                        new SqlParameter("@inform_doctor" , model.Add_side_effedcts.inform_doctor[0].ToString()),
                        new SqlParameter("@id" , model.Add_side_effedcts.id.ToString())
                    };
                var insert = db.Database.ExecuteSqlCommand("Update_Side_Effect @name, @prevalence_effect, @inform_doctor, @id", Up_side_effect);
                if (insert == -1 || insert == 1)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Edit_Active_drug", new { id = drug_id, Message = ManageMessageId.EdieSideEffedcts });
                }

            }
            return RedirectToAction("Edit_Active_drug", new { id = drug_id, Message = ManageMessageId.Error });
        }




        //save edit or add new Trade name in Edit Active Drug page**********************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public ActionResult Edit_Add_Trade_Name([Bind(Include = "Add_trade_names")] Add_ActiveViewModel model, int drug_id)
        {
            if (model.Add_trade_names.id <= 0)
            {
                var name = db.drug_active_names.Where(m => m.id == drug_id).FirstOrDefault();
                if (model.Add_trade_names.trade_name[0].ToLower() == name.name.ToLower())
                {
                    model.Add_trade_names.trade_name[0] = model.Add_trade_names.trade_name[0] + "/" + model.Add_trade_names.Manufacturer_company[0];
                }
                SqlParameter[] Trade_Name = new SqlParameter[]{
                    new SqlParameter("@trade_name" , model.Add_trade_names.trade_name[0].ToString()),
                    new SqlParameter("@Manufacturer_Company" , model.Add_trade_names.Manufacturer_company[0].ToString()),
                    new SqlParameter("@drug_active_nameId" , drug_id)
                    };
                var insert = db.Database.ExecuteSqlCommand("Set_Trade_For_New @trade_name , @Manufacturer_company , @drug_active_nameId", Trade_Name);
                if (insert == -1 || insert == 1)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Edit_Active_drug", new { id = drug_id, Message = ManageMessageId.AddTradeName });
                }
            }
            else
            {
                var name = db.drug_active_names.Where(m => m.id == drug_id).FirstOrDefault();
                if (model.Add_trade_names.trade_name[0].ToLower() == name.name.ToLower())
                {
                    model.Add_trade_names.trade_name[0] = model.Add_trade_names.trade_name[0] + "/" + model.Add_trade_names.Manufacturer_company[0];
                }
                SqlParameter[] Up_Trade_Name = new SqlParameter[]{
                        new SqlParameter("@trade_name" , model.Add_trade_names.trade_name[0].ToString()),
                        new SqlParameter("@Manufacturer_Company" , model.Add_trade_names.Manufacturer_company[0].ToString()),
                        new SqlParameter("@id" , model.Add_trade_names.id.ToString())
                    };
                var insert = db.Database.ExecuteSqlCommand("Update_Trade_Name @trade_name , @Manufacturer_company , @id", Up_Trade_Name);
                if (insert == -1 || insert == 1)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Edit_Active_drug", new { id = drug_id, Message = ManageMessageId.EdieTradeName });
                }

            }
            return RedirectToAction("Edit_Active_drug", new { id = drug_id, Message = ManageMessageId.Error });
        }




        //load Side Effect Information for Editing in Edit Active Drug page*************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult GetSideEffect([Bind(Include = "Add_side_effedcts")] Add_ActiveViewModel model, int id)
        {

            SqlParameter[] Effect_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
            var GET_EFFECT = db.Database.SqlQuery<side_effect>("get_side_effect @id", Effect_id).ToList();

            if (GET_EFFECT.Count() > 0)
            {
                return Json(GET_EFFECT[0], JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });
        }






        //load Trade Name Information for Editing in Edit Active Drug page*************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult GetTradeName([Bind(Include = "Add_trade_names")] Add_ActiveViewModel model, int id)
        {
            SqlParameter[] Trade_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
            var GET_Trade = db.Database.SqlQuery<drug_trade_name>("get_Trade_Name @id", Trade_id).ToList();

            if (GET_Trade.Count() > 0)
            {
                return Json(GET_Trade[0], JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });
        }






        //delete Stde Effect from Edit Active Drug page*************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult DeleteSideEffect(int id)
        {
            SqlParameter[] Effect_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
            var delete_effect = db.Database.ExecuteSqlCommand("delete_Side_Efeect @id", Effect_id);

            if (delete_effect == -1)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });
        }




        //delete trade name from Edit Active Drug page*************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult DeleteTradeName(int id)
        {
            SqlParameter[] Trade_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
            var delete_Trade = db.Database.ExecuteSqlCommand("delete_Trade_Name @id", Trade_id);

            if (delete_Trade == -1)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });
        }
        #endregion



        //********************************* More Information active drug *********************************************
        //********************load all data for more information page for active name*********************************
        [HttpGet]
        [Authorize(Roles = "pharmacists , Admins")]
        public ActionResult MoreActiveInfo(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.id = id;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                get_ActiveViewModel model = new get_ActiveViewModel();

                //main
                SqlParameter[] drug_id_main = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var drug_main = db.Database.SqlQuery<Add_ActiveViewModel>("get_active_main @id ", drug_id_main).ToList();

                //effect
                SqlParameter[] drug_id_effect = new SqlParameter[] {
                        new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var drug_effect = db.Database.SqlQuery<side_effedct_get>("get_side_effect_id @id", drug_id_effect).ToList();


                //dosage
                SqlParameter[] drug_id_dosage = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var drug_dosage = db.Database.SqlQuery<drug_dosage_get>("get_active_dosage @id", drug_id_dosage).ToList();


                //warnings
                SqlParameter[] drug_id_warnings = new SqlParameter[]
                        {
                            new SqlParameter("@id" , Convert.ToInt32(id))
                        };
                var drug_warnings = db.Database.SqlQuery<warning_get>("get_active_warnings @id ", drug_id_warnings).ToList();


                //trade
                SqlParameter[] drug_id_trade = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
                var drug_trade = db.Database.SqlQuery<Get_trade_name>("get_trade_name_id @id", drug_id_trade).ToList();

                if (drug_effect.Count > 0)
                {
                    model.Add_side_effedcts = new List<side_effedct_get>();
                    foreach (var effect in drug_effect)
                    {
                        model.Add_side_effedcts.Add(effect);
                    }
                }

                if (drug_trade.Count > 0)
                {
                    model.Add_trade_names = new List<Get_trade_name>();
                    foreach (var trade in drug_trade)
                    {
                        model.Add_trade_names.Add(trade);
                    }
                }


                if (drug_main.Count > 0)
                {
                    model.name = drug_main[0].name;
                    model.description = drug_main[0].description;
                }

                if (drug_warnings.Count > 0)
                {
                    model.warnings = new warning
                    {
                        During_pregnancy = drug_warnings[0].During_pregnancy,
                        Breast_feeding = drug_warnings[0].Breast_feeding,
                        Children_and_infants = drug_warnings[0].Children_and_infants,
                        Elderly = drug_warnings[0].Elderly,
                        Driving = drug_warnings[0].Driving,
                        Surgery_and_anesthesia = drug_warnings[0].Surgery_and_anesthesia
                    };
                }
                if (drug_dosage.Count > 0)
                {
                    model.drug_dosage = new drug_dosage
                    {
                        how_to_take = drug_dosage[0].how_to_take,
                        number_of_dosage = drug_dosage[0].number_of_dosage,
                        feed = drug_dosage[0].feed,
                        dosage = drug_dosage[0].dosage,
                        beginning_of_effectiveness = drug_dosage[0].beginning_of_effectiveness,
                        duration_of_effectiveness = drug_dosage[0].duration_of_effectiveness,
                        Storage_and_preservation = drug_dosage[0].Storage_and_preservation,
                        Forget_a_dose = drug_dosage[0].Forget_a_dose,
                        overdose = drug_dosage[0].overdose,
                    };
                }
                return View(model);
            }
        }




        #endregion










        //************************************* Trade Drug Page *********************************************
        #region Trade Alone
        //load partialview far trade drug page**********************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public PartialViewResult Trade_name_drugs()
        {
            var userId = User.Identity.GetUserId();
            List<SelectListItem> Actives = new List<SelectListItem>();
            List<drug_active_name> names = new List<drug_active_name>();
            if (User.IsInRole("Admins"))
            {
                names = db.drug_active_names.ToList();
            }
            else
            {
                names = db.drug_active_names.Where(M => M.Add_By.Id == userId).ToList();
            }

            foreach (var drig in names)
            {
                Actives.Add(new SelectListItem() { Text = drig.name, Value = drig.id.ToString() });
            }

            ViewBag.names = Actives;
            return PartialView("_Trade_name_drugs");
        }



        //load datatable for all trade names in database***********************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult LoadTradeNameAll()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;



                List<Get_trade_name> Name = new List<Get_trade_name>();
                var userId = User.Identity.GetUserId();
                if (User.IsInRole("Admins"))
                {
                    SqlParameter[] par_health = new SqlParameter[] {
                    new SqlParameter("@user_id" , "0")
                    };
                    Name = db.Database.SqlQuery<Get_trade_name>("get_Trade_Names @user_id", par_health).ToList();
                }
                else
                {
                    SqlParameter[] par_health = new SqlParameter[] {
                    new SqlParameter("@user_id" , userId)
                    };
                    Name = db.Database.SqlQuery<Get_trade_name>("get_Trade_Names @user_id", par_health).ToList();

                }


                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Name = Name.Where(m => (m.trade_name.ToLower().Contains(searchValue)) || (m.trade_name.Contains(searchValue))
                    || (m.active_name.ToLower().Contains(searchValue)) || (m.active_name.Contains(searchValue))
                    || (m.Manufacturer_company.ToLower().Contains(searchValue)) || (m.Manufacturer_company.Contains(searchValue))
                    || (m.added_by.ToLower().Contains(searchValue)) || (m.added_by.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Name.Count();
                //Paging     
                var data = Name.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }




        //check validation for add/edit trade name alone*******************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult ValidTradeNew([Bind(Include = "Add_trade_names")] Add_ActiveViewModel model)
        {
            if (model.Add_trade_names.Manufacturer_company[0] == "" || model.Add_trade_names.Manufacturer_company[0] == null)
            {
                ViewBag.Manufacturer_company = "You Have To Add Manufacturer company";
            }

            if (model.Add_trade_names.trade_name[0] == "" || model.Add_trade_names.trade_name[0] == null)
            {
                ViewBag.trade_name = "You Have To Add The Trade Name";
            }

            if (model.Add_trade_names.active_name == "--Select--" || model.Add_trade_names.active_name == null)
            {
                ViewBag.active_name = "You Have To Select Active Name";
            }
            if (ViewBag.Manufacturer_company == null && ViewBag.trade_name == null && ViewBag.active_name == null)
            {
                if (model.Add_trade_names.id <= 0)
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("AddTradeNew", "Manage");
                    TempData["Trade"] = model;
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("EditTradeNew", "Manage");
                    TempData["Trade"] = model;
                    return Json(new { Url = redirectUrl });
                }

            }

            List<SelectListItem> json = new List<SelectListItem> {
                        new SelectListItem{ Value = ViewBag.Manufacturer_company },
                        new SelectListItem{ Value = ViewBag.trade_name },
                        new SelectListItem{ Value = ViewBag.active_name },
                        };
            return Json(json, JsonRequestBehavior.AllowGet);
        }



        //add new trade name Alone********************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public ActionResult AddTradeNew([Bind(Include = "Add_trade_names")] Add_ActiveViewModel model)
        {
            model = (Add_ActiveViewModel)TempData["Trade"];
            if (model.Add_trade_names.active_name.ToLower() == model.Add_trade_names.trade_name[0].ToLower())
            {
                model.Add_trade_names.trade_name[0] = model.Add_trade_names.trade_name[0] + "/" + model.Add_trade_names.Manufacturer_company[0];
            }
            SqlParameter[] Trade_Name = new SqlParameter[]{
                    new SqlParameter("@trade_name" , model.Add_trade_names.trade_name[0].ToString()),
                    new SqlParameter("@Manufacturer_Company" , model.Add_trade_names.Manufacturer_company[0].ToString()),
                    new SqlParameter("@drug_active_nameId" , model.Add_trade_names.active_id)
                    };
            var insert = db.Database.ExecuteSqlCommand("Set_Trade_For_New @trade_name , @Manufacturer_company , @drug_active_nameId", Trade_Name);
            if (insert == -1)
            {
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.AddTradeNameNew });
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }





        //Load Trade Name withe his Active name For Editing*************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult GetTradeNameWithActive(int id)
        {
            SqlParameter[] Trade_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
            var GET_Trade = db.Database.SqlQuery<Get_trade_name>("get_trade_active @id", Trade_id).ToList();

            if (GET_Trade.Count() > 0)
            {
                return Json(GET_Trade[0], JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });
        }






        //edit trade in his page (Edit Alone)*****************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public ActionResult EditTradeNew([Bind(Include = "Add_trade_names")] Add_ActiveViewModel model)
        {
            model = (Add_ActiveViewModel)TempData["Trade"];
            if (model.Add_trade_names.active_name.ToLower() == model.Add_trade_names.trade_name[0].ToLower())
            {
                model.Add_trade_names.trade_name[0] = model.Add_trade_names.trade_name[0] + "/" + model.Add_trade_names.Manufacturer_company[0];
            }
            SqlParameter[] Up_Trade_Name = new SqlParameter[]{
                        new SqlParameter("@trade_name" , model.Add_trade_names.trade_name[0].ToString()),
                        new SqlParameter("@Manufacturer_Company" , model.Add_trade_names.Manufacturer_company[0].ToString()),
                        new SqlParameter("@id" , model.Add_trade_names.id.ToString()),
                        new SqlParameter("@active_id" , model.Add_trade_names.active_id.ToString())
                    };
            var insert = db.Database.ExecuteSqlCommand("Update_Trade_active @trade_name , @Manufacturer_company , @id , @active_id", Up_Trade_Name);
            if (insert == -1)
            {
                Session["change_done"] = "true";
                return RedirectToAction("Index", new { Message = ManageMessageId.EdieTradeName });
            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }
        #endregion










        //************************************* Drug Iteraction *********************************************
        #region Drug Iteraction
        //load partialview far trade drug page**********************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public PartialViewResult Drug_Interactions()
        {
            List<SelectListItem> Actives = new List<SelectListItem>();
            var names = db.drug_active_names.ToList();
            foreach (var drig in names)
            {
                Actives.Add(new SelectListItem() { Text = drig.name, Value = drig.id.ToString() });
            }

            ViewBag.names = Actives;
            return PartialView("_drug_interaction");
        }



        //load datatable for all trade names in database***********************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult LoadInteractionAll()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;




                List<Get_Drug_Interaction> Name = new List<Get_Drug_Interaction>();
                var userId = User.Identity.GetUserId();
                if (User.IsInRole("Admins"))
                {
                    SqlParameter[] par_Interaction = new SqlParameter[] {
                    new SqlParameter("@Add_by" , "0")
                    };
                    Name = db.Database.SqlQuery<Get_Drug_Interaction>("get_All_InterAction @Add_by", par_Interaction).ToList();
                }
                else
                {
                    SqlParameter[] par_Interaction = new SqlParameter[] {
                    new SqlParameter("@Add_by" , userId)
                    };
                    Name = db.Database.SqlQuery<Get_Drug_Interaction>("get_All_InterAction @Add_by", par_Interaction).ToList();
                }


                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Name = Name.Where(m => (m.with_drug.ToLower().Contains(searchValue)) || (m.with_drug.Contains(searchValue))
                    || (m.Main_drug.ToLower().Contains(searchValue)) || (m.Main_drug.Contains(searchValue))
                    || (m.Degree.ToLower().Contains(searchValue)) || (m.Degree.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Name.Count();
                //Paging     
                var data = Name.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }






        //check validation for add/edit drug interaction******************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult ValidInteractionNew([Bind(Include = "Add_interaction")] Add_ActiveViewModel model)
        {
            if (model.Add_interaction.drug_name == "--Select--" || model.Add_interaction.drug_name == null || model.Add_interaction.drug_id == 0)
            {
                ViewBag.drug_name = "You Have To Select The Main Drug ";
            }

            if (model.Add_interaction.with_name == "--Select--" || model.Add_interaction.with_name == null || model.Add_interaction.with_id == 0)
            {
                ViewBag.with_name = "You Have To Select A With Drug ";
            }

            if (model.Add_interaction.drug_name != "--Select--" && model.Add_interaction.with_name != "--Select--")
            {
                if (model.Add_interaction.drug_name == model.Add_interaction.with_name)
                {
                    ViewBag.with_name = "You can't interact the drug with it self";
                }
            }
            if (model.Add_interaction.Degree == null)
            {
                ViewBag.Degree = "You Have To Select The Degree Of The Interaction";
            }

            if (ViewBag.drug_name == null && ViewBag.with_name == null && ViewBag.Degree == null)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("AddEditInteraction", "Manage");
                TempData["Interaction"] = model;
                return Json(new { Url = redirectUrl });
            }

            List<SelectListItem> json = new List<SelectListItem> {
                        new SelectListItem{ Value = ViewBag.drug_name},
                        new SelectListItem{ Value = ViewBag.with_name},
                        new SelectListItem{ Value = ViewBag.Degree },
                        };
            return Json(json, JsonRequestBehavior.AllowGet);
        }






        //save add edit drug interaction *******************************************************************
        public ActionResult AddEditInteraction([Bind(Include = "Add_interaction")] Add_ActiveViewModel model)
        {
            model = (Add_ActiveViewModel)TempData["Interaction"];
            string Degree = "";
            if (model.Add_interaction.Degree == "1")
            {
                Degree = "major";
            }
            else if (model.Add_interaction.Degree == "2")
            {
                Degree = "moderate";
            }
            else if (model.Add_interaction.Degree == "3")
            {
                Degree = "minor";
            }
            if (model.Add_interaction.id <= 0)
            {
                var userId = User.Identity.GetUserId();
                SqlParameter[] par_Interaction = new SqlParameter[] {
                    new SqlParameter("@Degree" , Degree),
                    new SqlParameter("@add_by" , userId),
                    new SqlParameter("@drug_id" , model.Add_interaction.drug_id),
                    new SqlParameter("@with_id" , model.Add_interaction.with_id),
                    };
                var Insert = db.Database.ExecuteSqlCommand("set_interaction_drug @Degree , @add_by , @drug_id , @with_id", par_Interaction);
                if (Insert == -1)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Index", new { Message = ManageMessageId.AddInterActionNew });
                }
            }
            else
            {
                SqlParameter[] par_Interaction_ipdate = new SqlParameter[] {
                    new SqlParameter("@Degree" , Degree),
                    new SqlParameter("@drug_id" , model.Add_interaction.drug_id),
                    new SqlParameter("@with_id" , model.Add_interaction.with_id),
                    new SqlParameter("@id" , model.Add_interaction.id),
                    };
                var update = db.Database.ExecuteSqlCommand("update_interaction_drug @Degree , @drug_id , @with_id , @id", par_Interaction_ipdate);
                if (update == -1)
                {
                    Session["change_done"] = "true";
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditInterActionNew });
                }

            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });


        }





        //Load data for drug interaction edit *******************************************************************
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult GetInteraction(string id)
        {
            SqlParameter[] par_Interaction = new SqlParameter[] {
                    new SqlParameter("@id" , id)
                    };
            var interaction = db.Database.SqlQuery<Get_Drug_Interaction>("get_InterAction @id", par_Interaction).ToList();
            return Json(interaction[0], JsonRequestBehavior.AllowGet);
        }






        //delete trade name from Edit Active Drug page*************************************************
        [HttpPost]
        [Authorize(Roles = "pharmacists , Admins")]
        public JsonResult DeleteInteraction(int id)
        {
            SqlParameter[] Interaction_id = new SqlParameter[] {
                        new SqlParameter("@id" , id)
                        };
            var delete_Interaction = db.Database.ExecuteSqlCommand("delete_Interaction @id", Interaction_id);

            if (delete_Interaction == -1)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });
        }
        #endregion











        //******************************************** Users *********************************************
        #region Users
        //load partialview far trade drug page**********************************************************
        [Authorize(Roles = "Admins")]
        public PartialViewResult users()
        {
            return PartialView("_users_admin_index");
        }





        //load datatable for all trade names in database***********************************************
        [Authorize(Roles = "Admins")]
        public JsonResult LoadAllUsersRoles()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;



                var Users = db.Database.SqlQuery<GetUserWithRoelViewModel>("get_all_user_role").ToList();
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Users = Users.Where(m => (m.name.ToLower().Contains(searchValue)) || (m.name.Contains(searchValue))
                    || (m.UserName.ToLower().Contains(searchValue)) || (m.UserName.Contains(searchValue))
                    || (m.Email.ToLower().Contains(searchValue)) || (m.Email.Contains(searchValue))
                    || (m.role.ToLower().Contains(searchValue)) || (m.role.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Users.Count();
                //Paging     
                var data = Users.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }





        //check for all validation for user information****************************************************
        [Authorize(Roles = "Admins")]
        public JsonResult ValidationUser([Bind(Include = "addnewUser")] ManageAccountViewModel model)
        {
            bool flag = false;
            //name validation***********************************************
            if (model.addnewUser.Name == "" || model.addnewUser.Name == null)
            {
                ViewBag.name = "Please Enter The Name";
                flag = true;
            }

            //user name validation--------------------------------------------------------------
            //for add new user-------------------------------------------------------------------
            if (model.addnewUser.id == null)
            {
                if (model.addnewUser.Username == "" || model.addnewUser.Username == null)
                {
                    ViewBag.Username = "Please Enter The Username";
                    flag = true;
                }
                else if (!(Regex.IsMatch(model.addnewUser.Username, @"^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
                {
                    ViewBag.Username = "Special character should not be entered";
                    flag = true;
                }
                else
                {
                    var Username = db.Users.Where(x => x.UserName == model.addnewUser.Username).FirstOrDefault();
                    if (Username != null)
                    {
                        ViewBag.Username = model.addnewUser.Username + " Is Used Before";
                        flag = true;
                    }
                }
            }
            //for edit user-------------------------------------------------------------------
            else
            {
                if (model.addnewUser.Username == "" || model.addnewUser.Username == null)
                {
                    ViewBag.Username = "Please Enter The Username";
                    flag = true;
                }
                else if (!(Regex.IsMatch(model.addnewUser.Username, @"^[a-zA-Z0-9]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
                {
                    ViewBag.Username = "Special character should not be entered";
                    flag = true;
                }
                else
                {
                    var Username = db.Users.Where(x => (x.Id != model.addnewUser.id) && (x.UserName == model.addnewUser.Username)).FirstOrDefault();
                    if (Username != null)
                    {
                        ViewBag.Username = model.addnewUser.Username + " Is Used Before";
                        flag = true;
                    }
                }
            }

            //email validation-------------------------------------------------------------------
            //for add new user-------------------------------------------------------------------
            if (model.addnewUser.id == null)
            {
                if (model.addnewUser.Email == "" || model.addnewUser.Email == null)
                {
                    ViewBag.Email = "Please Enter The Email.";
                    flag = true;
                }
                else if (!(Regex.IsMatch(model.addnewUser.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
                {
                    ViewBag.Email = model.addnewUser.Email + " is Invalid Email Address.";
                    flag = true;
                }
                else
                {
                    var email = db.Users.Where(x => x.Email == model.addnewUser.Email).FirstOrDefault();
                    if (email != null)
                    {
                        ViewBag.Email = model.addnewUser.Email + " Is Used Before.";
                        flag = true;
                    }
                }
            }
            //for edit user-------------------------------------------------------------------
            else
            {
                if (model.addnewUser.Email == "" || model.addnewUser.Email == null)
                {
                    ViewBag.Email = "Please Enter The Email.";
                    flag = true;
                }
                else if (!(Regex.IsMatch(model.addnewUser.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))))
                {
                    ViewBag.Email = model.addnewUser.Email + " is Invalid Email Address.";
                    flag = true;
                }
                else
                {
                    var email = db.Users.Where(x => (x.Id != model.addnewUser.id) && (x.Email == model.addnewUser.Email)).FirstOrDefault();
                    if (email != null)
                    {
                        ViewBag.Email = model.addnewUser.Email + " Is Used Before.";
                        flag = true;
                    }
                }
            }

            //Password validation-------------------------------------------------------------------
            //for add new user-------------------------------------------------------------------
            if (model.addnewUser.id == null)
            {
                if (model.addnewUser.Password == "" || model.addnewUser.Password == null)
                {
                    ViewBag.Password = "The Password field is required.";
                    flag = true;
                }
                else if (!(model.addnewUser.Password.Any(char.IsUpper)))
                {
                    ViewBag.Password = "Passwords must have at least one uppercase ('A'-'Z').";
                    flag = true;
                }
                else if (model.addnewUser.Password.ToArray().Count() < 6)
                {
                    ViewBag.Password = "The Password must be at least 6 characters long.";
                    flag = true;
                }
            }
            //gender validation-------------------------------------------------------------------
            //for add and edit user-------------------------------------------------------------------
            if (model.addnewUser.gender == "--select--" || model.addnewUser.gender == null)
            {
                ViewBag.gender = "Please Select The Gender.";
                flag = true;
            }




            //Age validation-------------------------------------------------------------------
            //for add and edit user---------------------------------------------------------------
            if (model.addnewUser.Age == 0)
            {
                ViewBag.Age = "Please Enter Your Age.";
                flag = true;
            }
            else if (model.addnewUser.Age == -1239857)
            {
                ViewBag.Age = "Age must be a Number.";
                flag = true;
            }
            else if (model.addnewUser.Age > 60 || model.addnewUser.Age < 15)
            {
                ViewBag.Age = "Age must be Between 15 and 60.";
                flag = true;
            }
            




            //Roles validation-------------------------------------------------------------------
            //for add and edit user---------------------------------------------------------------
            if (model.addnewUser.Roles == "--select--" || model.addnewUser.Roles == null)
            {
                ViewBag.Roles = "Please Select The Role.";
                flag = true;
            }



            if (flag == false)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("AddEditUserByAdmin", "Manage");
                TempData["User"] = model;
                return Json(new { Url = redirectUrl });
            }

            List<SelectListItem> json = new List<SelectListItem> {
                        new SelectListItem{ Value = ViewBag.name },
                        new SelectListItem{ Value = ViewBag.Username },
                        new SelectListItem{ Value = ViewBag.Email },
                        new SelectListItem{ Value = ViewBag.Password },
                        new SelectListItem{ Value = ViewBag.gender },
                        new SelectListItem{ Value = ViewBag.Age },
                        new SelectListItem{ Value = ViewBag.Roles },
                        };
            return Json(json, JsonRequestBehavior.AllowGet);
        }






        //save edit and add user information*********************************************************************
        [Authorize(Roles = "Admins")]
        public async Task<ActionResult> AddEditUserByAdmin([Bind(Include = "addnewUser")] ManageAccountViewModel model)
        {
            model = (ManageAccountViewModel)TempData["User"];
            //Add USER******************************************************************************
            if (model.addnewUser.id == null)
            {
                var user = new ApplicationUser
                {
                    UserName = model.addnewUser.Username,
                    Email = model.addnewUser.Email,
                    age = model.addnewUser.Age,
                    Gender = model.addnewUser.gender,
                    Name = model.addnewUser.Name,
                };
                var result = await UserManager.CreateAsync(user, model.addnewUser.Password);
                if (result.Succeeded)
                {
                    if (model.addnewUser.Roles == "pharmacists")
                    {
                        UserManager.SetLockoutEnabled(user.Id, true);
                        await UserManager.AddToRoleAsync(user.Id, model.addnewUser.Roles);
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.AddNewpharmacists });
                    }
                    else
                    {
                        UserManager.SetLockoutEnabled(user.Id, false);
                        await UserManager.AddToRoleAsync(user.Id, model.addnewUser.Roles);
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.AddNewUser });
                    }
                   

                }
            }
            //EDIT USER******************************************************************************
            else
            {
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var userId = model.addnewUser.id;
                var user = db.Users.Where(x => x.Id == userId).AsQueryable().FirstOrDefault();

                var Role = db.Roles.ToList();
                var mainRoleId = "";
                var mainRoleIdName = "";
                foreach (var userRole in user.Roles)
                {
                    mainRoleId = userRole.RoleId;
                }
                foreach (var role in Role)
                {
                    if (role.Id == mainRoleId)
                    {
                        mainRoleIdName = role.Name;
                    }
                }
                if (mainRoleIdName != model.addnewUser.Roles)
                {
                    if (model.addnewUser.Roles == "pharmacists")
                    {
                        var pharmacists = db.pharmastics.Where(x => x.User.Id == model.addnewUser.id).FirstOrDefault();
                        if (!(pharmacists != null))
                        {
                            Session["change_done"] = "true";
                            return RedirectToAction("Index", new { Message = ManageMessageId.AddCertificationError});
                        }
                    }
                    UserManager.RemoveFromRole(user.Id, mainRoleIdName);
                    UserManager.AddToRole(user.Id, model.addnewUser.Roles);
                    
                }
                if (user.Name == model.addnewUser.Name && user.Email == model.addnewUser.Email && user.UserName == model.addnewUser.Username && user.Gender == model.addnewUser.gender && user.age == model.addnewUser.Age)
                {
                    if (mainRoleIdName != model.addnewUser.Roles)
                    {
                        UserManager.RemoveFromRole(user.Id, mainRoleIdName);
                        UserManager.AddToRole(user.Id, model.addnewUser.Roles);
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.EditUserRoleSuccess });
                    }
                    else
                    {
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.NothingChange });
                    }
                    
                }
                user.Name = model.addnewUser.Name;
                user.Email = model.addnewUser.Email;
                user.UserName = model.addnewUser.Username;
                user.Gender = model.addnewUser.gender;
                user.age = model.addnewUser.Age;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (mainRoleIdName != model.addnewUser.Roles)
                    {
                        UserManager.RemoveFromRole(user.Id, mainRoleIdName);
                        UserManager.AddToRole(user.Id, model.addnewUser.Roles);
                    }
                    db.SaveChanges();
                    Session["change_done"] = "true";
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditUserInformationSuccess });
                }
            }
            return RedirectToAction("Index", "Home");
        }







        //Load user information for edit*********************************************************************
        [Authorize(Roles = "Admins")]
        public JsonResult LoadUserForEdit(string id)
        {
            var user = db.Users.Where(x => x.Id == id).First();
            return Json(user, JsonRequestBehavior.AllowGet);
        }







        //Delete user if normal & user and certification if pharmacists*********************************************
        [Authorize(Roles = "Admins")]
        public JsonResult DeleteUser(string id, string RoleName)
        {
            var Ispharmacists = "True";
            if (RoleName == "pharmacists")
            {
                SqlParameter[] User_info = new SqlParameter[] {
                    new SqlParameter("@user_id" , id),
                    new SqlParameter("@Ispharmacists" , Ispharmacists),
                    };
                var delete_user = db.Database.ExecuteSqlCommand("delete_user @user_id , @Ispharmacists", User_info);
                if (delete_user == -1)
                {
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Ispharmacists = "False";
                SqlParameter[] User_info = new SqlParameter[] {
                    new SqlParameter("@user_id" , id),
                    new SqlParameter("@Ispharmacists" , Ispharmacists),
                    };
                var delete_user = db.Database.ExecuteSqlCommand("delete_user @user_id , @Ispharmacists", User_info);
                if (delete_user == -1)
                {
                    return Json(-2, JsonRequestBehavior.AllowGet);
                }
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });


        }





        //Load user information for edit*********************************************************************
        [Authorize(Roles = "Admins")]
        public JsonResult LoadUserForModreInfo(string id)
        {
            SqlParameter[] user_id = new SqlParameter[] {
                new SqlParameter("@id" , id)
           };

            var user = db.Database.SqlQuery<GetUserViewModel>("get_user_with_role @id", user_id).ToList();
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        #endregion










        //******************************************** pharmacist *********************************************
        #region pharmacist
        //load partialview far pharmacist**********************************************************
        [Authorize(Roles = "Admins")]
        public PartialViewResult pharmacist()
        {
            
            return PartialView("_pharmacist_admin_index");
        }





        //load datatable for all pharmacist certification***********************************************
        [Authorize(Roles = "Admins")]
        public JsonResult LoadAllpharmacist()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;



                var Users = db.Database.SqlQuery<UserPharmacistViewModel>("get_user__pharmacist").ToList();
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Users = Users.Where(m => (m.Name.ToLower().Contains(searchValue)) || (m.Name.Contains(searchValue))
                    || (m.UserName.ToLower().Contains(searchValue)) || (m.UserName.Contains(searchValue))
                    || (m.University_Name.ToLower().Contains(searchValue)) || (m.University_Name.Contains(searchValue))
                    || (m.Degree.ToLower().Contains(searchValue)) || (m.Degree.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Users.Count();
                //Paging     
                var data = Users.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }






        //check validation for add certification information by admin***********************************************
        [Authorize(Roles = "Admins")]
        public JsonResult AddCertificationValudationAdmin([Bind(Include = "Addcertification")] ManageAccountViewModel model)
        {
            if (model.Addcertification.University_Name == null || model.Addcertification.University_Name == "")
            {
                ViewBag.University_Name = "Please Enter The University Name";
            }
            if (model.Addcertification.Degree == null || model.Addcertification.Degree == "")
            {
                ViewBag.Degrees = "Please Select The Degree";
            }
            if (model.Addcertification.certification_img == null || model.Addcertification.certification_img == "")
            {
                ViewBag.certification_img = "Please Upload A Copy Of The Certification";
            }
            if (model.Addcertification.UserId == null || model.Addcertification.UserId == "")
            {
                ViewBag.UserId = "Please Select The User For Certification";
            }

            if (ViewBag.University_Name == null && ViewBag.Degrees == null && ViewBag.certification_img == null)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("SaveAddEditCertificationAdmin");
                TempData["NewCertification"] = model;
                return Json(new { Url = redirectUrl });
            }
            List<SelectListItem> json = new List<SelectListItem> {
                    new SelectListItem{ Value = ViewBag.University_Name },
                    new SelectListItem{ Value = ViewBag.Degrees },
                    new SelectListItem{ Value = ViewBag.certification_img },
                    new SelectListItem{ Value = ViewBag.UserId },
                };
            return Json(json, JsonRequestBehavior.AllowGet);
        }






        //save Add new or edit certification*********************************************************************** 
        public ActionResult SaveAddEditCertificationAdmin([Bind(Include = "Addcertification")] ManageAccountViewModel model , HttpPostedFileBase file)
        {
            model = (ManageAccountViewModel)TempData["NewCertification"];


            if (model.Addcertification != null)
            {
                if (model.Addcertification.NewUserId == null || model.Addcertification.NewUserId == "")
                {
                    file = model.Addcertification.Certification_url;
                    model.Addcertification.certification_img = file.ToString(); //getting complete url  
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg) 
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)
                    string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension
                    string myfile = name + "_" + model.Addcertification.UserId + ext; //appending the name with id  
                                                                                      // store the file inside ~/project folder(CERTIFICATION_IMG)  
                    var path = Path.Combine(Server.MapPath("~/CERTIFICATION_IMG"), myfile);
                    file.SaveAs(path);
                    path = "/CERTIFICATION_IMG/" + myfile;
                    model.Addcertification.certification_img = path;
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        SqlParameter[] Register_Pharmacist = new SqlParameter[]
                        {
                    new SqlParameter("@UnivarstyName" , model.Addcertification.University_Name.ToString()),
                    new SqlParameter("@Degree" , model.Addcertification.Degree.ToString()),
                    new SqlParameter("@UserId" ,model.Addcertification.UserId.ToString()),
                    new SqlParameter("@certificationImg" , model.Addcertification.certification_img.ToString())
                        };
                        var add_Pharmacist = db.Database.ExecuteSqlCommand("Add_Pharmacist @UnivarstyName , @Degree , @certificationImg , @UserId", Register_Pharmacist).ToString();
                        if (add_Pharmacist == "1" || add_Pharmacist == "-1")
                        {
                            var IfRole = UserManager.GetRoles(model.Addcertification.UserId);
                            if (IfRole[0] == "Normal_User")
                            {
                                UserManager.RemoveFromRole(model.Addcertification.UserId, "Normal_User");
                                UserManager.AddToRole(model.Addcertification.UserId, "pharmacists");
                            }
                            UserManager.SetLockoutEnabled(model.Addcertification.UserId, false);
                            Session["change_done"] = "true";
                            return RedirectToAction("Index", new { Message = ManageMessageId.AddCertificationSuccess });
                        }
                    }

                }
                else
                {
                    var userId = model.Addcertification.NewUserId;
                    SqlParameter[] par = new SqlParameter[] {
                        new SqlParameter( "@id" , userId ),
                    };
                    var certification = db.Database.SqlQuery<pharmacist_info>("get_pharmastics @id", par).ToList();
                    if (certification[0].University_Name == model.Addcertification.University_Name &&
                        certification[0].Degree == model.Addcertification.Degree &&
                        certification[0].certification_img == model.Addcertification.certification_img)
                    {
                        Session["change_done"] = "true";
                        return RedirectToAction("Index", new { Message = ManageMessageId.NothingChange });
                    }
                    else
                    {

                        if (model.Addcertification.Certification_url != null)
                        {
                            string realPath = certification[0].certification_img.Split('/')[2];
                            string fullPath = Request.MapPath("~/CERTIFICATION_IMG/" + realPath);
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            file = model.Addcertification.Certification_url;
                            model.Addcertification.certification_img = file.ToString(); //getting complete url  
                            var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg) 
                            var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)
                            string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension
                            string myfile = name + "_" + userId + ext; //appending the name with id  
                            var path = Path.Combine(Server.MapPath("~/CERTIFICATION_IMG"), myfile); // store the file inside ~/CERTIFICATION_IMG folder(CERTIFICATION_IMG)  
                            file.SaveAs(path);
                            path = "/CERTIFICATION_IMG/" + myfile;
                            model.Addcertification.certification_img = path;
                        }
                        SqlParameter[] Update = new SqlParameter[] {
                            new SqlParameter( "@id" , userId ),
                            new SqlParameter( "@Univarsty_Name" , model.Addcertification.University_Name ),
                            new SqlParameter( "@Degree" , model.Addcertification.Degree ),
                            new SqlParameter( "@certification_img" , model.Addcertification.certification_img )
                        };
                        var update = db.Database.ExecuteSqlCommand("Update_certification @id , @Univarsty_Name , @Degree , @certification_img", Update);
                        if (update == -1)
                        {
                            UserManager.SetLockoutEnabled(userId, false);
                            Session["change_done"] = "true";
                            return RedirectToAction("Index", new { Message = ManageMessageId.EditCertificationSuccess });
                        }
                    }


                }


            }
            Session["change_done"] = "true";
            return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }







        //Load Modal for Edit The certification By Admin*********************************************************
        [Authorize(Roles = "Admins")]
        public JsonResult LoadCertification(string id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SqlParameter[] par = new SqlParameter[] {
                        new SqlParameter( "@id" , id )
                    };
                ManageAccountViewModel model = new ManageAccountViewModel();
                var certification = db.Database.SqlQuery<pharmacist_info>("get_pharmastics @id", par);
                foreach (var certificat in certification)
                {
                    model.pharmacist_info = new pharmacist_info
                    {
                        University_Name = certificat.University_Name,
                        Degree = certificat.Degree,
                        certification_img = certificat.certification_img,
                    };
                    TempData["IMG"] = certificat.certification_img;
                }

                List<SelectListItem> degree = new List<SelectListItem>()
                {
                        new SelectListItem {  Text = "Associate degree", Value = "Associate degree"},
                        new SelectListItem { Text = "Bachelor's degree", Value = "Bachelor's degree"},
                        new SelectListItem { Text = "Master's degree", Value = "Master's degree"},
                        new SelectListItem { Text = "Doctoral degree", Value = "Doctoral degree"},
                        new SelectListItem { Text = "Professional degree", Value = "Professional degree"},
                };
                ViewBag.degree = degree.ToList();

                var users = db.Users.ToList();
                List<SelectListItem> UserName = new List<SelectListItem>();
                foreach (var name in users)
                {
                    var IfRole = UserManager.GetRoles(name.Id);
                    if (IfRole[0] == "pharmacists")
                    {
                        var IfCert = db.pharmastics.Where(x => x.User.Id == name.Id).FirstOrDefault();
                        if (IfCert == null)
                        {
                            UserName.Add(new SelectListItem() { Text = name.UserName.ToString(), Value = name.Id.ToString() });
                        }

                    }
                    else if (IfRole[0] == "Normal_User")
                    {
                        UserName.Add(new SelectListItem() { Text = name.UserName.ToString(), Value = name.Id.ToString() });
                    }

                }
                ViewBag.UserName = new SelectList(UserName, "Value", "Text");

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }







        //Load new more InFormation page for Certification by Admin*********************************************************
        [Authorize(Roles = "Admins")]
        public ActionResult LoadMoreInformationCertification(string id , string UserName)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                SqlParameter[] par = new SqlParameter[] {
                        new SqlParameter( "@id" , id )
                    };
                ManageAccountViewModel model = new ManageAccountViewModel();
                var certification = db.Database.SqlQuery<pharmacist_info>("get_pharmastics @id", par).ToList();
                foreach (var certificat in certification)
                {
                    model.pharmacist_info = new pharmacist_info
                    {
                        University_Name = certificat.University_Name,
                        Degree = certificat.Degree,
                        certification_img = certificat.certification_img,
                    };
                }

                
                ViewBag.UserName = UserName;

                return View(model);
            }
        }






        //Delete pharmacists Certification and Change Role To Normal_User*********************************************
        [Authorize(Roles = "Admins")]
        public JsonResult DeleteCertification(string Certificationid , string userId)
        {
            if (Certificationid != null && userId != null)
            {
                SqlParameter[] Certification_info = new SqlParameter[] {
                    new SqlParameter("@id" , Certificationid),
                    };
                var delete_user = db.Database.ExecuteSqlCommand("delete_Certification @id", Certification_info);
                if (delete_user == -1)
                {
                    UserManager.RemoveFromRole(userId, "pharmacists");
                    UserManager.AddToRole(userId, "Normal_User");
                    return Json(-1, JsonRequestBehavior.AllowGet);
                }
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });

        }
        #endregion











        //******************************************** pharmacist_request *********************************************
        #region pharmacist_request
        //load partialview pharmacist_request**********************************************************
        [Authorize(Roles = "Admins")]
        public PartialViewResult pharmacist_request()
        {

            return PartialView("_pharmacist_request");
        }






        //load datatable for all pharmacist certification***********************************************
        [Authorize(Roles = "Admins")]
        public JsonResult LoadAllRequests()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;



                var Users = db.Database.SqlQuery<UserPharmacistViewModel>("get_all_Requests").ToList();
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Users = Users.Where(m => (m.Name.ToLower().Contains(searchValue)) || (m.Name.Contains(searchValue))
                    || (m.UserName.ToLower().Contains(searchValue)) || (m.UserName.Contains(searchValue))
                    || (m.University_Name.ToLower().Contains(searchValue)) || (m.University_Name.Contains(searchValue))
                    || (m.Degree.ToLower().Contains(searchValue)) || (m.Degree.Contains(searchValue))).ToList();
                }

                //total number of rows count     
                recordsTotal = Users.Count();
                //Paging     
                var data = Users.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }






        //Accept THE pharmacist Certification ************************************************************************************
        public JsonResult AcceptCertification(string userId)
        {
            var result = UserManager.SetLockoutEnabled(userId, false);
            if (result.Succeeded)
            {
                return Json(-1 , JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });

        }





        //Reject THE pharmacist Certification ************************************************************************************
        public JsonResult RejectCertification(string userId)
        {
            SqlParameter[] Reject_Pharmacist = new SqlParameter[]
                        {
                    new SqlParameter("@id" , userId)
                        };
            var RejectCertification = db.Database.ExecuteSqlCommand("Reject_Certification @id", Reject_Pharmacist);
            if (RejectCertification == -1)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { Message = ManageMessageId.Error });
            Session["change_done"] = "true";
            return Json(new { Url = redirectUrl });

        }
        #endregion










        //************************************* Help classes *********************************************
        #region Helpers
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }


        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        //private bool HasPhoneNumber()
        //{
        //    var user = UserManager.FindById(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        return user.PhoneNumber != null;
        //    }
        //    return false;
        //}


        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            ChangePasswordError,
            ChangePasswordErrorUpperCase,
            ChangePasswordErrorSpecial,
            OldNewError,

            SetPasswordSuccess,
            SetPasswordError,
            SetPasswordErrorSpecial,

            EditInformationSuccess,
            EditInformationError,
            EmailExist,
            UserNameExist,
            NothingChange,

            AddHealthCase,
            EditHealthCase,
            errorHealthCase,

            AddNewDrug,
            EditDrugSuccess,
            AddSideEffedcts,
            AddTradeName,
            EdieSideEffedcts,
            EdieTradeName,
            AddTradeNameNew,


            AddInterActionNew,
            EditInterActionNew,


            AddNewUser,
            AddNewpharmacists,
            EditUserInformationSuccess,
            EditUserRoleSuccess,
            SetPasswordUserSuccess,
            AddCertificationError,
            SetPasswordErrorUpperCase,
            AddCertificationSuccess,
            EditCertificationSuccess,

            SetTwoFactorSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            AddPhoneSuccess,
            Error
        }
        #endregion










        //// POST: /Manage/RemoveLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message;
        //    var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("ManageLogins", new { Message = message });
        //}
        // GET: /Manage/AddPhoneNumber
        //public ActionResult AddPhoneNumber()
        //{
        //    return View();
        //}
        // POST: /Manage/AddPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Generate the token and send it
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
        //    if (UserManager.SmsService != null)
        //    {
        //        var message = new IdentityMessage
        //        {
        //            Destination = model.Number,
        //            Body = "Your security code is: " + code
        //        };
        //        await UserManager.SmsService.SendAsync(message);
        //    }
        //    return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        //}
        // POST: /Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> EnableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}
        // POST: /Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DisableTwoFactorAuthentication()
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", "Manage");
        //}
        // GET: /Manage/VerifyPhoneNumber
        //public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
        //    // Send an SMS through the SMS provider to verify the phone number
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}
        // POST: /Manage/VerifyPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
        //    if (result.Succeeded)
        //    {
        //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //        if (user != null)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //        }
        //        return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
        //    }
        //    // If we got this far, something failed, redisplay form
        //    ModelState.AddModelError("", "Failed to verify phone");
        //    return View(model);
        //}
        // POST: /Manage/RemovePhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RemovePhoneNumber()
        //{
        //    var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
        //    if (!result.Succeeded)
        //    {
        //        return RedirectToAction("Index", new { Message = ManageMessageId.Error });
        //    }
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user != null)
        //    {
        //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //    }
        //    return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        //}
        // GET: /Manage/ManageLogins
        //public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
        //    var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
        //    ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
        //    return View(new ManageLoginsViewModel
        //    {
        //        CurrentLogins = userLogins,
        //        OtherLogins = otherLogins
        //    });
        //}
        // POST: /Manage/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        //}

    }
}