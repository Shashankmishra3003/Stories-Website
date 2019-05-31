using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class Comments
    {
        public int CommentsID { get; set; }
        public String CommentData { get; set; }
        public int StoriesID { get; set; }
        public String Commenter { get; set; }

        public Stories Stories { get; set; }
    }
}
