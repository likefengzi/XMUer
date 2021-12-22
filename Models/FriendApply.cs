using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class FriendApply
    {
        public string FromId { get; set; }
        public string ToId { get; set; }
        public DateTime GmtCreate { get; set; }

        public virtual User From { get; set; }
        public virtual User To { get; set; }
    }
}
