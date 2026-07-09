using dCommon;

namespace dVehicle.Model
{
    public class IssueLocation : DbBase
    {
        public long RecordId { get; set; }
        public string? LocationName { get; set; }
        public string? LocationDescription { get; set; }
    }
}
