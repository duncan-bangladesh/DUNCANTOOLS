using System.ComponentModel.DataAnnotations;

namespace dSecurity.Model
{
    public class UserLogin
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
