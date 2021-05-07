using MHS_FINAL_PROJECT.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MHS_FINAL_PROJECT.Startup))]
namespace MHS_FINAL_PROJECT
{
    public partial class Startup
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreatRoles();
            CreatAdmin();
        }
        private void CreatRoles()
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            IdentityRole role;

            if (!RoleManager.RoleExists("Admins"))
            {
                role = new IdentityRole();
                role.Name = "Admins";
                RoleManager.Create(role);
            }

            if (!RoleManager.RoleExists("Normal_User"))
            {
                role = new IdentityRole();
                role.Name = "Normal_User";
                RoleManager.Create(role);
            }

            if (!RoleManager.RoleExists("pharmacists"))
            {
                role = new IdentityRole();
                role.Name = "pharmacists";
                RoleManager.Create(role);
            }

        }

        private void CreatAdmin()
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = new ApplicationUser();

            user.UserName = "Admin";
            user.Name = "Omar Alhourani";
            user.Email = "Admin@mhs.com";
            user.age = 23;
            user.Gender = "Male";

            var check = UserManager.Create(user, "Admin123@@");
            if (check.Succeeded)
            {
                UserManager.AddToRole(user.Id, "Admins");
            }
        }
    }

   

}
