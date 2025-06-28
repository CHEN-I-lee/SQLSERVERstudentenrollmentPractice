using StudentGradeAPI;

public class EnrollmentDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int Grade { get; set; }


}