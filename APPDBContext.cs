using Microsoft.EntityFrameworkCore;
using StudentGradeAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 🔑 設定 Enrollment 的主鍵為 EnrollmentId（單一欄位）
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => e.EnrollmentId);

        // 建立 Enrollment → Student 的關聯（多對一）
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 建立 Enrollment → Course 的關聯（多對一）
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);
    }
}