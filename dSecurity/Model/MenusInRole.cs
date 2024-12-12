using dCommon;

namespace dSecurity.Model
{
    public class MenusInRole : DbBase
    {
        public long RecordId { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        public string? ApprovalStatus { get; set; }
    }
}
