using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class Friend
    {
        public string MyId { get; set; }
        public string OtherId { get; set; }
        public DateTime GmtCreate { get; set; }

        public virtual User My { get; set; }
        public virtual User Other { get; set; }
    }
}
