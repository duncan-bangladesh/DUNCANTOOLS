namespace dFinance.Model
{
    public class WagesModel
    {
        public int PermanentAttendance { get; set; }
        public int TemporaryAttendance { get; set; }
        public int DoubleHazira { get; set; }
        public int TotalAttendance { get; set; }
        public int YTDPreviousMonth { get; set; }
        public int YTDAttendanceThisYear { get; set; }
        public int YTDAttendanceLastYear { get; set; }
        public string? AccountsCode { get; set; }
        public string? AccountsDescription { get; set; }
        public string? SubCode { get; set; }
        public string? SubCodeDescription { get; set; }
        public string? AccountsCategory { get; set; }
        public string? AccountsHead { get; set; }
        public int AccountsOrder { get; set; }
        public double PermanentAttendanceWages { get; set; }
        public double TemporaryAttendanceWages { get; set; }
        public double DoubleHaziraWages { get; set; }
        public double TotalAttendanceWages { get; set; }
        public double YTDWagesPreviousMonth { get; set; }
        public double YTDWagesThisYear { get; set; }
        public double YTDWagesLastYear { get; set; }
    }
}
