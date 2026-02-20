using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentGradeAPI.Models
{
    public class Course
    {
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("course_name")]
        public string CourseName { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}