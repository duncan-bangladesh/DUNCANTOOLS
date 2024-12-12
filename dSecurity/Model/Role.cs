using dCommon;
using System.ComponentModel.DataAnnotations;

namespace dSecurity.Model
{
    public class Role : DbBase
    {
        [Display(Name = "Id")]
        public long RoleId { get; set; }
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
    }
}
