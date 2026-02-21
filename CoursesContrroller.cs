using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradeAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    // 📘 取得所有課程
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _context.Courses
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName
            }).ToListAsync();

        return Ok(courses);
    }

    // ➕ 新增課程
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CourseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CourseName))
            return BadRequest("課程名稱不可為空");

        var course = new Course
        {
            CourseName = dto.CourseName
        };

        _context.Add(course);
        await _context.SaveChangesAsync();

        dto.CourseId = course.CourseId;
        return CreatedAtAction(nameof(GetCourses), new { id = course.CourseId }, dto);
    }

    // ✏️ 修改課程
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        course.CourseName = dto.CourseName;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 🗑 刪除課程
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}