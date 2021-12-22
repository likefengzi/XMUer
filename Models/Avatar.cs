using System;
using System.Collections.Generic;

#nullable disable

namespace XMUer.Models
{
    public partial class Avatar
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Path { get; set; }
        public DateTime GmtCreate { get; set; }
        public DateTime GmtModify { get; set; }

        public virtual User User { get; set; }
    }
}
