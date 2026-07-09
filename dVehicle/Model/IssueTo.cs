using dCommon;

namespace dVehicle.Model
{
    public class IssueTo : DbBase
    {
        public long RecordId { get; set; }
        public string? ReceiverName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailAddress { get; set; }
        public string? CurrentAddress { get; set; }
    }
}
