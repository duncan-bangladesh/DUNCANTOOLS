using dCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVehicle.Model
{
    public class DocumentType : DbBase
    {
        public int RecordId { get; set; }
        public string? DocumentTypeName { get; set; }
        public string? DocumentTypeDescription { get; set; }
    }
}
