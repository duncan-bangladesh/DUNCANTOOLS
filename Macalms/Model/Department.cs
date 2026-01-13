using dCommon;

namespace Macalms.Model
{
    public class Department: DbBase
    {
        public long RecordId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
