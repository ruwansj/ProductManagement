using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Core.Models
{
    public class StoredProcedureResults
    {
        public const string SP_CategoryAverages = "EXEC CalculateCategoryAverages";
        public const string SP_HighestStockValueCategory = "EXEC CalculateHighestStockValueCategory";
    }
}
