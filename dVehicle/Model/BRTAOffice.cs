using dCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVehicle.Model
{
    public class BRTAOffice : DbBase
    {
        public int RecordId { get; set; }
        public string? OfficeName { get; set; }
        public string? OfficeAddress { get; set; }
    }
}
