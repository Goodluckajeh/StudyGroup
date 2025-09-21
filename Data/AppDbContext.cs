using Microsoft.EntityFrameworkCore;
using StudyGroup.Models;

namespace StudyGroup.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudyGroups> StudyGroups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
