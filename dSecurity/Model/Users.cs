using dCommon;
using System.ComponentModel.DataAnnotations;

namespace dSecurity.Model
{
    public class Users : DbBase
    {
        [Display(Name = "Id")]
        public long UserId { get; set; }
        [Display(Name = "Name")]
        public string? FullName { get; set; }
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }
        [Display(Name = "Mobile")]
        public string? MobileNumber { get; set; }
        [Display(Name = "Company")]
        public long CompanyId { get; set; }
        [Display(Name = "Company")]
        public string? CompanyName { get; set; }
        [Required(ErrorMessage = "user name is required")]
        [MinLength(4)]
        [MaxLength(80)]
        [Display(Name = "User Name")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "password is required")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }
        [Display(Name = "Password Hash")]
        public string? PasswordHash { get; set; }
    }
    public class FraLoginInfo
    {
        public string? UserFullName { get; set; }
        public long LoginCompanyId { get; set; }
        public string? FraCompanyCode { get; set; }
        public string? EstateCode { get; set; }
        public string? FraCompanyName { get; set; }
        public string? FraDivisionCode { get; set; }
        public string? FraDivisionName { get; set; }
        public int OnLocationId { get; set; }
    }
}
