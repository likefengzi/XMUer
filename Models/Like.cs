using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class Like
    {
        public string UserId { get; set; }
        public string NewsId { get; set; }

        public virtual News News { get; set; }
        public virtual User User { get; set; }
    }
}
