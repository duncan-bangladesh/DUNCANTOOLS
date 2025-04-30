using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dShared.Model
{
    public class TransferData
    {   
        public long Id { get; set; }
        public string Year { get; set; } = string.Empty;
        public string Month { get; set; } = string.Empty;
        public string AccountNo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Crop { get; set; } = string.Empty;
        public double Amount { get; set; }
    }
}
