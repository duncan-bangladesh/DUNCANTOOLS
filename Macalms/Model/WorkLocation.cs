using dCommon;

namespace Macalms.Model
{
    public class WorkLocation : DbBase
    {
        public long RecordId { get; set; }
        public string? LocationName { get; set; }
        public string? LocationTag { get; set; }
    }
}
