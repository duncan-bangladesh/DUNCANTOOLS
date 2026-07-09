using dCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVehicle.Model
{
    public class VehicleType : DbBase
    {
        public int RecordId { get; set; }
        public string? TypeName { get; set; }
        public string? TypeDescription { get; set; }
    }
}
