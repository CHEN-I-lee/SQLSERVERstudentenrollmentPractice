using StudentGradeAPI.Models;

public class StudentCreateDto
{
    public string Name { get; set; }
    public List<EnrollmentInputDto> Enrollments { get; set; } = new();
}