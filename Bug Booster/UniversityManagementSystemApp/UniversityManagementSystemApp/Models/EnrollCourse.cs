using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace UniversityManagementSystemApp.Models
{
    public class EnrollCourse
    {
        public int EnrollCourseId { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public DateTime Date { get; set; }
    }
}