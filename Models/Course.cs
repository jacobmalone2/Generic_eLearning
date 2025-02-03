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

        [Required]
        public int CourseNumber { get; set; }

        [Required]
        public int Credits { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string MeetingTime { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
