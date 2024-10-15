using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
namespace Identity_final_attempt.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name {  get; set; }
       
        [Required(ErrorMessage = "Mobile number is required.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Mobile number must be a 10-digit number.")]
        public string? Number { get; set; }
        public int? AddressId { get; set; }

        public int PointBalance { get; set; }
        [MaxLength(255)]
        public string? ProfilePicUrl { get; set; }
        [Range(1, int.MaxValue)]
        public int? Age { get; set; }

        [RegularExpression("M|F|O")]
        public char? Gender { get; set; }
        
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }
    }

}
