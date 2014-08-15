using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityManagementSystemApp.Models
{
    public class ResultEntry
    {
        public int ResultEntryId { get; set; }
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public int GradeLetterId { get; set; }
        public virtual GradeLetter GradeLetter { get; set; }
    }
}