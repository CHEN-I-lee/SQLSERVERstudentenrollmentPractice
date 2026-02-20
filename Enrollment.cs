using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentGradeAPI.Models
{
    public class Enrollment
    {
        [Key]
        [Column("enrollment_id")]
        public int EnrollmentId { get; set; }

        [ForeignKey("Student")]
        [Column("student_id")]
        public int StudentId { get; set; }

        [ForeignKey("Course")]
        [Column("course_id")]
        public int CourseId { get; set; }

        public int Grade { get; set; }

        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}