using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MHS_FINAL_PROJECT.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace MHS_FINAL_PROJECT.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public AccountController()
        {
        }
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
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

        



        //Load the login page [httpget]********************************************************************
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Pharmacist = (string)TempData["done"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }






        //Login the user or return validation **********************************************************
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                bool is_pharmacists = false;
                var user = db.Users.Where(x => x.UserName == model.Username).FirstOrDefault();
                if (user != null)
                {
                    is_pharmacists = await UserManager.GetLockoutEnabledAsync(user.Id);
                }
                if (is_pharmacists == true)
                {
                    ViewBag.Pharmacist = "Your Account Locked Until The Admin Accept Your Certification";
                    return View(model);
                }
                else
                {
                    var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, change to shouldLockout: true
                    switch (result)
                    {
                        case SignInStatus.Success:
                            return RedirectToLocal(returnUrl);
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
            }
        }


        



        //Load the register page [Httpget]********************************************************
        [AllowAnonymous]
        public ActionResult Register()
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                 new SelectListItem {  Text = "Male", Value = "Male"},
                 new SelectListItem { Text = "Female", Value = "Female"},
            };
            ViewBag.gender = gender.ToList();
            return View();
        }

        




        //Register normal user or return the validation**************************************************
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            model.Roles = "Normal_User";
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                 new SelectListItem {  Text = "Male", Value = "Male"},
                 new SelectListItem { Text = "Female", Value = "Female"},
            };
            ViewBag.gender = gender.ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = model.Username, 
                    Email = model.Email,
                    age = model.Age,
                    Gender = model.gender,
                    Name = model.Name,
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    UserManager.SetLockoutEnabled(user.Id, false);
                    await UserManager.AddToRoleAsync(user.Id, model.Roles);
                    return RedirectToAction("Index", "Home");
                }
               
                AddErrors(result);
            }
            if (ModelState["Age"].Errors.Count > 0 && ModelState["Age"].Errors[0].ErrorMessage.Contains("is not valid for Age"))
            {
                ModelState["Age"].Errors.Clear();
                ModelState["Age"].Errors.Add("Age must be Number");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }









        //Load the register page for Pharmacist*******************************************************
        [AllowAnonymous]
        public ActionResult RegisterInfoPharmacist()
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                 new SelectListItem {  Text = "Male", Value = "Male"},
                 new SelectListItem { Text = "Female", Value = "Female"},
            };
            ViewBag.gender = gender.ToList();
            return View();
        }







        //Check the validation and store the data in TempData for saving after Certification*********************************************************
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterInfoPharmacist(RegisterViewModel model)
        {
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                 new SelectListItem {  Text = "Male", Value = "Male"},
                 new SelectListItem { Text = "Female", Value = "Female"},
            };
            ViewBag.gender = gender.ToList();
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var user_name = db.Users.Where(x => x.UserName == model.Username).FirstOrDefault();
                    var email = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                    if (user_name != null)
                    {
                        ViewBag.UserName = "User Name" + model.Username + "Exist";
                    }
                    if (email != null)
                    {
                        ViewBag.email = "email " + model.Email + " Exist";
                    }
                    if (email == null && user_name == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = model.Username,
                            Email = model.Email,
                            age = model.Age,
                            Gender = model.gender,
                            Name = model.Name,

                        };
                        TempData["user"] = user;
                        TempData["Password"] = model.Password;
                        TempData["Pharmacist_register"] = "true";
                        return RedirectToAction("RegisterCertificationPharmacist", "Account");
                    }
                    else
                    {
                        if (ModelState["Age"].Errors.Count > 0 && ModelState["Age"].Errors[0].ErrorMessage.Contains("is not valid for Age"))
                        {
                            ModelState["Age"].Errors.Clear();
                            ModelState["Age"].Errors.Add("Age must be Number");
                        }
                        return View(model);
                    }
                }
            }
            if (ModelState["Age"].Errors.Count > 0 && ModelState["Age"].Errors[0].ErrorMessage.Contains("is not valid for Age"))
            {
                ModelState["Age"].Errors.Clear();
                ModelState["Age"].Errors.Add("Age must be Number");
            }
            return View(model);
        }







        //Load the Certification page for Pharmacist*****************************************************
        [AllowAnonymous]
        public ActionResult RegisterCertificationPharmacist()
        {
            if (TempData["Pharmacist_register"] == null)
            {
                return RedirectToAction("RegisterInfoPharmacist", "Account");
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
            return View();
        }








        //Register the Pharmacist all information with the Certification*******************************************************
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCertificationPharmacist(HttpPostedFileBase file ,CERTIFICATIONViewModel model)
        {
            List<SelectListItem> degree = new List<SelectListItem>()
            {
                 new SelectListItem {  Text = "Associate degree", Value = "Two_years"},
                 new SelectListItem { Text = "Bachelors degree", Value = "Four_years"},
                 new SelectListItem { Text = "Masters degree", Value = "Two_years"},
                 new SelectListItem { Text = "Doctoral degree", Value = "Four_years"},
                 new SelectListItem { Text = "Professional degree", Value = "Four_seven _years"},
            };
            ViewBag.degree = degree.ToList();


            //add user normal information
            ApplicationUser user = (ApplicationUser)TempData["user"];
            string password = (string)TempData["Password"];
            var result = await UserManager.CreateAsync(user, password);
            //set roll to pharmacists*********************************
            await UserManager.AddToRoleAsync(user.Id, "pharmacists");
            //set The Account to Lockout*********************************
            UserManager.SetLockoutEnabled(user.Id , true);


            if (model.Certification_url != null && result.Succeeded)
            {
                file = model.Certification_url;
                model.img_path = file.ToString(); //getting complete url  
                var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg) 
                var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)
                string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension
                string myfile = name + "_" + user.Id + ext; //appending the name with id  
                                                            // store the file inside ~/project folder(CERTIFICATION_IMG)  
                var path = Path.Combine(Server.MapPath("~/CERTIFICATION_IMG"), myfile);
                file.SaveAs(path);
                path = "/CERTIFICATION_IMG/" + myfile;
                model.img_path = path;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    SqlParameter[] Register_Pharmacist = new SqlParameter[]
                    {
                    new SqlParameter("@UnivarstyName" , model.Name.ToString()),
                    new SqlParameter("@Degree" , model.Degree.ToString()),
                    new SqlParameter("@UserId" , user.Id.ToString()),
                    new SqlParameter("@certificationImg" , model.img_path.ToString())
                    };
                    var add_Pharmacist = db.Database.ExecuteSqlCommand("Add_Pharmacist @UnivarstyName , @Degree , @certificationImg , @UserId", Register_Pharmacist).ToString();
                    if (add_Pharmacist == "1" || add_Pharmacist == "-1")
                    {
                        TempData["done"] = "Your Account Will Be Locked Until The Admin Accept Your Certification";
                        return RedirectToAction("Login", "Account");
                    }
                }
            }            
            return View(model);
        }






        //LogOff The user and send it to home page**************************************************************
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }









        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}



        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}
        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}
        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }





        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}




        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}





        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            string Roles = "Normal_User";
            ApplicationDbContext db = new ApplicationDbContext();
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    if (User.Identity.IsAuthenticated)
                    {
                        return RedirectToAction("Index", "Manage");
                    }
                    if (ModelState.IsValid)
                    {
                        // Get the information about the user from the external login provider
                        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                        if (info == null)
                        {
                            return View("ExternalLoginFailure");
                        }
                        string name_check = loginInfo.Email.Split('@')[0];
                        var check = db.Users.Where(find => find.UserName == name_check).FirstOrDefault();
                        string name = string.Empty;
                        if (check == null)
                        {
                            name = loginInfo.Email.Split('@')[0];
                        }
                        else
                        {
                            name = loginInfo.Email;
                        }
                        var user = new ApplicationUser
                        {
                            UserName = name,
                            Email = loginInfo.Email,
                            Name = loginInfo.ExternalIdentity.Name,
                            age = 00,
                            Gender = "Anonymous"

                        };
                        var result_done = await UserManager.CreateAsync(user);
                        if (result_done.Succeeded)
                        {
                            result_done = await UserManager.AddLoginAsync(user.Id, info.Login);
                            if (result_done.Succeeded)
                            {
                                await UserManager.AddToRoleAsync(user.Id, Roles);
                                await UserManager.SetLockoutEnabledAsync(user.Id, false);
                                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                        AddErrors(result_done);
                        return View("ExternalLoginFailure");
                    }

                    ViewBag.ReturnUrl = returnUrl;
                    return RedirectToLocal(returnUrl);

                    //// If the user does not have an account, then prompt the user to create an account
                    //ViewBag.ReturnUrl = returnUrl;
                    //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    //ViewBag.email = loginInfo.Email;
                    //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email , Name = loginInfo.ExternalIdentity.Name , Age = 00 , gender = "anonymous" });
            }
        }

        


        //POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl , string email) 
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { 
        //            UserName = model.UserName  , 
        //            Email = model.Email,
        //            Name = model.Name,
        //            age = model.Age,
        //            Gender = model.gender
                
        //        };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        
        

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }



        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}






        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}








        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}






        #region Helpers
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}