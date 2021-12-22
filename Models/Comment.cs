using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class Comment
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Body { get; set; }
        public string NewsId { get; set; }
        public DateTime GmtCreate { get; set; }

        public virtual News News { get; set; }
        public virtual User User { get; set; }
    }
}
