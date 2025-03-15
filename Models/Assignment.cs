using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Assignment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CourseID { get; set; } // Foreign Key
        [ForeignKey("CourseID")]
        public Course? Course { get; set; }



        [Required]
        public string Title { get; set; }

        [StringLength(int.MaxValue)]
        public string? Description { get; set; }

        [Required]
        public int MaxPoints { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string DueDate { get; set; }

        [StringLength(7, MinimumLength = 0)]
        public string? DueTime { get; set; }

        public string? AcceptedFileTypes { get; set; }
    }
}
