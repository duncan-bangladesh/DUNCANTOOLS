using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dFinance.Model
{
    public class ChartOfAccounts
    {
        public string? SageAccountsId { get; set; }
        public string? SageAccountsDescription { get; set; }
        public string? CostCenter { get; set; }
        public string? LocationCode { get; set; }
        public string? AccountsGroupCode { get; set; }
        public string? AccountsGroupDescription { get; set; }
        public string? AccountsSubGroupCode { get; set; }
    }
}
