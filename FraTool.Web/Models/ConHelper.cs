namespace FraTool.Web.Models
{
    public class ConHelper
    {
        public string ConStrings(string? codeName)
        {
            string result = "";
            if (codeName != null)
            {
                result = codeName + "Connection";
            }
            return result;
        }
        public string EpmConStrings(string? codeName)
        {
            string result = "";
            if (codeName != null)
            {
                result = codeName + "_EPM";
            }
            return result;
        }
    }
}
