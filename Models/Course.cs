using System.ComponentModel.DataAnnotations;

namespace CS3750Assignment1.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public int InstructorID { get; set; } // Foreign Key

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public int CourseNumber { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public int Credits { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public int Capacity { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateOnly MeetingTime { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
