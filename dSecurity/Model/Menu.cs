using dCommon;
using System.ComponentModel.DataAnnotations;

namespace dSecurity.Model
{
    public class Menu : DbBase
    {
        [Display(Name = "Id")]
        public long MenuId { get; set; }
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }
        [Display(Name = "Controller Name")]
        public string? ControllerName { get; set; }
        [Display(Name = "Action Name")]
        public string? ActionName { get; set; }
        [Display(Name = "Url")]
        public string? MenuUrl { get; set; }
        [Display(Name = "Is Parent Menu?")]
        public int IsParentMenu { get; set; }
        [Display(Name = "Parent Menu Name")]
        public int ParentMenuId { get; set; }
        [Display(Name = "Icon Tag")]
        public string? IconTag { get; set; }
    }
    public class MenuViewModel : DbBase
    {
        public long MenuId { get; set; }
        public string? DisplayName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? MenuUrl { get; set; }
        public string? IsParentMenu { get; set; }
        public string? ParentMenuId { get; set; }
        public string? IconTag { get; set; }
    }
    public class PMenus
    {
        public long MenuId { get; set; }
        public string? DisplayName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? MenuUrl { get; set; }
        public int IsParentMenu { get; set; }
        public int ParentMenuId { get; set; }
        public string? IconTag { get; set; }
        public List<CMenus>? CMenus { get; set; }
    }
    public class CMenus
    {
        public long MenuId { get; set; }
        public string? DisplayName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? MenuUrl { get; set; }
        public int IsParentMenu { get; set; }
        public int ParentMenuId { get; set; }
        public string? IconTag { get; set; }
    }
}
