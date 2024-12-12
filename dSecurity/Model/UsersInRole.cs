using dCommon;

namespace dSecurity.Model
{
    public class UsersInRole : DbBase
    {
        public long RecordId { get; set; }
        public long RoleId { get; set; }
        public long UserId { get; set; }
        public string? ApprovalStatus { get; set; }
    }
}
