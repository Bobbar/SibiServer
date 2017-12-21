using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManager;
using System.Data;

namespace SibiServer
{
    public static class Attributes
    {
        public static Dictionary<string, string> SibiAttributes = new Dictionary<string, string>();

        public static void PopulateAttributes()
        {
            var selectSibiAttribs = "SELECT * FROM " + SibiComboCodesCols.TableName;
            var selectDeviceAttribs = "SELECT * FROM " + DeviceComboCodesCols.TableName;

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(selectSibiAttribs))
            {
                foreach (DataRow result in results.Rows)
                {
                    SibiAttributes.Add(result[SibiComboCodesCols.CodeValue].ToString(), result[SibiComboCodesCols.DisplayValue].ToString());
                }
            }

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(selectDeviceAttribs))
            {
                foreach (DataRow result in results.Rows)
                {
                    SibiAttributes.Add(result[SibiComboCodesCols.CodeValue].ToString(), result[SibiComboCodesCols.DisplayValue].ToString());
                }
            }

        }

    }
}
