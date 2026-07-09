using dCommon;

namespace dVehicle.Model
{
    public class Vehicles : DbBase
    {
        public long RecordId { get; set; }
        public string? VehicleName { get; set; }
        public long OwnerId { get; set; } = 0;
        public string? OwnerName { get; set; }
        public string? RegistrationDate { get; set; }
        public string? LicensePlate { get; set; }
        public long IssueLocationId { get; set; } = 0;
        public string? IssueLocationName { get; set; }
        public int CompanyId { get; set; }
        public long IssueToId { get; set; } = 0;
        public string? IssueToName { get; set; }
        public int VehicleTypeId { get; set; } = 0;
        public string? VehicleTypeName { get; set; }
        public int BRTAOfficeId { get; set; } = 0;
        public string? BRTAOfficeName { get; set; }
        public int DriverId { get; set; } = 0;
        public string? DriverName { get; set; }
        public int SeatCapacityWithDriver { get; set; } = 0;
        public string? Remarks { get; set; }
    }
}
