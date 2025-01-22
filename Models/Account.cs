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

        [DataType(DataType.Date)]
        [BirthdateValidation(ErrorMessage = "You must be at least 16 years old to sign up. Please contact your university for approval.")] //custom validation
        [Required]
        public DateOnly Birthdate { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 8)]
        [DataType(DataType.Password)] //gonna try without this, let's see what happens
        [MinLength(8)] //minimum password length is 8 characters
        [Required]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match. Please try again.")]
        [StringLength(int.MaxValue, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; } //extra field for password validation purposes

        [StringLength(10,  MinimumLength = 7)]
        [Required]
        public string AccountRole { get; set; } //can be instructor or student

    }

    /// <summary>
    /// Class extending ValidationAttribute used to check if a user's birthdate would make them younger than 16.
    /// </summary>
    public class BirthdateValidation : ValidationAttribute
    {
        /// <summary>
        /// BirthdateValidation constructor. Does not contain functioning code.
        /// </summary>
        public BirthdateValidation() { }

        /// <summary>
        /// Method overriding the ValidationAttribute IsValid method. 
        /// This method will check a given DateOnly object to determine if a person born on that date would be 16 years old at the time of validation.
        /// </summary>
        /// <param name="value">The input (nullable) DateOnly object containing the date staged for validation.</param>
        /// <returns>Returns true if the given date was 16 or more years ago from time of validation, returns false if the given date was less than 16 years ago or if a null value was given.</returns>
        public override bool IsValid(object? value)
        {
            //if the DateOnly is null, return false
            if (value == null)
            {
                return false;
            }

            DateOnly checkDate = (DateOnly)value; //cast value to DateOnly

            //var age = DateOnly.FromDateTime(DateTime.Now) - checkDate; //can't subtract dates :,(

            //snippet from asking chatgpt for a solution to the above:
            int age = DateTime.Now.Year - checkDate.Year;
            if (DateTime.Now.DayOfYear < checkDate.DayOfYear) //if a user has not had their birthday this year then they are a year younger than birthyear - thisyear
            {
                age--;
            }
            //end snippet

            return age >= 16; //return true if age is greater than or equal to 16, false if age is less than 16
        }
    }
}
