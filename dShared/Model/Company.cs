using dCommon;
using System.ComponentModel.DataAnnotations;

namespace dShared.Model
{
    public class Company : DbBase
    {
        public long CompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }
        public string CompanyCode { get; set; } = "";
        public string ShortCode { get; set; } = "";
        public int GardenId { get; set; }
        public int IsTeaEstate { get; set; }
    }
}
