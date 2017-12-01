using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Rad301ClubsV1.Models.ClubModel
{
    [Table("Student")]
    public class Student    {
        [Key]
        [Display(Name ="Student ID")]
        public string StudentID { get; set; }
        [Display(Name = "First Name")]
        public string Fname { get; set; }
        [Display(Name = "Family Name")]
        public string Sname { get; set; }

        //public virtual ICollection<Member> memberOfClubs { get; set; }
    }
}