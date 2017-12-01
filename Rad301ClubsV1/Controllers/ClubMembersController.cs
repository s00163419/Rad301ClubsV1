using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rad301ClubsV1.Controllers
{
    public class ClubMembersController : Controller
    {

        [Authorize(Roles = "ClubAdmin")]

        // GET: ClubMembers
        public ActionResult Index()
        {
            return View();
        }
    }
}