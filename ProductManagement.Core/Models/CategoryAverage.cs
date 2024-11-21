using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Core.Models
{
 
    [NotMapped]
    public class CategoryAverage
    {
        public string Category { get; set; } = string.Empty;
        public decimal AveragePrice { get; set; }
    }
}
