using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class StoryBlock
    {
        public int StoryBlockID { get; set; }
        public string StoryBlockName { get; set; }
        public string FileName { get; set; }
        public string ImageName { get; set; }
        public DateTime Timeline { get; set; }
        public int? StoriesID { get; set; }
  
        public ICollection<Stories> Stories { get; set; }
    }
}
