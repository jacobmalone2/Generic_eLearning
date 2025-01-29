using System.ComponentModel.DataAnnotations;

namespace CS3750Assignment1.Models
{
    public class Registration
    {
        public int Id { get; set; }

        [Required]
        public int CourseID { get; set; } // Foreign Key

        [Required]
        public int StudentID { get; set; } // Foreign Key
    }
}
