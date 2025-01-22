using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Account
    {
        public int Id { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        [Required]
        public required string Email { get; set; }

        [StringLength(30, MinimumLength = 2)]
        [Required]
        public string Username { get; set; } = string.Empty;

        [DataType(DataType.Password)] //gonna try without this, let's see what happens
        [Required]
        public required string Password { get; set; }

    }
}
