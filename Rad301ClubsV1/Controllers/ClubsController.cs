using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Rad301ClubsV1.Models.ClubModel;
using Rad301ClubsV1.Models;

namespace Rad301ClubsV1.Controllers
{
    [Authorize(Roles = "Admin,ClubAdmin")]
    public class ClubsController : Controller
    {
        private ClubContext db = new ClubContext();

        // GET: Clubs=============================================================================================================================
        public async Task<ActionResult> Index(string ClubName = null )
        {
            using (ApplicationDbContext adb = new ApplicationDbContext())
            {
                var adimUser = GetApplicationUsersInRole(adb, User.Identity.Name).FirstOrDefault();
            }

            var clubMembers = db.Clubs.Include("clubMembers").ToList();


            return View(await db.Clubs.Where(c => ClubName == null || c.ClubName.StartsWith(ClubName)).ToListAsync());
                
            }

        //==========================================================================================================================================

        public IEnumerable<ApplicationUser> GetApplicationUsersInRole(ApplicationDbContext context, string userName)
        {
                   return from role in context.Roles
                   //where role.Name == roleName
                   from userRoles in role.Users
                   join user in context.Users
                   on userRoles.UserId equals user.Id
                   where user.UserName == userName 
                   select user;
        }

        // GET: Clubs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = await db.Clubs.FindAsync(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // GET: Clubs/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> Create([Bind(Include = "ClubId,ClubName,CreationDate")] Club club)
        {
            if (ModelState.IsValid)
            {
                db.Clubs.Add(club);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(club);
        }

        // GET: Clubs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = await db.Clubs.FindAsync(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }
        //===========================================================================================================
        // POST: Clubs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //===========================================================================================================
        public async Task<ActionResult> Edit([Bind(Include = "ClubId,ClubName,CreationDate")] Club club)
        {
            if (ModelState.IsValid)
            {
                db.Entry(club).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(club);
        }

        // GET: Clubs/Delete/5=====================================================================================
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = await db.Clubs.FindAsync(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // POST: Clubs/Delete=====================================================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Club club = await db.Clubs.FindAsync(id);
            db.Clubs.Remove(club);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //========================================================================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
