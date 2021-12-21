using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class News
    {
        public News()
        {
            Comments = new HashSet<Comment>();
            Likes = new HashSet<Like>();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string Body { get; set; }
        public byte IsPublic { get; set; }
        public long? SharedNewsId { get; set; }
        public DateTime GmtCreate { get; set; }
        public DateTime GmtModify { get; set; }


        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
