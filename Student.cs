using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentGradeAPI.Models
{
    public class Student
    {
        [Key]
        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}