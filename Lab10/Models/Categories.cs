using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class Categories
    {
        public int CategoriesID { get; set; }
        public string categoryName { get; set; }
        public string ImageName { get; set; }
         public ICollection<Stories> Stories { get; set; }
    }
}
