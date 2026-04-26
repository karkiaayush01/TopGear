using System.ComponentModel.DataAnnotations;


namespace TopGear.Application.DTOs.CustomerDTO
{
    public class UpdateCustomerRequest
    {
        [Required]
        [StringLength(255)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;
    }
}
