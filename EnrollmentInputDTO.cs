namespace StudentGradeAPI.Models
{
    public class EnrollmentInputDto
    {
        public int CourseId { get; set; }   // ✅ 必須有這個欄位
        public int Grade { get; set; }
    }
}