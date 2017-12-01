using Rad301ClubsV1.Models;
using Rad301ClubsV1.Models.ClubModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rad301ClubsV1.Controllers
{
    public class ClubMembersController : Controller
    {

        private ClubContext db = new ClubContext();

        [Authorize(Roles = "ClubAdmin")]

        // GET: ClubMembers======================================================================================
        public ActionResult Index()
        {
            
            using (ApplicationDbContext adb = new ApplicationDbContext())
            {
                var adimUser = GetApplicationUsersInRole(adb, User.Identity.Name).FirstOrDefault();
            }

            var ClubMembers = db.Clubs.Include("Clubmembers").ToList(); 

            return View();
        }

        //========================================================================================================

        public IEnumerable<ApplicationUser> GetApplicationUsers(ApplicationDbContext context,string userName)                                      
        {
                   return from role in context.Roles
                       //where role.Name == roleName
                   from userRoles in role.Users
                   join user in context.Users
                   on userRoles.UserId equals user.Id
                   where user.UserName == userName
                   select user;
        }

    }
}