using System.Collections.Generic;

namespace StudentGradeAPI.Models
{
    public class StudentDto
    {
        public int StudentId { get; set; }         // ✅ 統一 PascalCase 命名
        public string Name { get; set; }
        public List<EnrollmentDto> Enrollments { get; set; } = new();
    }
}