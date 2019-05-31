using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class Stories
    {
        public int StoriesID { get; set; }
        public string storyName { get; set; }
        public string storyDescription { get; set; } 
        public string ImageName { get; set; }
        public int? CategoriesID { get; set; }

        public Categories Categories { get; set; }

    }
}
