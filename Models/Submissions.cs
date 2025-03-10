using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Submission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssignmentID { get; set; }
        [ForeignKey("AssignmentID")]
        public Assignment? Assignment { get; set; }

        [Required]
        public int StudentID { get; set; }
        [ForeignKey("StudentID")]
        public Account? Account { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public int? PointsEarned { get; set; }

        [Required]
        [Column("SubmittedAt")] 
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
