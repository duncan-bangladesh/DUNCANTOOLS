using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVoucher.Model
{
    public class ConfigConnection
    {
        public string DbConfig(string CurrentUserEstate)
        {
            if (!string.IsNullOrEmpty(CurrentUserEstate))
            {
                return CurrentUserEstate + "teConnection";
            }
            else
            {
                return "";
            }
        }
    }
}
