namespace Rad301ClubsV1.Migrations.ClubModelMigrations
{
    using CsvHelper;
    using Models.ClubModel;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using ClubModel;

    internal sealed class Configuration : DbMigrationsConfiguration<Rad301ClubsV1.Models.ClubModel.ClubContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\ClubModelMigrations";
        }
        //=======================================================================================================
        protected override void Seed(Rad301ClubsV1.Models.ClubModel.ClubContext context)
        {
            SeedStudents(context);
            SeedClubs(context);
            SeedClubMembers(context);
            SeedCourse(context);
        }
        //=======================================================================================================
        private void SeedStudents(ClubContext context)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Rad301ClubsV1.Migrations.ClubModelMigrations.TestStudents.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    var testStudents = csvReader.GetRecords<Student>().ToArray();
                    context.Students.AddOrUpdate(s => s.StudentID, testStudents);
                }
            }
        }
        //=======================================================================================================
        private void SeedClubs(ClubContext context)
        {
            // Seeding a club using AddOrUpdate

            List<Club> Clubs = new List<Club>();
                context.Clubs.AddOrUpdate(c => c.ClubName,
                        new Club
                        {
                            ClubName = "The Tiddly Winks Club",
                            CreationDate = DateTime.Now,
                            adminID = -1, // Choosing a negative to define unassigned as all members will have a positive id later
                                          // It seem you cannot reliably assign the result of a method to a field while using 
                                          // Add Or Update. My suspicion is that it cannot evaluate whether 
                                          // or not it is an update. There could also be a EF version issue
                                          // The club events assignment will work though as it is 
                clubEvents = new List<ClubEvent>()
                        {	// Create a new ClubEvent 
                        new ClubEvent { StartDateTime = DateTime.Now.Subtract( new TimeSpan(5,0,0,0,0)),
                           EndDateTime = DateTime.Now.Subtract( new TimeSpan(5,0,0,0,0)),
                           Location="Sligo", Venue="Arena",
                           // Update attendees with a method similar to the SeedClubMembers 
                           // See below
                        },
                        new ClubEvent { StartDateTime = DateTime.Now.Subtract( new TimeSpan(3,0,0,0,0)),
                           EndDateTime = DateTime.Now.Subtract( new TimeSpan(3,0,0,0,0)),
                           Location="Sligo", Venue="Main Canteen"
        },
                        }
                        });

            context.Clubs.AddOrUpdate(c => c.ClubName,
                new Club { ClubName = "The Chess Club", CreationDate = DateTime.Now });
            context.SaveChanges(); // Make sure you save the context before you attempt to add new members for the clubs
        }
        //=======================================================================================================
        private void SeedClubMembers(ClubContext context)
        {
            
            List<Student> selectedStudents = new List<Student>();// Create a list to hold students// It's important that you save any newly created clubs before retrieving them as a list
            
            foreach (var club in context.Clubs.ToList())
            {
                
                if (club.clubMembers == null || club.clubMembers.Count() < 1)// Get a set of members if none set yet
                {
                    
                    selectedStudents = GetStudents(context);// get a set of random candidates see method below
                    foreach (var m in selectedStudents)
                    {
                        
                        context.members.AddOrUpdate(member => member.StudentID,// Add a new member with a reference to a club// EF will pick up on the join fields later
                            new Member { ClubId = club.ClubId, StudentID = m.StudentID });
                    }
                }
            }
            context.SaveChanges();

        }
        //========================================================================================================
        private List<Student> GetStudents(ClubContext context)
        {
            // Create a random list of student ids
            var randomSetStudent = context.Students.Select(s => new { s.StudentID, r = Guid.NewGuid() });
            // sort them and take 10
            List<string> subset = randomSetStudent.OrderBy(s => s.r)
                .Select(s => s.StudentID.ToString()).Take(10).ToList();
            // return the selected students as a relaized list
            return context.Students.Where(s => subset.Contains(s.StudentID)).ToList();
        }
        //========================================================================================================
        private void SeedAdimForClub(ClubContext context)
        {
            List<Club> clubs = context.Clubs.Include("clubMembers").ToList();


            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                foreach (Club c in clubs)
                {
                    
                    c.adminID = c.clubMembers.First().memberID;
                    db.Users.AddOrUpdate(u => u.Email, new ApplicationUser
                    {
                        StudentID = c.clubMembers.First().StudentID,
                        Email = c.clubMembers.First().StudentID + "@mail.itsligo.ie",
                        DateJoined = DateTime.Now,
                        EmailConfirmed = true,
                        UserName = c.clubMembers.First().StudentID + "@mail.itsligo.ie",
                        PasswordHash = new PasswordHasher().HashPassword(c.clubMembers.First().StudentID + "$1"),
                        SecurityStamp = Guid.NewGuid().ToString(),

                    });
                    db.SaveChanges();
                    ApplicationUser user = manager.FindByEmail(c.clubMembers.First().StudentID + "@mail.itsligo.ie");
                    if (user !=null)
                    {
                        manager.AddToRole(user.Id, "ClubAdim");
                    }
                }
                context.Clubs.AddOrUpdate(c => c.ClubName, clubs.ToArray());
            }

        }
        //========================================================================================================
        private void SeedCourse(ClubContext context)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string resourceName = "Rad301ClubsV1.Migrations.ClubModelMigrations.Course.csv";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.HasHeaderRecord = false;
                    
                    var courseData = csvReader.GetRecords<CourseDataImport>().ToArray();
                    foreach(var dataItem in courseData)
                    {
                        context.Courses.AddOrUpdate(c => new { c.CourseName, c.CourseCode  },
                            new Course { CourseCode = dataItem.CourseName, CourseName = dataItem.CourseName,
                                Year = dataItem.Year });

                    }
                }
            }
        } 
        //========================================================================================================
        private void addRolesforAdims(ClubContext context)
        {
            List<Club> admins = context.Clubs.Include("clubMembers").ToList();

            var adminMembers = (from admin in admins join member in context.members.ToList() on admin.adminID equals member.memberID select member);

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));


            foreach(var item in adminMembers)
            {
                ApplicationUser user = db.Users.FirstOrDefault(u => u.StudentID == item.StudentID); 

                    if (user !=null)
                    {
                        manager.AddToRole(user.Id, "ClubAdmin");
                    }
            }
                db.SaveChanges();
        }
               
        }
        //========================================================================================================

    }


    //try
    //{
    //    StudentData testStudents = new StudentData();
    //}
    //catch(Exception e) {
    //    throw new Exception { Source = e.Message };
    //}
}
 
