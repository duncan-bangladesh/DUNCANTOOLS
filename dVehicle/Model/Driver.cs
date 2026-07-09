using dCommon;

namespace dVehicle.Model
{
    public class Drivers : DbBase
    {
        public int RecordId { get; set; }
        public string? DriverName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? CurrentAddress { get; set; }
        public string? MobileNo { get; set; }
    }
}
