using dCommon;

namespace dVehicle.Model
{
    public class Owners : DbBase
    {
        public long RecordId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerDescription { get; set; }
    }
}
