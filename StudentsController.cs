using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradeAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    // 查詢所有學生（包含選課與課程資料）
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
    {
        var students = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .ToListAsync();

        var result = students.Select(s => new StudentDto
        {
            StudentId = s.StudentId,
            Name = s.Name,
            Enrollments = s.Enrollments.Select(e => new EnrollmentDto
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                Grade = e.Grade,
                CourseName = e.Course.CourseName
            }).ToList()
        });

        return Ok(result);
    }

    // 根據 ID 查詢單一學生（包含選課）
    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        var student = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            return NotFound();

        return student;
    }

    // 新增學生（包含第一筆選課）
    [HttpPost]
    public async Task<ActionResult> PostStudent(StudentCreateDto dto)
    {
        var student = new Student
        {
            Name = dto.Name,
            Enrollments = dto.Enrollments.Select(e => new Enrollment
            {
                CourseId = e.CourseId,
                Grade = e.Grade
            }).ToList()
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudent), new { id = student.StudentId }, null);
    }

    // 為現有學生新增選課（新增一筆 enrollment）
    [HttpPost("{id}/enroll")]
    public async Task<IActionResult> AddEnrollment(int id, [FromBody] EnrollmentInputDto dto)
    {
        var student = await _context.Students
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            return NotFound();

        // ✅ 可以防止重複選課（可選用）
        bool alreadyEnrolled = student.Enrollments.Any(e => e.CourseId == dto.CourseId);
        if (alreadyEnrolled)
            return BadRequest("該學生已修過此課程");

        var enrollment = new Enrollment
        {
            StudentId = id,
            CourseId = dto.CourseId,
            Grade = dto.Grade
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // 編輯學生
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int id, Student student)
    {
        if (id != student.StudentId)
            return BadRequest();

        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    // ✏️ 修改選課成績或課程
    [HttpPut("/api/enrollments/{id}")]
    public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentInputDto dto)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null)
            return NotFound();

        // ✅ 如果 CourseId 沒變，就只更新成績
        if (enrollment.CourseId == dto.CourseId)
        {
            enrollment.Grade = dto.Grade;
        }
        else
        {
            // ✅ 如果 CourseId 有變，先檢查是否已存在該學生選過這門課
            bool exists = await _context.Enrollments.AnyAsync(e =>
                e.StudentId == enrollment.StudentId &&
                e.CourseId == dto.CourseId &&
                e.EnrollmentId != id);

            if (exists)
                return BadRequest("該學生已選修此課程，無法重複選課");

            enrollment.CourseId = dto.CourseId;
            enrollment.Grade = dto.Grade;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
    // 根據名稱查詢學生是否存在
    [HttpGet("exists")]
    public async Task<ActionResult<StudentDto?>> GetStudentByName([FromQuery] string name)
    {
        var student = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Name == name);

        if (student == null)
            return Ok(null);

        var dto = new StudentDto
        {
            StudentId = student.StudentId,
            Name = student.Name,
            Enrollments = student.Enrollments.Select(e => new EnrollmentDto
            {
                EnrollmentId = e.EnrollmentId,
                CourseId = e.CourseId,
                Grade = e.Grade,
                CourseName = e.Course.CourseName
            }).ToList()
        };

        return Ok(dto);
    }

    // 刪除學生
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}