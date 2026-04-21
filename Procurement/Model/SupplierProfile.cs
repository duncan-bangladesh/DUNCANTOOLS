namespace Procurement.Model
{
    public class SupplierProfile
    {
        public int SLNo { get; set; } = 0;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? email { get; set; }
        public string? ConPerson { get; set; }
        public string? Group { get; set; }
        public string? Company { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateUser { get; set; }
        public string? TIN { get; set; }
        public int Taxgroup { get; set; } = 0;
        public string? BIN { get; set; }
        public string? Bank { get; set; }
        public string? AccountNo { get; set; }
        public string? RoutingNo { get; set; }
    }
}
