using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InstructorID { get; set; } // Foreign Key
        [ForeignKey("InstructorID")]
        public Account? Account { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CourseNumber { get; set; }

        [Required]
        [Range(1, 80)]
        public int Credits { get; set; }

        [Required]
        [Range(1,200)]
        public int Capacity { get; set; }

        // List of the days of the week enum.
        [StringLength(80, MinimumLength = 0)]
        public string? MeetingDays { get; set; }

        [StringLength(15, MinimumLength = 0)]
        public string? MeetingTime { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
